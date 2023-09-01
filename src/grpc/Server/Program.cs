using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Server.Interceptors;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    // Defines a custom client certificate handling interceptor (NOT NECESSARY)
    // Could be used to do some stuff with the client certificate if it's necessary
    options.Interceptors.Add<CertificateHandlingInterceptor>();
});

var port = builder.Configuration.GetValue<int?>("PORT") ?? 
           throw new Exception("The environment variable \"PORT\" must be set");

var forwardingHeader = builder.Configuration.GetValue<string?>("CLIENT_CERT_FORWARDING_HEADER");
var hasForwarding = !string.IsNullOrEmpty(forwardingHeader);

// Defines Kestrel with SSL and Client Certificate
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port, listenOpts =>
    {
        listenOpts.Protocols = HttpProtocols.Http2;
        
        // The server certificate is copied during app build...
        var serverCert = new X509Certificate2(Path.Join(AppContext.BaseDirectory, "server.pfx"));
        listenOpts.UseHttps(serverCert, httpsOpts =>
        {  
            // IMPORTANT:
            // If client certificate forwarding is set, the kestrel can't require the certificate.
            // The proxy/load balancer is the responsible for managing the client certificate and do the handshake.
            // The kestrel will just receive the client certificate in a specific header.
            httpsOpts.ClientCertificateMode = hasForwarding
                ? ClientCertificateMode.AllowCertificate
                : ClientCertificateMode.RequireCertificate;
        });
    });
});

// Defines authentication with certificate scheme
// This is required for authentication purposes, and also require to do the certificate forwarding
// It's also possible to add other authentication schemes, like JWT...
builder.Services
    .AddAuthorization()
    .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(opts =>
    {
        // The type "All" was chosen because of self-signed certificates
        opts.AllowedCertificateTypes = CertificateTypes.All; 
        opts.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {
                // It's possible to validate the certificate here...
                // Using "context.Success();" will succeed de request
                // Using "context.Fail("some text");" will fail de request
                
                context.HttpContext.Connection.ClientCertificate = context.ClientCertificate;
                return Task.CompletedTask;
            }
        };
    });

// Defines the client certificate forwarding, if there has some proxy or load balancer
// The header name could be anything in this example (e.g.: For nginx, the common used is "ssl-client-cert").
// The header value must contains the entire client certificate encoded.
if (hasForwarding)
{
    builder.Services.AddCertificateForwarding(options =>
    {
        options.CertificateHeader = forwardingHeader!;
        options.HeaderConverter = (headerValue) =>
        {
            X509Certificate2? clientCertificate = null;
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                clientCertificate = X509Certificate2.CreateFromPem(WebUtility.UrlDecode(headerValue));
            }
            return clientCertificate!;
        };
    });
}

var app = builder.Build();

if (hasForwarding)
{
    app.UseCertificateForwarding();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
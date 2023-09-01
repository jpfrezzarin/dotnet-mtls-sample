using System.Security.Cryptography.X509Certificates;
using Greet;
using Grpc.Net.Client;

#if RELEASE
const string baseUrl = "https://mtls-example-grpc.friday.local";
#else
const string baseUrl = "https://localhost:6684";
#endif

// The client certificate is copied during app build...
var clientCertPath = Path.Join(AppContext.BaseDirectory, "client.pfx");
var clientCert = new X509Certificate2(clientCertPath);

using var channel = GrpcChannel.ForAddress(baseUrl, new GrpcChannelOptions
{
    HttpHandler = new HttpClientHandler { ClientCertificates = { clientCert }}
});

var client = new Greeter.GreeterClient(channel);

var request = new HelloRequest { Name = "Michael" };

Console.WriteLine("Michael says: Hello Server");

var response = await client.SayHelloAsync(request);

Console.WriteLine($"Server says: {response.Message}");


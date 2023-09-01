namespace Server.Middlewares;

public class ClientCertificateHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ClientCertificateHandlingMiddleware> _logger;

    public ClientCertificateHandlingMiddleware(ILogger<ClientCertificateHandlingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var clientCert = context.Connection.ClientCertificate;
        
        if (clientCert is not null)
            _logger.LogInformation("Client certificate subject: {ClientCertSubject}", clientCert.Subject);
        else 
            _logger.LogInformation("Client certificate not found");
        
        // Do some stuff with the client certificate, if its necessary...
        
        await next(context);
    }
}
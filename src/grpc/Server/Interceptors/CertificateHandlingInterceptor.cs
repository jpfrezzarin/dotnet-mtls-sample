using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Server.Interceptors;

public class CertificateHandlingInterceptor : Interceptor
{
    private readonly ILogger<CertificateHandlingInterceptor> _logger;

    public CertificateHandlingInterceptor(ILogger<CertificateHandlingInterceptor> logger)
    {
        _logger = logger;
    }
    
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var clientCert = context.GetHttpContext().Connection.ClientCertificate;
        
        if (clientCert is not null)
            _logger.LogInformation("Client certificate subject: {ClientCertSubject}", clientCert.Subject);
        else 
            _logger.LogInformation("Client certificate not found");
        
        // Do some stuff with the client certificate, if its necessary...
        
        return await continuation(request, context);
    }
}
namespace Server.Middlewares;

public static class MiddlewaresExtension
{
    public static void AddCertificateHandling(this IServiceCollection services)
    {
        services.AddScoped<ClientCertificateHandlingMiddleware>();
    }
    
    public static void UseCertificateHandling(this WebApplication app)
    {
        app.UseMiddleware<ClientCertificateHandlingMiddleware>();
    }
}
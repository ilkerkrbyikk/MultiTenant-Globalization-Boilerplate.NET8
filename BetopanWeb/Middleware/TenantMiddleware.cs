using System.IO;
using BetopanWeb.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace BetopanWeb.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;


        public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService _tenantService)
        {
            try
            {
                //host info al
                var host = context.Request.Host.Host.ToLowerInvariant();
                var port = context.Request.Host.Port;
                var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

                // Test endpoint'lerini bypass et
                if (path.StartsWith("/test"))
                {
                    await _next(context);
                    return;
                }
                // Test için localhost mapping
                string targetDomain = host;

                if (host == "localhost")
                {
                    targetDomain = port switch
                    {
                        5000 => "betopan.com.tr",  // Default port - TR site
                        5001 => "betopan.net",     // Secondary port - Global site
                        _ => "betopan.com.tr"      // Fallback
                    };

                    _logger.LogInformation("Localhost mapping: Port {Port} -> {Domain}", port, targetDomain);
                }

                _logger.LogInformation("Processing request for host: {Host}:{Port} -> {TargetDomain}", host, port, targetDomain);


                var tenant = await _tenantService.GetTenantByDomainAsync(targetDomain);

                if (tenant == null)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Tenant not found");
                    return;
                }

                if (!tenant.IsActive)
                {
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsync("Service temporarily unavailable");
                    return;
                }

                // Tenant bilgisini HttpContext.Items'a kaydet
                context.Items["CurrentTenant"] = tenant;
                context.Items["TenantId"] = tenant.Id;
                context.Items["TenantDomain"] = tenant.Domain;
                context.Items["ResolvedDomain"] = targetDomain; // Debug için

                _logger.LogInformation("Tenant resolved: {TenantId} - {TenantName} for domain {Domain}",
                        tenant.Id, tenant.Name, targetDomain);
                await _next(context);
            }

            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal server error");
            }
        }
    }
}

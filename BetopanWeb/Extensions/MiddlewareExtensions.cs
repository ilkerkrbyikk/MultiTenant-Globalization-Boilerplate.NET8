using BetopanWeb.Middleware;

namespace BetopanWeb.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}

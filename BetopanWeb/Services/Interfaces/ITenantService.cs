using BetopanWeb.Models.Domain;

namespace BetopanWeb.Services.Interfaces
{
    public interface ITenantService
    {
        long GetCurrentTenantId();
        string GetCurrentTenantDomain();
        Tenant? GetCurrentTenant();
        Task<Tenant?> GetTenantByDomainAsync(string domain);
        bool IsValidTenant(int tenantId);
    }
}

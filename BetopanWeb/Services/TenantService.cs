using BetopanWeb.Data.Context;
using BetopanWeb.Models.Domain;
using BetopanWeb.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BetopanWeb.Services
{
    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;
        private readonly BetopanDbContext _context;
        private readonly ILogger<TenantService> _logger;
   

        public TenantService(
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache cache,
            BetopanDbContext context,
            ILogger<TenantService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
            _context = context;
            _logger = logger;
        }

        public long GetCurrentTenantId()
        {
            var tenant = GetCurrentTenant();
            if (tenant == null)
            {
                throw new InvalidOperationException("Current tenant not found");
            }

            return tenant.Id;
        }

        public string GetCurrentTenantDomain()
        {
            var tenant = GetCurrentTenant();
            if (tenant.Domain == null)
            {
                throw new InvalidOperationException("Current tenant domain not found");
            }

            return tenant?.Domain;
        }

        public Tenant? GetCurrentTenant()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return null;
            }

            // Cache'den tenant bilgisini al
            if (httpContext.Items.TryGetValue("CurrentTenant", out var cachedTenant))
            {
                return cachedTenant as Tenant;
            }

            return null;
        }
        public async Task<Tenant?> GetTenantByDomainAsync(string domain)
        {
            // Bu domain için cache key oluşturuluyor
            var cacheKey = $"tenant_domain_{domain}";

            // Cache'de varsa oradan al yoksa veritabanından getir ve cache'e ekle
            return await _cache.GetOrCreateAsync(cacheKey, async cacheEntry =>
            {

                // Veritabanından domain'e göre tenant bilgisi çekilir
                var tenantFromDb = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Domain == domain && t.IsActive);

                // Veritabanından gelen tenant bilgisi döndürülüyor (ve cache'e ekleniyor)
                return tenantFromDb;
            });
        }

        public bool IsValidTenant(int tenantId)
        {
            var tenant = GetCurrentTenant();
            return tenant != null && tenant.Id == tenantId;
        }

    }
}


using System.Text.RegularExpressions;
using BetopanWeb.Data.Context;
using BetopanWeb.Models.Domain;
using BetopanWeb.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BetopanWeb.Localization
{
    public class LocalizationService : ILocalizationService
    {

        private readonly IMemoryCache _cache;
        private readonly BetopanDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LocalizationService> _logger;

        public LocalizationService(
        IMemoryCache cache,
        BetopanDbContext context,
        ITenantService tenantService,
        IHttpContextAccessor httpContextAccessor
        )
        {
            _cache = cache;
            _context = context;
            _tenantService = tenantService;
            _httpContextAccessor = httpContextAccessor;
           
        }

        public string GetText(string key, string? culture = null)
        {
            // Eğer culture verilmediyse default lang alıyoruz
            if (culture == null)
            {
                culture = GetCurrentCulture();
            }

            var cacheKey = "resources_" + culture;

            var resources = _cache.GetOrCreate(cacheKey, entry =>
            {

                
                return LoadResourcesFromDatabase(culture);
            });

            return resources?.GetValueOrDefault(key, $"[{key}]") ?? $"[{key}]";
        }

        public async Task<Dictionary<string,string>> GetAllResourcesAsync(string culture)
        {
            var cacheKey = "resources_" + culture;

            return await _cache .GetOrCreateAsync(cacheKey, async entry =>
            {
                return await LoadResourcesFromDatabaseAsync(culture);
            }) ?? new Dictionary<string, string>();
        }

        public async Task LoadResourcesAsync(string culture)
        {
            var cacheKey = "resources_" + culture;
            var resources = await LoadResourcesFromDatabaseAsync(culture);

            _cache.Set(cacheKey, resources);

        }

        public async Task<bool> AddOrUpdateResourceAsync(string key, string culture, string value)
        {
            try
            {
                var existing = await _context.LocalizationResources
                    .FirstOrDefaultAsync(r => r.ResourceKey == key && r.LanguageCode == culture);

                if (existing != null)
                {
                    existing.Value = value;
                }
                else
                {
                    _context.LocalizationResources.Add(new LocalizationResource
                    {
                        ResourceKey = key,
                        LanguageCode = culture,
                        Value = value
                    });
                }

                await _context.SaveChangesAsync();

                ClearCache(culture);

                return true;
            }
            catch (Exception ex)
            {
                //TODO: EXCEOTİON YAZ.
                return false;
            }
        }

        public void ClearCache(string? culture = null)
        {
            var cacheKey = "resources_" + culture;

            if (culture != null)
            {
                _cache.Remove(cacheKey);
            }
            else
            {
                // Tüm culture cache'lerini temizle 
                var supportedCultures = new[] { "tr", "en", "de", "fr", "ar", "ru" };
                foreach (var cult in supportedCultures)
                {
                    _cache.Remove($"resources_{cult}");
                }
            }
        }

        public string GetCurrentCulture()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Önce Items'tan bak (aynı request içinde)
                if (httpContext.Items.TryGetValue("CurrentCulture", out var itemCulture))
                {
                    return itemCulture?.ToString() ?? GetDefaultCultureForTenant();
                }

                // Sonra Session'dan bak
                var sessionCulture = httpContext.Session.GetString("CurrentCulture");
                if (!string.IsNullOrEmpty(sessionCulture))
                {
                    return sessionCulture;
                }
            }

            return GetDefaultCultureForTenant();
        }

        public void SetCulture(string culture)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Session'a kaydet
                httpContext.Session.SetString("CurrentCulture", culture);
                // Items'a da kaydet (aynı request içinde hemen çalışsın)
                httpContext.Items["CurrentCulture"] = culture;
            }
        }

        private string GetDefaultCultureForTenant()
        {
            try
            {
                var tenant = _tenantService.GetCurrentTenant();
                if (tenant != null && tenant.Domain.Contains("com.tr"))
                {
                    return "tr";
                }

                return "en";
            }
            catch
            {
                return "en";
            }
        }


        private Dictionary<string, string> LoadResourcesFromDatabase(string culture)
        {
            return _context.LocalizationResources.Where(r=> r.LanguageCode == culture)
                .ToDictionary(r=> r.ResourceKey,r=> r.Value);
        }

        private async Task<Dictionary<string, string>> LoadResourcesFromDatabaseAsync(string culture)
        {
            return await _context.LocalizationResources
                .Where(r => r.LanguageCode == culture)
                .ToDictionaryAsync(r => r.ResourceKey, r => r.Value);
        }
    }
}

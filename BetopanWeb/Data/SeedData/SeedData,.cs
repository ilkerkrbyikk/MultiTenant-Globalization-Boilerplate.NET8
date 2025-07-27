using BetopanWeb.Data.Context;
using BetopanWeb.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BetopanWeb.Data.SeedData
{
    // Migrations/SeedData.cs (manual olarak çalıştıracağız)
    public static class SeedData
    {
        public static async Task SeedAsync(BetopanDbContext context)
        {
            // Tenant'ları ekle
            if (!await context.Tenants.AnyAsync())
            {
                var tenants = new[]
                {
                new Tenant { Name = "Betopan Turkey", Domain = "betopan.com.tr", IsActive = true },
                new Tenant { Name = "Betopan Global", Domain = "betopan.net", IsActive = true }
            };

                context.Tenants.AddRange(tenants);
                await context.SaveChangesAsync();
            }

            // Sample localization resources
            if (!await context.LocalizationResources.AnyAsync())
            {
                var resources = new[]
                {
                new LocalizationResource { ResourceKey = "Welcome", LanguageCode = "tr", Value = "Hoş Geldiniz" },
                new LocalizationResource { ResourceKey = "Welcome", LanguageCode = "en", Value = "Welcome" },
                new LocalizationResource { ResourceKey = "HomePage.Title", LanguageCode = "tr", Value = "Ana Sayfa" },
                new LocalizationResource { ResourceKey = "HomePage.Title", LanguageCode = "en", Value = "Home Page" },
                new LocalizationResource { ResourceKey = "About", LanguageCode = "tr", Value = "Hakkımızda" },
                new LocalizationResource { ResourceKey = "About", LanguageCode = "en", Value = "About Us" }
            };

                context.LocalizationResources.AddRange(resources);
                await context.SaveChangesAsync();
            }
        }
    }
   
    
}

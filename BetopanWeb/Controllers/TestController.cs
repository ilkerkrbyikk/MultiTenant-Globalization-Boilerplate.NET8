using BetopanWeb.Data.Context;
using BetopanWeb.Localization;
using BetopanWeb.Models.Domain;
using BetopanWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BetopanWeb.Controllers
{
    [Route("test")]
    public class TestController : Controller
    {
        private readonly ITenantService _tenantService;
        private readonly ILocalizationService _localizationService;
        private readonly BetopanDbContext _context;

        public TestController(
            ITenantService tenantService,
            ILocalizationService localizationService,
            BetopanDbContext context)
        {
            _tenantService = tenantService;
            _localizationService = localizationService;
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var tenant = _tenantService.GetCurrentTenant();
            var culture = _localizationService.GetCurrentCulture();

            ViewBag.TenantName = tenant?.Name ?? "No Tenant";
            ViewBag.Culture = culture;
            ViewBag.Welcome = _localizationService.GetText("Welcome");
            ViewBag.WelcomeMessage = _localizationService.GetText("Welcome.Message");

            return View();
        }

        [HttpGet("debug")]
        public IActionResult Debug()
        {
            var host = HttpContext.Request.Host.Host;
            var port = HttpContext.Request.Host.Port;

            string targetDomain = host;
            if (host == "localhost")
            {
                targetDomain = port switch
                {
                    5000 => "betopan.com.tr",
                    5001 => "betopan.net",
                    _ => "betopan.com.tr"
                };
            }

            var result = new
            {
                OriginalHost = host,
                Port = port,
                TargetDomain = targetDomain,
                TenantFromItems = HttpContext.Items.TryGetValue("CurrentTenant", out var tenant) ? tenant : null
            };

            return Json(result);
        }

        [HttpGet("seed")]
        public async Task<IActionResult> SeedData()
        {
            if (!await _context.Tenants.AnyAsync())
            {
                _context.Tenants.AddRange(
                    new Tenant { Name = "Betopan Turkey", Domain = "betopan.com.tr", IsActive = true },
                    new Tenant { Name = "Betopan Global", Domain = "betopan.net", IsActive = true }
                );
                await _context.SaveChangesAsync();
            }

            if (!await _context.LocalizationResources.AnyAsync())
            {
                _context.LocalizationResources.AddRange(
                    new LocalizationResource { ResourceKey = "Welcome", LanguageCode = "tr", Value = "Hoş Geldiniz" },
                    new LocalizationResource { ResourceKey = "Welcome", LanguageCode = "en", Value = "Welcome" }
                );
                await _context.SaveChangesAsync();
            }

            return Json(new { Success = true });
        }

        [HttpPost("culture/{culture}")]
        public IActionResult SetCulture(string culture)
        {
            _localizationService.SetCulture(culture);
            return Json(new { Culture = culture });
        }


        [HttpGet("culture-debug")]
        public IActionResult CultureDebug()
        {
            var culture = _localizationService.GetCurrentCulture();
            var welcome = _localizationService.GetText("Welcome");
            var welcomeMessage = _localizationService.GetText("Welcome.Message");

            return Json(new
            {
                CurrentCulture = culture,
                Welcome = welcome,
                WelcomeMessage = welcomeMessage,
                SessionCulture = HttpContext.Items.TryGetValue("CurrentCulture", out var sessionCulture) ? sessionCulture : null
            });
        }

        [HttpGet("check-resources")]
        public async Task<IActionResult> CheckResources()
        {
            var resources = await _context.LocalizationResources
                .Where(r => r.ResourceKey.StartsWith("Welcome"))
                .ToListAsync();

            return Json(resources.Select(r => new { r.ResourceKey, r.LanguageCode, r.Value }));
        }
    }
}
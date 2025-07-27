namespace BetopanWeb.Enums
{
    public static class SupportedLanguages
    {
        public static readonly Dictionary<int, string[]> TenantLanguages = new()
    {
        { 1, new[] { "tr", "en" } }, // betopan.com.tr
        { 2, new[] { "en", "de", "fr", "ar", "ru" } } // betopan.net
    };

    }
}

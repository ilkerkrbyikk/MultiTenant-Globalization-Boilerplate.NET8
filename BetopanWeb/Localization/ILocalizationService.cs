namespace BetopanWeb.Localization
{
    public interface ILocalizationService
    {
        string GetText(string key, string? culture = null);
        Task<Dictionary<string, string>> GetAllResourcesAsync(string culture);
        Task LoadResourcesAsync(string culture);
        Task<bool> AddOrUpdateResourceAsync(string key, string culture, string value);
        void ClearCache(string? culture = null);
        string GetCurrentCulture();
        void SetCulture(string culture);
    }
}

using BetopanWeb.Models.Base;

namespace BetopanWeb.Models.Domain
{
    public class LocalizationResource : BaseEntity
    {
        public string ResourceKey { get; set; }
        public string LanguageCode { get; set; }
        public string Value { get; set; }
    }
}

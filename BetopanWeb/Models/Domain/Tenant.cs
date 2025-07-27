using BetopanWeb.Models.Base;

namespace BetopanWeb.Models.Domain
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } 
        public string Domain { get; set; } 
        public bool IsActive { get; set; }
    }
}

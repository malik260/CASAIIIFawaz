using Core.DTOs;
using Core.Enum;

namespace Core.ViewModels
{
    public class AffiliateVM : AffiliateRegistrationDto
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public RegistrationStatus Status { get; set; }
    }
}

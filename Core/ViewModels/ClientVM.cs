using Core.DTOs;
using Core.Enum;

namespace Core.ViewModels
{
    public class ClientVM : ClientRegistrationDto
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public RegistrationStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}

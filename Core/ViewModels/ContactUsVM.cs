using Core.DTOs;
using Core.Enum;

namespace Core.ViewModels
{
    public class ContactUsVM : ContactFormDto
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ContactFormStatus Status { get; set; }
    }
}

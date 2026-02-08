using Core.Enum;
using Core.Model;

namespace Core.Models
{
    public class Contact : BaseModel
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Request { get; set; }
        public string? Budget { get; set; }
        public string? Message { get; set; }
        public ContactFormStatus Status { get; set; } = ContactFormStatus.Pending;
    }
}

using Core.Enum;

namespace Core.Model
{
    public class Vendor : BaseModel
    {
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CACNumber { get; set; }
        public string TIN { get; set; }
        public string? BusinessCategory { get; set; }
        public string? BusinessAddress { get; set; }
        public string? FilePath { get; set; }
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Initiated;
    }
}

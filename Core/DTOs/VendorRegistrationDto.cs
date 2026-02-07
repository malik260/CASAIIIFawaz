using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class VendorRegistrationDto
    {
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CACNumber { get; set; }
        public string TIN { get; set; }
        public string BusinessCategory { get; set; }
        public string BusinessAddress { get; set; }

        public IFormFile File { get; set; }   // 👈 single
    }


}

using Core.Enum;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    public class ClientRegistrationDto
    {
        [Required]
        public ClientType ClientType { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public AddressType AddressType { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string Country { get; set; } = "Nigeria";

        [Required]
        public string State { get; set; }

        [Required]
        public string City { get; set; }

        public DateTime? DateOfBirthOrIncorporation { get; set; }
        public MeansOfID? MeansOfID { get; set; }
        public string? IDFilePath { get; set; }
        public IFormFile? IDFile { get; set; }

        // Next of Kin
        [Required]
        public string NextOfKinFirstName { get; set; }

        [Required]
        public string NextOfKinLastName { get; set; }

        public string? NextOfKinOtherNames { get; set; }

        [Required]
        public string NextOfKinPhoneNumber { get; set; }

        public string? NextOfKinEmail { get; set; }

        [Required]
        public NextOfKinRelationship NextOfKinRelationship { get; set; }

        [Required]
        public string NextOfKinResidentialAddress { get; set; }

        public string? NextOfKinOfficeAddress { get; set; }
    }

    public class ClientUpdateDto : ClientRegistrationDto
    {
        public string Id { get; set; }
        public RegistrationStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}

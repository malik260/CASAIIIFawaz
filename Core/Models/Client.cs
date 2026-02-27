using Core.Enum;

namespace Core.Model
{
    public class Client : BaseModel
    {
        public ClientType ClientType { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public AddressType AddressType { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string Country { get; set; } = "Nigeria";
        public string State { get; set; }
        public string City { get; set; }

        public DateTime? DateOfBirthOrIncorporation { get; set; }
        public MeansOfID? MeansOfID { get; set; }
        public string? IDFilePath { get; set; }

        // Next of Kin
        public string NextOfKinFirstName { get; set; }
        public string NextOfKinLastName { get; set; }
        public string? NextOfKinOtherNames { get; set; }
        public string NextOfKinPhoneNumber { get; set; }
        public string? NextOfKinEmail { get; set; }
        public NextOfKinRelationship NextOfKinRelationship { get; set; }
        public string NextOfKinResidentialAddress { get; set; }
        public string? NextOfKinOfficeAddress { get; set; }

        public RegistrationStatus Status { get; set; } = RegistrationStatus.Initiated;
        public string? Notes { get; set; }
    }
}

namespace Core.Enum
{
    /// <summary>Means of identification. Values 1–5 for Individual/Joint; 10+ for Corporate.</summary>
    public enum MeansOfID
    {
        // Individual & Joint (personal ID)
        NationalIDCard = 1,
        DriversLicense = 2,
        InternationalPassport = 3,
        VotersCard = 4,
        NINSlip = 5,

        // Corporate (business ID)
        CertificateOfIncorporation = 10,
        CACRegistration = 11,
        TaxIdentificationNumber = 12,
        BusinessRegistrationCertificate = 13
    }
}

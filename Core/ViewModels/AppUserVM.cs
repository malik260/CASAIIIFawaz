namespace Core.ViewModels
{
    public class AppUserVM
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public DateTime DateRegistered { get; set; }
        public bool IsDeactivated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}

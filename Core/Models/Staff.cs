using Core.Enum;

namespace Core.Model
{
    public class Staff : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? MemberInfo { get; set; }
        public TeamMemeberCategory Category { get; set; }
    }
}

using Core.Enum;

namespace Core.DTOs
{
    public class TeamMemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string ImageUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string MemberInfo { get; set; }
        public TeamMemeberCategory Category { get; set; }
    }
}

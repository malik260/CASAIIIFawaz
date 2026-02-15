namespace Core.DTOs
{
    public class UserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Admin or Staff
    }

    public class UserUpdateDto : UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? NewPassword { get; set; } // Optional - only if changing password
    }
}


namespace PersonalBudgetTrackerAPI.Models.Auth
{
    public class RegisterDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
    }

    public class RoleDto
    {
        public string RoleName { get; set; } = null!;
    }

    public class AssignRoleDto
    {
        public string Username { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }
}

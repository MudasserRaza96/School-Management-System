using System.Collections.Generic;

namespace SchoolApp.Models.DataModels.SecurityModels
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string>? Role { get; set; }
        public string? Password { get; set; }
    }
}

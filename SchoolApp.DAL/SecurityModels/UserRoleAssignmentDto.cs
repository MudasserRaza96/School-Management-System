using System.Collections.Generic;

namespace SchoolApp.Models.DataModels.SecurityModels
{
    public class UserRoleAssignmentDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}

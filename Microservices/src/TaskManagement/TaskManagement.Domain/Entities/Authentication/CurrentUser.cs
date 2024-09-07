using TaskManagement.Domain.Entities;
using System;

namespace TaskManagement.Domain.Entities.Authentication
{
    public class CurrentUser
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public RoleTypeEnum? Role { get; set; }
    }
}

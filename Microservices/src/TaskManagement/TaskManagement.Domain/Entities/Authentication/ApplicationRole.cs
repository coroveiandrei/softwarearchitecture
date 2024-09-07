
using Microsoft.AspNetCore.Identity;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Entities.Authentication
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }
        public RoleTypeEnum RoleType { get; set; }
    }
}

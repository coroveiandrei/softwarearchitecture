using InventoryService.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Common.Identity
{
    public class CurrentUser
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public Role? Role { get; set; }
    }
}

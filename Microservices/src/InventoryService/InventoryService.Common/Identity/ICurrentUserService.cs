using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Common.Identity
{
    public interface ICurrentUserService
    {
        CurrentUser GetCurrentUser();
    }
}

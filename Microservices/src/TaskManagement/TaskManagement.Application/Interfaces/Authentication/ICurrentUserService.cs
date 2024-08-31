using Quotation.Domain.Entities.Authentication;

namespace TaskManagement.Application.Interfaces.Authentication
{
    public interface ICurrentUserService
    {
        CurrentUser GetCurrentUser();
    }
}

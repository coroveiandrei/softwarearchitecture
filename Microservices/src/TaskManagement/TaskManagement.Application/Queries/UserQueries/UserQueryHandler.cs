using MediatR;
using TaskManagement.Application.Interfaces.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManagement.Application.Queries.UserQueries
{
    public class UserQueryHandler: IRequestHandler<GetUserDetailsQuery, GetUserDetailsQueryResponse>
    {
        private readonly IUserService _userService;
        public UserQueryHandler(IUserService userService)
        {
            this._userService = userService;
        }

        public async Task<GetUserDetailsQueryResponse> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
        {
            var user = _userService.GetUsersById(request.UserId);
            return new GetUserDetailsQueryResponse {  User = user };
        }
    }
}

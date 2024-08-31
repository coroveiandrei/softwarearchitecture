using TaskManagement.Application.Common;
using TaskManagement.Application.Models.AuthenticationModels;
using System;

namespace TaskManagement.Application.Queries.UserQueries
{
    public class GetUserDetailsQuery : BaseRequest<GetUserDetailsQueryResponse>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserDetailsQueryResponse
    {
        public UserModel User { get; set; }
    }
}

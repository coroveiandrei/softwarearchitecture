using MediatR;
using TaskManagement.Application.Common;
using System.Threading;
using System.Threading.Tasks;
using TaskManagement.Application.Interfaces.Authentication;

namespace TaskManagement.Infrastructure.RequestBehaviours
{
    public class CurrentUserBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;

        public CurrentUserBehavior(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var castedRequest = (request as BaseRequest<TResponse>);
            castedRequest.User = _currentUserService.GetCurrentUser();
            return next();
        }
    }
}

using MediatR;
using Newtonsoft.Json;
using TaskManagement.Domain.Entities.Authentication;

namespace TaskManagement.Application.Common
{
    public class BaseRequest<T> : IRequest<T>
    {
    }
}

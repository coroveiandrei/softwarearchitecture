using MediatR;
using Newtonsoft.Json;
using Quotation.Domain.Entities.Authentication;

namespace TaskManagement.Application.Common
{
    public class BaseRequest<T> : IRequest<T>
    {
        [JsonIgnore]
        public CurrentUser User;
    }
}

using MediatR;
using Newtonsoft.Json;
using InventoryService.Common.Identity;

namespace InventoryService.Application.Common
{
    public class BaseRequest<T> : IRequest<T>
    {
    }
}

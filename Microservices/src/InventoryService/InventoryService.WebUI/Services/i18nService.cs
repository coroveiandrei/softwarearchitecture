using Microsoft.Extensions.Localization;
using InventoryService.Common;
using InventoryService.Common.Localization;
using InventoryService.WebUI;

namespace InventoryService.Infrastructure.Services
{
    [MapServiceDependency(nameof(i18nService))]
    public class i18nService : Ii18nService
    {
        private readonly IStringLocalizer _localizer;
        public i18nService(IStringLocalizer<Resources> localizer)
        {
            this._localizer = localizer;

        }

        public string GetMessage(string key)
        {
            var message = _localizer.GetString(key);
            return message.Value;
        }
    }
}


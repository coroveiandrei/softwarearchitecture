using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Common.Localization
{
    public interface Ii18nService
    {
        string GetMessage(string key);
    }
}

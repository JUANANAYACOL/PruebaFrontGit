using System.Globalization;
using Telerik.Blazor.Services;

namespace Control.Endeavour.FrontEnd
{
    public class TelerikResxLocalizer : ITelerikStringLocalizer
    {
        public string this[string name]
        {
            get
            {
                string value = GetStringFromResource(name);
                return value;
            }
        }

        public string GetStringFromResource(string key)
        {
            return Resources.Translation.ResourceManager.GetString(key, Resources.Translation.Culture);
        }
    }
}

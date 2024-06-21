using Telerik.ReportViewer.BlazorNative.Services;

namespace Control.Endeavour.FrontEnd
{
    
    public class TelerikReportStringLocalizer : ITelerikReportingStringLocalizer
    {
        public string this[string name]
        {
            get
            {
                return this.GetStringFromResource(name);
            }
        }

        public string GetStringFromResource(string key)
        {
            return Resources.Translation.ResourceManager.GetString(key, Resources.Translation.Culture);
        }
    }
}

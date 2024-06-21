

namespace Control.Endeavour.FrontEnd.Models.Enums;
public class ControlEnumAttribute: Attribute
{
    public string CoreValue { get; set; }
    public string DisplayValue { get; set; }
    public string Prefix { get; set; }

    internal ControlEnumAttribute(string corevalue, string displayvalue, string prefix = null)
    {
        CoreValue = corevalue;
        DisplayValue = displayvalue;
        Prefix = prefix;
    }

    public static explicit operator short(ControlEnumAttribute v)
    {
        throw new NotImplementedException();
    }
}

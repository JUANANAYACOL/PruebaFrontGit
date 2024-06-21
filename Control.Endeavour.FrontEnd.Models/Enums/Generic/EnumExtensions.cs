

namespace Control.Endeavour.FrontEnd.Models.Enums.Generic;
public static class EnumExtensions
{
    public static TAttribute GetAttribute<TAttribute>(this Enum value)
       where TAttribute : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);

        return type.GetField(name)
            .GetCustomAttributes(false)
            .OfType<TAttribute>()
            .SingleOrDefault();
    }


    public static string GetCoreValue(this Enum value)
    {
        return value?.GetAttribute<ControlEnumAttribute>()?.CoreValue ?? Convert.ToString(value);
    }

    public static string GetPrefixValue(this Enum value)
    {
        return value?.GetAttribute<ControlEnumAttribute>()?.Prefix;
    }

    public static string GetDisplayValue(this Enum value)
    {
        return value?.GetAttribute<ControlEnumAttribute>()?.DisplayValue;
    }
}

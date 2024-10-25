using System.ComponentModel;
using System.Reflection;

namespace FarmManagerAPI.Models.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            if (field == null)
            {
                return string.Empty;
            }

            DescriptionAttribute attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute?.Description ?? string.Empty;
        }
    }
}

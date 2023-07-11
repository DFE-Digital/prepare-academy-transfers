using System.ComponentModel;
using System.Reflection;

namespace Dfe.PrepareTransfers.Data.TRAMS.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static string ToDescription<T>(this T source)
        {
            if (source == null)
            {
                return string.Empty;
            }

            FieldInfo fi = source.GetType().GetField(source.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            else
            {
                return source.ToString();
            }
        }
    }
}

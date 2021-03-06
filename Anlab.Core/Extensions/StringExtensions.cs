using System.Globalization;

namespace Anlab.Core.Extensions
{
    public static class StringExtensions
    {
        public static string SafeToUpper(this string value)
        {
            if (value == null)
            {
                return null;
            }

            return value.ToUpper(CultureInfo.InvariantCulture);
        }
    }
}

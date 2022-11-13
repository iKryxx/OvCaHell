using System.Text.RegularExpressions;

namespace BasicExtensions
{
    public static class BasicExtensions
    {
        public static bool IsWithin(this int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }
        public static string DeleteNums(this string str)
        {
            return Regex.Replace(str, "[0-9]", "");
        }
        public static string toFormat(this string str)
        {
            return Regex.Replace(str, "[0-9]", "").Replace("_", "");
        }
    }
}

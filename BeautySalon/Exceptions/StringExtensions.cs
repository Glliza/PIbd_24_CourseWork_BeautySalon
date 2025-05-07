
namespace BeautySalon.Exceptions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsGuid(this string str)
        {
            return Guid.TryParse(str, out var _);
        }
    }
}

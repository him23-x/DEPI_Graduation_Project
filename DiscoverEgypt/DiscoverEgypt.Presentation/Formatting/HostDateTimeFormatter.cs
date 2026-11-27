using System.Globalization;

namespace DiscoverEgypt.Presentation.Formatting
{
    public static class HostDateTimeFormatter
    {
        private static readonly TimeZoneInfo CairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "Egypt Standard Time" : "Africa/Cairo");
        private static readonly CultureInfo EnglishCulture = CultureInfo.GetCultureInfo("en-US");

        public static string Cairo(DateTime value, string format)
        {
            var utcValue = value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };

            return TimeZoneInfo.ConvertTimeFromUtc(utcValue, CairoTimeZone).ToString(format, EnglishCulture);
        }
    }
}

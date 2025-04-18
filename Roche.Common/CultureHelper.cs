using System.Globalization;

namespace Roche.Common;

public static class CultureHelper
{
    static CultureInfo Culture { get; set; }
    /// <summary>
    /// Defines the default culture like date and time used throughout reMIX.
    /// All format specifiers: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#table-of-format-specifiers
    /// </summary>
    /// <returns>Configured CultureInfo</returns>
    public static CultureInfo GetCulture()
    {
        if (Culture == null)
        {
            Culture = new CultureInfo("en-GB");
            Culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd"; // date.ToString("d")
            Culture.DateTimeFormat.LongDatePattern = "dd MMM yyyy"; // date.ToString("D")
            Culture.DateTimeFormat.ShortTimePattern = "HH:mm"; // date.ToString("t")
            Culture.DateTimeFormat.LongTimePattern = "HH:mm:ss"; // date.ToString("T")
            // short date time = date.ToString("g")
            // long date time = date.ToString("G")
            Culture.DateTimeFormat.YearMonthPattern = "MM-yyyy"; // date.ToString("y")
            Culture.DateTimeFormat.FullDateTimePattern = "yyyy-MM-dd HH:mm:ss"; // date.ToString("F")

            Culture.NumberFormat.NumberGroupSeparator = "";
            Culture.NumberFormat.PercentGroupSeparator = " ";
            Culture.NumberFormat.CurrencyGroupSeparator = " ";
        }

        return Culture;
    }
}

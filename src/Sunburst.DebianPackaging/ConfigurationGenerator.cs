using System;
using System.Collections.Generic;
using System.Text;
using Sunburst.Json;

namespace Sunburst.DebianPackaging
{
    internal sealed class ConfigurationGenerator
    {
        private static string FormatChangelogDate(DateTime date)
        {
            DateTime utcDate = date.ToUniversalTime();
            StringBuilder formattedDate = new StringBuilder();

            // strftime format: %a, %d %b %Y %H:%M:%S %z
            switch (utcDate.DayOfWeek)
            {
                case DayOfWeek.Sunday: formattedDate.Append("Sun"); break;
                case DayOfWeek.Monday: formattedDate.Append("Mon"); break;
                case DayOfWeek.Tuesday: formattedDate.Append("Tue"); break;
                case DayOfWeek.Wednesday: formattedDate.Append("Wed"); break;
                case DayOfWeek.Thursday: formattedDate.Append("Thu"); break;
                case DayOfWeek.Friday: formattedDate.Append("Fri"); break;
                case DayOfWeek.Saturday: formattedDate.Append("Sat"); break;
            }
            formattedDate.Append(" ");

            formattedDate.Append(utcDate.Day.ToString("D2"));
            formattedDate.Append(" ");

            switch (utcDate.Month)
            {
                case 1: formattedDate.Append("Jan"); break;
                case 2: formattedDate.Append("Feb"); break;
                case 3: formattedDate.Append("Mar");break;
                case 4: formattedDate.Append("Apr");break;
                case 5: formattedDate.Append("May");break;
                case 6: formattedDate.Append("Jun");break;
                case 7: formattedDate.Append("Jul");break;
                case 8: formattedDate.Append("Aug");break;
                case 9: formattedDate.Append("Sep");break;
                case 10: formattedDate.Append("Oct");break;
                case 11: formattedDate.Append("Nov");break;
                case 12: formattedDate.Append("Dec");break;
                default: throw new ArgumentException("Unrecognized month: this can't happen", nameof(date));
            }

            formattedDate.Append(" ");
            formattedDate.Append(utcDate.Year);
            formattedDate.Append(" ");

            formattedDate.Append(utcDate.Hour.ToString("D2"));
            formattedDate.Append(":");
            formattedDate.Append(utcDate.Minute.ToString("D2"));
            formattedDate.Append(":");
            formattedDate.Append(utcDate.Millisecond.ToString("D2"));
            formattedDate.Append(" +0000");

            return formattedDate.ToString();
        }
    }
}

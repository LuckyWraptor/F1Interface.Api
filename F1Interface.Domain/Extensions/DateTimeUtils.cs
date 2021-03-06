using System;

namespace F1Interface.Domain.Extensions
{
    public static class DateTimeUtils
    {
        public static DateTime UnixToDateTime(ulong unixEpochTime)
        {
            DateTime dateTime = new DateTime(1970, 1,1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddMilliseconds(unixEpochTime);
        }
    }
}
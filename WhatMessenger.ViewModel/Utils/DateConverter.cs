using System;
namespace WhatMessenger.ViewModel.Utils
{
    public class DateConverter
    {
        public static DateTime GetDateFromString(string dateString)
        {
            DateTime enteredDate = DateTime.Parse(dateString);
            return enteredDate.ToLocalTime();
        }
        public static int GetUnixTimeSpanFromDate(DateTime time)
        {
            Int32 unixTimestamp = (Int32)(time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }
        public static DateTime GetDateTimeFromUnixTimeStamp(int UnixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(UnixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}

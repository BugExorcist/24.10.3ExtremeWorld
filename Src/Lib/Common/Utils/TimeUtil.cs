using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utils
{
    public class TimeUtil
    {
        public static int timestamp
        {
            get { return GetTimestamp(DateTime.Now); }
        }

        public static DateTime GetTime(long timeStamp)
        {
            DateTime dateTimeSatrt = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = timeStamp * 10000000;
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeSatrt.Add(toNow);
        }

        public static int GetTimestamp(DateTime time)
        {
            DateTime startTiem = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(time - startTiem).TotalSeconds;
        }
    }
}
    

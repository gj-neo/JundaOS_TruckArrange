using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Tools
    {
        public static DateTime GetDatetimeNow()
        {
            DateTime nowTime = DateTime.Now;
            return new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, nowTime.Second);
        }
    }
}

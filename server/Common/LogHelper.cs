using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public  class LogHelper
    {
        /// <summary>
        /// 输出日志到debugView
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteLog(string msg)
        {
            System.Diagnostics.Trace.WriteLine($"[JundaOS_WebApi]{msg}");
        }
    }
}

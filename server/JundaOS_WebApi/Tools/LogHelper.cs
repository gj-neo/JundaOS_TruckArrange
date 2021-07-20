using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JundaOS_WebApi.Tools
{
    public class LogHelper
    {
        static ILog log = null;

        static LogHelper()
        {
            if (log == null)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "log4net.Config";
                XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
                log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }
        /// <summary>
        /// added by gaojiang at 2021.4.22 :重置Logger名称，客户端调用时方便修改
        /// </summary>
        /// <param name="loggerName"></param>
        public static void ResetLogger(string loggerName)
        {
            log = LogManager.GetLogger(loggerName);
        }
        public static void Debug(object o)
        {
            log.Debug(o);
        }

        public static void Debug(object o, Exception ex)
        {
            log.Debug(o, ex);
        }

        public static void Info(object o)
        {
            log.Info(o);
        }

        public static void Info(object o, Exception ex)
        {
            log.Info(o, ex);
        }

        public static void Warn(object o)
        {
            log.Warn(o);
        }

        public static void Warn(object o, Exception ex)
        {
            log.Warn(o, ex);
        }

        public static void Error(object o)
        {
            log.Error(o);
        }

        public static void Error(object o, Exception ex)
        {
            log.Error(o, ex);
        }

        public static void Fatal(object o)
        {
            log.Fatal(o);
        }

        public static void Fatal(object o, Exception ex)
        {
            log.Fatal(o, ex);
        }
    }
}
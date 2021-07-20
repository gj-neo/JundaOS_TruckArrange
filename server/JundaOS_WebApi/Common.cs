using JundaOS_WebApi.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace JundaOS_WebApi
{
    
    public static  class Common
    {
        public static Dictionary<string, UserInfoCache> UserInfoDict;
        static Common()
        {
            UserInfoDict = new Dictionary<string, UserInfoCache>();
        }
        public static DateTime GetDatetimeNow()
        {
            DateTime nowTime = DateTime.Now;
            return new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, nowTime.Second);
        }
        public static JundaOSEntities CreateDbInstance()
        {
            JundaOSEntities db = CallContext.GetData("JundaOSEntities") as JundaOSEntities;
            if (db == null)
            {
                db = new JundaOSEntities();
                CallContext.SetData("JundaOSEntities", db);
            }
            return db;
        }
        
    }
   
}
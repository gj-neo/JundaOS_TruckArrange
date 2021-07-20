using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DbModel
{
    public class ModelHelper
    {
        private static ModelHelper _instance;
        public static  ModelHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    return new ModelHelper();
                }
                return _instance;
            }
        }
        public Junda_TruckArrangeEntities TruckDB
        {
            get
            {
                Junda_TruckArrangeEntities db = null;
                if (HttpContext.Current.Items["tmpDb"] == null)
                {
                    db = new Junda_TruckArrangeEntities();
                    HttpContext.Current.Items["tmpDb"] = db;
                }
                else
                {
                    db = HttpContext.Current.Items["tmpDb"] as Junda_TruckArrangeEntities;
                }
                return db;
            }
        }
    }
}

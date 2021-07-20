using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace JundaOS_WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            #region  跨域

           
            var allowedMethods = ConfigurationManager.AppSettings["cors:allowedMethods"];
            var allowedOrigin = ConfigurationManager.AppSettings["cors:allowedOrigin"];
            var allowedHeaders = ConfigurationManager.AppSettings["cors:allowedHeaders"];
            var myCors = new EnableCorsAttribute("*", "*", "*")
            {
                SupportsCredentials = true
            };
            config.EnableCors(myCors);
            #endregion
            // Web API 路由
            config.MapHttpAttributeRoutes();
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

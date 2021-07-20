using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ApiTool
    {
        public HttpResponseMessage MsgFormat(ResponseCode _code, string msg, object _data)
        {
            var jsonModel = new
            {
                code = (int)_code,
                message = _code.ToString(),
                data = _data,
            };
            string json = JsonConvert.SerializeObject(jsonModel);
            var response = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            return response;
        }
        /// <summary>
        /// 匿名类转换为json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Object2Json(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
    
}

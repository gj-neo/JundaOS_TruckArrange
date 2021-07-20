using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    //Json消息类
    public class JsonModel
    {
        int code;
        string message;
        string data;
        public JsonModel(ResponseCode _code, string _data)
        {
            code = (int)_code;
            message = _code.ToString();
            data = _data;
        }
    }
}

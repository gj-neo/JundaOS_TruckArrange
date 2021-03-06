using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace JundaOS_WebApi.Tools
{
    /// <summary>
    /// 自定义错误码
    /// </summary>
    public enum ResponseCode
    {
        成功=1000,
        未处理=1001,
        //通用错误 2XXX
        服务端内部异常 = 2001,
        请求参数错误=2002,
        //用户校验错误 3XXX
        用户不存在 = 3001 ,
        密码错误 = 3002,
        原密码错误 = 3003,
        该用户已登录 =3004,
        用户未登录=3005,
        已有其他操作员登录=3006,
        Cookie校验失败=3007,
        Header中的Token为空=3008,
        Header中缺少Token = 3009,
        Header中的Token无法匹配已登录用户 = 3010,
        Header中的Group为空 = 3011,
        Header中的Group无法匹配 = 3012,
        Header中不存在Group = 3013,
        当前用户没有操作权限 = 3014,
        无法识别当前用户所在组 = 3015,
        //Truck  4XXX
        车辆编号已存在 =4001,
        车辆不存在=4002,
        车辆没有该通行证=4003,
        恢复运营的时间必须晚于停运或请假的时间=4004,
        当前未找到尚处于停运状态的车辆=4005,
        停运的时间必须晚于最后一次恢复运营时间 = 4006,
        有车辆仍处于停运状态_无法重复设置停运=4007,
        车辆编号有重复=4008,
        当前没有待补偿车辆=4108,
        //该牌照已存在=4002,
        //该司机名称已存在=4003,
        //Permit 5XXX
        通行证已存在 =5001,
        通行证不存在=5002,
        //site  6xxx
        工地已存在=6001,
        工地查询条件中的字段错误 = 6002,
        工地不存在 = 6003,
        工地已绑定该通行证=6004,
        消纳场已存在 = 6005,
        消纳场不存在 = 6006,
        消纳场已绑定该通行证 = 6007,
        消纳场未绑定该通行证 = 6008,
        消纳场查询条件中的字段错误 = 6009,
        工地名称不能为空 = 6010,
        工地类型超出有效值 = 6011,
        未获取到任何工地信息 = 6012,
        工地列表中包含有重复项 = 6013,
        批量添加请求中的工地参数为空 = 6014,
        工地绑定的通行证无效 = 6015,
        //calculate  7xxx
        车辆已停运 =7001,
        车辆已参与所选日期内同一班次排班 = 7002,
        车辆缺少该工地所需的通行证 = 7003,
        车辆缺少该消纳场所需的通行证 = 7004,
        所选的派车时间不在通行证允许的时间范围内 = 7005,
        所需车辆数目已经超过拥有的车辆数目 = 7006,
        所需车辆数目已经超过目前可用的车辆数目 = 7106,
        当前无可用车辆 = 7007,
        车辆列表中包含有重复项 = 7008,
        批量添加请求中的车辆参数为空 = 7009,
        车辆绑定的通行证中有重复项 = 7010,
        车辆绑定的通行证中有无效项 = 7011,
        你选择的排班时间不是最新时间=7012,
        所选班次参数错误=7013,
        所需车辆数不能小于或等于0=7014,
        任务信息不能为空=7015,
        车辆列表不能为空 = 7016,
        所选时间和班次内无排班记录=7017,
        未找到该任务=7018,
        你选择的时间已经有排班任务_请选择新的时间=7019,
        所需车辆总数已经超过目前可用车辆数=7020,
        // 统计项 8xxx
        当前系统内没有任何工地 =8001,
        参数错误_查询类型不能为空 = 8002,
        参数错误_不支持的查询类型 = 8003,
        参数错误_查询条件不能为空 = 8004,
        参数错误_无法解析出起始和终止时间 = 8005,
        //调试
        表内已有数据无法重复添加=9001,
        随机排班过程中出现异常_请重新清空后再尝试=9002,
        当前系统内没有任何排班记录 = 9003,
        一键连续排班必须是针对白班操作=9004,
        一键连续排班过程中有车辆已参与其他晚班任务=9005,
        生成的任务ID与已有任务ID重复_请联系开发人员排查=9006,
        一键连续排班必须是针对当前最新排班日期中的白班进行操作=-9007,
        一键连续排班中的部分车辆已经提前参与晚班的其他工地出勤_无法重复排班=9008
    }
    public class ApiTool
    {
        public HttpResponseMessage MsgFormat(ResponseCode _code, object _data)
        {
            var jsonModel = new
            {
                code = (int)_code,
                message = _code.ToString(),
                data = _data,
            };
            string json = JsonConvert.SerializeObject(jsonModel);
            var response= new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
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
    //Json消息类
    public class JsonModel
    {
        int code;
        string message;
        string data;
        public JsonModel(ResponseCode _code,string _data)
        {
            code = (int)_code;
            message = _code.ToString();
            data = _data;
        }
    }
}
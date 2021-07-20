using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using JundaOS_WebApi.Tools;
using JundaOS_WebApi.User;

namespace JundaOS_WebApi.Controllers
{

    /// <summary>
    /// 通行证Controller
    /// </summary>
    [UserTokenFilter]
    [OperatorAuthFilter]
    [RoutePrefix("api/schedule")]
    public class CalcController : ApiController
    {
        Tools.ApiTool apiTool = new Tools.ApiTool();
        private ScheduleCalcManage m_CalcManage=new ScheduleCalcManage();
        /// <summary>
        /// 自动排班
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("autoSend")]
        public HttpResponseMessage Auto_Schedule([FromBody]TrukSchedule_AutoModel newModel)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            LogHelper.Info($"收到autoSend请求");
            List<TruckSchedule_SingleSiteResultModel> returnModel = m_CalcManage.CalcScheduleAuto(newModel, ref code);
            LogHelper.Info($"返回请求结果，code={code.ToString()}");
            return apiTool.MsgFormat(code, returnModel);
        }
        /// <summary>
        /// 获取当前可用车辆
        /// </summary>
        /// <param name="current_time">选择的排班时间</param>
        /// <param name="schedule_type">选择的班次，1=白班，2=夜班</param>
        /// <returns></returns>
        [HttpPost]
        [Route("get_left_truck")]
        public HttpResponseMessage GetLeftTrucks([FromBody]QueryModel model)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            List<short> result = m_CalcManage.GetLeftTrucks(model, ref code);
            return apiTool.MsgFormat(code, result);
        }
        /// <summary>
        /// 获取当前补偿次数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get_current_compensation")]
        public HttpResponseMessage GetCurrentCompensation()
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            string result = m_CalcManage.HasCompensationCountTrucks(ref code);
            return apiTool.MsgFormat(code, result);
        }
        /// <summary>
        /// 获取最近一次排班的所有信息（任务及结果）
        /// </summary>
        /// <param name="current_time"></param>
        /// <param name="schedule_type">1=白班，2=夜班，</param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("load_recent_schedule")]
        //public HttpResponseMessage GetLeftTrucks([FromUri]DateTime current_time, int schedule_type)
        //{
        //    Tools.ResponseCode code = Tools.ResponseCode.未处理;
        //    RecentScheduleTask result = m_CalcManage.GerRecentSchedule(current_time, schedule_type, ref code);
        //    return apiTool.MsgFormat(code, result);
        //}
        /// <summary>
        /// 
        /// 获取指定日期内的所有排班记录（包括任务以及结果）
        /// </summary>
        /// <param name="current_time"></param>
        /// <param name="schedule_type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("load_all_schedule_by_date")]
        public HttpResponseMessage LoadAllScheduleByDate([FromUri]DateTime current_time)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            List<ScheduleTaskResult> result = m_CalcManage.GetAllScheduleByDate(current_time, ref code);
            return apiTool.MsgFormat(code, result);
        }
        /// <summary>
        /// 
        /// 根据任务TaskID撤回任务
        /// </summary>
        /// <param name="current_time"></param>
        /// <param name="schedule_type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("reback_task")]
        public HttpResponseMessage RebackTaskResult([FromUri]long task_id)
        {
            LogHelper.Info($"收到reback_task请求");
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
             m_CalcManage.RebackTaskByID(task_id, ref code);
            LogHelper.Info($"返回请求结果，code={code.ToString()}");
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 返回最新的排班时间
        /// </summary>       
        /// <returns></returns>
        [HttpGet]
        [Route("check_near_schedule_time")]
        public HttpResponseMessage CheckNearScheduleTime()
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            DateTime dt= m_CalcManage.CheckNearScheduleTime(ref code);
            return apiTool.MsgFormat(code, dt);
        }
        /// <summary>
        /// 一键连续排班
        /// </summary>
        /// <param name="current_time">选择的排班时间</param>
        /// <param name="schedule_type">选择的班次，1=白班，2=夜班</param>
        /// <returns></returns>
        [HttpGet]
        [Route("auto_continue_schedule")]
        public HttpResponseMessage AutoContinueSchedule([FromUri]long taskId)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            LogHelper.Info($"收到auto_continue_schedule请求");
            m_CalcManage.AutoContinueSchedule(taskId, ref code);
            LogHelper.Info($"返回请求结果，code={code.ToString()}");
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 查询可用车辆
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("query_left_invalid_trucks")]
        public HttpResponseMessage GetLeftEnableTrucks([FromBody]QueryModel model)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            List<short> result= m_CalcManage.GetLeftTrucks(model, ref code);
            return apiTool.MsgFormat(code, result);
        }
    }
}                      
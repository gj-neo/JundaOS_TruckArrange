using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using JundaOS_WebApi.User;
using static JundaOS_WebApi.StatisticsManage;

namespace JundaOS_WebApi.Controllers
{

    /// <summary>
    /// 通行证Controller
    /// </summary>
    [UserTokenFilter]
    [OperatorAuthFilter]
    [RoutePrefix("api/statistics")]
    public class StatisticsController : ApiController
    {
        Tools.ApiTool apiTool = new Tools.ApiTool();
        private StatisticsManage m_statisticManage = new StatisticsManage();
        
        /// <summary>
        /// 获取某个车辆的历史出勤记录（指定时间段)
        /// </summary>
        /// <param name="newSite"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get_single_truck_schedule_record")]
        public HttpResponseMessage GetScheduleRecord([FromUri] DateTime startTime, [FromUri] DateTime endTime, [FromUri] int truckNum)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            List<Schedule_Record_Model> vsd = m_statisticManage.GetSingleTruckScheduleRecord(startTime, endTime, truckNum, ref code);
            return apiTool.MsgFormat(code, vsd);
        }
        /// <summary>
        /// 获取某个工地所有车辆的出勤次数
        /// </summary>
        /// <param name="newSite"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("site_attend_statistic")]
        public HttpResponseMessage GetSiteDetailAttendance([FromUri] short site_id)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            List<Site_Attendance_Statistic> vsd = m_statisticManage.GetSiteDetailAttendances(site_id, ref code);
            return apiTool.MsgFormat(code, vsd);
        }
        ///// <summary>
        ///// 获取所有车辆的所有出勤（分为通行证类出勤、非通行证类出勤和二者之和
        ///// </summary>
        ///// <param name="newSite"></param>
        ///// <returns></returns>
        [HttpGet]
        [Route("get_truck_total_attend")]
        public HttpResponseMessage GetTruckTotalAttend([FromUri] DateTime startTime,[FromUri]DateTime endTime)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            List<ReturnTruckTotalAttendModel> vsd = m_statisticManage.GetTruckTotalAttendStatistic(startTime, endTime,ref code);
            return apiTool.MsgFormat(code, vsd);
        }
        /// <summary>
        /// 
        /// 获取排班任务记录（包括排班结果），可选择查询全部，或指定具体工地
        /// </summary>
        /// <param name="current_time"></param>
        /// <param name="schedule_type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get_task_record_by_site")]
        public HttpResponseMessage LoadAllScheduleByDate([FromUri] DateTime startTime,[FromUri]DateTime endTime,[FromUri]int siteId )
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            List<ScheduleTaskResult> result = m_statisticManage.GetSiteTaskRecord(startTime, endTime,siteId,ref code);
            return apiTool.MsgFormat(code, result);
        }
        ///// <summary>
        ///// 获取出勤平衡统计
        ///// </summary>
        ///// <param name="QueryParam"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("get_truck_attend_balance")]
        //public HttpResponseMessage LoadTruckAttendBalance([FromBody] QueryTruckAttendBalanceModel QueryParam)
        //{
        //    Tools.ResponseCode code = Tools.ResponseCode.未处理;
        //    List<TruckAttendBalance> result = m_statisticManage.GetTruckAttendBalance(QueryParam, ref code);
        //    return apiTool.MsgFormat(code, result);
        //}
    }

}
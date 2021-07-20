using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using JundaOS_WebApi.User;

namespace JundaOS_WebApi.Controllers
{
    /// <summary>
    /// 车辆管理类
    /// </summary>
    [UserTokenFilter]
    [OperatorAuthFilter]
    [RoutePrefix("api/truck")]
    public class TruckController : ApiController
    {
        Tools.ApiTool apiTool = new Tools.ApiTool();
        TruckManage m_TruckManage = new TruckManage();
       /// <summary>
       /// 新增车辆
       /// </summary>
       /// <param name="newTruck">车辆信息模型类</param>
       /// <returns></returns>
        [HttpPost] 
        [Route("add")]
        public HttpResponseMessage AddNewTruck([FromBody]TruckInfoModel  newTruck)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            code =m_TruckManage.InsertTruck(newTruck);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 批量添加新车辆
        /// </summary>
        /// <param name="newTruckLst">新车辆列表模型类</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add_multi")]
        public HttpResponseMessage AddMultiNewTruck([FromBody]MultiTruckInfoModel newTruckLst)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            List< MultiInsertTruckResultModel> m = m_TruckManage.MultiInsertTruck(newTruckLst,ref code);
            return apiTool.MsgFormat(code, m);
        }
        /// <summary>
        /// 删除车辆
        /// </summary>
        /// <param name="truck_id">车辆id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public HttpResponseMessage DeleteTruck([FromUri]int truck_id)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            code = m_TruckManage.DeleteTruck(truck_id);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 查询车辆
        /// </summary>
        /// <param name="query_type">查询类型，支持all/truck_id/truck_license/truck_driver_name/permit_id</param>
        /// <param name="query_condition">查询条件</param>
        /// <returns></returns>
        [HttpGet]
        [Route("query")]
        public HttpResponseMessage QueryTruckInfo([FromUri]string query_type, [FromUri]string query_condition)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            List<TruckInfoModel> resultLst = m_TruckManage.QueryCar(query_type, query_condition, ref code);
            return apiTool.MsgFormat(code, resultLst);
        }
        /// <summary>
        /// 修改车辆信息
        /// </summary>
        /// <param name="modifiedTruck">修改后的车辆信息类</param>
        /// <returns></returns>
        [HttpPost]
        [Route("update_truckinfo")]
        public HttpResponseMessage UpdateTruckInfo([FromBody]TruckInfoModel modifiedTruck)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            code = m_TruckManage.UpdateTruck(modifiedTruck);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 恢复运营
        /// </summary>
        /// <param name="modifiedTruck">修改后的车辆信息类</param>
        /// <returns></returns>
        [HttpGet]
        [Route("set_active")]
        public HttpResponseMessage SetTruckActive([FromUri]string truck_list)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            code = m_TruckManage.SetTruckActive(truck_list.Split(',').Select(x => Convert.ToInt32(x)).ToList());
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 停运
        /// </summary>
        [HttpGet]
        [Route("set_stop")]
        public HttpResponseMessage SetTruckStop([FromUri]string truck_list)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            
            //TODO:
            code = m_TruckManage.SetTruckStop(truck_list.Split(',').Select(x => Convert.ToInt32(x)).ToList());
            return apiTool.MsgFormat(code, null);
        }
        ///// <summary>
        ///// 请假
        ///// </summary>
        ///// <param name="truck_list"></param>
        ///// <param name="offline_invalid">本次请假是否真实停运，若真实停运，则不补</param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("set_leave")]
        //public HttpResponseMessage SetTruckLeave([FromUri]string truck_list, [FromUri]bool offline_invalid)
        //{
        //    Tools.ResponseCode code = Tools.ResponseCode.未处理;
            
        //    //TODO:
        //    code = m_TruckManage.SetTruckLeave(truck_list.Split(',').Select(x => Convert.ToInt32(x)).ToList(), offline_invalid);
        //    return apiTool.MsgFormat(code, null);
        //}
    }
}                      
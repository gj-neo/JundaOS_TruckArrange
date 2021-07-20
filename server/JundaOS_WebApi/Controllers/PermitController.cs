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
    /// 通行证Controller
    /// </summary>
    [UserTokenFilter]
    [OperatorAuthFilter]
    [RoutePrefix("api/permit")]
    public class PermitController : ApiController
    {
        Tools.ApiTool apiTool = new Tools.ApiTool();
        private PermitManage m_PermitManage=new PermitManage();
        /// <summary>
        /// 新增通行证
        /// </summary>
        /// <param name="newPermit">通行证实体类</param>
        /// <returns></returns>
        [HttpGet] 
        [Route("add")]
        public HttpResponseMessage AddNewPermit([FromUri]string newPermit)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            code = m_PermitManage.InsertPermit(newPermit);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 更新通行证
        /// </summary>
        /// <param name="newPermit">通行证实体类</param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public HttpResponseMessage UpdatePermit([FromBody]permit_info newPermit)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            code = m_PermitManage.UpdatePermit(newPermit);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 删除通行证
        /// </summary>
        /// <param name="permit_id">通行证id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public HttpResponseMessage DeletePermit([FromUri]int permit_id)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            code = m_PermitManage.DeletePermit(permit_id);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 查询通行证
        /// </summary>
        /// <param name="query_type">查询类型，支持permit_name 或all</param>
        /// <param name="query_condition">查询条件</param>
        /// <returns></returns>
        [HttpGet]
        [Route("query")]
        public HttpResponseMessage QueryPermitInfo([FromUri]string query_type,[FromUri]string query_condition)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            List<permit_info> resultLst = m_PermitManage.QueryPermit(query_type, query_condition,ref code);
            return apiTool.MsgFormat(code, resultLst);
        }
        /// <summary>
        /// 查询已绑定指定通行证的车辆，和未绑定该通行证的车辆
        /// </summary>
        /// <param name="query_type">查询类型，支持permit_name 或all</param>
        /// <param name="query_condition">查询条件</param>
        /// <returns></returns>
        [HttpGet]
        [Route("query_bind")]
        public HttpResponseMessage QueryPermitBindTruck([FromUri]int permit_id)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            List<TruckInfoModel> resultLst = m_PermitManage.GetPermitBindTrucks(permit_id, ref code);
            return apiTool.MsgFormat(code, resultLst);
        }
        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("unbind_permit")]
        public HttpResponseMessage UnbindPermitRelation([FromUri]int permit_id,string truckLst)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
             m_PermitManage.UnbindTruckPermit(permit_id, truckLst.Split(',').Select(x=>Convert.ToInt32(x)).ToList(), ref code);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 新增绑定
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("bind_permit")]
        public HttpResponseMessage AddNewRelation([FromUri]int permit_id, string truckLst)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            m_PermitManage.BindTruckPermit(permit_id, truckLst.Split(',').Select(x => Convert.ToInt32(x)).ToList(), ref code);
            return apiTool.MsgFormat(code, null);
        }
    }
}                      
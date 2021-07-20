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
    [RoutePrefix("api/site")]
    public class SiteController : ApiController
    {
        Tools.ApiTool apiTool = new Tools.ApiTool();
        private SiteManage m_siteManage=new SiteManage();
        /// <summary>
        /// 新增工地
        /// </summary>
        /// <param name="newSite">工地实体类</param>
        /// <returns></returns>
        [HttpGet] 
        [Route("add")]
        public HttpResponseMessage AddNewSite([FromUri]string site_name, [FromUri]int site_type)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            code = m_siteManage.InsertSite(site_name,site_type);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 批量新增工地
        /// </summary>
        /// <param name="newSiteLst">工地列表的实体类</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add_multi")]
        public HttpResponseMessage AddMultiNewSite([FromBody]MultiSiteInfoModel newSiteLst)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            List<MultiSiteInsertResultModel> m = m_siteManage.MultiInsertSite(newSiteLst,ref code);
            return apiTool.MsgFormat(code, m);
        }
        /// <summary>
        /// 更新工地名称
        /// </summary>
        /// <param name="newSite">工地实体类</param>
        /// <returns></returns>
        [HttpGet]
        [Route("update_site_name")]
        public HttpResponseMessage UpdateSiteBaseInfo([FromUri]int site_id, [FromUri]string site_name)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            code = m_siteManage.UpdateSiteBaseInfo(site_id, site_name);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 删除工地
        /// </summary>
        /// <param name="site_id">工地id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public HttpResponseMessage DeleteSite([FromUri]int site_id)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            code = m_siteManage.DeleteSite(site_id);
            return apiTool.MsgFormat(code, null);
        }
        /// <summary>
        /// 查询工地
        /// </summary>
        /// <param name="query_type">查询类型，支持site_name/permit_id/all</param>
        /// <param name="query_condition">查询条件</param>
        /// <returns></returns>
        [HttpGet]
        [Route("query")]
        public HttpResponseMessage QueryTruckInfo([FromUri]string query_type,[FromUri]string query_condition)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            List<SiteInfoModel> resultLst = m_siteManage.QuerySite(query_type, query_condition,ref code);
            return apiTool.MsgFormat(code, resultLst);
        }
        
    }
}                      
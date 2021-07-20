using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
namespace Api.Controllers
{
   
    public class ScheduleController : ApiController
    {
        private TruckSchedule.AutoSchedule m_AutoSchedule = new TruckSchedule.AutoSchedule();
        /// <summary>
        /// 自动排班
        /// </summary>
        /// <param name="newModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("auto")]
        public HttpResponseMessage Auto_Schedule([FromBody] TruckSchedule.TrukSchedule_AutoModel newModel)
        {
            Common.ResponseCode code = Common.ResponseCode.未处理;

            return m_AutoSchedule.TruckAutoSchedule(newModel, ref code);
        }
    }
}
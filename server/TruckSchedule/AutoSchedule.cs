using DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mydb = DbModel.ModelHelper;

namespace JundaOS_WebApi
{
    /// <summary>
    /// 通行证相关的信息类
    /// </summary>
    internal class PermitRalatedInfo
    {
        internal int permitId;
        internal List<int> truckLst;
        //最新一次排班开始的车辆ID
        internal int startTruckId;
    }
    public class AutoSchedule
    {
        DbModel.Junda_TruckArrangeEntities m_db = new Mydb().TruckDB;
        Common.ApiTool apiTool = new Common.ApiTool();
        Dictionary<int, TruckRelatedInfo> m_TruckRelatedInfoDict = new Dictionary<int, TruckRelatedInfo>();
        Dictionary<int, PermitRalatedInfo> m_PermitRelatedInfoDict = new Dictionary<int, PermitRalatedInfo>();
        /// <summary>
        /// 获取与通行证相关的信息，包括拥有该通行证的车辆列表，该通行证下记录的本次开始的车号等
        /// </summary>
        private Dictionary<int, PermitRalatedInfo> GetPermitRelatedInfo()
        {
            //区分不同通行证
            var startTruckIds = this.m_db.pre_schedule.ToList();
            var permitTruckRelations = this.m_db.truck_permit_relation.GroupBy(x => x.permit_id).ToList();
            Dictionary<int, PermitRalatedInfo> resultDict = new Dictionary<int, PermitRalatedInfo>();
            foreach (var item in permitTruckRelations)
            {
                resultDict.Add(item.Key, new PermitRalatedInfo()
                {
                    permitId = item.Key,
                    truckLst = item.Select(x => x.truck_id).ToList(),
                    startTruckId = startTruckIds.Where(x => x.permit_id == item.Key).FirstOrDefault().pre_truck_id,
                }) ;
            }
            return resultDict;
        }
        /// <summary>
        /// 获取所有车辆的相关状态，包括补偿次数
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, TruckRelatedInfo> GetTruckRelatedInfo()
        {
            Dictionary<int, TruckRelatedInfo> resultDict = new Dictionary<int, TruckRelatedInfo>();

            //获取车辆ID-车辆编号、状态、是否已使用等 字典
            resultDict = this.m_db.truck_info
                .ToDictionary(x => x.truck_id, y => new TruckRelatedInfo()
                {
                    truck_id = y.truck_id,
                    truck_num = y.truck_num,
                    //车辆运营状态 0=停运，1=运营中
                    truck_state = y.truck_state,
                    //车辆是否已被使用（当前请求中）
                    truck_uesd = false,
                });
            //添加补偿次数
            var truckCompensation = this.m_db.truck_compensation.ToList();
            foreach (var item in truckCompensation)
            {
                resultDict[item.truck_id].compensation_count = item.compensation_count;
            }
            return resultDict;
        }
        /// <summary>
        /// 生成预排班的车辆顺序，需要将补偿次数也考虑进去，待补偿车辆放到前面
        /// </summary>
        /// <returns></returns>
        private List<TruckRelatedInfo> CreatePreScheduleTruckOrder(Dictionary<int, TruckRelatedInfo> truckRelatedInfo,int startTruckId)
        {
            //
            List<TruckRelatedInfo> resultLst = new List<TruckRelatedInfo>();
            var tmpTruckRelatedInfoLst = truckRelatedInfo.Values.ToList();
            //先获取有补偿次数的车,排序为先按补偿次数从大往小排，再按车号从小往大排
            var hasCompensationTruckLst = tmpTruckRelatedInfoLst.Where(x => x.compensation_count > 0).OrderByDescending(x=>x.compensation_count).ThenBy(x=>x.truck_id).ToList();
            resultLst.AddRange(hasCompensationTruckLst);
            //再获取正常车辆的车顺序(根据startTruckID开始排）
            var existTruckCount = truckRelatedInfo.Keys.Count;
            for (int i = startTruckId; i <= existTruckCount; i++)
            {
                resultLst.Add(truckRelatedInfo[i]);
            }
            for(int i = 1; i < startTruckId; i++)
            {
                resultLst.Add(truckRelatedInfo[i]);
            }
            return resultLst;
           // Common.LogHelper.WriteLog($"生成预排班的车辆顺序.生成车辆排班顺序：{string.Join(",", resultLst.Select(x=>x.truck_id).ToList().ToArray().}");
        }
        /// <summary>
        /// 按顺序逐个分配，形成某个工地的最终排班结果
        /// </summary>
        private TruckSchedule_SingleSiteResultModel ScheduleByOrder(int fromSiteId, PermitRalatedInfo permitRelationInfo,List<TruckRelatedInfo> orderedTruckLst)
        {
            TruckSchedule_SingleSiteResultModel resultModel = new TruckSchedule_SingleSiteResultModel();
            foreach(var truck in orderedTruckLst)
            {
                if (permitRelationInfo.truckLst.Contains(truck.truck_id))
                {
                    if (truck.truck_uesd ==false && truck.truck_state == 1)
                    {
                        //如果排班的时候，遇到第二次排这个车辆，说明之前该车辆进行了补偿，此时再次排到，之前的补偿作废。
                        if (resultModel.resultTruckIdLst.Contains(truck.truck_id))
                        {
                            this.m_TruckRelatedInfoDict[truck.truck_id].compensation_count += 1;
                            this.m_TruckRelatedInfoDict[truck.truck_id].is_current_task_compensated = false;
                        }
                        else
                        {
                            //如果待补偿次数>0，则首次排到时，默认按补偿次数来算
                            if (this.m_TruckRelatedInfoDict[truck.truck_id].compensation_count > 0)
                            {
                                this.m_TruckRelatedInfoDict[truck.truck_id].compensation_count -= 1;
                                this.m_TruckRelatedInfoDict[truck.truck_id].is_current_task_compensated = true;
                            }
                        }
                        resultModel.resultTruckIdLst.Add(truck.truck_id);
                        resultModel.resultTrucksNum += "," + truck.truck_num;
                        this.m_TruckRelatedInfoDict[truck.truck_id].truck_uesd = true;
                    }
                }
            }
            return resultModel; 
        }
        /// <summary>
        /// 将排班结果保存至数据库
        /// </summary>
        private void SaveResultToDB(TrukSchedule_AutoModel taskAutoModel,List<TruckSchedule_SingleSiteResultModel> resultLst,List<TruckRelatedInfo> truckLst)
        {
            Common.SnowIdHelper snowIdWorker = new Common.SnowIdHelper(1);

            //通行证 临时字典,用于保存下一次排班的车辆ID
            Dictionary<int, int> permitLastTruckNumDict = new Dictionary<int, int>();
            //插入任务记录表task_record            
            foreach (var singleTask in taskAutoModel.taskLst)
            {
                //新record
                long newTaskId = snowIdWorker.nextId(); //新任务ID
                task_record tr = new task_record();
                tr.task_id = newTaskId;
                tr.from_site_id = singleTask.fromSiteId;
                tr.day_ninght = (short)taskAutoModel.day_night;
                tr.operate_time = DateTime.Now;
                tr.permit_id = singleTask.needPermitId;
                tr.send_time = taskAutoModel.sendTime;
                tr.task_type = taskAutoModel.task_type;
                tr.to_site_id = singleTask.toSiteId;
                tr.truck_count = singleTask.needTruckCount;
                this.m_db.task_record.Add(tr);
                //插入车辆出勤记录表truck_attend_record
                var singleSiteResult = resultLst.Where(x => x.fromSiteId == tr.from_site_id).FirstOrDefault();
                foreach (var resultTruck in singleSiteResult.resultTruckIdLst)
                {
                    this.m_db.truck_attend_record.Add(new truck_attend_record
                    {
                        task_id = newTaskId,
                        truck_id = resultTruck,
                        is_compensating = (byte)(truckLst.Where(x => x.truck_id == resultTruck).FirstOrDefault().is_current_task_compensated ? 1 : 0),
                    }) ;
                }
            }
            //更新 各个通行证下一次排班 的开始车辆ID   pre_schedule  按照resultLst的顺序排，同一通行证的最后一个工地排的结果为准
            foreach (var result in resultLst)
            {
                var fromSiteId = result.fromSiteId;
                var curPermitId = taskAutoModel.taskLst.Where(x => x.fromSiteId == fromSiteId).FirstOrDefault().needPermitId;
                if (!permitLastTruckNumDict.ContainsKey(curPermitId))
                {
                    permitLastTruckNumDict.Add(curPermitId, result.resultTruckIdLst.Last());
                }
                else
                {
                    permitLastTruckNumDict[curPermitId] = result.resultTruckIdLst.Last();
                }
            }
            foreach(var permit in this.m_db.pre_schedule)
            {
                if (permitLastTruckNumDict.ContainsKey(permit.permit_id))
                {
                    permit.pre_truck_id = permitLastTruckNumDict[permit.permit_id];
                    this.m_db.Entry<pre_schedule>(permit).State = System.Data.Entity.EntityState.Modified;
                }
            }
            //更新 车辆的补偿次数   truck_compensation
            foreach(var truck in truckLst)
            {
                var dbTruck = this.m_db.truck_compensation.Where(x => x.truck_id == truck.truck_id).FirstOrDefault();
                dbTruck.compensation_count = truck.compensation_count;
                this.m_db.Entry<truck_compensation>(dbTruck).State = System.Data.Entity.EntityState.Modified;
            }
            this.m_db.SaveChanges();
        }
        /// <summary>
        /// 自动排班
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HttpResponseMessage TruckAutoSchedule(TrukSchedule_AutoModel model,ref Common.ResponseCode code)
        {
            code = Common.ResponseCode.未处理;
            //判断是否为最新的任务 （日期最新，班次最新）；
            if (!CheckScheduleRequestTimeIsNew(model.sendTime, model.task_type))
            {
                code = Common.ResponseCode.你选择的排班时间不是最新时间;
                return apiTool.MsgFormat(Common.ResponseCode.你选择的排班时间不是最新时间, "", null);
            }
            //结果存储变量
            List<TruckSchedule_SingleSiteResultModel> resultLst = new List<TruckSchedule_SingleSiteResultModel>();
            m_PermitRelatedInfoDict = GetPermitRelatedInfo();
            m_TruckRelatedInfoDict = GetTruckRelatedInfo();
            //按照工地所需通行证 拥有车辆的多寡，确认工地的排班顺序（拥有车辆越少，越优先排）
            List<TruckSchedule_SingleTaskModel> sortedTaskLst = model.taskLst.OrderBy(x=> m_PermitRelatedInfoDict[x.needPermitId].truckLst.Count).ToList();
            //开始逐个工地排班
            foreach (var singleSiteTask in sortedTaskLst)
            {
                var newOrderedTruckLatedInfo = CreatePreScheduleTruckOrder(m_TruckRelatedInfoDict, m_PermitRelatedInfoDict[singleSiteTask.needPermitId].startTruckId);
                var singleResult=ScheduleByOrder(singleSiteTask.fromSiteId, m_PermitRelatedInfoDict[singleSiteTask.needPermitId], newOrderedTruckLatedInfo);
                resultLst.Add(singleResult); 
            }
            SaveResultToDB(model, resultLst, this.m_TruckRelatedInfoDict.Values.ToList());
            code = Common.ResponseCode.成功;
            return apiTool.MsgFormat(Common.ResponseCode.成功, "", resultLst);
        }
        /// <summary>
        /// 工地排序-按照每种通行证对应的车辆数来排，越少的排越靠前
        /// </summary>
        /// <param name="taskLst"></param>
        //private List<TruckSchedule_SingleTaskModel> SortSite(List<TruckSchedule_SingleTaskModel> taskLst)
        //{
        //    if (taskLst.Count <= 1) return taskLst;
        //    //每种通行证对应的车辆数,按升序排列
        //    var result = this.m_db.truck_permit_relation
        //        .GroupBy(x => x.permit_id)
        //        .Select(x => new { permit_id = x.Key, permit_truck_count = x.Count() })
        //        .OrderBy(x => x.permit_truck_count).Select(x => x.permit_id).ToList();            
        //    return taskLst.OrderBy(x => result.IndexOf(x.needPermitId)).ToList();
        //}
        /// <summary>
        /// 判断是否为最新排班任务
        /// </summary>
        /// <param name="sendTime"></param>
        /// <param name="scheduleType"></param>
        /// <returns></returns>
        private bool CheckScheduleRequestTimeIsNew(DateTime sendTime, int scheduleType)
        {
            var db = new Mydb().TruckDB;
            var task = db.task_record.OrderByDescending(x => x.send_time).FirstOrDefault();
            if (task == null)
            {
                //当前没有任务记录，说明是第一次任务
                return true;
            }
            Common.LogHelper.WriteLog($"send time={sendTime},schedule type={scheduleType},the exists newest task time={task.send_time},ref schedule={task.day_ninght}");
            if (task.send_time >= sendTime) return false;
            if (sendTime.Subtract(task.send_time).TotalDays < 1)
            {
                //最新的一条记录和当前选择时间是同一天，则判断班次；
                if (task.day_ninght == scheduleType) return false;
                if (task.day_ninght == 2 && scheduleType == 1) return false;
            }
            return true;
        }
    }
    /// <summary>
    /// 自动排班任务model
    /// </summary>
    public class TrukSchedule_AutoModel
    {
        public DateTime sendTime;
        public short day_night;//白班夜班
        public short task_type;//手动排班 自动排班
        public List<TruckSchedule_SingleTaskModel> taskLst;
    }
    public class TruckSchedule_SingleSiteResultModel
    {
        public int fromSiteId;
        public List<int> resultTruckIdLst;
        public string resultTrucksNum;
    }
    /// <summary>
    /// 单个任务model
    /// </summary>
    public class TruckSchedule_SingleTaskModel
    {
        public int fromSiteId;
        public int toSiteId;
        public int needPermitId;
        public int needTruckCount;
    }
    /// <summary>
    /// 车辆状态Model
    /// </summary>
    public class TruckRelatedInfo
    {
        public int truck_id;
        public int truck_num;
        public int truck_state;
        public bool truck_uesd;
        public int compensation_count;//补偿次数
        public bool is_current_task_compensated;//本次排班是否为补偿出勤
    }
}

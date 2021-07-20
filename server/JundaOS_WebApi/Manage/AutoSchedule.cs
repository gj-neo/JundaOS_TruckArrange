using JundaOS_WebApi.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


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
       
        Dictionary<int, TruckRelatedInfo> m_TruckRelatedInfoDict = new Dictionary<int, TruckRelatedInfo>();
        Dictionary<int, PermitRalatedInfo> m_PermitRelatedInfoDict = new Dictionary<int, PermitRalatedInfo>();
        /// <summary>
        /// 获取与通行证相关的信息，包括拥有该通行证的车辆列表，该通行证下记录的本次开始的车号等
        /// </summary>
        private Dictionary<int, PermitRalatedInfo> GetPermitRelatedInfo(JundaOSEntities m_db)
        {
            //区分不同通行证
            var startTruckIds = m_db.pre_schedule.ToList();
            var permitTruckRelations = m_db.truck_permit_relation.GroupBy(x => x.permit_id).ToList();
            Dictionary<int, PermitRalatedInfo> resultDict = new Dictionary<int, PermitRalatedInfo>();
            foreach (var item in permitTruckRelations)
            {
                resultDict.Add(item.Key, new PermitRalatedInfo()
                {
                    permitId = item.Key,
                    truckLst = item.Select(x => (int)x.truck_id).ToList(),
                    startTruckId = startTruckIds.Where(x => x.permit_id == item.Key).FirstOrDefault().pre_truck_id,
                });
            }
            return resultDict;
        }
        /// <summary>
        /// 获取所有车辆的相关状态，包括补偿次数
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, TruckRelatedInfo> GetTruckRelatedInfo(JundaOSEntities m_db, DateTime send_time, short m_day_night)
        {
            Dictionary<int, TruckRelatedInfo> resultDict = new Dictionary<int, TruckRelatedInfo>();
            LogHelper.Debug($"get related truck info");
            //获取车辆ID-车辆编号、状态、是否已使用等 字典，剔除已经参与本班次排班的车辆
            var tmpLst = m_db.truck_attend_record.Join(m_db.task_record, c => c.task_id, s => s.task_id, (c, s) =>
                     new
                     {
                         truck_id = c.truck_id,
                         send_time = s.send_time,
                         day_night = s.day_ninght,
                         is_task_deleted=s.is_deleted,
                     }).ToList();
            var alreadySendedTruckLst = tmpLst.Where(x =>send_time.Year==x.send_time.Year&&
            send_time.Month==x.send_time.Month&&send_time.Day==x.send_time.Day && m_day_night == x.day_night&& x.is_task_deleted==0).Select(x => x.truck_id).ToList();

           
            resultDict = m_db.truck_info.Where(x=>!alreadySendedTruckLst.Contains(x.truck_id))
                .ToDictionary(x => (int)x.truck_id, y => new TruckRelatedInfo()
                {
                    truck_id = y.truck_id,
                    truck_num = y.truck_num,
                    //车辆运营状态 0=停运，1=运营中
                    truck_state = y.truck_state,
                    //车辆是否已被使用（当前请求中->当前日期内的当前班次内）
                    truck_uesd = false,
                });
            //添加补偿次数
            var truckCompensation = m_db.truck_compensation.ToList();
            foreach (var item in truckCompensation)
            {
                if (resultDict.ContainsKey(item.truck_id))
                {
                    resultDict[item.truck_id].compensation_count = item.compensation_count;
                }
            }
            //foreach(var item in resultDict)
            //{
            //    var t = item.Value;
            //    LogHelper.Debug($"truck id={item.Value.truck_id}，truck num={item.Value.truck_num},is on={t.truck_state},is uesd={t.truck_uesd}");
            //}
            return resultDict;
        }
        /// <summary>
        /// 生成预排班的车辆顺序，需要将补偿次数也考虑进去，待补偿车辆放到前面
        /// </summary>
        /// <returns></returns>
        private List<TruckRelatedInfo> CreatePreScheduleTruckOrder(Dictionary<int, TruckRelatedInfo> truckRelatedInfo, int startTruckId, JundaOSEntities m_db)
        {
            //
            List<TruckRelatedInfo> resultLst = new List<TruckRelatedInfo>();
            var tmpTruckRelatedInfoLst = truckRelatedInfo.Values.ToList();
            //先获取补偿次数>0的车,排序按车号从小往大排
            var hasCompensationTruckLst = tmpTruckRelatedInfoLst.Where(x => x.compensation_count > 0).OrderBy(x => x.truck_id).ToList();
            resultLst.AddRange(hasCompensationTruckLst);
            //再获取正常车辆的车顺序(根据startTruckID开始排）
            var existTruckCount = truckRelatedInfo.Keys.Count;
            var relatedTruckLst = truckRelatedInfo.Values.Select(x => x.truck_id).OrderBy(x=>x).ToList();
            for (int i = relatedTruckLst.IndexOf(startTruckId); i <= existTruckCount-1; i++)
            {
                resultLst.Add(truckRelatedInfo[relatedTruckLst[i]]);
            }
            for (int i = 0; i < relatedTruckLst.IndexOf(startTruckId); i++)
            {
                resultLst.Add(truckRelatedInfo[relatedTruckLst[i]]);
            }
            LogHelper.Debug($"truck pre calculate order(truck_num)：{string.Join("、",resultLst.Select(x=>x.truck_num).ToArray())}");
            return resultLst;
            // Common.LogHelper.WriteLog($"生成预排班的车辆顺序.生成车辆排班顺序：{string.Join(",", resultLst.Select(x=>x.truck_id).ToList().ToArray().}");
        }
        /// <summary>
        /// 按顺序逐个分配，形成某个工地的最终排班结果
        /// </summary>
        private TruckSchedule_SingleSiteResultModel ScheduleByOrder(int fromSiteId, PermitRalatedInfo permitRelationInfo, List<TruckRelatedInfo> orderedTruckLst,int needTruckCount,JundaOSEntities m_db)
        {
            TruckSchedule_SingleSiteResultModel resultModel = new TruckSchedule_SingleSiteResultModel();
            List<int> resultTruckNumLst = new List<int>();
            resultModel.fromSiteId = fromSiteId;
            foreach (var truck in orderedTruckLst)
            {
                if (permitRelationInfo.truckLst.Contains(truck.truck_id))
                {
                    if (truck.truck_uesd == false && truck.truck_state == 1)
                    {
                        //如果排班的时候，遇到第二次排这个车辆，说明之前该车辆进行了补偿，此时再次排到，之前的补偿作废。
                        //if (resultModel.resultTruckIdLst.Contains(truck.truck_id))
                        //{
                        //    //LogHelper.Debug($"第二次遇到该车辆，说明之前该车辆进行了补偿，此时再次排到，之前的补偿作废");
                        //    this.m_TruckRelatedInfoDict[truck.truck_id].compensation_count += 1;
                        //    this.m_TruckRelatedInfoDict[truck.truck_id].is_current_task_compensated = false;
                        //}
                        //else
                        //{
                        //    //如果待补偿次数>0，则首次排到时，默认按补偿次数来算
                        //    if (this.m_TruckRelatedInfoDict[truck.truck_id].compensation_count > 0)
                        //    {
                        //        this.m_TruckRelatedInfoDict[truck.truck_id].compensation_count -= 1;
                        //        this.m_TruckRelatedInfoDict[truck.truck_id].is_current_task_compensated = true;
                        //    }
                        //}
                        //如果待补偿次数>0，则首次排到时，默认按补偿次数来算
                        if (this.m_TruckRelatedInfoDict[truck.truck_id].compensation_count > 0)
                        {
                            this.m_TruckRelatedInfoDict[truck.truck_id].compensation_count =0;
                            this.m_TruckRelatedInfoDict[truck.truck_id].is_current_task_compensated = true;
                        }
                        resultModel.resultTruckIdLst.Add(truck.truck_id);
                        resultTruckNumLst.Add(truck.truck_num);                        
                        this.m_TruckRelatedInfoDict[truck.truck_id].truck_uesd = true;
                        //如果所需车的数量已经满足
                        if (resultModel.resultTruckIdLst.Count >= needTruckCount)
                        {
                            break;
                        }
                    }
                }
            }
            resultModel.resultTrucksNum = string.Join("、", resultTruckNumLst) + "、";
            LogHelper.Debug($"calc result：{resultModel.resultTrucksNum}");
            return resultModel;
        }
        /// <summary>
        /// 将排班结果保存至数据库
        /// </summary>
        private List<TruckSchedule_SingleSiteResultModel> SaveResultToDB(TrukSchedule_AutoModel taskAutoModel, List<TruckSchedule_SingleSiteResultModel> resultLst, List<TruckRelatedInfo> truckLst, JundaOSEntities m_db)
        {
           
            SnowIdHelper snowIdWorker = new SnowIdHelper(1);
            
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
                tr.is_deleted = 0;
                m_db.task_record.Add(tr);
                //插入车辆出勤记录表truck_attend_record
                var tmp = resultLst;
                var singleSiteResult = resultLst.Where(x => x.fromSiteId == tr.from_site_id).FirstOrDefault();
                singleSiteResult.taskId = newTaskId;
                foreach (var resultTruck in singleSiteResult.resultTruckIdLst)
                {
                    m_db.truck_attend_record.Add(new truck_attend_record
                    {
                        task_id = newTaskId,
                        truck_id = resultTruck,
                        is_compensating = (byte)(truckLst.Where(x => x.truck_id == resultTruck).FirstOrDefault().is_current_task_compensated ? 1 : 0),
                    });
                }
            }
            //最大的车辆id，超过该id，则重新从1开始算起
            var allExistTruckLst = m_db.truck_info.Select(x => x.truck_id).OrderBy(x=>x).ToList();
            var maxTruckId = allExistTruckLst.Max();
            var minTruckId = allExistTruckLst.Min();
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
            foreach (var permit in m_db.pre_schedule)
            {
                if (permitLastTruckNumDict.ContainsKey(permit.permit_id))
                {
                    var tmpLastTruckID = permitLastTruckNumDict[permit.permit_id];
                    if (tmpLastTruckID >= maxTruckId)
                    {
                        permit.pre_truck_id = minTruckId;
                    }
                    else
                    {
                        //设置为大于当前truck id的下一个truck id
                        permit.pre_truck_id = allExistTruckLst.Where(x => x > tmpLastTruckID).First();
                    }
                    m_db.Entry<pre_schedule>(permit).State = System.Data.Entity.EntityState.Modified;
                }
            }
            //更新 车辆的补偿次数   truck_compensation
            foreach (var truck in truckLst)
            {
                var dbTruck = m_db.truck_compensation.Where(x => x.truck_id == truck.truck_id).FirstOrDefault();
                dbTruck.compensation_count = truck.compensation_count;
                m_db.Entry<truck_compensation>(dbTruck).State = System.Data.Entity.EntityState.Modified;
            }
            m_db.SaveChanges();
            return resultLst;
        }
        /// <summary>
        /// 自动排班
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<TruckSchedule_SingleSiteResultModel> TruckAutoSchedule(TrukSchedule_AutoModel model, ref Tools.ResponseCode code)
        {
            //结果存储变量
            List<TruckSchedule_SingleSiteResultModel> resultLst = new List<TruckSchedule_SingleSiteResultModel>();
            code = Tools.ResponseCode.未处理;
            using (JundaOSEntities m_db = Common.CreateDbInstance())
            {
                //判断是否为最新的任务 （日期最新，班次最新）；
                if (!CheckScheduleRequestTimeIsNew(model.sendTime, model.task_type,m_db))
                {
                    code = Tools.ResponseCode.你选择的排班时间不是最新时间;                   
                }
                else
                {
                    LogHelper.Debug($"排班时间为{model.sendTime},排班班次为{model.day_night}");
                    m_PermitRelatedInfoDict = GetPermitRelatedInfo(m_db);
                    m_TruckRelatedInfoDict = GetTruckRelatedInfo(m_db,model.sendTime,model.day_night);
                    int maxNeedCount = model.taskLst.Max(x => x.needTruckCount);
                    if (maxNeedCount > m_TruckRelatedInfoDict.Values.Where(x => x.truck_state == 1 && !x.truck_uesd).ToList().Count)
                    {
                        code = Tools.ResponseCode.所需车辆数目已经超过目前可用的车辆数目;
                        return null;
                    }
                    //按照工地所需通行证 拥有车辆的多寡，确认工地的排班顺序（拥有车辆越少，越优先排）
                    List<TruckSchedule_SingleTaskModel> sortedTaskLst = model.taskLst.OrderBy(x => m_PermitRelatedInfoDict[x.needPermitId].truckLst.Count).ToList();
                    //开始逐个工地排班
                    foreach (var singleSiteTask in sortedTaskLst)
                    {                        
                        LogHelper.Debug($"开始工地id={singleSiteTask.fromSiteId}的排班");                      
                        if (singleSiteTask.needTruckCount > m_TruckRelatedInfoDict.Values.Where( x => x.truck_state == 1 && !x.truck_uesd).ToList().Count)
                        {
                            code = Tools.ResponseCode.所需车辆数目已经超过目前可用的车辆数目;
                            return null;
                        }
                        var newOrderedTruckLatedInfo = CreatePreScheduleTruckOrder(m_TruckRelatedInfoDict, m_PermitRelatedInfoDict[singleSiteTask.needPermitId].startTruckId,m_db);
                        var singleResult = ScheduleByOrder(singleSiteTask.fromSiteId, m_PermitRelatedInfoDict[singleSiteTask.needPermitId], newOrderedTruckLatedInfo,singleSiteTask.needTruckCount,m_db);
                        resultLst.Add(singleResult);
                    }
                    resultLst=SaveResultToDB(model, resultLst, this.m_TruckRelatedInfoDict.Values.ToList(),m_db);
                    code = Tools.ResponseCode.成功;
                }
            }
            if (resultLst.Count == 0) return null;
            List<TruckSchedule_SingleSiteResultModel> finalResultLst = new List<TruckSchedule_SingleSiteResultModel>();
            //确保结果列表的顺序与前端发来的请求列表中的顺序一致，避免前端出现错误；
            foreach (var requestTask in model.taskLst)
            {
                var tmpFromSiteId = requestTask.fromSiteId;
                finalResultLst.Add(resultLst.Where(x => x.fromSiteId == tmpFromSiteId).FirstOrDefault());
            }
            return finalResultLst;
        }
        /// <summary>
        /// 判断是否为最新排班任务
        /// </summary>
        /// <param name="sendTime"></param>
        /// <param name="scheduleType"></param>
        /// <returns></returns>
        private bool CheckScheduleRequestTimeIsNew(DateTime sendTime, int scheduleType,JundaOSEntities m_db)
        {
            return true;
           
            var task = m_db.task_record.Where(x=>x.is_deleted==0).OrderByDescending(x => x.send_time).FirstOrDefault();
            if (task == null)
            {
                //当前没有任务记录，说明是第一次任务
                return true;
            }
            //LogHelper.WriteLog($"send time={sendTime},schedule type={scheduleType},the exists newest task time={task.send_time},ref schedule={task.day_ninght}");
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
        public TrukSchedule_AutoModel() 
        {
            taskLst = new List<TruckSchedule_SingleTaskModel>();
        }
    }
    public class TruckSchedule_SingleSiteResultModel
    {

        public long taskId;
        public int fromSiteId;
        public List<int> resultTruckIdLst;
        public string resultTrucksNum;

        public TruckSchedule_SingleSiteResultModel()
        {
            resultTruckIdLst = new List<int>();
        }
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

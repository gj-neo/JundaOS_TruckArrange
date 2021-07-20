using JundaOS_WebApi.Tools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace JundaOS_WebApi
{
    /// <summary>
    /// 排班计算类
    /// </summary>
    public class ScheduleCalcManage
    {
       
        public ScheduleCalcManage()
        {

        }
        /// <summary>
        /// 判断任务所需车辆是否超出已经拥有的数量
        /// </summary>
        /// <param name="myDb"></param>
        /// <param name="taskCount"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckTruckCount(JundaOSEntities myDb, int taskCount, ref Tools.ResponseCode code)
        {
            //判断是否超过拥有车辆的最大数
            int haveCount = myDb.truck_info.ToList().Count;
            if (taskCount > haveCount)
            {
                code = Tools.ResponseCode.所需车辆数目已经超过拥有的车辆数目;
                return false;
            }
            return true;
        }
        private bool CheckTaskInfo(JundaOSEntities myDb, TrukSchedule_AutoModel _taskModel, ref Tools.ResponseCode code)
        {
            var recentTask = myDb.task_record.Where(x=>x.is_deleted==0).OrderByDescending(x => x.send_time).FirstOrDefault();
            if (recentTask != null)
            {
                DateTime recentTaskTime = recentTask.send_time;
                if (_taskModel.sendTime < recentTaskTime)
                {
                    code = Tools.ResponseCode.你选择的排班时间不是最新时间;
                    return false;
                }
                else if (_taskModel.sendTime == recentTaskTime)
                {
                    code = Tools.ResponseCode.你选择的时间已经有排班任务_请选择新的时间;
                    return false;
                }
            }
            if (_taskModel.day_night != 1 && _taskModel.day_night != 2)
            {
                code = Tools.ResponseCode.所选班次参数错误;
                return false;
            }
            if (_taskModel.taskLst == null || _taskModel.taskLst.Count == 0)
            {
                code = Tools.ResponseCode.任务信息不能为空;
                return false;
            }
            foreach (var item in _taskModel.taskLst)
            {
                if (myDb.site_info.Where(x => x.site_id == item.fromSiteId).Count() == 0)
                {
                    code = Tools.ResponseCode.工地不存在;
                    return false;
                }
                if (item.needTruckCount <= 0)
                {
                    code = Tools.ResponseCode.所需车辆数不能小于或等于0;
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 自动排班
        /// </summary>
        /// <param name="autoTaskModel"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<TruckSchedule_SingleSiteResultModel> CalcScheduleAuto(TrukSchedule_AutoModel autoTaskModel, ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;
            using (JundaOSEntities mydb=new JundaOSEntities())
            {
                if (!CheckTaskInfo(mydb,autoTaskModel,ref code))
                {
                    return null;
                }
            }
            try
            {
                AutoSchedule autoSchedule = new AutoSchedule();
                return autoSchedule.TruckAutoSchedule(autoTaskModel, ref code);
            }
            catch (Exception e)
            {
                LogHelper.Error(e.ToString());
                return null;
            }
        }
       
        /// <summary>
        /// 获取剩余可用车辆
        /// </summary>
        /// <param name="_selectedTime">选择的时间</param>
        /// <param name="_schedule_type">选择的班次</param>
        /// <returns></returns>
        public List<short> GetLeftTrucks(QueryModel _queryModel, ref Tools.ResponseCode code)
        {
            List<short> leftTruckLst = new List<short>();
            List<short> resultLst = new List<short>();
            List<short> finalResultLst = new List<short>();
            code = Tools.ResponseCode.未处理;
            if (_queryModel == null) return null;
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //判断当前是否为最新时间
                var selectedTime = _queryModel.current_time;
                //检查时间是否为最新
                int count = myDb.truck_attend_record.Count();
                if (count > 0)
                {
                    DateTime newestTime = myDb.task_record.Where(x=>x.is_deleted==0).OrderByDescending(x => x.send_time).First().send_time;
                    if (selectedTime < newestTime)
                    {
                        code = Tools.ResponseCode.你选择的排班时间不是最新时间;
                        return null;
                    }
                }
                //先获取符合基本条件（通行证）的车辆列表
                ////有通行证,
                //if (_queryModel.permit_id > 0)
                //{
                //    leftTruckLst = myDb.truck_permit_relation.Where(x => x.permit_id == _queryModel.permit_id).Select(x => x.truck_id).ToList();

                //}
                ////没通行证
                //else
                //{
                //    leftTruckLst = myDb.truck_info.Select(x => x.truck_id).ToList();
                //}
                ////判断是否停运
                //var currentLeftTruckInfoLst = myDb.truck_info.Where(x => leftTruckLst.Contains(x.truck_id) && x.truck_status == 1).ToList();
                ////根据班次判断是否有可用的
                //if (_queryModel.day_night == 1)
                //{
                //    //判断
                //}
                //foreach (var truck in currentLeftTruckInfoLst)
                //{
                //    //没有出勤
                //    if (truck.recent_attend_time == null)
                //    {
                //        finalResultLst.Add(truck.truck_id);
                //    }
                //    //距离上次出勤时间>=1天
                //    else if ((_queryModel.current_time - truck.recent_attend_time.Value).Days != 0)
                //    {
                //        finalResultLst.Add(truck.truck_id);
                //    }
                //    //距离上次出勤时间小于1天，且班次不一样
                //    else if ((_queryModel.current_time - truck.recent_attend_time.Value).Days == 0 && truck.recent_schedule_type < _queryModel.schedule_type)
                //    {
                //        finalResultLst.Add(truck.truck_id);
                //    }
                //}
                //if (finalResultLst.Count == 0)
                //{
                //    code = Tools.ResponseCode.当前无可用车辆;
                //    return null;
                //}
            }
            code = Tools.ResponseCode.成功;
            return finalResultLst;
        }
        /// <summary>
        /// 获取最近一次排班的任务列表及结果列表
        /// </summary>
        /// <param name="_selectedTime">选择的时间</param>
        /// <param name="_schedule_type">选择的班次</param>
        /// <returns></returns>
        public RecentScheduleTask GerRecentSchedule(DateTime _selectedTime, int day_night, ref Tools.ResponseCode code)
        {
            RecentScheduleTask result = new RecentScheduleTask();
            List<SingleScheduleTask> taskLst = new List<SingleScheduleTask>();
            List<SingleScheduleResult> resultLst = new List<SingleScheduleResult>();
            List<short> leftTruckLst = new List<short>();

            code = Tools.ResponseCode.未处理;
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {

                var recentTask = myDb.task_record.Where(x=>x.is_deleted==0).Where(x => (_selectedTime - x.send_time).Days == 0 && x.day_ninght == day_night).ToList();
                foreach (var t in recentTask)
                {
                    taskLst.Add(new SingleScheduleTask()
                    {
                        day_night = t.day_ninght,
                        from_site_id = (short)t.from_site_id,
                        permit_id =(short)t.permit_id,
                        send_site_id = 0,// (short)t.send_site_id,
                        task_type = t.task_type,
                        send_time = t.send_time,
                        truck_count = t.truck_count,
                    });
                }
                if (taskLst.Count == 0)
                {
                    code = Tools.ResponseCode.所选时间和班次内无排班记录;
                }
                //获取符合班次和日期的排班结果
                //var recentTaskResult = myDb.schedule_record.Where(x => x.schedule_type == _schedule_type && (_selectedTime - x.truck_send_time).Days == 0 ).ToList();
                ////拆分并入各个工地结果
                //foreach(SingleScheduleTask s in taskLst)
                //{
                //    SingleScheduleResult sResult = new SingleScheduleResult();
                //    sResult.auto_manual = s.auto_manual;
                //    sResult.from_site_id = s.from_site_id;
                //    sResult.send_site_id = s.send_site_id;
                //    var attendedTruckLst = recentTaskResult.Where(x => x.from_site_id == s.from_site_id && x.send_site_id == s.send_site_id && x.auto_manual == s.auto_manual).Select(x => x.truck_id).ToList();
                //    sResult.truck_count =(short) attendedTruckLst.Count();
                //    sResult.truck_result = attendedTruckLst;
                //    resultLst.Add(sResult);
                //}
                result.TaskList = taskLst;
                result.ResultList = resultLst;
                code = Tools.ResponseCode.成功;
            }
            return null;
        }
        /// <summary>
        /// 获取指定日期的排班结果
        /// </summary>
        /// <param name="_selectedTime">选择的时间</param>
        /// <param name="_schedule_type">选择的班次</param>
        /// <returns></returns>
        public List<ScheduleTaskResult> GetAllScheduleByDate(DateTime _selectedTime, ref Tools.ResponseCode code)
        {
            List<ScheduleTaskResult> resultLst = new List<ScheduleTaskResult>();

            List<short> leftTruckLst = new List<short>();

            code = Tools.ResponseCode.未处理;
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                var task_record_lst = myDb.task_record.Where(x =>x.is_deleted==0&& System.Data.Entity.DbFunctions.DiffDays(_selectedTime, x.send_time).Value == 0).ToList();

                foreach (var t in task_record_lst)
                {
                    ScheduleTaskResult sr = new ScheduleTaskResult();
                    var totalResulLst = myDb.truck_attend_record
                        .Join(myDb.truck_info,c=>c.truck_id,s=>s.truck_id,
                        (c,s)=>new {task_id=c.task_id,truck_num=s.truck_num }).Where(x => x.task_id == t.task_id).Select(x => x.truck_num).ToList();
                    string strCalcResult =string.Join("、", totalResulLst.ToArray())+"、";
                    sr.day_night = t.day_ninght;
                    sr.calc_result = strCalcResult;
                    sr.send_time = t.send_time;
                    sr.task_type = t.task_type;
                    sr.from_site_id = t.from_site_id;
                    sr.to_site_id = t.to_site_id;
                    sr.permit_id = t.permit_id;
                    sr.task_id = t.task_id.ToString();
                    sr.truck_count = totalResulLst.Count();// calcResult.Count;
                    resultLst.Add(sr);
                }
                if (resultLst.Count == 0)
                {
                    code = Tools.ResponseCode.所选时间和班次内无排班记录;
                }
                code = Tools.ResponseCode.成功;
            }
            return resultLst;
        }
        /// <summary>
        /// 根据taskId取消对应的任务及排班结果
        /// </summary>
        /// <param name="_task_id"></param>
        /// <param name="code"></param>
        public void RebackTaskByID(long _task_id, ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                var tmpTaskId = _task_id;
                var curTaskRecord = myDb.task_record.Where(x => x.task_id == _task_id&&x.is_deleted==0).FirstOrDefault();
                if (curTaskRecord == null)
                {
                    code = Tools.ResponseCode.未找到该任务;
                    return;
                }
                
                var truck_attend_list = myDb.truck_attend_record.Where(x => x.task_id == _task_id).ToList();
                //myDb.Entry<task_record>(task_record).State = System.Data.Entity.EntityState.Deleted;
                foreach (var t in truck_attend_list)
                {
                    //1、清空在truck_attend_record中的记录
                    //myDb.Entry<truck_attend_record>(t).State = System.Data.Entity.EntityState.Deleted;
                    //一键连续排班撤销后，不进行补偿
                    if (curTaskRecord.task_type != 3)
                    {
                        //2、补偿次数+1
                        var truckCompensation = myDb.truck_compensation.Where(x => x.truck_id == t.truck_id).FirstOrDefault();
                        truckCompensation.compensation_count = 1;
                        myDb.Entry<truck_compensation>(truckCompensation).State = EntityState.Modified;
                    }
                }
                //撤销标记
                curTaskRecord.is_deleted = 1;
                myDb.SaveChanges();
                code = Tools.ResponseCode.成功;
            }
            return;
        }
        /// <summary>
        /// 返回当前有补偿次数的车辆列表
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string HasCompensationCountTrucks(ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;
            string strResult = string.Empty;
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {

                var r = myDb.truck_compensation.Join(myDb.truck_info,c=>c.truck_id,s=>s.truck_id ,(c, s) => new 
                {
                    compensation_count=c.compensation_count,
                    truck_num=s.truck_num,
                    truck_state=s.truck_state,
                }).Where(x=>x.compensation_count>0&&x.truck_state==1).Select(x=>x.truck_num).ToList();
                if (r.Count == 0)
                {
                    code = ResponseCode.当前没有待补偿车辆;
                }
                else
                {
                    strResult = string.Join("、", r);
                    code = Tools.ResponseCode.成功;
                }
            }
            return strResult;
        }
        /// <summary>
        /// 返回最新的排班时间
        /// </summary>
        /// <param name="_task_id"></param>
        /// <param name="code"></param>
        public DateTime CheckNearScheduleTime(ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;
            DateTime nearTime = DateTime.Now;
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                int count = myDb.truck_attend_record.Count();
                if (count == 0)
                {
                    code = Tools.ResponseCode.当前系统内没有任何排班记录;
                }
                else
                {
                    var result = myDb.task_record.Where(x=>x.is_deleted==0).OrderByDescending(x => x.send_time).First();
                    nearTime = result.send_time;
                    code = Tools.ResponseCode.成功;
                }
            }
            return nearTime;
        }
        /// <summary>
        /// 一键连续排班，限制条件：白班
        /// </summary>
        /// <param name="_task_id"></param>
        /// <param name="code"></param>
        public void AutoContinueSchedule(long _task_id, ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;

            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                try
                {
                    var taskInfo = myDb.task_record.Where(x =>x.is_deleted==0&& x.task_id == _task_id).First();

                    long newTaskId = new SnowIdHelper(1).nextId();
                    if (taskInfo.day_ninght == 2)
                    {
                        code = Tools.ResponseCode.一键连续排班必须是针对白班操作;
                        return;
                    }
                    //一键排班必须是针对最新排班日期
                    var result = myDb.task_record.Where(x=>x.is_deleted==0).OrderByDescending(x => x.send_time).First();
                    var newTime = result.send_time;
                    if ((taskInfo.send_time.Date - newTime.Date).TotalDays >= 1)
                    {
                        code = Tools.ResponseCode.一键连续排班必须是针对当前最新排班日期中的白班进行操作;
                        return;
                    }

                    //存储出勤记录
                    var truckLst = myDb.truck_attend_record.Where(x => x.task_id == _task_id).Select(x => x.truck_id).ToList();
                    //判断当天晚班是否已经派出了部分车辆
                    var curTask = myDb.task_record.Where(x =>x.is_deleted==0&& x.task_id == _task_id).FirstOrDefault();
                    var curSendTime = curTask.send_time;
                    var sameDayTask = myDb.task_record.Where(x => x.is_deleted == 0 && DbFunctions.DiffDays(curSendTime, x.send_time) < 1&&x.send_time>curSendTime && x.day_ninght == 2)
                        .Join(myDb.truck_attend_record, c => c.task_id, s => s.task_id, (c, s) => new { task_id = c.task_id, truck_id = s.truck_id }).ToList();
                    if (sameDayTask.Count > 0)
                    {
                        foreach (var t in sameDayTask)
                        {
                            if (truckLst.Contains(t.truck_id))
                            {
                                code = Tools.ResponseCode.一键连续排班中的部分车辆已经提前参与晚班的其他工地出勤_无法重复排班;
                                return;
                            }
                        }
                    }
                    //存储任务记录到task_record表
                    myDb.task_record.Add(new task_record()
                    {
                        day_ninght = 2,
                        from_site_id = taskInfo.from_site_id,
                        to_site_id = taskInfo.to_site_id,
                        task_type = (byte)3,
                        send_time = taskInfo.send_time.Date.AddHours(20),//默认一键连续作业的时间为当天晚上20点
                        permit_id = taskInfo.permit_id,
                        truck_count = (short)taskInfo.truck_count,
                        task_id = newTaskId,
                        is_deleted=0,
                    });
                    foreach (var truck in truckLst)
                    {
                        myDb.truck_attend_record.Add(new truck_attend_record()
                        {
                            truck_id = truck,
                            task_id = newTaskId,
                        });
                    }
                    myDb.SaveChanges();
                    code = Tools.ResponseCode.成功;
                }
                catch (Exception e)
                {
                    return;
                }
            }
        }

    }
    /// <summary>
    /// 查询出来的
    /// </summary>
    public class TruckOfflineQueryModel
    {
        public short truck_id;
        public short offline_count;
        public DateTime recent_restore_time;
    }
    /// <summary>
    /// 工地排班任务的参数模型
    /// </summary>
    public class CalcParamModel
    {
        public DateTime send_time { get; set; }
        public short schedule_type { get; set; }
        public List<AutoTaskModel> task_model_list { get; set; }
        public CalcParamModel()
        {
            task_model_list = new List<AutoTaskModel>();
        }

    }
    public class AutoTaskModel
    {
        public short from_site_id { get; set; }
        public short to_site_id { get; set; }
        public short permit_id { get; set; }
        public short truck_count { get; set; }
        public string task_id { get; set; }
        public List<int> truck_result { get; set; }
        public AutoTaskModel()
        {
            truck_result = new List<int>();
        }
    }
    /// <summary>
    /// 一键连续作业model
    /// </summary>
    public class AutoContinueModel
    {
        public string task_id { get; set; }
        public List<short> truckLst { get; set; }
    }
    /// <summary>
    /// 手动排班时的参数模型
    /// </summary>
    public class ManualParamModel
    {
        public string task_id { get; set; }
        public short from_site_id { get; set; }
        public short send_site_id { get; set; }
        public short permit_id { get; set; }
        public List<short> truckLst { get; set; }
        public DateTime send_time { get; set; }
        public short day_night { get; set; }
        public short scheduleType { get; set; }//排班类型  手动、自动

    }
    /// <summary>
    /// 某天内的所有排班记录（包括结果）
    /// </summary>
    public class ScheduleTaskResult
    {
        public int day_night;
        public DateTime send_time;
        public int task_type;
        public int from_site_id;
        public int to_site_id;
        public int permit_id;
        public int truck_count;
        public string calc_result;
        public string task_id;
        public int is_deleted;
    }
    /// <summary>
    /// 最近一次排班任务列表及排班结果
    /// </summary>
    public class RecentScheduleTask
    {
        public List<SingleScheduleTask> TaskList { get; set; }
        public List<SingleScheduleResult> ResultList { get; set; }
        public RecentScheduleTask()
        {
            TaskList = new List<SingleScheduleTask>();
            ResultList = new List<SingleScheduleResult>();
        }
    }
    public class SingleScheduleTask
    {
        public int task_type { get; set; }
        public short from_site_id { get; set; }
        public short send_site_id { get; set; }
        public short permit_id { get; set; }
        public int truck_count { get; set; }
        public DateTime send_time { get; set; }
        public short day_night { get; set; }
    }
    public class SingleScheduleResult
    {
        public int auto_manual { get; set; }
        public short from_site_id { get; set; }
        public short send_site_id { get; set; }
        public short truck_count { get; set; }
        public List<short> truck_result { get; set; }
        public SingleScheduleResult()
        {
            truck_result = new List<short>();
        }
    }
    public class QueryModel
    {
        //DateTime current_time,int schedule_type,short from_site_id,short send_site_id
        public DateTime current_time;
        public int day_night;
        public short permit_id;
    }
}
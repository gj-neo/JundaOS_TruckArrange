
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace JundaOS_WebApi
{
    /// <summary>
    /// 统计类
    /// </summary>
    public class StatisticsManage
    {
        public StatisticsManage()
        {

        }
        /// <summary>
        /// 选择指定单个或多个工地，指定通行证下的出勤统计
        /// </summary>
        /// <returns></returns>
        //public List<ReturnTruckAttendModel> GetTruckAttendStatistic(GetTruckAttendModel _model, ref Tools.ResponseCode code)
        //{
        //    code = Tools.ResponseCode.未处理;
        //    List<ReturnTruckAttendModel> resultLst = new List<ReturnTruckAttendModel>();
        //    using (JundaOSEntities myDb = Common.CreateDbInstance())
        //    {
        //        List<truck_attend_record> recordData;
        //        var invalidTimeData = myDb.truck_attend_record.Where(x => x >= _model.startTime && x.send_time <= _model.endTime);
        //        if (_model.hasPermit)
        //        {
        //            recordData = invalidTimeData.Where(x => x.permit_id > 0).ToList();
        //        }
        //        else
        //        {
        //            recordData = invalidTimeData.Where(x => x.permit_id == 0).ToList();
        //        }
        //        //
        //        var allTruckLst = myDb.truck_info.Select(x => x.truck_id).ToList();
        //        foreach (var truck in allTruckLst)
        //        {
        //            ReturnTruckAttendModel r = new ReturnTruckAttendModel();
        //            r.truck_id = truck;
        //            var relateAttendRecord = recordData.Where(x => x.truck_id == truck).ToList();
        //            if (relateAttendRecord == null || relateAttendRecord.Count() == 0)
        //            {
        //                //部分车辆从来没到过任何工地
        //                foreach (var item in _model.siteIdLst)
        //                {
        //                    r.site_count_list.Add(new SiteCountModel()
        //                    {
        //                        site_id = item,
        //                        site_count = 0,
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                var siteCountLst = relateAttendRecord.GroupBy(x => x.site_id).Select(x => new { site_id = x.Key, site_count = x.Count() }).ToList();
        //                foreach (var item in siteCountLst)
        //                {
        //                    //如果所查询的工地列表中包含该工地（有历史数据的工地），则填充至结果
        //                    if (_model.siteIdLst.Contains(item.site_id))
        //                    {
        //                        r.site_count_list.Add(new SiteCountModel()
        //                        {
        //                            site_id = item.site_id,
        //                            site_count = item.site_count,
        //                        });
        //                    }

        //                }
        //                var existedSiteLst = siteCountLst.Select(x => x.site_id).ToList();
        //                foreach (var eachSite in _model.siteIdLst)
        //                {
        //                    //如果已经查询出的结果中不包含该工地，说明该工地没有出勤记录，应该为0
        //                    if (!existedSiteLst.Contains(eachSite))
        //                    {
        //                        r.site_count_list.Add(new SiteCountModel()
        //                        {
        //                            site_id = eachSite,
        //                            site_count = 0,
        //                        });
        //                    }
        //                }
        //            }
        //            resultLst.Add(r);
        //        }
        //        code = Tools.ResponseCode.成功;
        //    }
        //    return resultLst;
        //}
        
        
        /// <summary>
        /// 具体工地的车辆出勤统计   
        /// </summary>
        /// <returns></returns>
        public List<Site_Attendance_Statistic> GetSiteDetailAttendances(short _site_id, ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;
            return null;
            //List<Site_Attendance_Statistic> result = new List<Site_Attendance_Statistic>() ;
            ////校验用户名、密码
            //using (JundaOSEntities myDb = Common.CreateOnly())
            //{
            //    if (myDb.site_info.Where(x=>x.site_id==_site_id).Count()==0)
            //    {
            //        code = Tools.ResponseCode.工地不存在;
            //        return null;
            //    }
            //    else
            //    {
            //        //该工地的出勤统计
            //        var site_attendanceLst = myDb.schedule_record.Where(x => x.from_site_id == _site_id).GroupBy(x => x.truck_id).Select((x) => new { truck_id = x.Key, attend_count = x.Count() }).ToList();

            //        //车辆的有效出勤时间查询
            //        var total_invalid_time_lst = (from p1 in myDb.view_truck_attend_statistics                                                  
            //                                      select new
            //                                      {
            //                                          truck_id = p1.truck_id,
            //                                          join_time = p1.join_time,
            //                                          offline_count = p1.offline_day_count,
            //                                      }).ToList();
            //        //汇总结果
            //        var truck_attend_tmp = (from p1 in site_attendanceLst
            //                                 join p2 in total_invalid_time_lst on p1.truck_id equals p2.truck_id
            //                                select new
            //                                {
            //                                    truck_id = p1.truck_id,
            //                                    attend_count = p1.attend_count,
            //                                    total_join_time = (DateTime.Now - p2.join_time).Days - p2.offline_count,
            //                                }).ToList();
            //      foreach(var tmp in truck_attend_tmp)
            //        {
            //            Site_Attendance_Statistic s = new Site_Attendance_Statistic();
            //            s.truck_id = tmp.truck_id;
            //            s.attend_count = tmp.attend_count;
            //            s.total_join_time = tmp.total_join_time;
            //            if (s.total_join_time == 0)
            //            {
            //                s.total_join_time = 1;
            //            }
            //            s.balance = Math.Round((s.attend_count / (1.0 * s.total_join_time)),2);
            //            result.Add(s);
            //        }
            //    }
            //    code = Tools.ResponseCode.成功;

            //}
            //return result;
        }

        /// <summary>
        /// 获取指定工地在指定时间段内的任务记录（包括结果
        /// </summary>
        /// <param name="_selectedTime">选择的时间</param>
        /// <param name="_schedule_type">选择的班次</param>
        /// <returns></returns>
        public List<ScheduleTaskResult> GetSiteTaskRecord(DateTime startTime, DateTime endTime, int siteId, ref Tools.ResponseCode code)
        {
            List<ScheduleTaskResult> resultLst = new List<ScheduleTaskResult>();

            List<short> leftTruckLst = new List<short>();

            code = Tools.ResponseCode.未处理;
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                var relatedTaskRecord = myDb.task_record.Where(x => x.send_time >= startTime && x.send_time <= endTime);
                foreach (var t in relatedTaskRecord)
                {
                    //若为指定工地的查询，当前工地不符的话，则跳过
                    if (siteId > 0 && t.from_site_id != siteId) continue;
                    ScheduleTaskResult sr = new ScheduleTaskResult();
                    var totalResulLst = myDb.truck_attend_record
                        .Join(myDb.truck_info, c => c.truck_id, s => s.truck_id, (c, s) => new { task_id = c.task_id, truck_id = c.truck_id, truck_num = s.truck_num })
                        .Where(x => x.task_id == t.task_id).Select(x => x.truck_num).ToList();
                    string strCalcResult = "、" + string.Join("、", totalResulLst.ToArray()) + "、";
                    sr.task_id = t.task_id.ToString();
                    sr.day_night = t.day_ninght;
                    sr.calc_result = strCalcResult;
                    sr.send_time = t.send_time;
                    sr.task_type = t.task_type;
                    sr.from_site_id = t.from_site_id;
                    sr.to_site_id = t.to_site_id;
                    sr.permit_id = t.permit_id;
                    sr.is_deleted = t.is_deleted;
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
        /// 根据车辆编号和起始时间，查询车辆的具体出勤记录,不包括已被撤销的出勤任务
        /// query_type=all/time/site_name/truck_id/task_type/need_permit/schedule_type
        /// </summary>
        /// <returns></returns>
        public List<Schedule_Record_Model> GetSingleTruckScheduleRecord(DateTime startTime, DateTime endTime, int truckNum, ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;

            try
            {
                List<Schedule_Record_Model> result = new List<Schedule_Record_Model>();

                //校验用户名、密码
                using (JundaOSEntities myDb = Common.CreateDbInstance())
                {
                    var truckInfoLst = myDb.truck_info.Where(x => x.truck_num == truckNum).ToList();
                    if (truckInfoLst==null||truckInfoLst.Count==0)
                    {
                        code = Tools.ResponseCode.车辆不存在;
                        return null;
                    }
                    else if (truckInfoLst.Count > 1)
                    {
                        code = Tools.ResponseCode.车辆编号有重复;
                        return null;
                    }
                    var tmpTruckId = truckInfoLst[0].truck_id;
                    var permitDict = myDb.permit_info.ToDictionary(x => x.permit_id, y => y.permit_name);
                    var siteDict = myDb.site_info.ToDictionary(x => x.site_id, y => y.site_name);
                    var truckRecordLst = myDb.truck_attend_record.Where(x=>x.truck_id==tmpTruckId)
                    .Join(myDb.task_record, c => c.task_id, s => s.task_id, (c, s) => new 
                    {
                        truck_id = (short)c.truck_id,
                        task_id = c.task_id.ToString(),
                        send_time = s.send_time,
                        task_type = (byte)s.task_type,
                        from_site_id = s.from_site_id,
                        to_site_id = s.to_site_id,
                        day_night = (byte)s.day_ninght,
                        permit_id = s.permit_id,
                        is_compensate=c.is_compensating,
                        is_task_deleted=s.is_deleted,
                    }).Where(x=>x.is_task_deleted==0).ToList();
                    result = truckRecordLst.Select(s => new Schedule_Record_Model
                    {
                        truck_id = (short)s.truck_id,
                        task_id = s.task_id.ToString(),
                        send_time = s.send_time,
                        task_type = (byte)s.task_type,
                        from_site_name = siteDict[s.from_site_id],
                        to_site_name = siteDict[s.to_site_id],
                        day_night = (byte)s.day_night,
                        permit_name = permitDict[s.permit_id],
                        is_compensate=s.is_compensate,

                    }).ToList();
                    code = Tools.ResponseCode.成功;
                }
                return result;
            }
            catch (Exception e)
            {

                code = Tools.ResponseCode.服务端内部异常;
                return null;
            }
        }
        /// <summary>
        ///获取所有车辆的总出勤次数统计
        /// </summary>
        /// <returns></returns>
        public List<ReturnTruckTotalAttendModel> GetTruckTotalAttendStatistic(DateTime startTime, DateTime endTime, ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;

            try
            {
                List<ReturnTruckTotalAttendModel> result = new List<ReturnTruckTotalAttendModel>();

                // 所有车辆的 通行证数量，以及各自累计出勤次数
                using (JundaOSEntities myDb = Common.CreateDbInstance())
                {
                    //统计车辆拥有的通行证数量
                    //无通行证也算通行证中的一种，因此次数-1
                    var truckPermitCountDict = myDb.truck_permit_relation.GroupBy(x => x.truck_id).ToDictionary(x => x.Key, y => y.Count()-1);
                    var truckAttendCountDict = myDb.truck_attend_record.Join(myDb.task_record,c=>c.task_id,s=>s.task_id,(c,s)=>new 
                    {
                        truck_id=c.truck_id,
                        is_task_deleted=s.is_deleted,
                        is_compensating= c.is_compensating
                    }).Where(x=>x.is_task_deleted==0).GroupBy(x => x.truck_id).ToDictionary(x => x.Key, y => y.Count());
                    var truckInfoLst = myDb.truck_info.ToList();
                    foreach (var truck in truckInfoLst)
                    {
                        ReturnTruckTotalAttendModel rtm = new ReturnTruckTotalAttendModel();
                        if (truckPermitCountDict.ContainsKey(truck.truck_id))
                        {
                            rtm.own_permit_count = (short)truckPermitCountDict[truck.truck_id];
                        }
                        if (truckAttendCountDict.ContainsKey(truck.truck_id))
                        {
                            rtm.total_attend_count = (short)truckAttendCountDict[truck.truck_id];
                        }
                        else
                        {
                            rtm.total_attend_count = 0;
                        }
                        rtm.truck_id = (short)truck.truck_id;
                        rtm.truck_num = (short)truck.truck_num;
                        result.Add(rtm);
                    }
                    code = Tools.ResponseCode.成功;
                }
                return result;
            }
            catch (Exception e)
            {

                code = Tools.ResponseCode.服务端内部异常;
                return null;
            }
        }
        /// <summary>
        /// 获取指定工地在指定时间段内的工地平衡统计
        /// </summary>
        /// <param name="_selectedTime">选择的时间</param>
        /// <param name="_schedule_type">选择的班次</param>
        /// <returns></returns>
        //public List<TruckAttendBalance> GetTruckAttendBalance(QueryTruckAttendBalanceModel QueryParam, ref Tools.ResponseCode code)
        //{
        //    short _siteId = QueryParam.site_id;
        //    List<TruckAttendBalance> resultLst = new List<TruckAttendBalance>();

        //    List<short> leftTruckLst = new List<short>();
        //    //车辆，加入时间，累计停运时间，有效时间，出勤次数，出勤比例
        //    code = Tools.ResponseCode.未处理;
        //    using (JundaOSEntities myDb = Common.CreateDbInstance())
        //    {
        //        var truckInfoLst = myDb.truck_info.ToList();
        //        var truckOfflineLst = myDb.truck_offline_record.Where(x => x.end_time != null && x.offline_invalid == 1)
        //            .GroupBy(x => x.truck_id)
        //            .Select(x =>
        //            new
        //            {
        //                truck_id = x.Key,
        //                offline_days = x.Sum(w => w.offline_days_count)
        //            }).ToList();
        //        var truckSiteStatistic = myDb.truck_site_attend_statistic.ToList();
        //        foreach (var truck in truckInfoLst)
        //        {
        //            TruckAttendBalance t = new TruckAttendBalance();
        //            t.join_time = truck.join_time;
        //            if (QueryParam.hasPermit == 1)
        //            {
        //                t.total_attend_count = truckSiteStatistic.Where(x => x.truck_id == truck.truck_id && x.site_id == QueryParam.site_id).First().attend_count_with_permit;
        //            }
        //            else
        //            {
        //                t.total_attend_count = truckSiteStatistic.Where(x => x.truck_id == truck.truck_id && x.site_id == QueryParam.site_id).First().attend_count_without_permit;
        //            }
        //            t.truck_id = truck.truck_id;
        //            if (truckOfflineLst.Exists(x => x.truck_id == truck.truck_id))
        //            {
        //                var truckOffline = truckOfflineLst.Where(x => x.truck_id == truck.truck_id).FirstOrDefault();
        //                t.offline_days = truckOffline.offline_days;
        //            }
        //            else
        //            {
        //                t.offline_days = 0;
        //            }
        //            t.valid_days = (DateTime.Now - t.join_time).Days;
        //            t.attend_time_proportion = Math.Round(t.total_attend_count * 1.0 / t.valid_days, 3);
        //            resultLst.Add(t);
        //        }
        //        code = Tools.ResponseCode.成功;
        //    }
        //    return resultLst;
        //}
        /// <summary>
        /// 将从数据库获取的RecordLst转化为发给前端用的ModelLst
        /// </summary>
        /// <param name="siteDict"></param>
        /// <param name="tmpResult"></param>
        /// <returns></returns>
        private List<Schedule_Record_Model> RecordLst2ModelLst(Dictionary<int, string> siteDict, List<truck_attend_record> tmpResult)
        {
            return null;
            //List<Schedule_Record_Model> result = new List<Schedule_Record_Model>();
            //foreach (var tmp in tmpResult)
            //{
            //    result.Add(new Schedule_Record_Model()
            //    {
            //        truck_id = tmp.truck_id,
            //        from_site_id = tmp.from_site_id,
            //        from_site_name = siteDict[tmp.from_site_id],
            //        to_site_id = tmp.to_site_id.Value,
            //        to_site_name = tmp.to_site_id == null ? "" : siteDict[tmp.to_site_id.Value],
            //        send_time = tmp.truck_send_time,
            //        schedule_type = tmp.schedule_type,
            //        permit_id = tmp.permit_id,
            //        auto_manual = tmp.auto_manual,
            //        task_id=tmp.task_id,

            //    });
            //}
            //return result;
        }
        /// <summary>
        /// 派车历史记录
        /// </summary>
        public class Schedule_Record_Model
        {
            public short truck_id;
            public int from_site_id;
            public string from_site_name;
            public int to_site_id;
            public string to_site_name;
            public byte permit_id;
            public string permit_name;
            public byte task_type;
            public DateTime send_time;
            public byte day_night;
            public string task_id;
            public byte is_compensate;
        }
        /// <summary>
        /// 具体某工地的车辆出勤统计类
        /// </summary>
        public class Site_Attendance_Statistic
        {
            public short truck_id;
            public int attend_count;
            public int total_join_time;
            public double balance;
            //public Site_Attendance_Statistic()
            //{
            //    if (total_join_time == 0)
            //    {
            //        total_join_time = 1;
            //    }
            //    balance = 1.0 * attend_count / total_join_time;
            //}
        }
        public class Truck_Attend_Statistic
        {
            public short truck_id;
        }
        public class GetTotalTruckAttendModel
        {
            public DateTime startTime;
            public DateTime endTime;
        }
        public class GetTruckAttendModel
        {
            public DateTime startTime;
            public DateTime endTime;
            public List<short> siteIdLst;
            public bool hasPermit;
        }

        public class QueryTruckAttendBalanceModel
        {

            public int hasPermit;
            public short site_id;
        }
        public class TruckAttendBalance
        {
            public short truck_id;
            public DateTime join_time;
            public decimal offline_days;
            public int valid_days;
            public int total_attend_count;
            public double attend_time_proportion;
        }
        public class GetSiteAttendModel
        {
            public DateTime startTime;
            public DateTime endTime;
            public short siteId;
        }
        public class ReturnTruckTotalAttendModel
        {
            public short truck_id;
            public short truck_num;
            public short own_permit_count;
            ////停运次数
            //public short offline_count;
            ////停运累计天数
            //public double offline_day;
            public short total_attend_count;
        }
        public class PermitAttendModel
        {
            public short permit_id;
            public int site_count;
        }
        public class ReturnTruckAttendModel
        {
            public short truck_id;
            public List<SiteCountModel> site_count_list;
            public ReturnTruckAttendModel()
            {
                site_count_list = new List<SiteCountModel>();
            }
        }
        public class SiteCountModel
        {
            public short site_id;
            public int site_count;

        }
    }
}
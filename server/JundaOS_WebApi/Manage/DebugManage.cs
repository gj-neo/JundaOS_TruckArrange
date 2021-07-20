//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace JundaOS_WebApi
//{
//    /// <summary>
//    /// 统计类
//    /// </summary>
//    public class DebugManage
//    {        
//        public DebugManage()
//        {
           
//        }
//        /// <summary>
//        /// 调试功能：补充truck_site_attend_statistic表数据  
//        /// </summary>
//        /// <returns></returns>
//        public  void  FillTruckSiteAttendStatistic(string debug_type,ref Tools.ResponseCode code)
//        {
//            code = Tools.ResponseCode.未处理;
//            List< view_site_divation> result = null;
//            //校验用户名、密码
//            //using (JundaOSEntities myDb = Common.CreateDbInstance())
//            {
//                int existCount = myDb.truck_site_attend_statistic.ToList().Count();
//                if (existCount > 0)
//                {
//                    code = Tools.ResponseCode.表内已有数据无法重复添加;
//                    return;
//                }
//                var truckLst = myDb.truck_info.Select(x => x.truck_id).ToList();
//                var siteLst = myDb.site_info.Where(x => x.site_type == 1).Select(x => x.site_id).ToList();
//                foreach(var truck in truckLst)
//                {
//                    foreach(var site in siteLst)
//                    {
//                        truck_site_attend_statistic t = new truck_site_attend_statistic();
//                        t.site_id = site;
//                        t.truck_id = truck;
//                        myDb.truck_site_attend_statistic.Add(t);
//                    }
//                }
//                myDb.SaveChanges();
//                code = Tools.ResponseCode.成功;
//            }
//            return ;
//        }
//        /// <summary>
//        /// 调试功能：清空车辆的所有统计项 
//        /// </summary>
//        /// <returns></returns>
//        public void ClearAllStatisticData(string debug_type, ref Tools.ResponseCode code)
//        {
//            code = Tools.ResponseCode.未处理;
            
//            //校验用户名、密码
//            using (JundaOSEntities myDb = Common.CreateDbInstance())
//            {
//                int n=myDb.Database.ExecuteSqlCommand("exec [ClearAllScheduleData]");
//                //foreach (var item in myDb.task_record.ToList())
//                //{
//                //    myDb.Entry<task_record>(item).State = System.Data.Entity.EntityState.Deleted;
//                //}
//                //foreach(var item in myDb.truck_attend_record.ToList())
//                //{
//                //    myDb.Entry<truck_attend_record>(item).State = System.Data.Entity.EntityState.Deleted;
//                //}
//                //foreach(var item in myDb.truck_info.ToList())
//                //{
//                //    item.recent_attend_time = null;
//                //    item.recent_schedule_type = null;
//                //    item.total_attend_count_without_permit = 0;
//                //    item.total_attend_count_with_permit = 0;
//                //    myDb.Entry<truck_info>(item).State = System.Data.Entity.EntityState.Modified;
//                //}
//                //foreach(var item in myDb.truck_site_attend_statistic.ToList())
//                //{
//                //    item.attend_count_without_permit = 0;
//                //    item.attend_count_with_permit = 0;
//                //    myDb.Entry<truck_site_attend_statistic>(item).State = System.Data.Entity.EntityState.Modified;

//                //}
//                //foreach(var item in myDb.truck_offline_record.ToList())
//                //{
//                //    myDb.Entry<truck_offline_record>(item).State = System.Data.Entity.EntityState.Deleted;
//                //}
//                myDb.SaveChanges();
//                code = Tools.ResponseCode.成功;
//            }
//            return;
//        }
//        /// <summary>
//        /// 调试功能：随机排班
//        /// </summary>
//        /// <returns></returns>
//        public void RandomSchedule(int _month_len,int type, ref Tools.ResponseCode code)
//        {
//            code = Tools.ResponseCode.未处理;
//            ScheduleCalcManage calcInstance = new ScheduleCalcManage();
//            Random calcRandom = new Random();
//            using (JundaOSEntities myDb = Common.CreateDbInstance())
//            {
//                var from_site_info_lst = myDb.site_info.Where(x => x.site_type == 1).ToList();
//                var truck_info_lst = myDb.truck_info.Where(x => x.truck_status == 1).ToList();
//                var permit_info_lst = myDb.region_permit.ToList();
//                //通行证仅包含少数，所以需要主动填充一些0
//                var permitIdLst = permit_info_lst.Select(x => x.permit_id).ToList();
//                int permitCount = permitIdLst.Count();
//                for(int i = 0; i < permitCount * 1.5; i++)
//                {
//                    permitIdLst.Add(0);
//                }
//                Tools.ResponseCode tmpCode= Tools.ResponseCode.Cookie校验失败;
//                DateTime startTime = calcInstance.CheckNearScheduleTime(ref tmpCode).AddDays(1);

//                //仅非通行证
//                if (type == 1)
//                {
//                    //random schedule without permit   type=1
//                    //逐天计算
//                    for (int i = 0; i < _month_len * 30; i++)
//                    {
//                        //早上8点排班
//                        RandomSchedule_DaySchedule(startTime, calcRandom, from_site_info_lst, truck_info_lst, calcInstance, i, type);
//                        //晚上7点排班
//                        RandomSchedule_NightSchedule(startTime, calcRandom, from_site_info_lst, truck_info_lst, calcInstance, i, type);
//                    }
//                }
//                //仅通行证
//                else if (type == 2)
//                {
//                    //random schedule without permit   type=1
//                    //逐天计算
//                    for (int i = 0; i < _month_len * 30; i++)
//                    {
//                        //早上8点排班
//                        RandomSchedule_DaySchedule(startTime, calcRandom, from_site_info_lst, truck_info_lst, calcInstance, i, type, permitIdLst);
//                        //晚上7点排班
//                        RandomSchedule_NightSchedule(startTime, calcRandom, from_site_info_lst, truck_info_lst, calcInstance, i, type, permitIdLst);
//                    }
//                }
//                //混合
//                else  if (type == 3)
//                {

//                    //random schedule without permit   type=1
//                    //逐天计算
//                    for (int i = 0; i < _month_len * 30; i++)
//                    {
//                        //早上8点排班
//                        RandomSchedule_DaySchedule(startTime, calcRandom, from_site_info_lst, truck_info_lst, calcInstance, i, type, permitIdLst);
//                        //晚上7点排班
//                        RandomSchedule_NightSchedule(startTime, calcRandom, from_site_info_lst, truck_info_lst, calcInstance, i, type, permitIdLst);
//                    }
//                }
//                code = Tools.ResponseCode.成功;
//            }
//            return;
//        }
//        /// <summary>
//        /// 随机排班-某天一天早上8点白板
//        /// </summary>
//        /// <param name="startTime"></param>
//        /// <param name="calcRandom"></param>
//        /// <param name="from_site_info_lst"></param>
//        /// <param name="truck_info_lst"></param>
//        /// <param name="calcInstance"></param>
//        /// <param name="addDay"></param>
//        /// <param name="code"></param>
//        private void  RandomSchedule_DaySchedule(DateTime startTime,Random calcRandom,List<site_info> from_site_info_lst,List<truck_info> truck_info_lst, ScheduleCalcManage calcInstance, int addDay,int selectedType,List<short> permitIdLst=null)
//        {
//            //早上8点排班
//            DateTime calcTime = startTime.AddDays(addDay).AddHours(8);
//            int site_need = calcRandom.Next(1, 6);
//            List<AutoTaskModel> autoModelLst = new List<AutoTaskModel>();
//            var leftTruckCount = truck_info_lst.Count();
//            short randomPermitId = 0;
//            for (int j = 0; j < site_need; j++)
//            {
//                var site_id = from_site_info_lst[calcRandom.Next(0, from_site_info_lst.Count - 1)].site_id;
//                if (autoModelLst.Exists(x => x.from_site_id == site_id)) { continue; }
//                if (leftTruckCount <= 1) { break ; }
//                var TmptruckCount = calcRandom.Next(1, leftTruckCount);
//                //获取随机通行证
//                if (selectedType == 1)
//                {
//                    //非通行证
//                    randomPermitId = 0;
//                }
//                else if (selectedType == 2)
//                {
//                    //纯通行证
//                    List<short> tmpInvalidPermitLst = permitIdLst.Where(x => x > 0).ToList();
//                    randomPermitId = tmpInvalidPermitLst[calcRandom.Next(0, tmpInvalidPermitLst.Count - 1)];
//                }
//                else if (selectedType == 3)
//                {
//                    //混合
//                    randomPermitId = permitIdLst[calcRandom.Next(0, permitIdLst.Count - 1)];
//                }
//                autoModelLst.Add(new AutoTaskModel()
//                {
//                    from_site_id = (short)site_id,
//                    to_site_id = 97,
//                    permit_id = randomPermitId,
//                    truck_count = (short)TmptruckCount,
//                });
//                leftTruckCount -= TmptruckCount;
//            }
//            if (autoModelLst.Count == 0)
//            {
//                //code = Tools.ResponseCode.随机排班过程中出现异常_请重新清空后再尝试;
//                return ;
//            }
//            CalcParamModel calcParam = new CalcParamModel();
//            calcParam.schedule_type = 1;
//            calcParam.send_time = calcTime;
//            calcParam.task_model_list = autoModelLst;
//            Tools.ResponseCode code = Tools.ResponseCode.成功;
//            //CalcParamModel resultModel= calcInstance.CalcScheduleNew(calcParam, ref  code);
//        }
//        /// <summary>
//        /// 随机排班-某天一天晚上6点晚班
//        /// </summary>
//        /// <param name="startTime"></param>
//        /// <param name="calcRandom"></param>
//        /// <param name="from_site_info_lst"></param>
//        /// <param name="truck_info_lst"></param>
//        /// <param name="calcInstance"></param>
//        /// <param name="addDay"></param>
//        /// <param name="code"></param>
//        private void RandomSchedule_NightSchedule(DateTime startTime, Random calcRandom, List<site_info> from_site_info_lst, List<truck_info> truck_info_lst, ScheduleCalcManage calcInstance, int addDay, int selectedType, List<short> permitIdLst = null)
//        {
//            //早上8点排班
//            DateTime calcTime = startTime.AddDays(addDay).AddHours(18);
//            int site_need = calcRandom.Next(1, 6);
//            List<AutoTaskModel> autoModelLst = new List<AutoTaskModel>();
//            var leftTruckCount = truck_info_lst.Count();
//            short randomPermitId = 0;
//            for (int j = 0; j < site_need; j++)
//            {
//                var site_id = from_site_info_lst[calcRandom.Next(0, from_site_info_lst.Count - 1)].site_id;
//                if (autoModelLst.Exists(x => x.from_site_id == site_id)) { continue; }
//                if (leftTruckCount <= 1) { break; }
//                var TmptruckCount = calcRandom.Next(1, leftTruckCount);
               
//                //获取随机通行证
//                if (selectedType == 1)
//                {
//                    //非通行证
//                    randomPermitId = 0;
//                }
//                else if (selectedType == 2)
//                {
//                    //纯通行证
//                    List<short> tmpInvalidPermitLst = permitIdLst.Where(x => x > 0).ToList();
//                    randomPermitId = tmpInvalidPermitLst[calcRandom.Next(0, tmpInvalidPermitLst.Count - 1)];
//                }
//                else if (selectedType == 3)
//                {
//                    //混合
//                    randomPermitId = permitIdLst[calcRandom.Next(0, permitIdLst.Count - 1)];
//                }
//                autoModelLst.Add(new AutoTaskModel()
//                {
//                    from_site_id = (short)site_id,
//                    to_site_id = 97,
//                    permit_id = randomPermitId,
//                    truck_count = (short)TmptruckCount,
//                });
//                leftTruckCount -= TmptruckCount;
//            }
//            if (autoModelLst.Count == 0)
//            {
//                //code = Tools.ResponseCode.随机排班过程中出现异常_请重新清空后再尝试;
//                return;
//            }
//            CalcParamModel calcParam = new CalcParamModel();
//            calcParam.schedule_type = 2;
//            calcParam.send_time = calcTime;
//            calcParam.task_model_list = autoModelLst;
//            Tools.ResponseCode code = Tools.ResponseCode.成功;
//            //calcInstance.CalcScheduleNew(calcParam, ref code);

//        }
//    }
//}
using JundaOS_WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JundaOS_WebApi
{
    /// <summary>
    /// 车辆信息类
    /// </summary>
    public class TruckManage
    {        
        public TruckManage()
        {
           
        }
        /// <summary>
        /// 批量新增车辆
        /// 需注意 ,同步创建浏览用户(TODO)
        /// </summary>
        /// <returns></returns>
        public List<MultiInsertTruckResultModel> MultiInsertTruck(MultiTruckInfoModel _multiInfoModel,ref Tools.ResponseCode code)
        {
             code = Tools.ResponseCode.未处理;
            List<TruckInfoModel> newTruckLst = _multiInfoModel.truckInfoLst;
            if (newTruckLst.Count == 0)
            {
                code = Tools.ResponseCode.批量添加请求中的车辆参数为空;
                return null;
            }
            return null;
            //List<MultiInsertTruckResultModel> resultModelLst = new List<MultiInsertTruckResultModel>();

            //List<short> tmpNewTruckNameLst = newTruckLst.Select(x => x.truck_id).ToList();
            //if (tmpNewTruckNameLst.Distinct().Count() != newTruckLst.Count())
            //{
            //    code = Tools.ResponseCode.车辆列表中包含有重复项;
            //    return null ;
            //}

            ////

            //using (JundaOSEntities myDb = Common.CreateOnly())
            //{
            //    foreach(TruckInfoModel newTruck in newTruckLst)
            //    {
            //        MultiInsertTruckResultModel resultModel = new MultiInsertTruckResultModel()
            //        {
            //            truck_id = newTruck.truck_id,
            //        };
            //        //whether truck order repeated 
            //        var selecteInfo = myDb.truck_info.Where(x => x.truck_id == newTruck.truck_id);
            //        if (selecteInfo.Count() > 0)
            //        {
            //            //truck is  exist
            //            resultModel.insert_result= Tools.ResponseCode.车辆编号已存在.ToString();                    
            //        }
            //        else
            //        {
            //            myDb.truck_info.Add(new truck_info()
            //            {
            //                truck_license = newTruck.truck_license,
            //                truck_driver_name = newTruck.truck_driver_name,
            //                truck_id = newTruck.truck_id,
            //                truck_status = newTruck.truck_status,
            //                join_time = newTruck.join_time,
            //            });
            //            //添加通行证
            //            if (newTruck.permit_count > 0)
            //            {
            //                List<truck_permit_relation> tmpLst = newTruck.truck_permit_list;
            //                myDb.truck_permit_relation.AddRange(tmpLst);
            //            }
            //            resultModel.insert_result = "成功";                        
            //        }
            //        resultModelLst.Add(resultModel);
            //    }
            //    myDb.SaveChanges();
            //    code = Tools.ResponseCode.成功;
            //}
            //return resultModelLst;
        }
        /// <summary>
        /// 新增车辆
        /// 需注意 ,同步创建浏览用户(TODO)
        /// </summary>
        /// <returns></returns>
        public Tools.ResponseCode  InsertTruck(TruckInfoModel newTruck)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
           
            //校验用户名、密码
            using (JundaOSEntities myDb=Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.truck_info.Where(x => x.truck_id == newTruck.truck_id);
               
                if (selecteInfo.Count() >0)
                {
                    //user is not exist
                    code = Tools.ResponseCode.车辆编号已存在;
                    return code;
                }
                myDb.truck_info.Add(new truck_info()
                {
                    truck_license = newTruck.truck_license,
                    driver_name = newTruck.truck_driver_name,
                    truck_num=newTruck.truck_id,
                    truck_state=1,//默认为运营中
                });
                var fromSiteLst = myDb.site_info.Where(x => x.site_type == 1 && x.is_deleted == 0);
               
                myDb.SaveChanges();
                code = Tools.ResponseCode.成功;
            }
            return code;
        }
        /// <summary>
        /// 查询车辆
        /// query_type=all/truck_id/truck_license/truck_driver_name/permit_id
        /// query_condition=具体
        /// </summary>
        /// <returns></returns>
        public List<TruckInfoModel>  QueryCar(string query_type,string query_condition,ref Tools.ResponseCode code)
        {
            List<TruckInfoModel> resultLst = new List<TruckInfoModel>();
            code = Tools.ResponseCode.未处理;
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //查询全部
                if (query_type.ToLower()=="all")
                {                    
                    var truckInfoLst = myDb.truck_info.ToList();
                    foreach(var truckInfo in truckInfoLst)
                    {
                        var truckPermit = myDb.truck_permit_relation.Where(x => x.truck_id == truckInfo.truck_id).ToList();
                        resultLst.Add(new TruckInfoModel()
                        {
                            truck_num=(short)truckInfo.truck_num,
                            truck_id =(short) truckInfo.truck_id,
                            truck_driver_name = truckInfo.driver_name,
                            truck_license = truckInfo.truck_license,
                            truck_status = (byte)truckInfo.truck_state,                            
                                                  
                        });
                    }
                    code = Tools.ResponseCode.成功;
                }
                else
                {
                    string strSql = string.Empty;
                    //按照通行证查询（truck_permit和truck_permit_relation表）
                    if (query_type.ToLower() == "permit_id"||query_type.ToLower()=="permit_name")
                    {
                        if (query_type.ToLower() == "permit_id")
                        {
                            //
                            strSql = $"select * from truck_info where truck_id in (select truck_id from truck_permit_relation where permit_id={query_condition})";
                        }
                       else if (query_type.ToLower() == "permit_name")
                        {
                            //
                            strSql = $"select * from truck_info where truck_id in (select truck_id from truck_permit_relation where permit_name='{query_condition}')";
                        }
                    }
                    //字符串类型
                    else if(query_type.ToLower()=="truck_license"||query_type.ToLower()== "truck_driver_name")
                    {
                        strSql = $"select * from truck_info where {query_type} like '%{query_condition}%'";
                    }
                    //按照车辆信息查询（在truck_info表）
                    else
                    {
                        strSql = $"select * from truck_info where {query_type}={query_condition}";
                    }
                    //whether truck order repeated 
                    var selecteInfo = myDb.Database.SqlQuery<truck_info>(strSql).ToList();
                    if (selecteInfo.Count() == 0)
                    {
                        //user is not exist
                        code = Tools.ResponseCode.车辆不存在;
                        //resultLst = null;
                    }
                    else
                    {
                        foreach (var truckInfo in selecteInfo)
                        {
                            var truckPermit = myDb.truck_permit_relation.Where(x => x.truck_id == truckInfo.truck_id).ToList();
                            resultLst.Add(new TruckInfoModel()
                            {
                                truck_num = (short)truckInfo.truck_num,
                                truck_id = (short)truckInfo.truck_id,
                                truck_driver_name = truckInfo.driver_name,
                                truck_license = truckInfo.truck_license,
                                truck_status = (byte)truckInfo.truck_state,

                            }) ;
                        }
                        code = Tools.ResponseCode.成功;
                       
                    }
                }
            }
            return resultLst;
        }
        /// <summary>
        /// 修改车辆基本信息，不包括通行证
        /// </summary>
        /// <returns></returns>
        public Tools.ResponseCode UpdateTruck(TruckInfoModel _truckModel)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;           
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.truck_info.Where(x => x.truck_id == _truckModel.truck_id);

                if (selecteInfo.Count() == 0)
                {
                    //user is not exist
                    code = Tools.ResponseCode.车辆不存在;
                    return code;
                }
                truck_info tmpTruckInfo = selecteInfo.First();
                tmpTruckInfo.truck_license = _truckModel.truck_license;
                tmpTruckInfo.driver_name = _truckModel.truck_driver_name;
                //tmpTruckInfo.truck_status = _truckModel.truck_status;
                
                myDb.SaveChanges();                
                code = Tools.ResponseCode.成功;
            }
            return code;
        }
        /// <summary>
        /// 删除车辆
        /// </summary>
        /// <param name="_username"></param>
        /// <param name="_userpwd"></param>
        /// <returns></returns>
        public Tools.ResponseCode DeleteTruck(int _truck_id)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;            
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.truck_info.Where(x => x.truck_id == _truck_id);
                if (selecteInfo.Count() > 0)
                {
                    myDb.Entry<truck_info>(selecteInfo.First()).State = System.Data.Entity.EntityState.Deleted;
                    
                    //删除offline_record表
                    var tmpOfflineRecord = myDb.truck_offline_record.Where(x => x.truck_id == _truck_id);
                    foreach(var tmp in tmpOfflineRecord)
                    {
                        myDb.Entry<truck_offline_record>(tmp).State = System.Data.Entity.EntityState.Deleted;
                    }
                    var tmpPermitRelation = myDb.truck_permit_relation.Where(x => x.truck_id == _truck_id);
                    foreach (var tmp in tmpPermitRelation)
                    {
                        myDb.Entry<truck_permit_relation>(tmp).State = System.Data.Entity.EntityState.Deleted;
                    }

                    var tmpTruckAttendRecord = myDb.truck_attend_record.Where(x => x.truck_id == _truck_id);
                    foreach(var tmp in tmpTruckAttendRecord)
                    {
                        myDb.Entry<truck_attend_record>(tmp).State = System.Data.Entity.EntityState.Deleted;
                    }
                    var tmpTruckCompensationRecord = myDb.truck_compensation.Where(x => x.truck_id == _truck_id);
                    foreach (var tmp in tmpTruckCompensationRecord)
                    {
                        myDb.Entry<truck_compensation>(tmp).State = System.Data.Entity.EntityState.Deleted;
                    }
                    var tmpPreScheduleRecord = myDb.pre_schedule.Where(x => x.pre_truck_id == _truck_id);
                    var allExistTruckLst = myDb.truck_info.Select(x => x.truck_id).OrderBy(x => x).ToList();
                    foreach (var tmp in tmpPreScheduleRecord)
                    {
                        int curIndex = allExistTruckLst.IndexOf(tmp.pre_truck_id);
                        if (curIndex != (allExistTruckLst.Count - 1))
                        {
                            tmp.pre_truck_id = allExistTruckLst[curIndex + 1];
                        }
                        else
                        {
                            tmp.pre_truck_id = allExistTruckLst[0];
                        }
                        myDb.Entry<pre_schedule>(tmp).State = System.Data.Entity.EntityState.Modified;
                    }
                    myDb.SaveChanges();
                    code = Tools.ResponseCode.成功;                    
                }
                else
                {
                    code = Tools.ResponseCode.车辆不存在;
                }
            }
            return code;
        }
        /// <summary>
        /// 设置车辆恢复运营
        /// </summary>
        /// <param name="truckLst"></param>
        /// <returns></returns>
        public Tools.ResponseCode SetTruckActive(List<int> truckLst)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            DateTime _selectTime = DateTime.Now;
            if (truckLst.Count == 0)
            {
                code = Tools.ResponseCode.车辆列表不能为空;
                return code;
            }
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.truck_info.Where(x => truckLst.Contains(x.truck_id));
                if (selecteInfo.Count() > 0)
                {
                    //offline_record表
                    var tmpOfflineRecord = myDb.truck_offline_record.Where(x => truckLst.Contains(x.truck_id) && x.end_time == null);
                    //先检查时间是否合理   1 如果没有尚处于请假或停运状态的车，则直接返回;2 如果设置恢复运营的时间早于最后一次的请假时间
                    if (tmpOfflineRecord.Count() == 0)
                    {
                        code = Tools.ResponseCode.当前未找到尚处于停运状态的车辆;
                        return code;
                    }
                    foreach (var tmp in tmpOfflineRecord)
                    {
                        if (tmp.start_time >= _selectTime)
                        {
                            code = Tools.ResponseCode.恢复运营的时间必须晚于停运或请假的时间;
                            return code;
                        }                        
                    }
                    foreach (var tmp in tmpOfflineRecord)
                    {
                        tmp.end_time = _selectTime;
                        tmp.duration = (int)(_selectTime - tmp.start_time).TotalDays;
                        myDb.Entry<truck_offline_record>(tmp).State = System.Data.Entity.EntityState.Modified;
                    }

                    foreach (var item in selecteInfo)
                    {
                        item.truck_state = 1;
                        myDb.Entry<truck_info>(item).State = System.Data.Entity.EntityState.Modified;
                    }
                   
                    myDb.SaveChanges();
                    code = Tools.ResponseCode.成功;
                }
                else
                {
                    code = Tools.ResponseCode.车辆不存在;
                }
            }
            return code;
        }
        /// <summary>
        /// 设置车辆停运
        /// </summary>
        /// <param name="truckLst"></param>
        /// <returns></returns>
        public Tools.ResponseCode SetTruckStop(List<int> truckLst)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            DateTime _selectTime = DateTime.Now;
            if (truckLst.Count == 0)
            {
                code = Tools.ResponseCode.车辆列表不能为空;
                return code;
            }
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.truck_info.Where(x => truckLst.Contains(x.truck_id));
                if (selecteInfo.Count() > 0)
                {
                    //offline_record表  
                    //检查 1  是否已经处于停运状态， 2   停运的时间是否晚于最后一次的恢复运营时间
                    var tmpOfflineRecord = myDb.truck_offline_record.Where(x => truckLst.Contains(x.truck_id)).OrderByDescending(x => x.start_time).ToList();
                   
                    foreach (var item in selecteInfo)
                    {
                        //check
                        if (tmpOfflineRecord.Exists(x => item.truck_id == x.truck_id))
                        {
                            var tmpT = tmpOfflineRecord.Where(x => x.truck_id == item.truck_id).First();
                            if (tmpT.end_time == null)
                            {
                                code = Tools.ResponseCode.有车辆仍处于停运状态_无法重复设置停运;
                                return code;
                            }
                            else
                            {
                                if (tmpT.end_time >= _selectTime)
                                {
                                    code = Tools.ResponseCode.停运的时间必须晚于最后一次恢复运营时间;
                                    return code;
                                }
                            }
                        }
                        item.truck_state = 0;
                        myDb.Entry<truck_info>(item).State = System.Data.Entity.EntityState.Modified;
                        truck_offline_record newOfflineRecord = new truck_offline_record()
                        {
                            truck_id = item.truck_id,
                            start_time = _selectTime,
                        };
                        myDb.truck_offline_record.Add(newOfflineRecord);
                    }
                    myDb.SaveChanges();
                    code = Tools.ResponseCode.成功;
                }
                else
                {
                    code = Tools.ResponseCode.车辆不存在;
                }
            }
            return code;
        }
        
        ///// 设置车辆请假
        ///// </summary>
        ///// <param name="truckLst"></param>
        ///// <returns></returns>
        //public Tools.ResponseCode SetTruckLeave(List<int> truckLst,bool IsOfflineInvalid)
        //{
        //    Tools.ResponseCode code = Tools.ResponseCode.未处理;
        //    DateTime _selectTime = DateTime.Now;
        //    if (truckLst.Count == 0)
        //    {
        //        code = Tools.ResponseCode.车辆列表不能为空;
        //        return code;
        //    }
        //    //校验用户名、密码
        //    using (JundaOSEntities myDb = Common.CreateDbInstance())
        //    {
        //        //whether truck order repeated 
        //        var selecteInfo = myDb.truck_info.Where(x => truckLst.Contains(x.truck_id));
        //        if (selecteInfo.Count() > 0)
        //        {
        //            //offline_record表  
        //            //检查 1  是否已经处于停运状态， 2   停运的时间是否晚于最后一次的恢复运营时间
        //            var tmpOfflineRecord = myDb.truck_offline_record.Where(x => truckLst.Contains(x.truck_id)).OrderByDescending(x => x.start_time).ToList();

        //            foreach (var item in selecteInfo)
        //            {
        //                //check
        //                if (tmpOfflineRecord.Exists(x => item.truck_id == x.truck_id))
        //                {
        //                    var tmpT = tmpOfflineRecord.Where(x => x.truck_id == item.truck_id).First();
        //                    if (tmpT.end_time == null)
        //                    {
        //                        code = Tools.ResponseCode.有车辆仍处于停运状态_无法重复设置停运;
        //                        return code;
        //                    }
        //                    else
        //                    {
        //                        if (tmpT.end_time >= _selectTime)
        //                        {
        //                            code = Tools.ResponseCode.停运的时间必须晚于最后一次恢复运营时间;
        //                            return code;
        //                        }
        //                    }
        //                }
        //                item.truck_status = 2;
        //                myDb.Entry<truck_info>(item).State = System.Data.Entity.EntityState.Modified;
        //                truck_offline_record newOfflineRecord = new truck_offline_record()
        //                {
        //                    truck_id = item.truck_id,
        //                    start_time = _selectTime,
        //                    offline_type = 2,
        //                    offline_invalid = Convert.ToInt16(IsOfflineInvalid),
        //                    offline_days_count = 0,
        //                };
        //                myDb.truck_offline_record.Add(newOfflineRecord);
        //            }
        //            myDb.SaveChanges();
        //            code = Tools.ResponseCode.成功;
        //        }
        //        else
        //        {
        //            code = Tools.ResponseCode.车辆不存在;
        //        }
        //    }
        //    return code;
        //}
    }
    /// <summary>
    /// 车辆信息模型类
    /// </summary>
    public class TruckInfoModel
    {
        /// <summary>
        /// 车辆ID
        /// </summary>
        public short truck_id { get; set; }
        public short truck_num { get; set; }
        /// <summary>
        /// 车辆牌照
        /// </summary>
        public string truck_license { get; set; }
        /// <summary>
        /// 司机姓名
        /// </summary>
        public string truck_driver_name { get; set; }
        /// <summary>
        /// 车辆状态（停运/运营中）
        /// </summary>
        public byte truck_status { get; set; }
        /// <summary>
        /// 车辆加入时间
        /// </summary>
        public System.DateTime join_time { get; set; }
       
    }
    /// <summary>
    /// 批量导入车辆的实体类
    /// </summary>
    public class MultiTruckInfoModel
    {
        /// <summary>
        /// 车辆列表
        /// </summary>
        public List<TruckInfoModel> truckInfoLst { get; set; }
    }
    /// <summary>
    /// 批量导入车辆的返回实体类
    /// </summary>
    public class MultiInsertTruckResultModel
    {
        /// <summary>
        /// 车辆id
        /// </summary>
        public short truck_id { get; set; }
        /// <summary>
        /// 返回信息（成功 or ...)
        /// </summary>
        public string insert_result { get; set; }
    }
}
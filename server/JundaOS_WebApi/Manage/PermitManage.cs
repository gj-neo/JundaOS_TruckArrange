using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JundaOS_WebApi
{
    /// <summary>
    /// 通行证管理类
    /// </summary>
    public class PermitManage
    {        
        public PermitManage()
        {
           
        }
        /// <summary>
        /// 新增通行证
        /// </summary>
        /// <returns></returns>
        public Tools.ResponseCode  InsertPermit(string newPermitName)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //校验用户名、密码
            using (JundaOSEntities myDb=Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.permit_info.Where(x => x.permit_name == newPermitName);
               
                if (selecteInfo.Count() >0)
                {
                    //user is not exist
                    code = Tools.ResponseCode.通行证已存在;
                    return code;
                }
                myDb.permit_info.Add(new permit_info()
                {
                     permit_name=newPermitName,
                });
                myDb.SaveChanges();
                code = Tools.ResponseCode.成功;
            }
            return code;
        }
        /// <summary>
        /// 修改通行证
        /// </summary>
        /// <returns></returns>
        public Tools.ResponseCode UpdatePermit(permit_info newPermit)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.permit_info.Where(x => x.permit_id == newPermit.permit_id);

                if (selecteInfo.Count() == 0)
                {
                    //user is not exist
                    code = Tools.ResponseCode.通行证不存在;
                    return code;
                }
                else
                {
                    myDb.Entry<permit_info>(newPermit).State = System.Data.Entity.EntityState.Modified;                    
                    myDb.SaveChanges();
                }
                code = Tools.ResponseCode.成功;
            }
            return code;
        }
        /// <summary>
        /// 查询所有通行证,支持模糊查询
        /// query_type:permit_name/all;query_condition:xxx
        /// </summary>
        /// <returns></returns>
        public List<permit_info>  QueryPermit(string query_type,string query_condition,ref Tools.ResponseCode code)
        {
            List<permit_info> resultLst;
            code = Tools.ResponseCode.未处理;
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //查询全部
                if (query_type == "all")
                {
                    resultLst = myDb.permit_info.ToList();
                }
                else if (query_type == "permit_name")
                {
                    string strSql = $"select * from region_permit where permit_name like '%{query_condition}%'";
                    var selectInfo= myDb.Database.SqlQuery<permit_info>(strSql).ToList();
                    if (selectInfo.Count() == 0)
                    {
                        //没找到
                        code = Tools.ResponseCode.通行证不存在;
                        return null;
                    }
                    else
                    {
                        resultLst = selectInfo;                        
                    }                    
                }
                else
                {
                    //没找到
                    code = Tools.ResponseCode.通行证不存在;
                    return null;
                }
                code = Tools.ResponseCode.成功;
            }
            return resultLst;
        }
        /// <summary>
        /// 删除通行证
        /// </summary>
        /// <param name="_username"></param>
        /// <param name="_userpwd"></param>
        /// <returns></returns>
        public Tools.ResponseCode DeletePermit(int _permit_id)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;            
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.permit_info.Where(x => x.permit_id == _permit_id);
                if (selecteInfo.Count() > 0)
                {
                    myDb.Entry<permit_info>(selecteInfo.First()).State = System.Data.Entity.EntityState.Deleted;
                    //删除该通行证与车辆的绑定关系
                    var refTruckLst=myDb.truck_permit_relation.Where(x => x.permit_id == _permit_id).ToList();
                    foreach(truck_permit_relation tpr in refTruckLst)
                    {
                        myDb.Entry<truck_permit_relation>(tpr).State = System.Data.Entity.EntityState.Deleted;
                    }                    
                    myDb.SaveChanges();
                    code = Tools.ResponseCode.成功;                    
                }
                else
                {
                    code = Tools.ResponseCode.通行证不存在;
                }
            }
            return code;
        }
        /// <summary>
        /// 获取指定通行证绑定的车辆列表
        /// </summary>
        /// <param name="_permit_id"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<TruckInfoModel> GetPermitBindTrucks(int _permit_id, ref Tools.ResponseCode code)
        {
             code = Tools.ResponseCode.未处理;
            List<TruckInfoModel> resultLst = new List<TruckInfoModel>();
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.permit_info.Where(x => x.permit_id == _permit_id);
                if (selecteInfo.Count() == 0)
                {
                    code = Tools.ResponseCode.通行证不存在;
                    return null;
                }
                var permitExistTruckLst = myDb.truck_permit_relation.Where(x => x.permit_id == _permit_id).Select(x => x.truck_id).ToList();
                resultLst = myDb.truck_info.Where(x => permitExistTruckLst.Contains(x.truck_id)).Select(x => new TruckInfoModel()
                {
                     truck_id=(short)x.truck_id,
                     truck_num=(short)x.truck_num,
                }).OrderBy(x=>x.truck_id).ToList(); 
                code = Tools.ResponseCode.成功;              
            }
            return resultLst;
        }
        /// <summary>
        /// 解除车辆与特定通行证的绑定关系
        /// </summary>
        /// <param name="_permit_id"></param>
        /// <param name="truckLst"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public void UnbindTruckPermit(int _permit_id,List<int> truckLst, ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;
            List<short> resultLst = new List<short>();
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                var permitRelationLst = myDb.truck_permit_relation.Where(x => x.permit_id == _permit_id).ToList();
                if (permitRelationLst.Count() == 0)
                {
                    code = Tools.ResponseCode.通行证不存在;
                    return ;
                }
                var toBeDeletedLst = permitRelationLst.Where(x => truckLst.Contains(x.truck_id));
                myDb.truck_permit_relation.RemoveRange(toBeDeletedLst);
                myDb.SaveChanges();
                code = Tools.ResponseCode.成功;
            }            
        }
        /// <summary>
        /// 解除车辆与特定通行证的绑定关系
        /// </summary>
        /// <param name="_permit_id"></param>
        /// <param name="truckLst"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public void BindTruckPermit(int _permit_id, List<int> truckLst, ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;
            List<truck_permit_relation> newRelationLst = new List<truck_permit_relation>();
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                var bindedTruckLst = myDb.truck_permit_relation.Where(x => x.permit_id == _permit_id).Select(x=>x.truck_id).ToList();
                foreach (var item in truckLst)
                {
                    //如果已绑定，则跳过
                    if (bindedTruckLst.Contains((short)item))
                    {
                        continue;
                    }
                    truck_permit_relation t = new truck_permit_relation();
                    t.truck_id = (short)item;
                    t.permit_id = (short)_permit_id;
                    newRelationLst.Add(t);
                }                
                myDb.truck_permit_relation.AddRange(newRelationLst);
                myDb.SaveChanges();
                code = Tools.ResponseCode.成功;
            }
        }
    }
    public class PermitBindTruck
    {
        public List<int> bindTruckLst;
        public List<int> unbindTruckLst;
        public PermitBindTruck()
        {
            bindTruckLst = new List<int>();
            unbindTruckLst = new List<int>();
        }
    }
}
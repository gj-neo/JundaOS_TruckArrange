using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JundaOS_WebApi
{
    /// <summary>
    /// 工地管理类
    /// </summary>
    public class SiteManage
    {        
        public SiteManage()
        {
           
        }
        /// <summary>
        /// 批量新增工地
        /// </summary>
        /// <returns></returns>
        public List<MultiSiteInsertResultModel> MultiInsertSite(MultiSiteInfoModel newMultiSiteModel,ref Tools.ResponseCode code)
        {
            code = Tools.ResponseCode.未处理;
            List<site_info> newSiteLst = newMultiSiteModel.siteInfoLst;
            if (newSiteLst.Count == 0)
            {
                code = Tools.ResponseCode.批量添加请求中的工地参数为空;
                return null;
            }
            List<MultiSiteInsertResultModel> resultModelLst = new List<MultiSiteInsertResultModel>();
            List<string> tmpNewSiteNameLst = newSiteLst.Select(x=>x.site_name).ToList() ;
            if (tmpNewSiteNameLst.Distinct().Count() != newSiteLst.Count())
            {
                code = Tools.ResponseCode.工地列表中包含有重复项;
                return null;
            }
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                foreach(site_info newSite in newSiteLst)
                {
                    MultiSiteInsertResultModel result_model = new MultiSiteInsertResultModel()
                    {
                        site_name = newSite.site_name,
                    };
                    if (string.IsNullOrEmpty(newSite.site_name))
                    {
                        result_model.insert_result = Tools.ResponseCode.工地名称不能为空.ToString();
                    }
                    else if (newSite.site_type > 3 || newSite.site_type <= 0)
                    {
                        result_model.insert_result = Tools.ResponseCode.工地类型超出有效值.ToString();
                    }
                    //whether truck order repeated 
                    var selecteInfo = myDb.site_info.Where(x => x.site_name == newSite.site_name);
                    if (selecteInfo.Count() > 0)
                    {
                        //user is not exist
                        result_model.insert_result = Tools.ResponseCode.工地已存在.ToString();                       
                    }
                    else
                    {
                        myDb.site_info.Add(newSite);
                        result_model.insert_result = Tools.ResponseCode.成功.ToString();
                    }
                    resultModelLst.Add(result_model);
                }
                myDb.SaveChanges();
                code = Tools.ResponseCode.成功;
            }
            return resultModelLst;
        }
        /// <summary>
        /// 新增工地
        /// </summary>
        /// <returns></returns>
        public Tools.ResponseCode  InsertSite(string _siteName,int _siteType)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;            
            
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                if (string.IsNullOrEmpty(_siteName))
                {
                    code = Tools.ResponseCode.工地名称不能为空;
                    return code;
                }
                else if(_siteType>3|| _siteType <= 0)
                {
                    code = Tools.ResponseCode.工地类型超出有效值;
                    return code;
                }
                //whether truck order repeated 
                var selecteInfo = myDb.site_info.Where(x => x.site_name==_siteName);
                if (selecteInfo.Count() > 0)
                {
                    //user is not exist
                    code = Tools.ResponseCode.工地已存在;
                    return code;
                }
                site_info s = new site_info()
                {
                    site_name = _siteName,
                    site_type = (byte)_siteType,
                    is_deleted = 0,
                };
                myDb.site_info.Add(s);
                myDb.SaveChanges();
                //在truck_site_attend_statistic中增加该工地的对应统计项
                
                myDb.SaveChanges();
                code = Tools.ResponseCode.成功;
            }
            return code;
        }
        /// <summary>
        /// 修改工地信息
        /// </summary>
        /// <returns></returns>
        public Tools.ResponseCode UpdateSiteBaseInfo(int _site_id, string _site_new_name)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                if (string.IsNullOrEmpty(_site_new_name))
                {
                    code = Tools.ResponseCode.工地名称不能为空;
                    return code;
                }                
                //whether truck order repeated 
                var selecteInfo = myDb.site_info.Where(x => x.site_id == _site_id);

                if (selecteInfo.Count() == 0)
                {
                    //user is not exist
                    code = Tools.ResponseCode.工地不存在;
                    return code;
                }
                var s = selecteInfo.First();
                s.site_name = _site_new_name;
                myDb.Entry<site_info>(s).State = System.Data.Entity.EntityState.Modified;
                myDb.SaveChanges();
                code = Tools.ResponseCode.成功;
            }
            return code;
        }

        /// <summary>
        ///  查询工地  site_name/permit_id/site_type/all
        /// 支持模糊查询,支持按通行证查询
        /// </summary>
        /// <param name="query_type">查询类型</param>
        /// <param name="query_condition">查询条件</param>
        /// <param name="code">返回状态码</param>
        /// <returns></returns>
        public List<SiteInfoModel>  QuerySite(string query_type,string query_condition,ref Tools.ResponseCode code)
        {
            List<SiteInfoModel> resultLst = new List<SiteInfoModel>();
            code = Tools.ResponseCode.未处理;
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //查询全部
                if (query_type.ToLower() == "all")
                {
                    var siteInfoLst = myDb.site_info.Where(x=>x.is_deleted==0).ToList();
                    foreach (var siteInfo in siteInfoLst)
                    {
                        
                        resultLst.Add(new SiteInfoModel()
                        {
                            site_id = siteInfo.site_id,
                            site_name = siteInfo.site_name,                            
                            site_type = siteInfo.site_type,                            
                        });
                    }
                    code = Tools.ResponseCode.成功;
                }
                else
                {
                    string strSql = string.Empty;
                    //按照通行证查询
                    if (query_type.ToLower() == "permit_id")
                    {
                        //TODO
                        strSql = $"select * from site_info where   permit_id={query_condition}";
                    }
                    //按照工地信息查询
                    else if(query_type.ToLower()=="site_name")
                    {
                        strSql = $"select * from site_info where site_name like '%{query_condition}%'";
                    }
                    //按照工地类型查询
                    else if (query_type.ToLower() == "site_type")
                    {
                        strSql = $"select * from site_info where site_type ={query_condition}";
                    }
                    else
                    {
                        code = Tools.ResponseCode.工地查询条件中的字段错误;
                        return resultLst;
                    }
                    var selecteInfo = myDb.Database.SqlQuery<site_info>(strSql).Where(x => x.is_deleted == 0).ToList();
                    if (selecteInfo.Count() == 0)
                    {
                        //user is not exist
                        code = Tools.ResponseCode.工地不存在;
                        return resultLst;
                    }
                    else
                    {
                        foreach (var siteInfo in selecteInfo)
                        {
                            permit_info sitePermit = new permit_info();
                            
                            resultLst.Add(new SiteInfoModel()
                            {
                                site_id = siteInfo.site_id,
                                site_name = siteInfo.site_name,
                                site_type = siteInfo.site_type,
                               
                            });
                        }
                        code = Tools.ResponseCode.成功;
                    }
                }
            }            
            return resultLst;
        }
        /// <summary>
        /// 删除工地,注意同步删除权重表
        /// </summary>
        /// <param name="_username"></param>
        /// <param name="_userpwd"></param>
        /// <returns></returns>
        public Tools.ResponseCode DeleteSite(int _site_id)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;            
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //whether truck order repeated 
                var selecteInfo = myDb.site_info.Where(x => x.site_id == _site_id);
                if (selecteInfo.Count() > 0)
                {
                    var curSite = selecteInfo.First();
                    int curSiteType = curSite.site_type;
                    curSite.is_deleted = 1;
                    myDb.Entry<site_info>(curSite).State = System.Data.Entity.EntityState.Modified;
                    if (curSiteType == 1)
                    {                       

                    }
                    else if (curSiteType == 2)
                    {
                        //消纳场
                    }

                    myDb.SaveChanges();
                    code = Tools.ResponseCode.成功;
                    
                }
                else
                {
                    code = Tools.ResponseCode.工地不存在;
                }
            }
            return code;
        }
    }
    public class SiteInfoModel
    {

        public int site_id { get; set; }
        public string site_name { get; set; }
       
        public short site_type { get; set; }
        
    }
    public class MultiSiteInfoModel
    {
        public List<site_info> siteInfoLst { get; set;}
    }
    /// <summary>
    /// 批量导入工地后的反馈结果
    /// </summary>
    public class MultiSiteInsertResultModel
    {
        public string site_name { get; set; }
        public string insert_result { get; set; }
    }
}
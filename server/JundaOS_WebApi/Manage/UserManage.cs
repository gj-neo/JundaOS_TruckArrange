using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JundaOS_WebApi.User
{
    public enum LoginType
    {
        FIRST_LOGIN=1,
        FORCE_LOGIN=2,//强制登录
    }
    public class UserManage
    {        
        public UserManage()
        {
           
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="_username"></param>
        /// <param name="_userpwd"></param>
        /// <returns></returns>
        public UserInfoCache Login(string _username, string _userpwd,LoginType selectedType,ref Tools.ResponseCode code)
        {
            UserInfoCache user = null;
            var tmp = Common.UserInfoDict.Values.Where(x => x.UserName == _username);
           
            //校验用户名、密码
            using (JundaOSEntities myDb=Common.CreateDbInstance())
            {
                //whether user exist
                var selecteInfo = myDb.user_info.Where(x => x.name == _username);
               
                if (selecteInfo.Count() == 0)
                {
                    //user is not exist
                    code = Tools.ResponseCode.用户不存在;
                }
                else
                {
                    user_info userInfo = selecteInfo.First();
                    if (userInfo.password != _userpwd)
                    {
                        //passwird is wrong
                        code = Tools.ResponseCode.密码错误;
                    }
                    else if (Common.UserInfoDict.Values.Where(x => x.UserGroup == "visitor").Count() >= 1)
                    {
                        code = Tools.ResponseCode.已有其他操作员登录;
                    }
                    else
                    {
                        //强制登录，需要挤掉原登录账号的缓存
                        if (tmp.Count() > 0)
                        {
                            if (selectedType == LoginType.FORCE_LOGIN)
                            {
                                Common.UserInfoDict.Remove(tmp.First().TokenId);
                            }
                            else
                            {
                                code = Tools.ResponseCode.该用户已登录;
                                return null;
                            }
                        }
                        //user info correct
                        code = Tools.ResponseCode.成功;
                       
                        myDb.SaveChanges();
                        user = new UserInfoCache()
                        {
                            UserName=_username,
                          
                        };

                        Common.UserInfoDict.Add(user.TokenId, user);
                    }
                }
            }
            return user;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="_username"></param>
        /// <param name="_original_pwd"></param>
        /// <param name="_new_pwd"></param>
        /// <returns></returns>
        public Tools.ResponseCode UpdatePassword(string _username,string _original_pwd,string _new_pwd)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //校验用户名、密码
            using (JundaOSEntities myDb = Common.CreateDbInstance())
            {
                //whether user exist
                var selecteInfo = myDb.user_info.Where(x => x.name == _username);

                if (selecteInfo.Count() == 0)
                {
                    //user is not exist
                    code = Tools.ResponseCode.用户不存在;
                }
                else
                {
                    user_info userInfo = selecteInfo.First();
                    if (userInfo.password != _original_pwd)
                    {
                        //passwird is wrong
                        code = Tools.ResponseCode.原密码错误;
                    }
                    else 
                    {
                        userInfo.password = _new_pwd;
                        myDb.Entry<user_info>(userInfo).State = System.Data.Entity.EntityState.Modified;
                        myDb.SaveChanges();
                        code = Tools.ResponseCode.成功;
                    }
                }
            }
            return code;
        }
        /// <summary>
        /// 退出，根据header中的token直接退出
        /// </summary>
        /// <param name="_username"></param>
        /// <param name="_userpwd"></param>
        /// <returns></returns>
        public Tools.ResponseCode  Logout(string _tokenid)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //判断是否登录
            if (!Common.UserInfoDict.Keys.Contains(_tokenid))
            {
                code = Tools.ResponseCode.用户未登录;
            }
            else
            {
                //更新缓存数据
                Common.UserInfoDict.Remove(_tokenid);
                code = Tools.ResponseCode.成功;
            }
            //返回code
            return code;
        }
    }
    /// <summary>
    /// 缓存记录用户信息
    /// </summary>
    public class UserInfoCache
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TokenId { get; set; }
        public string LoginTime { get; set; }
        public string UserGroup { get; set; }
        public string LoginIp { get; set; }
        public UserInfoCache()
        {
            TokenId = Guid.NewGuid().ToString();
            LoginTime = DateTime.Now.ToString();
            UserGroup = "visitor";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using JundaOS_WebApi.User;

namespace JundaOS_WebApi.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        Tools.ApiTool apiTool = new Tools.ApiTool();
        User.UserManage userManage = new User.UserManage();
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="inputModel">Post请求只能输入一个参数</param>
        /// <returns></returns>
        [HttpPost]    
        [Route("login")]
        public HttpResponseMessage Login([FromBody]User.UserInfoCache inputModel)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            User.UserInfoCache userCache= userManage.Login(inputModel.UserName, inputModel.Password, LoginType.FIRST_LOGIN,ref code);
            if(code== Tools.ResponseCode.成功)
            {
                var userStatus = new
                {
                    userCache.TokenId,
                    userCache.UserGroup,
                };
                HttpResponseMessage hrm= apiTool.MsgFormat(code, null);
                //hrm.Headers.AddCookies(new CookieHeaderValue[] { cookie1,cookie2});
                hrm.Headers.Add("token", userCache.TokenId);
                hrm.Headers.Add("group", userCache.UserGroup);
                return hrm;
            }
            else
            {
                return apiTool.MsgFormat(code, null);
            }
        }
        /// <summary>
        /// 强制登录，同一个用户重复登录
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("force_login")]
        public HttpResponseMessage Force_Login([FromBody]User.UserInfoCache inputModel)
        {
            Tools.ResponseCode code = Tools.ResponseCode.未处理;
            //TODO:
            User.UserInfoCache userCache = userManage.Login(inputModel.UserName, inputModel.Password, LoginType.FORCE_LOGIN, ref code);
            if (code == Tools.ResponseCode.成功)
            {
                var userStatus = new
                {
                    userCache.TokenId,
                    userCache.UserGroup,
                };
                return apiTool.MsgFormat(code, apiTool.Object2Json(userStatus));
            }
            else
            {
                return apiTool.MsgFormat(code, null);
            }
        }
        [HttpGet]
        [UserTokenFilter]
        [Route("logout")]
        public HttpResponseMessage Logout()
        {
            Tools.ResponseCode code = Tools.ResponseCode.成功;
            //TODO:
            string tokenId= Request.Headers.GetValues("token").First();
           
            code=userManage.Logout(tokenId);
           
            return apiTool.MsgFormat(code,null);
        }
        [HttpPost]
        [UserTokenFilter]
        [Route("update_pwd")]
        public HttpResponseMessage ChangePassword([FromUri]string user_name,[FromUri]string original_pwd,[FromUri]string new_pwd)
        {
            Tools.ResponseCode code = Tools.ResponseCode.成功;
            //TODO:
            code = userManage.UpdatePassword(user_name, original_pwd, new_pwd);
            return apiTool.MsgFormat(code, null);
        }
    }
}                      
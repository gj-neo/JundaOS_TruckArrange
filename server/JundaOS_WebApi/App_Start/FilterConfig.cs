using JundaOS_WebApi.Tools;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace JundaOS_WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
    /// <summary>
    /// 用户身份校验
    /// </summary>
    public class UserTokenFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            return;
            Tools.ResponseCode code = ResponseCode.未处理;
            if (!actionContext.Request.Headers.Contains("token"))
            {
                code = ResponseCode.Header中缺少Token;
                actionContext.Response = new ApiTool().MsgFormat(code, null);
            }
            else
            {
                var tokenId = actionContext.Request.Headers.GetValues("token").First();

                //if (tokenId == null)
                //{
                //    code = ResponseCode.Header中的Token为空;
                //    actionContext.Response = new ApiTool().MsgFormat(code, null);
                //}
                //else if (!Common.UserInfoDict.ContainsKey(tokenId))
                //{
                //    code = ResponseCode.Header中的Token无法匹配已登录用户;
                //    actionContext.Response = new ApiTool().MsgFormat(code, null);
                //}
            }
        }
    }
    /// <summary>
    /// 操作员权限校验
    /// </summary>
    public class OperatorAuthFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            return;
            Tools.ResponseCode code = ResponseCode.未处理;
            if (!actionContext.Request.Headers.Contains("group"))
            {
                code = ResponseCode.Header中不存在Group;
                actionContext.Response = new ApiTool().MsgFormat(code, null);
            }
            else
            {
                var group = actionContext.Request.Headers.GetValues("group").First();
                if (group == null)
                {
                    code = ResponseCode.Header中的Group为空;
                    actionContext.Response = new ApiTool().MsgFormat(code, null);
                }
                else if (group.ToLower() != "operator")
                {
                    switch (group.ToLower())
                    {
                        case "visitor":
                            code = ResponseCode.当前用户没有操作权限;
                            break;
                        case "admin":
                            code = ResponseCode.当前用户没有操作权限;
                            break;
                        default:
                            code = ResponseCode.无法识别当前用户所在组;
                            break;
                    }
                    actionContext.Response = new ApiTool().MsgFormat(code, null);
                }
            }
            
        }
    }
    public class AuthFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var tokenId = actionContext.Request.Headers.GetValues("token").First();
            var group = actionContext.Request.Headers.GetValues("group");
            Tools.ResponseCode code = ResponseCode.未处理;
            //if (tokenId==null)
            //{
            //    code = ResponseCode.Header中的Token为空;
            //    actionContext.Response = new ApiTool().MsgFormat(code, null);
            //}
            //else if (!Common.UserInfoDict.ContainsKey(tokenId))
            //{
            //    code = ResponseCode.Header中的Token无法匹配已登录用户;
            //    actionContext.Response = new ApiTool().MsgFormat(code, null);
            //}
            //base.OnAuthorization(actionContext);
        }
    }
}

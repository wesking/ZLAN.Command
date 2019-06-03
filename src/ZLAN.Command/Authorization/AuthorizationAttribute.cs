using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLAN.Command.Abs;
using ZLAN.Command.Cachable;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ZLAN.Command.Authorization
{
    public enum IdentityMode
    {
        /// <summary>
        /// 通过签名方式进行身份识别
        /// </summary>
        Credential,

        /// <summary>
        /// 通过cookie方式进行授权
        /// </summary>
        Cookies,

        /// <summary>
        /// 混合(默认)
        /// </summary>
        Hybird
    }

    public class AuthorizationAttribute : ActionFilterAttribute
    {
        public string Scheme { get; set; }

        public IdentityMode IdentityMode { get; set; } = IdentityMode.Hybird;

        /// <summary>
        /// IdentityMode == Credential时，需要验证appid是否具备此授权
        /// </summary>
        public string CredentialCode { get; set; }

        /// <summary>
        /// 授权码
        /// </summary>
        public string[] AuthorizationCode { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Controller controller = context.Controller as Controller;
         
            string contraint = null;
            var request = controller.Request;
            var appid = request.Headers["appid"].FirstOrDefault();

            //验证服务调用授权
            if ((IdentityMode == IdentityMode.Hybird || IdentityMode == IdentityMode.Credential) && !string.IsNullOrEmpty(appid))
            {
                var cacheExecutor = context.HttpContext.RequestServices.GetService<CommandCacheExecutor>();

                var service = request.Headers["service"].FirstOrDefault();
                var nonce = request.Headers["nonce"].FirstOrDefault();
                var sign = request.Headers["sign"].FirstOrDefault();

                request.Body.Position = 0;
                StreamReader streamReader = new StreamReader(request.Body);
                var data = streamReader.ReadToEnd();

                var getAppAuthorizationParameter = new GetAppAuthorizationParameter()
                {
                    AppId = appid,
                    ServiceId = service
                };
                var getAppAuthorizationResult = cacheExecutor.Execute(new GetAppAuthorizationCommand(),
                        getAppAuthorizationParameter, new CommandCacheOptions()
                        {
                             Key = "appAuthorization"
                        });
                if (getAppAuthorizationResult.Code != 0)
                {
                    context.Result = new ObjectResult(ErrorResult<int>.GetErrorResult(getAppAuthorizationResult.Code, "获取授权信息错误"));
                    return;
                }

                string rawString = string.Format("service={0}&appid={1}&data={2}&nonce={3}&appsecret={4}",
                    service, appid, data, nonce, getAppAuthorizationResult.Data.Appsecret);

                var md5 = System.Security.Cryptography.MD5.Create();
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(rawString));

                var signCheck = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();

                if (signCheck != sign)
                {
                    context.Result = new ObjectResult(ErrorResult<int>.SignatureError);
                    return;
                }

                //无访问权
                if (string.IsNullOrEmpty(CredentialCode) || !getAppAuthorizationResult.Data.AuthorizationData.ContainsKey(CredentialCode))
                {
                    context.Result = new ObjectResult(ErrorResult<int>.NoAuthorization);
                    return;
                }
                try
                {
                    var param = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    if (param.ContainsKey("Identity"))
                    {
                        contraint = param["Identity"] as string;
                    }
                }
                catch(Exception ex)
                {
                    context.Result = new ObjectResult(ErrorResult<int>.Exception(ex));
                    return;
                }
                if (string.IsNullOrEmpty(contraint))
                {
                    contraint = getAppAuthorizationResult.Data.Parameter;
                }
                
            }

            if ((IdentityMode == IdentityMode.Hybird || IdentityMode == IdentityMode.Cookies) && string.IsNullOrEmpty(appid))
            {
                contraint = request.Headers["Identity"].FirstOrDefault();
                if (string.IsNullOrEmpty(contraint))
                {
                    var auth = await controller.HttpContext.AuthenticateAsync(Scheme);
                    if (!auth.Succeeded)
                    {
                        context.Result = new ObjectResult(ErrorResult<int>.NoAuthorization);
                        return;
                    }

                    contraint = (auth.Principal.Identity as ClaimsIdentity).Claims.FirstOrDefault().Value;
                }
            }

            IParameter parameter = null;
            if (context.ActionArguments.Count() == 1 && context.ActionArguments.First().Value is IParameter)
            {
                parameter = context.ActionArguments.First().Value as IParameter;
            }

            //若当前为权限控制接口，则根据当前设定的角色来设置接口权限信息
            if (parameter is AuthorizationParameter)
            {
                (parameter as AuthorizationParameter).Initialize(context.HttpContext.RequestServices, contraint, AuthorizationCode);
            }
            await next();
        }

    }
}

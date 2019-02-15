using System.Collections;
using System.Linq;
using ZL.CommandCore;
using ZL.CommandCore.Abs;
using ZL.CommandCore.Data;

namespace ZL.CommandCore.Authorization
{
	public class GetAppAuthorizationParameter:IParameter
	{
        public string ServiceId { get; set; }

        public string AppId { get; set; }
	}

    //public class GetServiceAuthorizationCache : ICachable
    //{
    //    public GetServiceAuthorizationCache(GetServiceAuthorizationParameter parameter)
    //    {
    //        _appid = parameter.AppId;
    //        _serviceId = parameter.ServiceId;
    //    }

    //    private string _serviceId { get; set; }

    //    private string _appid { get; set; }
        

    //    public string CacheKey { get => "zlan_command_GetServiceAuthorization_" + _appid + _serviceId; }

    //    public int CacheSeconds { get => 3600; }
    //}

    public class AppAuthorizationResult
    {
        public string Appsecret { get; set; }

        public string Parameter { get; set; }

        public Hashtable AuthorizationData { get; set; }
    }
    
    public class GetAppAuthorizationCommand : Command<AppAuthorizationResult>
    {
        protected override IResult<AppAuthorizationResult> OnExecute(IParameter commandParameter)
        {
            IResult<AppAuthorizationResult> result = new Result<AppAuthorizationResult>();
            GetAppAuthorizationParameter parameter = commandParameter as GetAppAuthorizationParameter;

            result.Data = new AppAuthorizationResult();
            //接口逻辑代码
            using (ServiceContext context = new ServiceContext())
            {
                var appSecret = context.SvrAppInfo.Where(p => p.AppId == parameter.AppId && p.Status == 1).Select(p => p.AppSecret).FirstOrDefault();
                if (appSecret == null)
                {
                    return ErrorResult<AppAuthorizationResult>.ParameterError;
                }
                var appParameter = context.SvrAppService.Where(p => p.AppId == parameter.AppId && p.ServiceId == parameter.ServiceId && p.Status == 1).Select(p => p.Parameter).FirstOrDefault();
                if (appParameter == null)
                {
                    return ErrorResult<AppAuthorizationResult>.ParameterError;
                }
                result.Data.Parameter = appParameter;
                result.Data.Appsecret = appSecret;
                var authList = context.SvrAppAuthorization.Where(p => p.AppId == parameter.AppId && p.Status == 1).ToList();
                result.Data.AuthorizationData = new Hashtable();
                foreach (var item in authList)
                {
                    result.Data.AuthorizationData[item.AuthorizationCode] = 1;
                }
            }
            return result;
        }
    }
}

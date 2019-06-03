using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLAN.Command.Abs;
using ZLAN.Command.Data;

namespace ZLAN.Command.Cachable
{
    public class CacheAttribute : ActionFilterAttribute
    {
        public string Key { get; set; }


        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ServiceContext serviceContext = context.HttpContext.RequestServices.GetService<ServiceContext>();
            var appCache = serviceContext.SvrAppCache.Where(api => api.Key == Key).FirstOrDefault();

            if (appCache == null)
            {
                await next();
                return;
            }
            IMemoryCache cache = context.HttpContext.RequestServices.GetService<IMemoryCache>();

            
            ControllerBase controller = context.Controller as ControllerBase;
            //计算缓存数据的接口
            var request = controller.Request;
            

            var paramHash = "";
            if (request.Body != null)
            {
                IParameter parameter = null;
                if (context.ActionArguments.Count() == 1 && context.ActionArguments.First().Value is IParameter)
                {
                    parameter = context.ActionArguments.First().Value as IParameter;
                }

                var md5 = System.Security.Cryptography.MD5.Create();
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(parameter)));

                paramHash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }

            CacheContext cacheContext = new CacheContext()
            {
                AppCache = appCache,
                NeedSetCache = true,
                //缓存的版本信息一致
                ParameterKey = Key + "#" + appCache.CacheVersion + "#" + paramHash
            };
            
            if (cache.TryGetValue(cacheContext.ParameterKey, out object result))
            {
                cacheContext.NeedSetCache = false;
                context.Result = new ObjectResult(result);
                return;
            }

            concurentDictionary.TryAdd(context.HttpContext.TraceIdentifier, cacheContext);
            await next();
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (concurentDictionary.ContainsKey(context.HttpContext.TraceIdentifier))
            {
                concurentDictionary.TryRemove(context.HttpContext.TraceIdentifier, out CacheContext cacheContext);
                Serilog.Log.Information($"ConcurentDictionary length:{concurentDictionary.Keys.Count}");
                if (!(context.Result is ObjectResult objectResult))
                {
                    await next();
                    return;
                }
                if (!(objectResult.Value is IResultBase commandResult))
                {
                    await next();
                    return;
                }

                if (commandResult.Code == 0)
                {
                    IMemoryCache cache = context.HttpContext.RequestServices.GetService<IMemoryCache>();

                    cache.Set(
                          cacheContext.ParameterKey,
                          commandResult,
                          new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds((double)cacheContext.AppCache.CacheTime)));
                }

            }

            await next();
        }

        class CacheContext
        {
            public string ParameterKey { get; set; }

            public bool NeedSetCache { get; set; }

            public SvrAppCache AppCache { get; set; }
        }
        private readonly ConcurrentDictionary<string, CacheContext> concurentDictionary = new ConcurrentDictionary<string, CacheContext>();
    }
}

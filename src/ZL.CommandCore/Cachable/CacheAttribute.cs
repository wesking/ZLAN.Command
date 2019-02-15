using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZL.CommandCore.Abs;
using ZL.CommandCore.Data;

namespace ZL.CommandCore.Cachable
{
    public class CacheAttribute : ActionFilterAttribute
    {
        public string Key { get; set; }


        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            SvrApiInfo apiDb = null;

            ServiceContext serviceContext = context.HttpContext.RequestServices.GetService<ServiceContext>();
            apiDb = serviceContext.SvrApiInfo.Where(api => api.Key == Key).FirstOrDefault();

            if (apiDb == null)
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
                StreamReader streamReader = new StreamReader(request.Body);
                var data = streamReader.ReadToEnd();

                var md5 = System.Security.Cryptography.MD5.Create();
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(data));

                paramHash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }

            CacheContext cacheContext = new CacheContext()
            {
                ApiDb = apiDb,
                NeedSetCache = true,
                ParameterKey = Key + "_" + paramHash
            };

            cache.TryGetValue(apiDb.Key, out string cacheVersion);

            //缓存的版本信息一致，尝试使用缓存的内容
            if (apiDb.CacheTime > 0 && cacheVersion == apiDb.CacheVersion)
            {
                if (cache.TryGetValue(cacheContext.ParameterKey, out object result))
                {
                    cacheContext.NeedSetCache = false;
                    context.Result = new ObjectResult(result);
                    return;
                }
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

                    cache.Set(Key, cacheContext.ApiDb.CacheVersion);
                    cache.Set(
                          cacheContext.ParameterKey,
                          commandResult,
                          new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds((double)cacheContext.ApiDb.CacheTime)));
                }

            }

            await next();
        }

        class CacheContext
        {
            public string ParameterKey { get; set; }

            public bool NeedSetCache { get; set; }

            public SvrApiInfo ApiDb { get; set; }
        }
        private readonly ConcurrentDictionary<string, CacheContext> concurentDictionary = new ConcurrentDictionary<string, CacheContext>();
    }
}

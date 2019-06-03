using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZLAN.Command.Abs;
using ZLAN.Command.Data;

namespace ZLAN.Command.Cachable
{
    public class CommandCacheOptions
    {
        public string Key { get; set; }
    }

    public class CommandCacheExecutor
    {
        public CommandCacheExecutor(IMemoryCache memoryCache, ServiceContext serviceContext)
        {
            _memoryCache = memoryCache;
            _serviceContext = serviceContext;
        }

        private IMemoryCache _memoryCache { get; set; }
        private ServiceContext _serviceContext { get; set; }

        public IResult<T> Execute<T>(ICommand<T> command, IParameter parameter, CommandCacheOptions options)
        {
            var appCache = _serviceContext.SvrAppCache.Where(api => api.Key == options.Key).FirstOrDefault();

            //不使用缓存
            if (appCache == null)
            {
                return command.Execute(parameter);
            }


            var md5 = System.Security.Cryptography.MD5.Create();
            var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(parameter)));

            var paramHash = BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();

            var cacheKey = options.Key + "#" + appCache.CacheVersion + "#" + paramHash;

            if(!_memoryCache.TryGetValue(cacheKey, out IResult<T> commandResult))
            {
                commandResult = command.Execute(parameter);

                //接口调用成功才能缓存
                if(commandResult.Code == 0)
                {
                    _memoryCache.Set(
                          cacheKey,
                          commandResult,
                          new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds((double)appCache.CacheTime)));
                }
            }
            return commandResult;
        }
    }
}

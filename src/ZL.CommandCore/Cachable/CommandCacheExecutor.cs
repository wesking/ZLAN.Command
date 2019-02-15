using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using ZL.CommandCore.Abs;

namespace ZL.CommandCore.Cachable
{
    public class CommandCacheOptions
    {
        public int CacheSeconds { get; set; }

        public string CacheKey { get; set; }
    }

    public class CommandCacheExecutor
    {
        public CommandCacheExecutor(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        private IMemoryCache _memoryCache { get; set; }

        public IResult<T> Execute<T>(ICommand<T> command, IParameter parameter, CommandCacheOptions options)
        {
            if(options.CacheSeconds <= 0)
            {
                return command.Execute(parameter);
            }

            if(!_memoryCache.TryGetValue(options.CacheKey, out IResult<T> commandResult))
            {
                commandResult = command.Execute(parameter);

                //接口调用成功才能缓存
                if(commandResult.Code == 0)
                {
                    _memoryCache.Set(
                          options.CacheKey,
                          commandResult,
                          new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(options.CacheSeconds)));
                }
            }
            return commandResult;
        }
    }
}

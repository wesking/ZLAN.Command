using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using ZL.CommandCore.Abs;
using ZL.CommandCore.Abs.Observable;
using ZL.CommandCore.Authorization;
using ZL.CommandCore.Cachable;
using ZL.CommandCore.Data;
using ZL.CommandCore.Observable;

namespace ZL.CommandCore
{
    public static class CommandBuilderExtensions
    {
        public static IServiceCollection AddCommand(this IServiceCollection services, Action<CommandOptions> options)
        {
            //options.
            //InvokeContext.ConnectionString = options.ConnectionString;
            CommandOptions option = new CommandOptions();
            options.Invoke(option);

            Subject.Instance.Seeker = new ObserverSeeker();

            services.AddMemoryCache();
            //设置接口依赖注入


            services.AddTransient<CommandCacheExecutor>();


            services.AddDbContext<ServiceContext>(optionsBuilder => {
                optionsBuilder.UseMySql(option.ConnectionString);
            });

            ServiceContext.ConnectionString = option.ConnectionString;

            //设置接口失败重试
            if (option.InvokerHostEnable)
            {
                services.AddHostedService<InvokerHostedService>();
            }
            return services;
        }
        
    }
}

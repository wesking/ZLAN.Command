using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using ZLAN.Command.Abs;
using ZLAN.Command.Abs.Observable;
using ZLAN.Command.Authorization;
using ZLAN.Command.Cachable;
using ZLAN.Command.Data;
using ZLAN.Command.Logger;
using ZLAN.Command.Observable;

namespace ZLAN.Command
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

            services.AddScoped<CommandLogger>( );
            services.AddMvc(mvc =>
            {
                mvc.Filters.Add(typeof(CommandLogger));
            });

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

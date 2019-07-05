using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLAN.Command.Abs;

namespace ZLAN.Command.Logger
{
    public class CommandLogger : IAsyncActionFilter, IAsyncResultFilter, IOrderedFilter
    {
        public int Order { get; set; } = 99;

        private string _watchKey = "command_logger_watcher";

        private string _parameterKey = "command_logger_parameter";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IParameter parameter = null;
            if (context.ActionArguments.Count() == 1 && context.ActionArguments.First().Value is IParameter)
            {
                parameter = context.ActionArguments.First().Value as IParameter;
            }
            context.HttpContext.Items[_parameterKey] = parameter;

            context.HttpContext.Items[_watchKey] = Stopwatch.StartNew();
            await next();
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var watch = context.HttpContext.Items[_watchKey] as Stopwatch;
            watch.Stop();
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

            var loger = Serilog.Log.Logger;

            var elapsed = watch.Elapsed.TotalMilliseconds;
            var param = context.HttpContext.Items[_parameterKey];
            var path = context.HttpContext.Request.Path;
            if (commandResult.Code == 0)
            {
                loger.Information("{path}   {elapsed}   {@parameter} {@result}", path, elapsed, param, commandResult);
            }
            else
            {
                loger.Error(commandResult.GetException(), "{path}   {elapsed}   {@parameter} {@result}", path, elapsed, param, commandResult);
            }
            await next();
        }
    }
}

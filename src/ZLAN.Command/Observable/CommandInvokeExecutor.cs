using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZLAN.Command.Abs;
using ZLAN.Command.Abs.Observable;

namespace ZLAN.Command.Observable
{
    public class CommandInvokeExecutor : IExecutor
    {
        /// <summary>
        /// {api:"",service:"",appid:""}
        /// </summary>
        /// <param name="json"></param>
        public CommandInvokeExecutor(string json)
        {
            var config = JsonConvert.DeserializeObject<IDictionary<string, string>>(json);

            _appid = config["appid"];
            _service = config["service"];
            _api = config["api"];
        }

        public CommandInvokeExecutor(string appid, string service, string api)
        {
            _appid = appid;
            _service = service;
            _api = api;
        }

        protected virtual CommandInfo GetCommandInfo(MessageContext context)
        {
            return new CommandInfo()
            {
                Api = _api,
                Appid = _appid,
                Service = _service,
                Parameter = context.Message
            };
        }

        public IResult<T> Execute<T>(MessageContext context)
        {
            var commandInfo = GetCommandInfo(context);

            var invoker = new HttpInvoker<T>();
            int id = invoker.Prepare(commandInfo);
            return invoker.Invoke(id);
        }

        protected readonly string _appid = null;
        protected readonly string _service = null;
        protected readonly string _api = null;
    }
}

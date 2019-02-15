

using ZL.CommandCore.Abs;
using ZL.CommandCore.Abs.Observable;

namespace ZL.CommandCore.Observable
{
	public class MessageInvokeSender : CommandInvokeExecutor
    {

        /// <summary>
        /// {api:"",service:"",appid:""}
        /// </summary>
        /// <param name="json"></param>
        public MessageInvokeSender(string json) : base(json)
        {
        }

        public MessageInvokeSender(string appid, string service, string api):base(appid, service, api)
        {
        }


        protected override CommandInfo GetCommandInfo(MessageContext context)
        {
            return new CommandInfo()
            {
                Api = _api,
                Appid = _appid,
                Service = _service,
                Parameter = new { context.Key, MessageObject = context.Message }
            };
        }

    }

}

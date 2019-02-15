using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZL.CommandCore.Abs.Observable;
using ZL.CommandCore.Data;

namespace ZL.CommandCore.Observable
{
    public class ObserverSeeker : IObserverSeeker
    {
        public void Dispose()
        {
            _serviceContext.Dispose();
        }

        private readonly ServiceContext _serviceContext = new ServiceContext();
        public List<IExecutor> Seek(string key)
        {
            List<IExecutor> result = new List<IExecutor>();

            var list = _serviceContext.SvrCommandSubscriber.Where(p => p.CommandKey == key && p.Status == 1).OrderBy(p => p.Sort).ToList();
            foreach (var item in list)
            {
                IExecutor executor = null;
                switch (item.ExecutorType)
                {
                    case "COMMAND_EXECUTOR":
                        {
                            executor = new CommandInvokeExecutor(item.ExecutorConfig);
                        }
                        break;
                    case "MESSAGE_SENDER":
                        {
                            executor = new MessageInvokeSender(item.ExecutorConfig);
                        }
                        break;
                }
                if (executor != null)
                {
                    result.Add(executor);
                }

            }
            return result;
        }
    }
}

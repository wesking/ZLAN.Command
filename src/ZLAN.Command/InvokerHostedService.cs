using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZLAN.Command.Data;

namespace ZLAN.Command
{
    public class InvokerHostedService : IHostedService, IDisposable
    {
        public InvokerHostedService()
        {
        }

        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _serviceContext = new ServiceContext();
            _hostedStatus = 1;
            Log.Information("InvokerHostedService is starting."); 
            Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _serviceContext.Dispose();
            _hostedStatus = 0;
            Log.Information("InvokerHostedService is stopping.");
            return Task.CompletedTask;
        }

        private void Start()
        {
            while (_hostedStatus == 1)
            {
                var invokerIds =  _serviceContext.SvrInvokeInfo.Where(p => p.Status == 0 && p.NextInvokeTime < DateTime.Now && p.EndTime > DateTime.Now).Select(p => p.Id).ToList();
                
                foreach (var id in invokerIds)
                {
                    new HttpInvoker<object>().Invoke(id);
                }

                Thread.Sleep(10000);
            }
        }

        private ServiceContext _serviceContext = null;
        private int _hostedStatus = 0;
    }
}

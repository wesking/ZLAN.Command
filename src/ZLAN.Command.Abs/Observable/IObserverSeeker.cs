using System;
using System.Collections.Generic;
using System.Text;

namespace ZLAN.Command.Abs.Observable
{
    public interface IObserverSeeker : IDisposable
    {
        List<IExecutor> Seek(string key);
    }
}

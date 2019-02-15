using System;
using System.Collections.Generic;
using System.Text;

namespace ZL.CommandCore.Abs.Observable
{
    public interface IObserverSeeker : IDisposable
    {
        List<IExecutor> Seek(string key);
    }
}

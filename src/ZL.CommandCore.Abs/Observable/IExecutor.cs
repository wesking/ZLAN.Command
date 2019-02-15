using System;
using System.Collections.Generic;
using System.Text;

namespace ZL.CommandCore.Abs.Observable
{
    public interface IExecutor
    {
        IResult<T> Execute<T>(MessageContext context);
    }
}

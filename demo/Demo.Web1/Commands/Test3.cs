using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZL.CommandCore;
using ZL.CommandCore.Abs;

namespace Demo.Web1.Commands
{

    public class Test3Parameter : IParameter
    {
    }

    public class Test3Result : Result<string>
    {
    }
    public class Test3Command : Command<string>
    {

        protected override IResult<string> OnExecute(IParameter parameter)
        {
            Test3Parameter test2Parameter = parameter as Test3Parameter;

            Test3Result result = new Test3Result() { };
            //在此实现相关的业务逻辑

            return result;
        }
    }
}

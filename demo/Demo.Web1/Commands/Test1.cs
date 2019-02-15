using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZL.CommandCore;
using ZL.CommandCore.Abs;

namespace Demo.Web1.Commands
{
    public class Test1Parameter : IParameter
    {
    }

    public class Test1Result: Result<string>
    {
    }

    public class Test1Command : Command<string>
    {

        protected override IResult<string> OnExecute(IParameter parameter)
        {
            Test1Parameter test1Parameter = parameter as Test1Parameter;

            Result<string> result = new Result<string>();

            return result;
        }
    }
}

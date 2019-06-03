using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZLAN.Command;
using ZLAN.Command.Abs;

namespace Demo.Web1.Commands
{

    public class Test2Parameter : IParameter
    {
    }

    public class Test2Result : Result<string>
    {
    }
    public class Test2Command : Command<string>
    {

        protected override IResult<string> OnExecute(IParameter parameter)
        {
            Test2Parameter test2Parameter = parameter as Test2Parameter;

            Test2Result result = new Test2Result();
            return result;
        }
    }
}

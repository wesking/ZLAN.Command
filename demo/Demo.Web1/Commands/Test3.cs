using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZLAN.Command;
using ZLAN.Command.Abs;
using ZLAN.Command.Authorization;

namespace Demo.Web1.Commands
{

    public class Test3Parameter : AuthorizationParameter
    {
    }

    public class Test3Result : Result<string>
    {
    }
    public class Test3Command : AuthorizationCommand<string>
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

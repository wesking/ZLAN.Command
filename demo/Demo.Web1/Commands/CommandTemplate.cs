using System;
using System.Linq;
using ZL.CommandCore;
using ZL.CommandCore.Abs;

namespace $rootnamespace$
{
	public class $safeitemrootname$Parameter : IParameter
	{
	}
    

    public class $safeitemrootname$Result : Result<T>
	{
	}


    public class $safeitemrootname$Command : Command<T>
	{       
        public override IResult<T> OnExecute(IParameter parameter)
        {   
            $safeitemrootname$Result result = new $safeitemrootname$Result();
            $safeitemrootname$Parameter $safeitemrootname$Parameter = parameter as $safeitemrootname$Parameter;
           
            //接口逻辑


            return result;
        }

	}
}

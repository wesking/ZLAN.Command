using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZLAN.Command;
using ZLAN.Command.Abs;

namespace Demo.Web1.Commands
{
    /// <summary>
    /// 参数说明
    /// </summary>
    public class Test4Parameter : IParameter
    {
        /// <summary>
        /// Parame1说明
        /// </summary>
        public int Param1 { get; set; }

        /// <summary>
        /// Parame2说明
        /// </summary>
        public DateTime Param2 { get; set; }

        /// <summary>
        /// Parame3说明
        /// </summary>
        public string Param3 { get; set; }
    }

    public class Test4Result : Result<string>
    {
    }

    /// <summary>
    /// 接口说明
    /// </summary>
    public class Test4Command : Command<string>
    {

        protected override IResult<string> OnExecute(IParameter parameter)
        {
            Test4Parameter test4Parameter = parameter as Test4Parameter;

            Test4Result result = new Test4Result() { };
            //在此实现相关的业务逻辑
            result.Data = "some thing";
            return result;
        }
    }
}

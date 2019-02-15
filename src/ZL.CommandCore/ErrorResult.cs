using System;
using System.Collections.Generic;
using System.Text;
using ZL.CommandCore.Abs;

namespace ZL.CommandCore
{
    public class ErrorResult<T>
    {
        public static IResult<T> ParameterError
        {
            get
            {
                return new Result<T>()
                {
                    Code = 101,
                    Message = "param error"
                };
            }
        }


        public static IResult<T> NoAuthentication
        {
            get
            {
                return new Result<T>()
                {
                    Code = 105,
                    Message = "no authentication"
                };
            }
        }

        public static IResult<T> NoAuthorization
        {
            get
            {
                return new Result<T>()
                {
                    Code = 106,
                    Message = "no authorization"
                };
            }
        }
        

       
    }
}

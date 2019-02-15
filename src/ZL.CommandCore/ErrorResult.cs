using System;
using System.Collections.Generic;
using System.Text;

namespace ZL.CommandCore
{
    public class ErrorResult<T>
    {
        public static Result<T> GetErrorResult(int code, string message)
        {
            return new Result<T>()
            {
                Code = code,
                Message = message
            };
        }

        public static Result<T> ParameterError
        {
            get
            {
                return new Result<T>()
                {
                    Code = 1,
                    Message = "param error"
                };
            }
        }

        public static Result<T> Exception(Exception ex)
        {
            return new Result<T>()
            {
                Code = 4,
                Message = ex.Message
            };
        }

        public static Result<T> NoAuthorization
        {
            get
            {
                return new Result<T>()
                {
                    Code = 5,
                    Message = "no authorization"
                };
            }
        }


        public static Result<T> SignatureError
        {
            get
            {
                return new Result<T>()
                {
                    Code = 6,
                    Message = "signature error"
                };
            }
        }
    }
}

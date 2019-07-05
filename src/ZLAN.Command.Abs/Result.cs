using System;

namespace ZLAN.Command.Abs
{
    /// <summary>
    /// 接口返回结果
    /// </summary>
    /// <typeparam name="T">接口调用正常的返回类型</typeparam>
    public class Result<T> : IResult<T>
    {
        public Result()
        {
            Message = "ok";
            Code = 0;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }

        public int Total { get; set; }

        public Exception GetException()
        {
            return _exception;
        }

        public void SetException(Exception ex)
        {
            _exception = ex;
        }

        private Exception _exception = null;
    }
    
}

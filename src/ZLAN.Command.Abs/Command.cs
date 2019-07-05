using Newtonsoft.Json;
using System;
using System.Diagnostics;
using ZLAN.Command.Abs.Observable;

namespace ZLAN.Command.Abs
{
    /// <summary>
    /// 命令
    /// </summary>
    public abstract class Command<T> : ICommand<T>
    {
        /// <summary>
        /// 通知内容，若定义了接口调用成功后将启用通知
        /// </summary>
        protected virtual MessageContext MessageContext { get; } = null;

        /// <summary>
        /// 准备执行,返回false的则不执行
        /// </summary>
        protected virtual IResult<T> PrepareExecute(IParameter parameter)
        {
            return null;
        }

        /// <summary>
        /// 执行命令处理
        /// </summary>
        protected abstract IResult<T> OnExecute(IParameter parameter);

        /// <summary>
        /// 命令成功执行后，将调用此方法
        /// </summary>
        protected virtual void AfterExecute(IParameter parameter, IResult<T> result)
        {
            if (result.Code == 0 && MessageContext != null)
            {
                MessageContext.Parameter = parameter;
                MessageContext.Result = result;

                Subject.Instance.Notify(MessageContext);
            }
        }

        public IResult<T> Execute(IParameter parameter)
        {
            IResult<T> result = null;
            try
            {
                result = PrepareExecute(parameter);
                if (result == null)
                {
                    result = OnExecute(parameter);
                    if (result.Code == 0)
                    {
                        AfterExecute(parameter, result);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Result<T> { Code = 4, Message = ex.Message };
                result.SetException(ex);
            }
            return result;
        }
    }
    
}

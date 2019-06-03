using System;
using System.Collections.Generic;
using System.Text;

namespace ZLAN.Command.Abs
{
    /// <summary>
    /// 接口调用，若接口返回的Code!=0将进行重试，重试机制默认30s/次，尝试120次
    /// 被调用的接口要支持幂等
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInvoker<T>
    {
        /// <summary>
        /// 存储调用信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        int Prepare(CommandInfo info);

        /// <summary>
        /// 执行接口调用
        /// </summary>
        /// <param name="invokeId"></param>
        IResult<T> Invoke(int invokeId);
    }
}

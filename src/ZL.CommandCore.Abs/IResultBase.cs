namespace ZL.CommandCore.Abs
{
    /// <summary>
    /// 结果
    /// </summary>
    /// <typeparam name="T">接口调用正常的返回类型</typeparam>
    public interface IResultBase
    {
        /// <summary>
        /// Code=0,表示接口执行成功
        /// </summary>
        int Code { get; set; }

        /// <summary>
        /// 信息描述
        /// </summary>
        string Message { get; set; }

        
    }
    
}

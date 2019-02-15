namespace ZL.CommandCore.Abs
{
    /// <summary>
    /// 结果
    /// </summary>
    /// <typeparam name="T">接口调用正常的返回类型</typeparam>
    public interface IResult<T>: IResultBase
    {
        /// <summary>
        /// 当Code=0时才有效
        /// </summary>
        T Data { get; set; }
        
    }
    
}

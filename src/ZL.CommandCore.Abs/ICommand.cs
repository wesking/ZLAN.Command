
namespace ZL.CommandCore.Abs
{
    /// <summary>
    /// 接口定义
    /// </summary>
    public interface ICommand<T>: ICommandBase
    {
        IResult<T> Execute(IParameter parameter);
    }
}

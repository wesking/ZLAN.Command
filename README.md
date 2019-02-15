ZL.CommandCore是基于dotnet core，旨在为前端提供统一的接口交互
## 添加项目引用

```
Install-Package ZL.CommandCore
```

## 初始化ZL.CommandCore

```
public void ConfigureServices(IServiceCollection services)
{
	...

    services.AddCommand(opt => {
		opt.ServiceKey = "DemoWeb1";
    });

    //
    //动态添加当前项目所有接口的依赖注入
    //
    var assembly = typeof(Startup).Assembly;

    //实现了接口ICommandBase接口的添加依赖注入
    var types = assembly.ExportedTypes.Where(x => x.IsClass && x.IsPublic && x.GetInterface("ICommandBase") != null);

    foreach (var type in types)
    {
        services.AddScoped(type, type);
    }
	...
}
``` 

## 接口创建

```
//定义接口传入参数
public class Test1Parameter : IParameter
{
	//在此定义属性
}

//定义接口返回结果
public class Test1Result: Result<string>
{
}

//定义接口
public class Test1Command : Command<string>
{
	//接口的处理逻辑
    protected override IResult<string> OnExecute(IParameter parameter)
    {
        Test1Parameter test1Parameter = parameter as Test1Parameter;

        Result<string> result = new Result<string>();

        return result;
    }
}
```

## 在控制器中声明接口

```
[Route("api/test/[action]")]//声名接口的调用地址
[ApiController]
public class TestController : ControllerBase
{
    public TestController(IServiceProvider provider)
    {
        _p = provider;
    }

    /// <summary>
    /// 接口调用例子1
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [HttpPost]
    public Test1Result Test1(Test1Parameter parameter)
    {
        return _p.GetService<Test1Command>().Execute(parameter) as Test1Result;
    }
    private IServiceProvider _p;
}
```

## 接口缓存
使用接口缓存需要初始化的时候指定数据库连接串
```
services.AddCommand(opt => {
	//数据库连接地址
    opt.ConnectionString = "server=localhost;userid=root;password=123456;database=zl_command;";
    opt.ServiceKey = "DemoWeb1";
});
```
使用CacheAttribute指定方法使用缓存
```
[HttpPost, Cache(Key = "test1")]
public Test1Result Test1(Test1Parameter parameter)
{
	return _p.GetService<Test1Command>().Execute(parameter) as Test1Result;
}
```

## 接口授权
参数需要继承AuthorizationParameter，接口需要继承AuthorizationCommand

```
public class Test3Parameter : AuthorizationParameter
{
}

public class Test3Result : Result<string>
{
}
public class Test3Command : AuthorizationCommand<string>
{

    protected override IResult<string> OnExecute(IParameter parameter)
    {
        Test3Parameter test2Parameter = parameter as Test3Parameter;

        Test3Result result = new Test3Result() { };
        //在此实现相关的业务逻辑

        return result;
    }
}
```
在控制器用使用AuthorizationAttribute属性

```
/// <summary>
/// 接口调用例子3（带授权控制）
/// </summary>
/// <param name="parameter"></param>
/// <returns></returns>
[HttpPost, Authorization]
public Test3Result Test3(Test3Parameter parameter)
{
    return _p.GetService<Test3Command>().Execute(parameter) as Test3Result;
}
```

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using ZLAN.Command.Abs;
using ZLAN.Command.Data;

namespace ZLAN.Command
{
    public class HttpInvoker<T>: IInvoker<T>, IDisposable 
    {
        public HttpInvoker()
        {
            _invokeContext = new ServiceContext();
        }

        public IResult<T> Invoke(int invokeId)
        {
            //执行接口调用
            IResult<T> result = new Result<T>() { Code = 102, Message = "未知错误" };
            //开启事务
            var tran = _invokeContext.Database.BeginTransaction();

            try
            {
                //行锁，把状态置为处理中
                if (0 == _invokeContext.Database.
                    ExecuteSqlCommand($"update svr_invoke_info set status=1 where id={invokeId} and status=0"))
                {
                    throw new Exception("no invoker to invoke");
                }
                var invoker = _invokeContext.SvrInvokeInfo.Where(p => p.Id == invokeId).FirstOrDefault();
                if (invoker == null)
                {
                    throw new Exception("can't get invoker");
                }

                using (var http = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip }))
                {
                    http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var httpContent = new StringContent(invoker.RequestBody, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(invoker.RequestHeaders))
                    {
                        var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(invoker.RequestHeaders);
                        foreach (var key in dic.Keys)
                        {
                            httpContent.Headers.Add(key, dic[key]);
                        }
                    }
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var startTime = DateTime.Now;

                    invoker.NextInvokeTime = DateTime.Now.AddSeconds(Convert.ToDouble(invoker.Interval));


                    var log = new SvrInvokeLog()
                    {
                        CreateTime = DateTime.Now,
                        AppId = invoker.AppId,
                        Api = invoker.Api,
                        InvokeId = invoker.Id,
                        RequestBody = invoker.RequestBody,
                        RequestHeaders = invoker.RequestHeaders,
                        ResponseTime = -1,
                        ResponseData = "",
                        ServiceId = invoker.ServiceId,
                        Url = invoker.Url
                    };
                    _invokeContext.SvrInvokeLog.Add(log);
                    _invokeContext.SaveChanges();


                    try
                    {
                        var response = http.PostAsync(invoker.Url, httpContent).Result;

                        //接口调用时长
                        var timeSpan = DateTime.Now.Subtract(startTime).TotalMilliseconds;

                        log.ResponseTime = timeSpan;
                        log.ResponseData = response.Content.ReadAsStringAsync().Result;
                        result = JsonConvert.DeserializeObject<Result<T>>(log.ResponseData);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "call remove service exception");
                        result.Code = 103;
                        result.Message = log.ResponseTime > 0 ? log.ResponseData : ex.Message;
                    }

                    //接口调用成功,记录接口调用时长和相应内容
                    if (result.Code == 0)
                    {
                        invoker.ResponseTime = log.ResponseTime;
                        invoker.ResponseData = log.ResponseData;
                    }
                }
                _invokeContext.SaveChanges();
                //若接口调用成功，则结束本次接口调用，否则置为0，继续等待尝试
                int statusCode = result.Code == 0 ? 9 : 0;

                _invokeContext.Database.ExecuteSqlCommand($"update svr_invoke_info set status={statusCode} where id={invokeId} and status=1");

                //提交事务
                tran.Commit();

                //
                //todo
                //考虑是否需要添加接口告警机制！
                //
            }
            catch (Exception ex)
            {
                Log.Error(ex, "unknown error ,rollback data.");
                tran.Rollback();
            }
            return result;
        }

        public int Prepare(CommandInfo info)
        {
            SvrServiceInfo service = _invokeContext.SvrServiceInfo.Where(p => p.ServiceId == info.Service).FirstOrDefault();
            SvrAppInfo app = _invokeContext.SvrAppInfo.Where(p => p.AppId == info.Appid && p.Status == 1).FirstOrDefault();
            if (service == null || app == null)
            {
                throw new Exception("Error command info");
            }

            var time = DateTime.Now;

            var requestBody = JsonConvert.SerializeObject((object)info.Parameter);

            //请求头
            Dictionary<string, string> header = new Dictionary<string, string>();
            var nonce = Guid.NewGuid().ToString("N");
            header["nonce"] = nonce;
            header["appid"] = info.Appid;
            header["service"] = info.Service;

            //计算签名
            var md5 = MD5.Create();
            var inputByte = Encoding.UTF8.GetBytes(string.Format("service={0}&appid={1}&data={2}&nonce={3}&appsecret={4}",
 info.Service, info.Appid, requestBody, nonce, app.AppSecret));
            var outputByte = md5.ComputeHash(inputByte);

            var sign = BitConverter.ToString(outputByte).Replace("-", "").ToUpper();
            header["sign"] = sign;


            //记录调用信息
            var invokeOjb = new SvrInvokeInfo()
            {
                Api = info.Api,
                AppId = info.Appid,
                CreateTime = time,
                EndTime = time.AddSeconds((double)(info.InvokeInteralOnError * info.InvokeTimesOnError)),
                Interval = info.InvokeInteralOnError,
                NextInvokeTime = time.AddSeconds((double)info.InvokeInteralOnError),
                ServiceId = info.Service,
                RequestBody = requestBody,
                RequestHeaders = JsonConvert.SerializeObject(header),
                Url = service.ServiceUrl + info.Api,
                ResponseTime = -1,
                ResponseData = "",
                Status = 0//初始状态
            };
            _invokeContext.SvrInvokeInfo.Add(invokeOjb);
            _invokeContext.SaveChanges();

            return invokeOjb.Id;

        }

        public void Dispose()
        {
            _invokeContext.Dispose();
        }

        private readonly ServiceContext _invokeContext = null;
        
    }
}

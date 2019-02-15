using System;
using System.Collections.Generic;
using System.Text;

namespace ZL.CommandCore.Abs
{
    public class CommandInfo
    {
        /// <summary>
        /// 调用者
        /// </summary>
        public string Appid { get; set; }

        /// <summary>
        /// 服务标识
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// 接口名
        /// </summary>
        public string Api { get; set; }

        /// <summary>
        /// 失败重试时间间隔（秒）
        /// </summary>
        public int InvokeInteralOnError { get; set; } = 30;

        /// <summary>
        /// 重试次数
        /// </summary>
        public int InvokeTimesOnError { get; set; } = 120;

        /// <summary>
        /// 传入参数
        /// </summary>
        public Object Parameter { get; set; }
    }
}


using Serilog;
using System;

namespace ZLAN.Command.Abs.Observable
{
    /// <summary>
    /// 被观察者，事件发起者
    /// </summary>
    public class Subject
    {
        static Subject()
        {
            Instance = new Subject();
        }

        private Subject()
        {
        }

        ~Subject()
        {
            if (Seeker != null)
            {
                Seeker.Dispose();
            }
        }

        public static Subject Instance { get; set; }


        public IObserverSeeker Seeker { get; set; }

        public void Notify(MessageContext context)
        {
            if (Seeker == null) {
                return;
            }

            var obsvList = Seeker.Seek(context.Key);
            //线程调用
            System.Threading.Tasks.Task.Run(() =>
            {
                foreach (var obsv in obsvList)
                {
                    try
                    {
                        obsv.Execute<object>(context);
                    }
                    catch(Exception ex)
                    {
                        Log.Error("notify exception", ex);
                        continue;
                    }
                }
            });
        }
        

    }
}

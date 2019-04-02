using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MCS.Standard.Library.Core.Diagnostics
{
    public static class PerformanceExtensions
    {
        /// <summary>
        /// 得到某个操作的action的执行时间
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TimeSpan Duration(this Action action)
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            try
            {
                if (action != null)
                    action();

                return sw.Elapsed;
            }
            finally
            {
                sw.Stop();
            }
        }

        /// <summary>
        /// 执行某个方法，执行完后，会调用afterAction，处理执行时间
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="func"></param>
        /// <param name="afterAction"></param>
        /// <returns></returns>
        public static R Duration<R>(this Func<R> func, Action<TimeSpan> afterAction)
        {
            R result = default(R);

            Stopwatch sw = new Stopwatch();

            sw.Start();

            try
            {
                if (func != null)
                    result = func();

                if (afterAction != null)
                    afterAction(sw.Elapsed);

                return result;
            }
            finally
            {
                sw.Stop();
            }
        }
    }
}

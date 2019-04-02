using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Standard.Library.Core.Extensions
{
    public static class BooleanExtensions
    {
        /// <summary>
        /// 当bool参数为true时，调用后续的比较函数。用于连续的条件比较。只要有一个为false，则返回false
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool TrueFunc(this bool flag, Func<bool> func)
        {
            bool result = flag;

            if (flag && func != null)
                result = func();

            return result;
        }

        /// <summary>
        /// 当bool参数为true时，调用操作函数
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool TrueAction(this bool flag, Action action)
        {
            if (flag && action != null)
                action();

            return flag;
        }

        /// <summary>
        /// 当bool参数为false时，调用操作函数
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool FalseAction(this bool flag, Action action)
        {
            if (flag == false && action != null)
                action();

            return flag;
        }
    }
}

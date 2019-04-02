using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Standard.Library.Core.Extensions
{
    public static class DataTimeExtensions
    {
        /// <summary>
        /// 转换成Javascript的日期对应的整数（从1970年1月1日开始的毫秒数）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ToJavascriptDateNumber(this DateTime dt)
        {
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, dt.Kind);

            return Convert.ToInt64((dt - baseTime).TotalMilliseconds);
        }

        /// <summary>
        /// 转换为ISO8601格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToISO8601(this DateTime dt)
        {
            return dt.ToString("o");
        }

        /// <summary>
        /// Javascript的日期对应的整数（从1970年1月1日开始的毫秒数）转换成DateTime
        /// </summary>
        /// <param name="jsMilliseconds"></param>
        /// <returns></returns>
        public static DateTime JavascriptDateNumberToDateTime(this long jsMilliseconds)
        {
            DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return baseTime.AddMilliseconds(jsMilliseconds).ToLocalTime();
        }

        /// <summary>
        /// 将日期类型转换为一个Dictionary，里面包含Ticks和DateKind，主要是和Json序列化中的类型转换相关
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionary(this DateTime dt)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict["DateValue"] = dt.Ticks;
            dict["DateKind"] = dt.Kind;

            return dict;
        }

        /// <summary>
        /// 将一个Dictionary转换成DateTime，需要这个Dictionaty中有DateValue值(Ticks)和DateKind值(DateTimeKind)。
        /// 如果没有，则返回DateTime.MinValue
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this IDictionary<string, object> dictionary)
        {
            DateTime result = DateTime.MinValue;

            long ticks = dictionary.GetValue("DateValue", -1L);
            DateTimeKind kind = dictionary.GetValue("DateKind", DateTimeKind.Local);

            if (ticks != -1)
            {
                result = new DateTime(ticks, kind);

                if (result != DateTime.MinValue && result.Kind == DateTimeKind.Utc)
                    result = result.ToLocalTime();
            }

            return result;
        }
    }
}

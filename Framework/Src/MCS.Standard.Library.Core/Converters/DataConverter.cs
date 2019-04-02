using MCS.Standard.Library.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Standard.Library.Core.Converters
{
    public static class DataConverter
    {
        /// <summary>
        /// 类型转换，提供字符串与枚举型、TimeSpan与整型之间的转换
        /// </summary>
        /// <typeparam name="TSource">源数据的类型</typeparam>
        /// <typeparam name="TResult">目标数据的类型</typeparam>
        /// <param name="srcValue">源数据的值</param>
        /// <returns>类型转换结果</returns>
        /// <remarks>
        /// 数据转换，主要调用系统Convert类的ChangeType方法，但是对于字符串与枚举，整型与TimeSpan类型之间的转换，进行了特殊处理。
        /// <seealso cref="MCS.Library.Core.XmlHelper"/>
        /// </remarks>
        public static TResult ChangeType<TSource, TResult>(TSource srcValue)
        {
            return (TResult)ChangeType(srcValue, typeof(TResult));
        }

        /// <summary>
        /// 字符串与枚举型、TimeSpan与整型之间转换的方法。
        /// </summary>
        /// <typeparam name="TSource">源数据类型</typeparam>
        /// <param name="srcValue">源数据的值</param>
        /// <param name="targetType">目标数据类型</param>
        /// <returns>类型转换后的结果</returns>
        /// <remarks>字符串与枚举型、TimeSpan与整型之间转换的方法。
        /// <seealso cref="MCS.Library.Core.XmlHelper"/>
        /// </remarks>
        public static object ChangeType<TSource>(TSource srcValue, System.Type targetType)
        {
            System.Type srcType = typeof(TSource);

            return ChangeType(srcType, srcValue, targetType);
        }

        private delegate void ChangeTypeAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed);

        private static readonly List<ChangeTypeAction> _changeTypeActions = new List<ChangeTypeAction>()
        {
            SameTypeAction,
            TargetIsObjectAction,
            TargetIsEnumAction,
            DateTimeToStringAction,
            TargetIsTimeSpanAction,
            StringToBoolAction,
            TargetIsDateTimeAction,
            DictionaryToDateTimeAction,
            GuidToStringAction,
            StringToGuidAction
        };

        /// <summary>
        /// 字符串与枚举型、TimeSpan与整型之间转换的方法。
        /// </summary>
        /// <param name="srcType">源数据类型</param>
        /// <param name="srcValue">源数据的值</param>
        /// <param name="targetType">目标数据类型</param>
        /// <returns>类型转换后的结果</returns>
        /// <remarks>字符串与枚举型、TimeSpan与整型之间转换的方法。
        /// <seealso cref="MCS.Library.Core.XmlHelper"/>
        /// </remarks>
        public static object ChangeType(System.Type srcType, object srcValue, System.Type targetType)
        {
            targetType.NullCheck(nameof(targetType));

            bool dealed = false;
            object result = null;

            if (srcType == typeof(object))
                if (srcValue != null)
                    srcType = srcValue.GetType();

            foreach (ChangeTypeAction action in _changeTypeActions)
            {
                action(srcType, srcValue, targetType, ref result, ref dealed);

                if (dealed)
                    break;
            }

            if (dealed == false)
            {
                if (targetType != typeof(object) && targetType.IsAssignableFrom(srcType))
                {
                    result = srcValue;
                }
                else if (targetType == typeof(DateTime))
                {
                    result = Convert.ToDateTime(srcValue);
                }
                else
                {
                    result = Convert.ChangeType(srcValue, targetType);
                }
            }

            return result;
        }

        #region Actions
        private static void SameTypeAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if (srcType == targetType)
            {
                result = srcValue;
                dealed = true;
            }
        }

        private static void TargetIsObjectAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if (targetType == typeof(object))
            {
                result = srcValue;
                dealed = true;
            }
        }

        private static void TargetIsEnumAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if (targetType.IsEnum)
            {
                if (srcType == typeof(string) || srcType == typeof(int) || srcType == typeof(long))
                {
                    if (srcValue is string && (srcValue).ToString().IsNullOrEmpty())
                    {
                        result = Enum.Parse(targetType, "0", true);
                    }
                    else
                    {
                        result = Enum.Parse(targetType, srcValue.ToString(), true);
                    }

                    dealed = true;
                }
            }
        }

        private static void DateTimeToStringAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if (targetType == typeof(string) && srcType == typeof(DateTime))
            {
                result = string.Format("{0:yyyy-MM-ddTHH:mm:ss.fff}", srcValue);

                dealed = true;
            }
        }

        private static void TargetIsTimeSpanAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if (targetType == typeof(TimeSpan))
            {
                double tsValue = 0;

                if (srcValue is string)
                {
                    if (double.TryParse((string)srcValue, out tsValue))
                        result = TimeSpan.FromSeconds(tsValue);
                    else
                        result = TimeSpan.Parse((string)srcValue);
                }
                else
                {
                    tsValue = (double)Convert.ChangeType(srcValue, typeof(double));
                    result = TimeSpan.FromSeconds(tsValue);
                }

                dealed = true;
            }
        }

        private static void StringToBoolAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if (targetType == typeof(bool) && srcType == typeof(string))
            {
                result = StringToBool(srcValue.ToString(), out dealed);
            }
        }

        private static void TargetIsDateTimeAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if (targetType == typeof(DateTime))
            {
                if (srcType == typeof(string))
                {
                    if (srcValue == null || srcValue.ToString() == string.Empty)
                    {
                        result = DateTime.MinValue;
                        dealed = true;
                    }
                    else
                    {
                        double oaDate;

                        if (double.TryParse((string)srcValue, out oaDate))
                        {
                            result = DateTime.FromOADate(oaDate);
                            dealed = true;
                        }
                    }
                }
                else if (typeof(IDictionary<string, object>).IsAssignableFrom(srcType))
                {
                    result = ((IDictionary<string, object>)srcValue).ToDateTime();
                    dealed = true;
                }
                else if (srcType == typeof(double))
                {
                    result = DateTime.FromOADate((double)srcValue);
                    dealed = true;
                }
            }
        }

        private static void DictionaryToDateTimeAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if ((typeof(IDictionary<string, object>).IsAssignableFrom(targetType)) && srcType == typeof(DateTime))
            {
                result = ((DateTime)srcValue).ToDictionary();
                dealed = true;
            }
        }

        private static void GuidToStringAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if (targetType == typeof(string) && srcType == typeof(Guid))
            {
                result = srcValue.ToString();
                dealed = true;
            }
        }

        private static void StringToGuidAction(System.Type srcType, object srcValue, System.Type targetType, ref object result, ref bool dealed)
        {
            if (targetType == typeof(Guid) && srcType == typeof(string))
            {
                string strValue = (string)srcValue;

                if (strValue.IsNullOrEmpty())
                    result = Guid.Empty;
                else
                    result = Guid.Parse(strValue);

                dealed = true;
            }
        }
        #endregion Actions

        private static bool StringToBool(string srcValue, out bool dealed)
        {
            bool result = false;
            dealed = false;

            srcValue = srcValue.Trim();

            if (srcValue.Length > 0)
            {
                if (srcValue.Length == 1)
                {
                    result = string.Compare(srcValue, "0") != 0 && string.Compare(srcValue, "n", true) != 0 && string.Compare(srcValue, "否", true) != 0;
                    dealed = true;
                }
                else
                {
                    if (string.Compare(srcValue, "YES", true) == 0 || string.Compare(srcValue, "TRUE", true) == 0)
                    {
                        result = true;
                        dealed = true;
                    }
                    else
                    {
                        if (string.Compare(srcValue, "NO", true) == 0 || string.Compare(srcValue, "FALSE", true) == 0)
                        {
                            result = false;
                            dealed = true;
                        }
                    }
                }
            }
            else
            {
                dealed = true;	//空串表示False
            }

            return result;
        }
    }
}

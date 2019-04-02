using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MCS.Standard.Library.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 如果对象为空，则执行Action
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <returns>返回传入的data</returns>
        public static object IsNull(this object data, Action action)
        {
            if (data == null && action != null)
                action();

            return data;
        }

        /// <summary>
        /// 如果对象不为空，则执行Action
        /// </summary>
        /// <typeparam name="T">对象的类型泛型</typeparam>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <returns>返回传入的data</returns>
        public static T IsNotNull<T>(this T data, Action<T> action)
        {
            if (data != null && action != null)
                action(data);

            return data;
        }

        /// <summary>
        /// 如果对象不为空，则执行Func，返回某个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="data"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static R IsNotNull<T, R>(this T data, Func<T, R> func)
        {
            R result = default(R);

            if (data != null && func != null)
                result = func(data);

            return result;
        }

        /// <summary>
        /// 比较两个对象的引用，如果都是null，返回true，如果有一个null，hasNull返回true
        /// </summary>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <param name="hasNull"></param>
        /// <returns></returns>
        public static bool ReferenceEqualWithNull(this object objA, object objB, out bool hasNull)
        {
            bool result = object.ReferenceEquals(objA, objB);

            if (objA == null || objB == null)
                hasNull = true;
            else
                hasNull = false;

            return result;
        }

        /// <summary>
        /// 对象类型是否是枚举类型，且TypeCode为Object
        /// </summary>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        public static bool IsEnumerableObject(this object objectValue)
        {
            bool result = false;

            if (objectValue != null && Type.GetTypeCode(objectValue.GetType()) == TypeCode.Object)
                result = objectValue is IEnumerable;

            return result;
        }
    }
}

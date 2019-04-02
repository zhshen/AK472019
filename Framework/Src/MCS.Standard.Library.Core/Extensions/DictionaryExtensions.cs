using MCS.Standard.Library.Core.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace MCS.Standard.Library.Core.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 读取泛型字典中的某一项，如果不存在此项，返回缺省值
        /// </summary>
        /// <typeparam name="TKey">字典Key的类型</typeparam>
        /// <typeparam name="TValue">字典项的类型</typeparam>
        /// <typeparam name="TReturnValue">返回值的类型</typeparam>
        /// <param name="dict">泛型字典对象</param>
        /// <param name="key">需要访问的key值</param>
        /// <param name="defaultValue">如果不存在key时，返回的缺省值</param>
        /// <returns></returns>
        public static TReturnValue GetValue<TKey, TValue, TReturnValue>(this IDictionary<TKey, TValue> dict, TKey key, TReturnValue defaultValue)
        {
            TReturnValue result = defaultValue;

            if (dict != null)
            {
                TValue oResult = default(TValue);

                if (dict.TryGetValue(key, out oResult))
                    result = (TReturnValue)DataConverter.ChangeType(oResult, typeof(TReturnValue));
            }

            return result;
        }

        /// <summary>
        /// 读取字典中的某一项，如果不存在此项，返回缺省值
        /// </summary>
        /// <typeparam name="TReturnValue">字典的返回类型</typeparam>
        /// <param name="dict">字典对象</param>
        /// <param name="key">需要访问的key值</param>
        /// <param name="defaultValue">如果不存在key时，返回的缺省值</param>
        /// <returns></returns>
        public static TReturnValue GetValue<TReturnValue>(this IDictionary dict, object key, TReturnValue defaultValue)
        {
            TReturnValue result = defaultValue;

            if (dict != null)
            {
                if (dict.Contains(key))
                    result = (TReturnValue)DataConverter.ChangeType(dict[key], typeof(TReturnValue));
            }

            return result;
        }

        /// <summary>
        /// 读取字典中的某一项，如果为空，则返回缺省值
        /// </summary>
        /// <typeparam name="TReturnValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TReturnValue GetValue<TReturnValue>(this StringDictionary dict, string key, TReturnValue defaultValue)
        {
            TReturnValue result = defaultValue;

            if (dict != null)
            {
                if (dict[key] != null)
                    result = (TReturnValue)DataConverter.ChangeType(dict[key], typeof(TReturnValue));
            }

            return result;
        }

        /// <summary>
        /// 当添加的字典项不是该类型的缺省值时，则添加此项
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns>是否添加了该项</returns>
        public static bool AddNonDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue data)
        {
            dict.NullCheck(nameof(dict));

            bool added = false;

            if (data != null)
                if (data is string)
                {
                    if (data.ToString() != string.Empty)
                    {
                        dict.Add(key, data);
                        added = true;
                    }
                }
                else
                    if (data.Equals(default(TValue)) == false)
                {
                    dict.Add(key, data);
                    added = true;
                }

            return added;
        }

        /// <summary>
        /// 当添加的字典项不是指定的缺省值时，则添加此项
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="defaultValue"></param>
        /// <returns>是否添加了该项</returns>
        public static bool AddNonDefaultValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue data, TValue defaultValue)
        {
            dict.NullCheck(nameof(dict));

            bool added = false;

            if (data != null)
            {
                if (data.Equals(defaultValue) == false)
                {
                    dict.Add(key, data);
                    added = true;
                }
            }
            else
                if (defaultValue != null)
            {
                dict.Add(key, data);
                added = true;
            }

            return added;
        }

        /// <summary>
        /// 如果字典中没有指定的key，则添加。否则忽略
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> AddNotExistsItem<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue data)
        {
            dict.NullCheck("dict");

            if (dict.ContainsKey(key) == false)
                dict.Add(key, data);

            return dict;
        }

        /// <summary>
        /// 从一个字典合并到另一个字典，原来如果存在，则不增加
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> MergeFrom<TKey, TValue>(this IDictionary<TKey, TValue> target, IDictionary<TKey, TValue> source)
        {
            if (target != null && source != null)
            {
                foreach (KeyValuePair<TKey, TValue> kp in source)
                {
                    if (target.ContainsKey(kp.Key) == false)
                        target.Add(kp.Key, kp.Value);
                }
            }

            return target;
        }
    }
}

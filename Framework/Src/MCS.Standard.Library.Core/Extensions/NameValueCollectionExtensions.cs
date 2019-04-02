using MCS.Standard.Library.Core.Converters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace MCS.Standard.Library.Core.Extensions
{
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// NameValueCollection的扩展，获取参数的值，并且转换为目标类型。如果不存在，则返回defaultValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(this NameValueCollection collection, string name, T defaultValue)
        {
            return GetValue(collection, name, false, defaultValue);
        }

        /// <summary>
        ///  NameValueCollection的扩展，获取参数的值，并且转换为目标类型。如果不存在，则返回defaultValue
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="collection">集合对象</param>
        /// <param name="name">集中的Key</param>
        /// <param name="urlDecode">是否进行urlDecode</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T GetValue<T>(this NameValueCollection collection, string name, bool urlDecode, T defaultValue)
        {
            name.CheckStringIsNullOrEmpty("name");

            T result = defaultValue;

            if (collection != null)
            {
                string data = collection[name];

                if (data.IsNotEmpty())
                {
                    if (urlDecode)
                        data = HttpUtility.UrlDecode(data);

                    result = (T)DataConverter.ChangeType(data, typeof(T));
                }
            }

            return result;
        }

        /// <summary>
        /// 从一个NameValueCollection复制到另一个NameValueCollection
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="src"></param>
        public static void CopyFrom(this NameValueCollection dest, NameValueCollection src)
        {
            if (dest != null && src != null)
            {
                dest.Clear();

                dest.Add(src);
            }
        }

        /// <summary>
        /// 删除NameValueCollection中指定的Key
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="keysToRemove"></param>
        public static void RemoveKeys(this NameValueCollection collection, params string[] keysToRemove)
        {
            if (collection != null && keysToRemove != null)
                keysToRemove.ForEach(key => collection.Remove(key));
        }

        /// <summary>
        /// 转换为Url的参数串
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="encodeUrl"></param>
        /// <returns></returns>
        public static string ToUrlParameters(this NameValueCollection collection, bool encodeUrl)
        {
            return ToUrlParameters(collection, encodeUrl, null);
        }

        /// <summary>
        /// 转换为Url的参数串
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="encodeUrl"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToUrlParameters(this NameValueCollection collection, bool encodeUrl, Encoding encoding)
        {
            StringBuilder strB = new StringBuilder();

            if (collection != null)
            {
                if (encoding == null)
                    encoding = Encoding.UTF8;

                foreach (string key in collection)
                {
                    if (key.IsNotEmpty())
                    {
                        if (strB.Length > 0)
                            strB.Append("&");

                        strB.Append(key);
                        strB.Append("=");

                        string value = collection[key];

                        if (value.IsNotEmpty())
                        {
                            if (encodeUrl)
                                value = HttpUtility.UrlEncode(value, encoding);

                            strB.Append(value);
                        }
                    }
                }
            }

            return strB.ToString();
        }

        /// <summary>
        /// 从参数中解析回字典
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="paramString"></param>
        /// <param name="urlDecode"></param>
        /// <returns></returns>
        public static NameValueCollection ParseParameters(this NameValueCollection parameters, string paramString, bool urlDecode = true)
        {
            if (parameters == null)
                parameters = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

            if (paramString.IsNotEmpty())
            {
                string[] parts = paramString.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < parts.Length; i++)
                {
                    int equalsSignIndex = parts[i].IndexOf("=");

                    string paramName = string.Empty;
                    string paramValue = string.Empty;

                    if (equalsSignIndex >= 0)
                    {
                        //存在等号
                        paramName = parts[i].Substring(0, equalsSignIndex);
                        paramValue = parts[i].Substring(equalsSignIndex + 1);
                    }

                    if (string.IsNullOrEmpty(paramName) == false)
                    {
                        if (urlDecode)
                        {
                            paramName = HttpUtility.UrlDecode(paramName);
                            paramValue = HttpUtility.UrlDecode(paramValue);
                        }

                        AddValueToCollection(paramName, paramValue, parameters);
                    }
                }
            }

            return parameters;
        }

        private static void AddValueToCollection(string paramName, string paramValue, NameValueCollection result)
        {
            string oriValue = result[paramName];

            if (oriValue == null)
                result.Add(paramName, paramValue);
            else
            {
                string rValue = oriValue;

                if (oriValue.Length > 0)
                    rValue += ",";

                rValue += paramValue;

                result[paramName] = rValue;
            }
        }
    }
}

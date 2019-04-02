using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MCS.Standard.Library.Core.Extensions
{
    #region 字符串处理的相关结构
    /// <summary>
    /// 三部分字符串，分为前置部分、中间部分和后续部分
    /// </summary>
    public class ThreeSegmentsString
    {
        /// <summary>
        /// 前置部分
        /// </summary>
        public string Prep
        {
            get;
            set;
        }

        /// <summary>
        /// 主体部分
        /// </summary>
        public string Body
        {
            get;
            set;
        }

        /// <summary>
        /// 后继部分
        /// </summary>
        public string Succ
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 字符串替换的片段定义
    /// </summary>
    public class ReplaceSegment
    {
        /// <summary>
        /// 源的起始位置
        /// </summary>
        public int SourceIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 源的长度
        /// </summary>
        public int SourceLength
        {
            get;
            set;
        }

        /// <summary>
        /// 行
        /// </summary>
        public int Line
        {
            get;
            set;
        }

        /// <summary>
        /// 列
        /// </summary>
        public int Column
        {
            get;
            set;
        }

        /// <summary>
        /// 需要替换的字符串
        /// </summary>
        public string Destination
        {
            get;
            set;
        }
    }
    #endregion 字符串处理的相关结构

    /// <summary>
    /// 字符串操作的扩展方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 空的数组
        /// </summary>
        public static readonly string[] EmptyStringArray = new string[0];

        /// <summary>
        /// IP地址的分隔符
        /// </summary>
        public static readonly char[] IPAddresSplitChar = new char[] { ',', ' ' };

        private static readonly char[] ControlAndSpaceChars = null;
        private static readonly SortedSet<char> ControlAndSpaceCharSet = new SortedSet<char>();

        static StringExtension()
        {
            ControlAndSpaceChars = new char[33];

            for (int i = 0; i < 33; i++)
            {
                ControlAndSpaceChars[i] = (char)i;
                ControlAndSpaceCharSet.Add((char)i);
            }
        }

        /// <summary>
        /// 字符串不是Null且Empty
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string data)
        {
            bool result = false;

            if (data != null)
                result = (string.IsNullOrEmpty(data) == false);

            return result;
        }

        /// <summary>
        /// 如果字符串不为空，则执行Action
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <returns>返回传入的data</returns>
        public static string IsNotEmpty(this string data, Action<string> action)
        {
            if (data.IsNotEmpty() && action != null)
                action(data);

            return data;
        }

        /// <summary>
        /// 如果字符串不为空，则执行Func
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="data"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static R IsNotEmpty<R>(this string data, Func<string, R> func)
        {
            R result = default(R);

            if (data.IsNotEmpty() && func != null)
                result = func(data);

            return result;
        }

        /// <summary>
        /// 字符串是否为Null或Empty
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string data)
        {
            return string.IsNullOrEmpty(data);
        }

        /// <summary>
        /// 如果字符串为空，则返回替代的字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="replacedData"></param>
        /// <returns></returns>
        public static string NullOrEmptyIs(this string data, string replacedData)
        {
            string result = data;

            if (result.IsNullOrEmpty())
                result = replacedData;

            return result;
        }

        /// <summary>
        /// 如果字符串为空，则执行Action
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <returns>返回传入的data</returns>
        public static string IsNullOrEmpty(this string data, Action action)
        {
            if (string.IsNullOrEmpty(data) && action != null)
                action();

            return data;
        }

        /// <summary>
        /// 如果字符串为空，则执行Func
        /// </summary>
        /// <param name="data"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static string IsNullOrEmpty(this string data, Func<string> func)
        {
            string result = data;

            if (string.IsNullOrEmpty(data) && func != null)
                result = func();

            return result;
        }

        /// <summary>
        /// 字符串是否为Null、Empty和WhiteSpace
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string data)
        {
            return string.IsNullOrWhiteSpace(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <returns>返回传入的data</returns>
        public static string IsNullOrWhiteSpace(this string data, Action action)
        {
            if (string.IsNullOrWhiteSpace(data) && action != null)
                action();

            return data;
        }

        /// <summary>
        /// 字符串不是Null、Empty和WhiteSpace
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNotWhiteSpace(this string data)
        {
            bool result = false;

            if (data != null)
                result = (string.IsNullOrWhiteSpace(data) == false);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void IsNotWhiteSpace(this string data, Action<string> action)
        {
            if (data.IsNotWhiteSpace() && action != null)
                action(data);
        }

        /// <summary>
        /// 大小写无关的比较
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static int IgnoreCaseCompare(this string strA, string strB)
        {
            return string.Compare(strA, strB, true);
        }

        /// <summary>
        /// 大小写无关的等于判断
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns></returns>
        public static bool IgnoreCaseEquals(this string strA, string strB)
        {
            return string.Compare(strA, strB, true) == 0;
        }

        /// <summary>
        /// 从一个ip地址串中，得到第一个ip。这个地址串以逗号分隔。如果地址非法，则返回null
        /// </summary>
        /// <param name="ipsString">逗号分隔的ip地址</param>
        /// <returns></returns>
        public static IPAddress GetFirstIPAddress(this string ipsString)
        {
            IPAddress ip = null;

            if (ipsString.IsNotEmpty())
            {
                string[] ipParts = ipsString.Split(IPAddresSplitChar, StringSplitOptions.RemoveEmptyEntries);

                if (ipParts.Length > 0)
                    IPAddress.TryParse(ipParts[0], out ip);
            }

            return ip;
        }

        /// <summary>
        /// 将byte数组转换为base16的字符串
        /// </summary>
        /// <param name="data">待转换的byte数组</param>
        /// <returns>转换好的16进制字符串</returns>
        public static string ToBase16String(this byte[] data)
        {
            StringBuilder strB = new StringBuilder();

            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                    strB.AppendFormat("{0:x2}", data[i]);
            }

            return strB.ToString();
        }

        /// <summary>
        /// 将保存好的16进制字符串转换为byte数组
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static byte[] ToBase16Bytes(this string strData)
        {
            byte[] data = null;

            if (strData != null)
            {
                data = new Byte[strData.Length / 2];

                for (int i = 0; i < strData.Length / 2; i++)
                    data[i] = Convert.ToByte(strData.Substring(i * 2, 2), 16);
            }
            else
                data = new byte[0];

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strB"></param>
        /// <param name="data"></param>
        public static void AppendWithSplitChars(this StringBuilder strB, string data)
        {
            AppendWithSplitChars(strB, data, " ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strB"></param>
        /// <param name="data"></param>
        /// <param name="splitChars"></param>
        public static void AppendWithSplitChars(this StringBuilder strB, string data, string splitChars)
        {
            if (data.IsNotEmpty())
            {
                if (strB.Length > 0 && splitChars.IsNotEmpty())
                    strB.Append(splitChars);

                strB.Append(data);
            }
        }

        private static readonly Tuple<int, int>[] ChineseCharScopes = new Tuple<int, int>[] {
                Tuple.Create(0x4E00, 0x9FA5),
                Tuple.Create(0x9FA6, 0x9FCB),
                Tuple.Create(0x3400, 0x4DB5),
                Tuple.Create(0x20000, 0x2A6D6),
                Tuple.Create(0x2A700, 0x2B734),
                Tuple.Create(0x2B740, 0x2B81D),
                Tuple.Create(0x2F00, 0x2FD5),
                Tuple.Create(0x2E80, 0x2EF3),
                Tuple.Create(0xF900, 0xFAD9),
                Tuple.Create(0x2F800, 0x2FA1D),
                Tuple.Create(0xE815, 0xE86F),
                Tuple.Create(0xE400, 0xE5E8),
                Tuple.Create(0xE600, 0xE6CF),
                Tuple.Create(0x31C0, 0x31E3),
                Tuple.Create(0x2FF0, 0x2FFB),
                Tuple.Create(0x3105, 0x3120),
                Tuple.Create(0x31A0, 0x31BA),
                Tuple.Create(0x3007, 0x3007)
            };

        /// <summary>
        /// 字符是否是中文
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsChinese(this char data)
        {
            bool result = false;

            if (data > 0x7F)
            {
                for (int i = 0; i < ChineseCharScopes.Length; i++)
                {
                    if (data >= ChineseCharScopes[i].Item1 && data <= ChineseCharScopes[i].Item2)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 是否包含中文
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool HasChinese(this string data)
        {
            bool result = false;

            if (data.IsNotEmpty())
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].IsChinese())
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 按照中文拆分字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string[] SplitByChinese(this string data)
        {
            return data.SplitBySpecialChar(ch => ch.IsChinese());
        }

        /// <summary>
        /// 按照指定字符规则拆分字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string[] SplitBySpecialChar(this string data, Func<Char, bool> condition)
        {
            List<string> result = new List<string>();
            StringBuilder buffer = null;

            if (data.IsNotEmpty())
            {
                bool isSpecialSegment = false;

                foreach (char ch in data)
                {
                    if (condition(ch) != isSpecialSegment || buffer == null)
                    {
                        if (buffer != null)
                        {
                            result.Add(buffer.ToString());

                            buffer.Clear();
                        }
                        else
                            buffer = new StringBuilder();

                        isSpecialSegment = condition(ch);
                    }

                    buffer.Append(ch);
                }

                if (buffer != null)
                    result.Add(buffer.ToString());
            }

            return result.ToArray();
        }

        /// <summary>
        /// 通过控制字符和空格进行Trim
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string TrimByControlAndSpaceChar(this string data)
        {
            string result = data;

            if (data.IsNotEmpty())
            {
                result = data.Trim(ControlAndSpaceChars);
            }

            return result;
        }

        /// <summary>
        /// 拆分控制字符
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ThreeSegmentsString SplitByCotrolAndSpaceChar(this string data)
        {
            ThreeSegmentsString result = new ThreeSegmentsString()
            {
                Prep = string.Empty,
                Body = string.Empty,
                Succ = string.Empty
            };

            if (data.IsNotEmpty())
            {
                int bodyStart = 0;
                int bodyEnd = data.Length;

                for (int i = 0; i < data.Length; i++)
                {
                    if (ControlAndSpaceCharSet.Contains(data[i]))
                        bodyStart++;
                    else
                        break;
                }

                for (int i = data.Length - 1; i > bodyStart; i--)
                {
                    if (ControlAndSpaceCharSet.Contains(data[i]))
                        bodyEnd--;
                    else
                        break;
                }

                result.Prep = data.Substring(0, bodyStart);
                result.Body = data.Substring(bodyStart, bodyEnd - bodyStart);
                result.Succ = data.Substring(bodyEnd, data.Length - bodyEnd);
            }

            return result;
        }

        /// <summary>
        /// 根据正则表达式拆分字符串。
        /// 例如需要将“Hi,{{Hello}} {{World \nPerfect}} !"中的{{ }}包含的内容拆分出来。
        /// 则使用"{{([\s \S]*?)}}"，结果是Hi, {{Hello}} {{World...组成的数组
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] SplitByRegEx(this string data, string pattern)
        {
            List<string> parts = new List<string>();

            data.SplitByRegEx(pattern, (s, matched) => parts.Add(s));

            return parts.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pattern"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string SplitByRegEx(this string data, string pattern, Action<string, bool> action)
        {
            if (data.IsNotEmpty())
            {
                Regex reg = new Regex(pattern);

                MatchCollection matches = reg.Matches(data);

                int index = 0;

                foreach (Match match in matches)
                {
                    if (match.Index > 0)
                    {
                        int length = match.Index - index;

                        action(data.Substring(index, length), false);

                        index += length;
                    }

                    action(match.Value, true);

                    index += match.Value.Length;
                }

                if (index < data.Length)
                    action(data.Substring(index, data.Length - index), false);
            }

            return data;
        }

        /// <summary>
        /// 根据一组ReplaceSegment替换内容。segments的SourceIndex一定是从小到大排序的
        /// </summary>
        /// <param name="source"></param>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static string Replace(this string source, IEnumerable<ReplaceSegment> segments)
        {
            StringBuilder strB = new StringBuilder();

            if (source.IsNotEmpty() && segments != null)
            {
                int index = 0;

                foreach (ReplaceSegment segment in segments)
                {
                    if (segment.SourceIndex > 0)
                    {
                        int length = segment.SourceIndex - index;

                        strB.Append(source.Substring(index, length));

                        index += length;
                    }

                    strB.Append(segment.Destination);

                    index += segment.SourceLength;
                }

                if (index < source.Length)
                    strB.Append(source.Substring(index, source.Length - index));
            }

            return strB.ToString();
        }

        /// <summary>
        /// 根绝Segment范围，得到源字符串
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetSourceString(this ReplaceSegment segment, string source)
        {
            string result = string.Empty;

            if (segment != null && source.IsNotEmpty())
                result = source.Substring(segment.SourceIndex, segment.SourceLength);

            return result;
        }

        /// <summary>
        /// 根绝Segment范围，得到引号内的字符串（包括转义符自身）
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetSourceInnerString(this ScriptStringReplaceSegment segment, string source)
        {
            string result = string.Empty;

            if (segment != null && source.IsNotEmpty())
                result = source.Substring(segment.InnerStringStartIndex, segment.InnerStringLength);

            return result;
        }

        /// <summary>
        /// StringWithPosition的StringData是否等于某个字符串。如果sp为null，则相当于比较空串
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="actual"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static bool AreEqual(this StringWithPosition sp, string actual, bool ignoreCase = false)
        {
            bool result = false;

            if (sp != null)
                result = string.Compare(sp.StringData, actual, ignoreCase) == 0;
            else
                result = actual.IsNullOrEmpty();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="actual"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static bool AreNotEqual(this StringWithPosition sp, string actual, bool ignoreCase = false)
        {
            return AreEqual(sp, actual, ignoreCase) == false;
        }

        /// <summary>
        /// 转换为位置，如果为null，则返回-1
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public static int ToPosition(this CharWithPosition cp)
        {
            int result = -1;

            if (cp != null)
                result = cp.Position;

            return result;
        }

        /// <summary>
        /// 转换为位置，如果为null，则返回-1
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static int ToPosition(this StringWithPosition sp)
        {
            int result = -1;

            if (sp != null)
                result = sp.Position;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static string ToStringData(this StringWithPosition sp)
        {
            string result = string.Empty;

            if (sp != null)
                result = sp.StringData;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool AreEqual(this CharWithPosition cp, char ch)
        {
            bool result = false;

            if (cp != null)
                result = cp.Character == ch;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool AreNotEqual(this CharWithPosition cp, char ch)
        {
            return AreNotEqual(cp, ch) == false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public static char ToCharacter(this CharWithPosition cp)
        {
            char result = '\0';

            if (cp != null)
                result = cp.Character;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="actual"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static bool Include(this StringWithPosition sp, string actual, bool ignoreCase = false)
        {
            bool result = false;

            if (sp != null && sp.StringData != null)
            {
                StringComparison sc = StringComparison.Ordinal;

                if (ignoreCase)
                    sc = StringComparison.OrdinalIgnoreCase;

                result = sp.StringData.IndexOf(actual, sc) >= 0;
            }
            else
                result = actual.IsNullOrEmpty();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifiers"></param>
        /// <param name="maxPosition"></param>
        /// <returns></returns>
        public static StringWithPosition FindPreviousIdentifier(this IList<StringWithPosition> identifiers, int maxPosition = int.MaxValue)
        {
            StringWithPosition result = null;

            if (identifiers != null)
            {
                for (int i = identifiers.Count - 1; i >= 0; i--)
                {
                    StringWithPosition identifier = identifiers[i];

                    if (identifier.Position < maxPosition)
                    {
                        result = identifier;
                        break;
                    }
                }
            }

            return result;
        }

        #region MatchWithAsterisk
        /// <summary>
        /// 某个字符串是否匹配通配符原则
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool MatchWithAsterisk(this string data, string pattern)
        {
            if (data.IsNullOrEmpty() || pattern.IsNullOrEmpty())
                return false;

            string[] ps = pattern.Split('*');

            if (ps.Length == 1) // 没有*的模型
                return MatchWithInterrogation(data, ps[0]);

            var si = data.IndexOf(ps[0], 0);	// 从string头查找第一个串

            if (si != 0)
                return false; // 第一个串没找到或者不在string的头部

            si += ps[0].Length; // 找到了串后,按就近原则,移到未查询过的最左边

            int plast = ps.Length - 1; // 最后一串应单独处理,为了提高效率,将它从循环中取出
            int pi = 0; // 跳过之前处理过的第一串

            while (++pi < plast)
            {
                if (ps[pi] == "")
                    continue; //连续的*号,可以忽略

                si = data.IndexOf(ps[pi], si);	// 继续下一串的查找

                if (-1 == si)
                    return false; // 没有找到

                si += ps[pi].Length; // 就近原则
            }

            if (ps[plast] == "") // 模型尾部为*,说明所有有效字符串部分已全部匹配,string后面可以是任意字符
                return true;

            // 从尾部查询最后一串是否存在
            int last_index = data.LastIndexOf(ps[plast]);

            // 如果串存在,一定要在string的尾部, 并且不能越过已查询过部分
            return (last_index == data.Length - ps[plast].Length) && (last_index >= si);
        }

        private static bool MatchWithInterrogation(string data, string pattern)
        {
            bool result = false;

            if (data.Length == pattern.Length)
                result = data.IndexOf(pattern) > -1;

            return result;
        }
        #endregion MatchWithAsterisk
    }
}

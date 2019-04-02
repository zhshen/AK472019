using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MCS.Standard.Library.Core.Extensions
{
    /// <summary>
    /// 资源访问的Extensions
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// 从资源中加载字符串
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadStringFromResource(this Assembly assembly, string path)
        {
            using (Stream stm = GetResourceStream(assembly, path))
            {
                StreamReader sr = new StreamReader(stm);

                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 异步从资源中读取字符串信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<string> LoadStringFromResourceAsync(this Assembly assembly, string path)
        {
            using (Stream stm = GetResourceStream(assembly, path))
            {
                StreamReader sr = new StreamReader(stm);

                return sr.ReadToEndAsync();
            }
        }

        /// <summary>
        /// 从资源中加载xml文档对象
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlDocument LoadXmlFromResource(this Assembly assembly, string path)
        {
            using (Stream stm = GetResourceStream(assembly, path))
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(stm);

                return xmlDoc;
            }
        }

        /// <summary>
        /// 从资源中异步加载xml对象
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<XmlDocument> LoadXmlFromResourceAsync(this Assembly assembly, string path)
        {
            string content = await assembly.LoadStringFromResourceAsync(path);

            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(content);

            return xmlDoc;
        }

        /// <summary>
        /// 从资源中加载xml Reader
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <param name="stm"></param>
        /// <returns></returns>
        public static XmlReader LoadXmlReaderFromResource(this Assembly assembly, string path, out Stream stm)
        {
            stm = GetResourceStream(assembly, path);

            return XmlReader.Create(stm);
        }

        /// <summary>
        /// 从资源中加载XElement
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XElement LoadXElementFromResource(this Assembly assembly, string path)
        {
            using (Stream stm = GetResourceStream(assembly, path))
            {
                return XElement.Load(stm);
            }
        }

        /// <summary>
        /// 从资源中异步加载XElement
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<XElement> LoadXElementFromResourceAsync(this Assembly assembly, string path)
        {
            string content = await assembly.LoadStringFromResourceAsync(path);

            return XElement.Parse(content);
        }

        /// <summary>
        /// 从资源中得到流
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Stream GetResourceStream(this Assembly assembly, string path)
        {
            (assembly != null).FalseThrow<ArgumentNullException>(nameof(assembly));
            path.CheckStringIsNullOrEmpty(nameof(path));

            Stream stm = assembly.GetManifestResourceStream(path);

            (stm != null).FalseThrow("不能在Assembly:{0}中找到资源{1}", assembly.FullName, path);

            return stm;
        }

        /// <summary>
        /// 得到资源的二进制流
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] GetResourceBytes(this Assembly assembly, string path)
        {
            Stream stm = GetResourceStream(assembly, path);

            using (MemoryStream result = new MemoryStream(4096))
            {
                stm.CopyTo(result);

                return result.ToArray();
            }
        }
    }
}

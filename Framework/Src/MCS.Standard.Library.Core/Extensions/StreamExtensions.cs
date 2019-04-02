using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MCS.Standard.Library.Core.Extensions
{
    /// <summary>
    /// IO操作的扩展方法
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="action"></param>
        public static void DoCreateFileAction(string fileName, Action<Stream> action)
        {
            fileName.CheckStringIsNullOrEmpty(nameof(fileName));

            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                if (action != null)
                    action(stream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="action"></param>
        public static void DoOpenFileAction(string fileName, Action<Stream> action)
        {
            fileName.CheckStringIsNullOrEmpty("fileName");

            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                if (action != null)
                    action(stream);
            }
        }
    }
}

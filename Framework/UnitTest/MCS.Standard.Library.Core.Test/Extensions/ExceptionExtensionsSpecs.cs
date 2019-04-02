using MCS.Standard.Library.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Standard.Library.Core.Test.Extensions
{
    [TestClass]
    public class ExceptionExtensionsSpecs
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullObjectCheckSpec()
        {
            object data = null;

            data.NullCheck(nameof(data));
        }

        [TestMethod]
        public void NotNullObjectCheckSpec()
        {
            object data = "Hello";

            data.NullCheck(nameof(data));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyStringCheckSpec()
        {
            string data = string.Empty;

            data.CheckStringIsNullOrEmpty(nameof(data));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullStringCheckSpec()
        {
            string data = null;

            data.CheckStringIsNullOrEmpty(nameof(data));
        }

        [TestMethod]
        public void NotEmptyStringCheckSpec()
        {
            string data = "Hello world!";

            data.CheckStringIsNullOrEmpty(nameof(data));
        }

        [TestMethod]
        [ExpectedException(typeof(SystemSupportException))]
        public void TrueThrowSpec()
        {
            (3 > 1).TrueThrow("It's true");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TrueThrowWithSpecialExceptionSpec()
        {
            (3 > 1).TrueThrow<ArgumentException>("It's true");
        }
    }
}

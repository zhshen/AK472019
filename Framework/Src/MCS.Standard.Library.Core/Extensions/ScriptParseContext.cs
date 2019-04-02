using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MCS.Standard.Library.Core.Extensions
{
    internal enum ScriptParsingStatus
    {
        Normal,

        /// <summary>
        /// 在普通字符串状态下
        /// </summary>
        InString,

        /// <summary>
        /// 在@形式下的字符串
        /// </summary>
        InVerbatimString,

        /// <summary>
        /// 普通字符串中的Escape状态，一般是反斜线转义
        /// </summary>
        InEscape,

        /// <summary>
        /// 在Verbatim下的转义
        /// </summary>
        InVerbatimEscape,

        /// <summary>
        /// 在块注释中
        /// </summary>
        InBlockComment,

        /// <summary>
        /// 在行注释中
        /// </summary>
        InLineComment,

        /// <summary>
        /// 在类似于#region这种directive中
        /// </summary>
        InDirective,

        /// <summary>
        /// 在操作数当中
        /// </summary>
        InOperand,

        /// <summary>
        /// 在标识符中
        /// </summary>
        InIdentifier
    }

    internal class ScriptParseContext
    {
        private int _StringStartIndex = -1;
        private int _Line = 1;
        private int _Column = 1;

        public Func<string, ScriptStringReplaceSegment, bool> OnAddStringSegment
        {
            get;
            set;
        }

        public Func<string, ScriptStringReplaceSegment, bool> OnAddIdentifierSegment
        {
            get;
            set;
        }

        public Func<string, ScriptStringReplaceSegment, bool> OnAddBracketSegment
        {
            get;
            set;
        }

        public char[] QuotationMarks
        {
            get;
            set;
        }

        public char VerbatimModeChar
        {
            get;
            set;
        }

        public char PropertySeperator
        {
            get;
            set;
        }

        public string Source
        {
            get;
            set;
        }

        public ScriptParsingStatus Status
        {
            get;
            set;
        }

        public char PreviousChar
        {
            get;
            set;
        }

        public char CurrentChar
        {
            get;
            set;
        }

        public char NextChar
        {
            get;
            set;
        }

        public char StringQuotationMark
        {
            get;
            set;
        }

        public int CurrentIndex
        {
            get;
            set;
        }

        public int Line
        {
            get
            {
                return this._Line;
            }
            set
            {
                this._Line = value;
            }
        }

        public int Column
        {
            get
            {
                return this._Column;
            }
            set
            {
                this._Column = value;
            }
        }

        public int StringStartIndex
        {
            get
            {
                return this._StringStartIndex;
            }
            set
            {
                this._StringStartIndex = value;
            }
        }

        /// <summary>
        /// 是否停止字符指针自动增加
        /// </summary>
        public bool StopIncrement
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int OuterStringStartIndex
        {
            get;
            set;
        }

        private StringBuilder _StringBuffer = new StringBuilder();

        public StringBuilder StringBuffer
        {
            get
            {
                return this._StringBuffer;
            }
        }


        private List<ScriptStringReplaceSegment> _Segments = new List<ScriptStringReplaceSegment>();

        public List<ScriptStringReplaceSegment> Segments
        {
            get
            {
                return this._Segments;
            }
        }

        /// <summary>
        /// 最近的括号
        /// </summary>
        public CharWithPosition PreviousOperator
        {
            get;
            set;
        }

        /// <summary>
        /// 最近的括号
        /// </summary>
        public CharWithPosition PreviousBracket
        {
            get;
            set;
        }

        /// <summary>
        /// 最近的分隔符
        /// </summary>
        public CharWithPosition PreviousSeperator
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public StringWithPosition PreviousIdentifier
        {
            get
            {
                StringWithPosition result = null;
                if (this.Identifiers.Count > 0)
                    result = this.Identifiers[this.Identifiers.Count - 1];

                return result;
            }
        }

        private List<StringWithPosition> _Identifiers = new List<StringWithPosition>();

        public List<StringWithPosition> Identifiers
        {
            get
            {
                return this._Identifiers;
            }
        }

        private Dictionary<string, bool> _FunctionWithInjectors = new Dictionary<string, bool>();

        public Dictionary<string, bool> FunctionWithInjectors
        {
            get
            {
                return this._FunctionWithInjectors;
            }
        }

        private HashSet<int> _RegisteredPosition = new HashSet<int>();

        public HashSet<int> RegisteredPosition
        {
            get
            {
                return this._RegisteredPosition;
            }
        }
    }

    /// <summary>
    /// 字符和位置
    /// </summary>
    public class CharWithPosition
    {
        /// <summary>
        /// 
        /// </summary>
        public char Character
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Position
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
    }

    /// <summary>
    /// 
    /// </summary>
    public enum IdentifierType
    {
        /// <summary>
        /// 
        /// </summary>
        Identifier,

        /// <summary>
        /// 
        /// </summary>
        Function,

        /// <summary>
        /// 
        /// </summary>
        Property
    }

    /// <summary>
    /// 
    /// </summary>
    public class StringWithPosition
    {
        /// <summary>
        /// 
        /// </summary>
        public string StringData
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Position
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
        /// 
        /// </summary>
        public IdentifierType Type
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 脚本中的字符串信息
    /// </summary>
    public class ScriptStringReplaceSegment : ReplaceSegment
    {
        /// <summary>
        /// 
        /// </summary>
        public int InnerStringStartIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int InnerStringLength
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public char StringQuotationMark
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string InnerString
        {
            get;
            set;
        }

        /// <summary>
        /// 最近的括号
        /// </summary>
        public CharWithPosition PreviousOperator
        {
            get;
            set;
        }

        /// <summary>
        /// 最近的括号
        /// </summary>
        public CharWithPosition PreviousBracket
        {
            get;
            set;
        }

        /// <summary>
        /// 最近的分隔符
        /// </summary>
        public CharWithPosition PreviousSeperator
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public StringWithPosition PreviousIdentifier
        {
            get
            {
                StringWithPosition result = null;

                if (this.Identifiers != null && this.Identifiers.Count > 0)
                    result = this.Identifiers[this.Identifiers.Count - 1];

                return result;
            }

        }

        /// <summary>
        /// 左引号部分，包括左边的引号和@
        /// </summary>
        public string LeftQuotationMarkPart
        {
            get
            {
                string result = string.Empty;

                if (this.Destination.IsNotEmpty() && this.InnerStringStartIndex > this.SourceIndex)
                    result = this.Destination.Substring(0, this.InnerStringStartIndex - this.SourceIndex);

                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RightQuotationMarkPart
        {
            get
            {
                string result = string.Empty;

                if (this.Destination.IsNotEmpty() && this.InnerStringStartIndex > this.SourceIndex)
                    result = this.Destination.Substring((this.InnerStringStartIndex - this.SourceIndex) + this.InnerStringLength);

                return result;
            }
        }

        /// <summary>
        /// 历史标识符
        /// </summary>
        public List<StringWithPosition> Identifiers
        {
            get;
            internal set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, bool> FunctionWithInjectors
        {
            get;
            internal set;
        }

        /// <summary>
        /// 
        /// </summary>
        public HashSet<int> RegisteredPosition
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// 脚本分析异常
    /// </summary>
    [Serializable]
    public class ScriptParsingException : System.Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public ScriptParsingException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ScriptParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Line = info.GetInt32("Line");
            this.Column = info.GetInt32("Column");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ScriptParsingException(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public ScriptParsingException(string message, System.Exception ex) : base(message, ex)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <param name="ex"></param>
        public ScriptParsingException(string message, int line, int column, System.Exception ex) : base(message, ex)
        {
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Line
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Column
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Line", this.Line);
            info.AddValue("Column", this.Column);
        }
    }
}

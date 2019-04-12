#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Text;

namespace Unity.Properties.Codegen.CSharp
{
    public enum BraceLayout
    {
        EndOfLine,
        EndOfLineSpace,
        NextLine,
        NextLineIndent
    }

    public class CodeStyle
    {
        public string Indent { get; set; }
        public BraceLayout BraceLayout { get; set; }
        public string BeginBrace { get; set; }
        public string EndBrace { get; set; }
        public string NewLine { get; set; }

        public static CodeStyle CSharp => new CodeStyle()
        {
            Indent = "    ",
            BraceLayout = BraceLayout.NextLine,
            BeginBrace = "{",
            EndBrace = "}",
            NewLine = "\n"
        };
    }

    public interface IScope : IDisposable
    {
        
    }

    public class CodeWriter
    {
        private class ActionScope : IScope
        {
            private Action m_Disposed;

            public ActionScope(Action disposed)
            {
                m_Disposed = disposed;
            }

            public void Dispose()
            {
                if (null == m_Disposed)
                {
                    return;
                }

                m_Disposed.Invoke();
                m_Disposed = null;
            }
        }
        
        private readonly StringBuilder m_StringBuilder;
        private int m_Indent;
        
        public CodeStyle CodeStyle { get; set; }

        public int Length
        {
            get { return m_StringBuilder.Length; }
            set { m_StringBuilder.Length = value; }
        }

        public CodeWriter() : this(CodeStyle.CSharp)
        {
        }

        public CodeWriter(CodeStyle codeStyle)
        {
            m_StringBuilder = new StringBuilder();
            CodeStyle = codeStyle;
        }

        public CodeWriter Write(string value)
        {
            m_StringBuilder.Append(value);
            return this;
        }

        public CodeWriter Line()
        {
            m_StringBuilder.Append(CodeStyle.NewLine);
            return this;
        }

        public CodeWriter Line(string content)
        {
            WriteIndent();
            m_StringBuilder.Append(content);
            m_StringBuilder.Append(CodeStyle.NewLine);
            return this;
        }

        public IScope Scope(string content, bool endLine = true)
        {
            return Scope(content, CodeStyle.BraceLayout, endLine);
        }

        public IScope Scope(string content, BraceLayout layout, bool endLine = true)
        {
            WriteIndent();

            m_StringBuilder.Append(content);

            WriteBeginScope(layout);

            return new ActionScope(() => { WriteEndScope(layout, endLine); });
        }

        private void WriteBeginScope(BraceLayout layout)
        {
            switch (layout)
            {
                case BraceLayout.EndOfLine:
                    m_StringBuilder.Append(CodeStyle.BeginBrace);
                    m_StringBuilder.Append(CodeStyle.NewLine);
                    break;
                case BraceLayout.EndOfLineSpace:
                    m_StringBuilder.AppendFormat(" {0}", CodeStyle.BeginBrace);
                    m_StringBuilder.Append(CodeStyle.NewLine);
                    break;
                case BraceLayout.NextLine:
                    m_StringBuilder.Append(CodeStyle.NewLine);
                    WriteIndent();
                    m_StringBuilder.Append(CodeStyle.BeginBrace);
                    m_StringBuilder.Append(CodeStyle.NewLine);
                    break;
                case BraceLayout.NextLineIndent:
                    m_StringBuilder.Append(CodeStyle.NewLine);
                    IncrementIndent();
                    WriteIndent();
                    m_StringBuilder.Append(CodeStyle.BeginBrace);
                    m_StringBuilder.Append(CodeStyle.NewLine);
                    break;
            }

            IncrementIndent();
        }

        private void WriteEndScope(BraceLayout layout, bool endLine)
        {
            switch (layout)
            {
                case BraceLayout.EndOfLine:
                case BraceLayout.EndOfLineSpace:
                case BraceLayout.NextLine:
                    DecrementIndent();
                    WriteIndent();
                    Write(CodeStyle.EndBrace);
                    break;
                case BraceLayout.NextLineIndent:
                    DecrementIndent();
                    Write(CodeStyle.EndBrace);
                    WriteIndent();
                    DecrementIndent();
                    break;
            }

            if (endLine)
            {
                Write(CodeStyle.NewLine);
            }
        }

        public void IncrementIndent()
        {
            m_Indent++;
        }

        public void DecrementIndent()
        {
            if (m_Indent > 0)
            {
                m_Indent--;
            }
        }

        public void Clear()
        {
            m_StringBuilder.Length = 0;
        }

        public void WriteIndent()
        {
            for (var i = 0; i < m_Indent; i++)
            {
                m_StringBuilder.Append(CodeStyle.Indent);
            }
        }

        public override string ToString()
        {
            return m_StringBuilder.ToString();
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)
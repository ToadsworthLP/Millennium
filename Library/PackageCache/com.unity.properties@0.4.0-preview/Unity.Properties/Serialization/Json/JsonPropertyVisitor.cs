#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Globalization;

namespace Unity.Properties.Serialization
{
    public class JsonPropertyVisitor : PropertyVisitor, ICustomVisitPrimitives
    {
        public static class Style
        {
            public const int Space = 4;
        }
        
        private static readonly StringBuffer s_StringBuffer = new StringBuffer(1024);

        public StringBuffer StringBuffer = s_StringBuffer;
        public int Indent;

        public override string ToString()
        {
            return StringBuffer?.ToString() ?? string.Empty;
        }

        protected void AppendPrimitive(string value)
        {
            if (IsListItem)
            {
                StringBuffer.Append(' ', Style.Space * Indent);
                StringBuffer.Append(value);
                StringBuffer.Append(",\n");
            }
            else
            {
                StringBuffer.Append(' ', Style.Space * Indent);
                StringBuffer.Append("\"");
                StringBuffer.Append(Property.Name);
                StringBuffer.Append("\": ");
                StringBuffer.Append(value);
                StringBuffer.Append(",\n");
            }
        }

        protected void AppendPrefix()
        {
            if (IsListItem)
            {
                StringBuffer.Append(' ', Style.Space * Indent);
            }
            else
            {
                StringBuffer.Append(' ', Style.Space * Indent);
                StringBuffer.Append("\"");
                StringBuffer.Append(Property.Name);
                StringBuffer.Append("\": ");
            }
        }

        protected void AppendSuffix()
        {
            StringBuffer.Append(",\n");
        }

        protected override void Visit<TValue>(TValue value)
        {
            if (typeof(TValue).IsEnum)
            {
                AppendPrimitive(Convert.ToInt32(value).ToString());
            }
            else
            {
                AppendPrimitive(Json.EncodeJsonString(value?.ToString()));
            }
        }

        void ICustomVisit<bool>.CustomVisit(bool value)
        {
            AppendPrimitive(value ? "true" : "false");
        }

        void ICustomVisit<byte>.CustomVisit(byte value)
        {
            AppendPrefix();
            StringBuffer.Append(value);
            AppendSuffix();
        }

        void ICustomVisit<sbyte>.CustomVisit(sbyte value)
        {
            AppendPrefix();
            StringBuffer.Append(value);
            AppendSuffix();
        }

        void ICustomVisit<ushort>.CustomVisit(ushort value)
        {
            AppendPrefix();
            StringBuffer.Append(value);
            AppendSuffix();
        }

        void ICustomVisit<short>.CustomVisit(short value)
        {
            AppendPrefix();
            StringBuffer.Append(value);
            AppendSuffix();
        }

        void ICustomVisit<uint>.CustomVisit(uint value)
        {
            AppendPrefix();
            StringBuffer.Append(value);
            AppendSuffix();
        }

        void ICustomVisit<int>.CustomVisit(int value)
        {
            AppendPrefix();
            StringBuffer.Append(value);
            AppendSuffix();
        }

        void ICustomVisit<ulong>.CustomVisit(ulong value)
        {
            AppendPrefix();
            StringBuffer.Append(value);
            AppendSuffix();
        }

        void ICustomVisit<long>.CustomVisit(long value)
        {
            AppendPrefix();
            StringBuffer.Append(value);
            AppendSuffix();
        }

        void ICustomVisit<char>.CustomVisit(char value)
        {
            AppendPrimitive(Json.EncodeJsonString(string.Empty + value));
        }

        void ICustomVisit<float>.CustomVisit(float value)
        {
            AppendPrefix();
            StringBuffer.Append(value);
            AppendSuffix();
        }

        void ICustomVisit<double>.CustomVisit(double value)
        {
            AppendPrefix();
            StringBuffer.Append(value.ToString(CultureInfo.InvariantCulture));
            AppendSuffix();
        }

        void ICustomVisit<string>.CustomVisit(string value)
        {
            AppendPrimitive(Json.EncodeJsonString(value));
        }

        protected override bool BeginContainer()
        {
            if (IsListItem)
            {
                Indent--;
                StringBuffer.Length -= 1;
                StringBuffer.Append(StringBuffer[StringBuffer.Length - 1] == ',' ? " {\n" : "{\n");
            }
            else
            {
                StringBuffer.Append(' ', Style.Space * Indent);
                StringBuffer.Append("\"");
                StringBuffer.Append(Property.Name);
                StringBuffer.Append("\": {\n");
            }

            Indent++;
            return true;
        }

        protected override void EndContainer()
        {
            Indent--;

            // Remove the trailing comma
            if (StringBuffer[StringBuffer.Length - 2] == ',')
            {
                StringBuffer.Length -= 2;
                StringBuffer.Append('\n');
                StringBuffer.Append(' ', Style.Space * Indent);
            }
            else
            {
                StringBuffer.Length -= 1;
            }

            if (IsListItem)
            {
                Indent++;
            }

            StringBuffer.Append("},\n");
        }

        protected override bool BeginCollection()
        {
            StringBuffer.Append(' ', Style.Space * Indent);
            StringBuffer.Append('\"');
            StringBuffer.Append(Property.Name);
            StringBuffer.Append("\": [\n");
            Indent++;
            return true;
        }

        protected override void EndCollection()
        {
            Indent--;

            // Remove the trailing comma
            if (StringBuffer[StringBuffer.Length - 2] == ',')
            {
                StringBuffer.Length -= 2;
            }
            else
            {
                StringBuffer.Length -= 1;
            }

            var skipNewline = StringBuffer[StringBuffer.Length - 1] == '}' &&
                StringBuffer[StringBuffer.Length - 3] == ' ';
            skipNewline = skipNewline | StringBuffer[StringBuffer.Length - 1] == '[';

            if (!skipNewline)
            {
                StringBuffer.Append("\n");
                StringBuffer.Append(' ', Style.Space * Indent);
            }

            StringBuffer.Append("],\n");
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)
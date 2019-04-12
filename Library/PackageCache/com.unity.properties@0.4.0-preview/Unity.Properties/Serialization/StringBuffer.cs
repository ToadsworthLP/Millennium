namespace Unity.Properties.Serialization
{
    ///<summary>
    /// Based on https://pastebin.com/HqAw2pTG
    ///</summary>
    public class StringBuffer
    {
        ///<summary>
        /// Immutable string. Generated at last moment, only if needed
        /// </summary>
        private string m_String = "";

        ///<summary>
        /// Is m_StringGenerated is up to date?
        /// </summary>
        private bool m_Generated;

        ///<summary>
        /// Working mutable string
        /// </summary>
        private char[] m_Buffer;

        private int m_Position;
        private int m_Capacity;

        public int Length
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        public char[] Buffer => m_Buffer;

        public char this[int index]
        {
            get { return m_Buffer[index]; }
            set { m_Buffer[index] = value; }
        }

        public StringBuffer(char[] buffer)
        {
            m_Buffer = buffer;
        }
        
        public StringBuffer(int initialCapacity = 32)
        {
            m_Buffer = new char[m_Capacity = initialCapacity];
        }

        public bool IsEmpty()
        {
            return m_Generated ? m_String == null : m_Position == 0;
        }

        ///<summary>
        /// Return the string
        /// </summary>
        public override string ToString()
        {
            if (m_Generated)
            {
                return m_String;
            }
            
            m_String = new string(m_Buffer, 0, m_Position);
            m_Generated = true;

            return m_String;
        }

        ///<summary>
        /// Reset the m_char array
        /// </summary>
        public void Clear()
        {
            m_Position = 0;
            m_Generated = false;
        }

        /// <summary>
        /// Appends a char
        /// </summary>
        public void Append(char value, int repeatCount = 1)
        {
            EnsureCapacity(repeatCount);
            for (var i = 0; i < repeatCount; i++)
            {
                m_Buffer[m_Position + i] = value;
            }
            m_Position += repeatCount;
            m_Generated = false;
        }
        
        ///<summary>
        /// Append a string
        /// </summary>
        public void Append(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            
            EnsureCapacity(value.Length);
            var len = value.Length;
            for (var i = 0; i < len; i++)
            {
                m_Buffer[m_Position + i] = value[i];
            }
            m_Position += len;
            m_Generated = false;
        }

        ///<summary>
        /// Append an object.ToString()
        /// </summary>
        public void Append(object value)
        {
            Append(value?.ToString());
        }

        ///<summary>
        /// Append an int 
        /// </summary>
        public void Append(int value)
        {
            // Allocate enough memory to handle any int number
            EnsureCapacity(16);

            // Handle the negative case
            if (value < 0)
            {
                value = -value;
                m_Buffer[m_Position++] = '-';
            }

            // Copy the digits in reverse order
            var nbChars = 0;
            do
            {
                m_Buffer[m_Position++] = (char) ('0' + value % 10);
                value /= 10;
                nbChars++;
            } while (value != 0);

            // Reverse the result
            for (var i = nbChars / 2 - 1; i >= 0; i--)
            {
                var c = m_Buffer[m_Position - i - 1];
                m_Buffer[m_Position - i - 1] = m_Buffer[m_Position - nbChars + i];
                m_Buffer[m_Position - nbChars + i] = c;
            }
            m_Generated = false;
        }

        ///<summary>
        /// Append a float value
        /// </summary>
        public void Append(float valueF)
        {
            double value = valueF;
            m_Generated = false;
            EnsureCapacity(32); // Check we have enough buffer allocated to handle any float number

            if (float.IsInfinity(valueF))
            {
                foreach (var c in $"\"{(float.IsNegativeInfinity(valueF) ? "-": "")}Infinity\"")
                {
                    m_Buffer[m_Position++] = c;
                }
                return;
            }
            
            // Handle the 0 case
            if (value == 0)
            {
                m_Buffer[m_Position++] = '0';
                return;
            }

            // Handle the negative case
            if (value < 0)
            {
                value = -value;
                m_Buffer[m_Position++] = '-';
            }

            // Get the 7 meaningful digits as a long
            var nbDecimals = 0;
            while (value < 1000000)
            {
                value *= 10;
                nbDecimals++;
            }
            var valueLong = (long) System.Math.Round(value);

            // Parse the number in reverse order
            var nbChars = 0;
            var isLeadingZero = true;
            while (valueLong != 0 || nbDecimals >= 0)
            {
                // We stop removing leading 0 when non-0 or decimal digit
                if (valueLong % 10 != 0 || nbDecimals <= 0)
                    isLeadingZero = false;

                // Write the last digit (unless a leading zero)
                if (!isLeadingZero)
                    m_Buffer[m_Position + nbChars++] = (char) ('0' + valueLong % 10);

                // Add the decimal point
                if (--nbDecimals == 0 && !isLeadingZero)
                    m_Buffer[m_Position + nbChars++] = '.';

                valueLong /= 10;
            }
            m_Position += nbChars;

            // Reverse the result
            for (var i = nbChars / 2 - 1; i >= 0; i--)
            {
                var c = m_Buffer[m_Position - i - 1];
                m_Buffer[m_Position - i - 1] = m_Buffer[m_Position - nbChars + i];
                m_Buffer[m_Position - nbChars + i] = c;
            }
        }

        /// <summary>
        /// Ensures the capacity of the underlying buffer
        /// </summary>
        /// <param name="count">Number of characters being added</param>
        public void EnsureCapacity(int count)
        {
            if (m_Position + count <= m_Capacity)
            {
                return;
            }

            m_Capacity = System.Math.Max(m_Capacity + count, m_Capacity * 2);
            var chars = new char[m_Capacity];
            m_Buffer.CopyTo(chars, 0);
            m_Buffer = chars;
        }
    }
}
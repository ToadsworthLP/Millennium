#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections;
using System.Collections.Generic;

namespace Unity.Properties
{
    /// <summary>
    /// A dynamic list that can be mutated during migration
    /// </summary>
    public class MigrationList<TValue> : IList<TValue>
    {
        private readonly IList<TValue> m_Data = new List<TValue>();

        public int Count => m_Data.Count;
        public bool IsReadOnly => false;

        public TValue this[int index]
        {
            get { return m_Data[index]; }
            set { m_Data[index] = value; }
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return m_Data.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return m_Data.GetEnumerator();
        }

        public void Add(TValue value)
        {
            m_Data.Add(value);
        }

        public void Clear()
        {
            m_Data.Clear();
        }

        public bool Contains(TValue value)
        {
            return m_Data.Contains(value);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            for (int i = arrayIndex, count = Count; i < count; i++)
            {
                array[i] = this[i];
            }
        }

        public int IndexOf(TValue value)
        {
            return m_Data.IndexOf(value);
        }

        public void Insert(int index, TValue value)
        {
            m_Data.Insert(index, value);
        }

        public bool Remove(TValue value)
        {
            return m_Data.Remove(value);
        }

        public void RemoveAt(int index)
        {
            m_Data.RemoveAt(index);
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)
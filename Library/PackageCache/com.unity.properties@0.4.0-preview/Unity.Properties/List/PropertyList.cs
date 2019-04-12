#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections;
using System.Collections.Generic;

namespace Unity.Properties
{
    /// <summary>
    /// Wrapper class to provide an IList interface for a given Property/Container combination
    ///
    /// @USAGE
    ///
    /// IList MyList => new PropertyList(s_ListProperty, this);
    /// 
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public struct PropertyList<TContainer, TValue> : IList<TValue>
        where TContainer : class, IPropertyContainer
    {
        private struct Enumerator : IEnumerator<TValue>
        {
            private readonly IListClassProperty<TContainer, TValue> m_Property;
            private readonly TContainer m_Container;

            private int m_Index;
            
            public TValue Current { get; private set; }
            object IEnumerator.Current => Current;

            public Enumerator(IListClassProperty<TContainer, TValue> property, TContainer container)
            {
                m_Property = property;
                m_Container = container;
                m_Index = 0;
                Current = default(TValue);
            }
            
            public bool MoveNext()
            {
                var count = m_Property.Count(m_Container);
                
                if (m_Index >= 0 && m_Index < count)
                {
                    Current = m_Property.GetAt(m_Container, m_Index);
                    m_Index++;
                    return true;
                }

                m_Index = count + 1;
                Current = default(TValue);
                
                return false;
            }

            public void Reset()
            {
                m_Index = 0;
                Current = default(TValue);
            }
            
            public void Dispose()
            {
                
            }
        }
        
        private readonly IListClassProperty<TContainer, TValue> m_Property;
        private readonly TContainer m_Container;

        public bool IsReadOnly => false;
        public int Count => m_Property.Count(m_Container);

        public IProperty Property => m_Property;

        public TValue this[int index]
        {
            get { return m_Property.GetAt(m_Container, index); }
            set { m_Property.SetAt(m_Container, index, value); }
        }
        
        public PropertyList(IListClassProperty<TContainer, TValue> property, TContainer container)
        {
            m_Property = property;
            m_Container = container;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator(m_Property, m_Container);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TValue item)
        {
            m_Property.Add(m_Container, item);
        }

        public void Clear()
        {
            m_Property.Clear(m_Container);
        }

        public bool Contains(TValue item)
        {
            return m_Property.Contains(m_Container, item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            for (int i = 0, count = m_Property.Count(m_Container); i < count; i++)
            {
                array[arrayIndex + i] = m_Property.GetAt(m_Container, i);
            }
        }

        public bool Remove(TValue item)
        {
            return m_Property.Remove(m_Container, item);
        }
        
        public int IndexOf(TValue item)
        {
            return m_Property.IndexOf(m_Container, item);
        }

        public void Insert(int index, TValue item)
        {
            m_Property.Insert(m_Container, index, item);
        }

        public void RemoveAt(int index)
        {
            m_Property.RemoveAt(m_Container, index);
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)
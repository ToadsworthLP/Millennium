#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Assertions;

namespace Unity.Properties
{
    /// <summary>
    /// Represents a path to a given property.
    /// </summary>
    public sealed class PropertyPath
    {
        public const int InvalidListIndex = -1;

        public struct Part
        {
            public string propertyName;
            public int listIndex;

            public bool IsListItem => listIndex >= 0;
        }
        
        private readonly List<Part> m_Parts;

        public int PartsCount => m_Parts.Count;

        public Part this[int index] => m_Parts[index];

        public PropertyPath()
        {
            m_Parts = new List<Part>(32);
        }
        
        public PropertyPath(string path)
        {
            PropertyPath result;
            if (TryParse(path, out result))
            {
                m_Parts = result.m_Parts;
            }
        }

        public PropertyPath(PropertyPath path)
        {
            m_Parts = new List<Part>(path.m_Parts);
        }

        public void Insert(int index, string propertyName, int listIndex = InvalidListIndex)
        {
            Assert.IsFalse(string.IsNullOrEmpty(propertyName));

            if (listIndex < 0)
            {
                listIndex = InvalidListIndex;
            }
            m_Parts.Insert(index, new Part()
            {
                propertyName = propertyName,
                listIndex = listIndex
            });
        }

        public void Push(string propertyName, int listIndex = InvalidListIndex)
        {
            Assert.IsFalse(string.IsNullOrEmpty(propertyName));

            if (listIndex < 0)
            {
                listIndex = InvalidListIndex;
            }
            m_Parts.Add(new Part()
            {
                propertyName = propertyName,
                listIndex = listIndex
            });
        }

        public void Pop()
        {
            m_Parts.RemoveAt(m_Parts.Count - 1);
        }

        public struct Resolution
        {
            public bool success;
            public IPropertyContainer container;
            public IProperty property;
            public int listIndex;
            public object value;
        }

        public Resolution Resolve(IPropertyContainer root)
        {
            Assert.IsNotNull(root);
            var result = new Resolution
            {
                container = root,
                value = root,
                listIndex = InvalidListIndex
            };

            foreach (var part in m_Parts)
            {
                result.listIndex = part.listIndex;
                var container = result.value as IPropertyContainer;
                
                if (result.listIndex >= 0)
                {
                    if (container == null)
                    {
                        // failure: list property not in a container
                        return result;
                    }

                    result.container = container;
                    result.property = container.PropertyBag.FindProperty(part.propertyName);

                    // @TODO Struct lists
                    var listProperty = result.property as IListClassProperty;
                    if (listProperty == null || listProperty.Count(result.container) <= part.listIndex)
                    {
                        // failure: property is not a list, or index is out of range
                        return result;
                    }
                    result.value = listProperty.GetObjectAt(result.container, part.listIndex);
                }
                else
                {
                    if (container != null)
                    {
                        result.container = container;
                        result.property = container.PropertyBag.FindProperty(part.propertyName);
                        if (result.property == null)
                        {
                            // failure: cannot find property
                            return result;
                        }

                        result.value = (result.property as IValueProperty)?.GetObjectValue(result.container);
                    }
                    else
                    {
                        // failure: intermediate property is not a container
                        return result;
                    }
                }
            }

            result.success = true;
            return result;
        }

        public override string ToString()
        {
            switch (m_Parts.Count)
            {
                case 0:
                    return string.Empty;
                default:
                {
                    var sb = new StringBuilder(16);
                    foreach (var part in m_Parts)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append('.');
                        }

                        sb.Append(part.propertyName);
                        if (part.listIndex >= 0)
                        {
                            sb.Append("[" + part.listIndex + "]");
                        }
                    }
                    return sb.ToString();
                }
            }
        }

        public static bool TryParse(string path, out PropertyPath result)
        {
            Assert.IsNotNull(path);

            result = new PropertyPath();
            var token = new StringBuilder();
            var listPropertyName = string.Empty;
            var listIndex = InvalidListIndex;
            var inListIndex = false;
            foreach (var c in path)
            {
                switch (c)
                {
                    case '.':
                        if (inListIndex) return false;
                        if (token.Length == 0 && listIndex == InvalidListIndex) return false;
                        if (listIndex == InvalidListIndex)
                        {
                            result.Push(token.ToString(), listIndex);
                        }
                        token.Clear();
                        listIndex = InvalidListIndex;
                        break;
                    case '[':
                        if (token.Length == 0) return false;
                        if (inListIndex) return false;
                        listPropertyName = token.ToString();
                        token.Clear();
                        listIndex = InvalidListIndex;
                        inListIndex = true;
                        break;
                    case ']':
                        if (token.Length == 0) return false;
                        if (!inListIndex) return false;
                        if (!int.TryParse(token.ToString(), out listIndex)) return false;
                        if (listIndex < 0) return false;
                        result.Push(listPropertyName, listIndex);
                        token.Clear();
                        inListIndex = false;
                        break;
                    default:
                        token.Append(c);
                        break;
                }
            }

            if (token.Length > 0)
            {
                if (inListIndex) return false;
                result.Push(token.ToString(), InvalidListIndex);
            }

            return true;
        }

        public static PropertyPath Parse(string path)
        {
            PropertyPath result;
            if (!TryParse(path, out result))
            {
                throw new Exception("Invalid property path: " + path);
            }

            return result;
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)


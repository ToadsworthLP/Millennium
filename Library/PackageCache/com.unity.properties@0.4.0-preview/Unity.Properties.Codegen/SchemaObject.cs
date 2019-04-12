#if (NET_4_6 || NET_STANDARD_2_0)

using UnityEngine;

namespace Unity.Properties.Codegen
{
    public class SchemaObject : ScriptableObject
    {
        [SerializeField] private string m_JsonSchema;

        public string JsonSchema
        {
            get { return m_JsonSchema; }
            set { m_JsonSchema = value; }
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)
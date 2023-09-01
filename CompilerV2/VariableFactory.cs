using System.Collections.Generic;

namespace CompilerV2
{
    internal class VariableFactory
    {
        public List<Token> Tokens { get { return m_tokens; } }
        private List<Token> m_tokens;
        public List<string> Names { get { return m_names; } }
        private List<string> m_names;
        public string DataType { set { m_dataType = value; } }
        private string m_dataType;

        public bool IsArray { set { m_isArray = value; } }
        private bool m_isArray;

        public int ArraySize { set { m_arraySize = value; } }
        private int m_arraySize;

        public VariableFactory()
        {
            m_tokens = new List<Token>();
            m_names = new List<string>();
        }

        public Variable[] GetVariables()
        {
            if (m_tokens.Count != m_names.Count) return null;

            List<Variable> vars = new List<Variable>();
            for (int i = 0; i < m_names.Count; i++)
                vars.Add(new Variable(m_names[i], m_tokens[i], m_dataType, m_isArray, m_arraySize));

            return vars.ToArray();
        }
    }
}


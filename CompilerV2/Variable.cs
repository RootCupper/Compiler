namespace CompilerV2
{
    internal class Variable
    {
        public string Name { get { return m_name; } }
        private readonly string m_name;
        public Token Token { get { return m_token; } }
        private Token m_token;
        public string DataType { get { return m_dataType; } }
        private readonly string m_dataType;
        public bool IsArray { get { return m_isArray; } }
        private readonly bool m_isArray;

        public int ArraySize { get { return m_arraySize; } }
        private readonly int m_arraySize;

        public Variable(string name, Token token, string dataType, bool isArray, int arraySize) 
        {
            m_name = name;
            m_token = token;
            m_dataType = dataType;
            m_isArray = isArray;
            m_arraySize = arraySize;
        }
    }
}


using System;

namespace CompilerV2
{
    internal class Token
    {
        public TokenType Type { get { return type; } }
        private TokenType type;

        public int Index { get { return index; } }
        private int index;

        public string Value { get { return value; } }
        private string value;

        public int SymbolIndex { get { return symbolIndex; } }
        private int symbolIndex;

        public int LineIndex { get { return lineIndex; } }
        private int lineIndex;

        public Token(TokenType type, int index, string value, int symbolIndex, int lineIndex)
        {
            this.type = type;
            this.index = index;
            this.value = value;
            this.symbolIndex = symbolIndex;
            this.lineIndex = lineIndex;
        }

        public static Token ErrorToken() { return new Token(TokenType.ErrorToken, -1, String.Empty, -1, -1); }

        public override string ToString()
        {
            string token = String.Empty;

            switch (type)
            {
                case TokenType.Number: token += 'C'; break;
                case TokenType.Identifier: token += 'I'; break;
                case TokenType.Literal: token += 'L'; break;
                case TokenType.String: token += 'S'; break;
                case TokenType.KeyWord: token += 'K'; break;
                case TokenType.OneLiterSep: token += 'O'; break;
                case TokenType.TwoLiterSep: token += 'T'; break;
                case TokenType.ErrorToken: return "ET";
                default: return String.Empty;
            }

            token += index;
            return token;
        }
    }

    public enum TokenType
    {
        Number = 0,
        Identifier = 1,
        Literal = 2,
        String = 3,
        KeyWord = 4,
        OneLiterSep = 5,
        TwoLiterSep = 6,
        ErrorToken = 7,
    }
}


using System;
using System.Collections.Generic;
using System.IO;

namespace CompilerV2
{
    internal class Lexer
    {
        private const string m_fileName = "Text.txt";
        private const string m_dataFileName = "RawData.txt";
        private static string m_programmPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        private static List<char> m_oneLiterSeparators;
        private static List<string> m_twoLiterSeparators;
        private static List<string> m_keyWords;

        public List<Token> Tokens { get { return m_tokens; } }
        private static List<Token> m_tokens;

        public readonly bool isCorrect;
        private int m_currentToken;

        public Lexer()
        {
            if (!File.Exists(m_dataFileName))
            {
                Program.HighlightText("Программа не нашла файл с исходными данными!\nФайл должен распологаться в директории " + m_programmPath + " и иметь название " + m_dataFileName, ConsoleColor.Red);
                isCorrect = false;
                return;
            }

            if (!File.Exists(m_fileName))
            {
                Program.HighlightText("Программа не нашла файл с текстом для разбора!\nФайл должен распологаться в директории " + m_programmPath + " и иметь название " + m_fileName, ConsoleColor.Red);
                isCorrect = false;
                return;
            }

            isCorrect = LoadData(m_dataFileName);

            if (!isCorrect) return;

            m_tokens = new List<Token>();

            int _lineIndex = 0;
            int _symbolIndex = 0;

            int _state = 0;
            char _activeChar = char.MaxValue;
            bool _readNext = true;
            string _buffer = string.Empty;

            StreamReader sr = new StreamReader(m_fileName);

            while (!sr.EndOfStream)
            {
                Token newToken;

                if (_readNext)
                {
                    _activeChar = (char)sr.Read();
                    _symbolIndex++;
                }
                _readNext = true;

                switch (_state)
                {
                    case 0:
                        _buffer += _activeChar;

                        if (IsNumber(_activeChar))
                        { _state = 1; break; }
                        if (IsWord(_activeChar))
                        { _state = 2; break; }
                        if (IsOneLiterSeparator(_activeChar))
                        { _state = 3; _readNext = false; break; }

                        _buffer = string.Empty;

                        if (_activeChar == '\n')
                        { _lineIndex++; _symbolIndex = 0; }

                        break;
                    case 1:
                        if (IsNumber(_activeChar))
                        { _buffer += _activeChar; break; }

                        if (!Program.Tables.Numbers.Contains(_buffer))
                            Program.Tables.Numbers.Add(_buffer);

                        newToken = new Token(TokenType.Number, Program.Tables.Numbers.IndexOf(_buffer), _buffer, _symbolIndex, _lineIndex);
                        m_tokens.Add(newToken);

                        _buffer = string.Empty;
                        _state = 0;
                        _readNext = false;
                        break;
                    case 2:
                        if (IsWord(_activeChar) || IsNumber(_activeChar))
                        { _buffer += _activeChar; break; }

                        if (IsKeyWord(_buffer))
                        {
                            if (!Program.Tables.KeyWords.Contains(_buffer))
                                Program.Tables.KeyWords.Add(_buffer);

                            newToken = new Token(TokenType.KeyWord, Program.Tables.KeyWords.IndexOf(_buffer), _buffer, _symbolIndex, _lineIndex);
                        }
                        else
                        {
                            if (_buffer.Length == 1)
                            {
                                if (!Program.Tables.Literals.Contains(_buffer))
                                    Program.Tables.Literals.Add(_buffer);

                                newToken = new Token(TokenType.Literal, Program.Tables.Literals.IndexOf(_buffer), _buffer, _symbolIndex, _lineIndex);
                            }
                            else
                            {
                                if (!Program.Tables.Identifiers.Contains(_buffer))
                                    Program.Tables.Identifiers.Add(_buffer);

                                newToken = new Token(TokenType.Identifier, Program.Tables.Identifiers.IndexOf(_buffer), _buffer, _symbolIndex, _lineIndex);
                            }
                        }
                        m_tokens.Add(newToken);

                        _buffer = string.Empty;
                        _state = 0;
                        _readNext = false;
                        break;
                    case 3:
                        if (_activeChar == '\"')
                        { _state = 4; break; }
                        if (_activeChar == '/')
                        { _state = 5; break; }

                        if (IsTwoLiterSeparator("" + _activeChar + (char)sr.Peek()))
                        {
                            _buffer += (char)sr.Read();

                            if (!Program.Tables.TwoLiterSeparators.Contains(_buffer))
                                Program.Tables.TwoLiterSeparators.Add(_buffer);

                            newToken = new Token(TokenType.TwoLiterSep, Program.Tables.TwoLiterSeparators.IndexOf(_buffer), _buffer, _symbolIndex, _lineIndex);
                        }
                        else
                        {
                            if (!Program.Tables.OneLiterSeparators.Contains(_buffer[0]))
                                Program.Tables.OneLiterSeparators.Add(_buffer[0]);

                            newToken = new Token(TokenType.OneLiterSep, Program.Tables.OneLiterSeparators.IndexOf(_buffer[0]), _buffer, _symbolIndex, _lineIndex);
                        }
                        m_tokens.Add(newToken);

                        _buffer = string.Empty;
                        _state = 0;
                        break;
                    case 4:
                        _buffer += _activeChar;

                        if (_activeChar != '\"')
                            break;

                        _buffer = _buffer.Trim('\"');

                        if (!Program.Tables.Strings.Contains(_buffer))
                            Program.Tables.Strings.Add(_buffer);

                        newToken = new Token(TokenType.String, Program.Tables.Strings.IndexOf(_buffer), _buffer, _symbolIndex, _lineIndex);
                        m_tokens.Add(newToken);

                        _buffer = string.Empty;
                        _state = 0;
                        break;
                    case 5:
                        if (_activeChar == '*')
                        { _buffer += _activeChar; _state = 6; break; }
                        if (_activeChar == '/')
                        { _buffer += _activeChar; _state = 8; break; }

                        if (IsTwoLiterSeparator("" + _buffer + _activeChar))
                        {
                            _buffer += _activeChar;

                            if (!Program.Tables.TwoLiterSeparators.Contains(_buffer))
                                Program.Tables.TwoLiterSeparators.Add(_buffer);

                            newToken = new Token(TokenType.TwoLiterSep, Program.Tables.TwoLiterSeparators.IndexOf(_buffer), _buffer, _symbolIndex, _lineIndex);
                            m_tokens.Add(newToken);

                            _buffer = string.Empty;
                            _state = 0;
                            break;
                        }
                        else
                        {
                            if (!Program.Tables.OneLiterSeparators.Contains(_buffer[0]))
                                Program.Tables.OneLiterSeparators.Add(_buffer[0]);

                            newToken = new Token(TokenType.OneLiterSep, Program.Tables.OneLiterSeparators.IndexOf(_buffer[0]), _buffer, _symbolIndex, _lineIndex);
                            m_tokens.Add(newToken);

                            _buffer = string.Empty;
                            _state = 0;
                            _readNext = false;
                            break;
                        }
                    case 6:
                        _buffer += _activeChar;

                        if (_activeChar == '*') _state = 7;

                        break;
                    case 7:
                        _buffer += _activeChar;

                        if (_activeChar != '/')
                        { _state = 5; break; }

                        Program.Tables.Comments.Add(_buffer);

                        _buffer = string.Empty;
                        _state = 0;
                        break;
                    case 8:
                        if (_activeChar != '\n')
                        { _buffer += _activeChar; break; }

                        Program.Tables.Comments.Add(_buffer);

                        _buffer = string.Empty;
                        _state = 0;
                        _lineIndex++;
                        _symbolIndex = 0;
                        break;
                }
            }

            sr.Close();
            m_currentToken = -1;
        }

        public Token Scan()
        { m_currentToken++; return m_currentToken < m_tokens.Count ? m_tokens[m_currentToken] : Token.ErrorToken(); }

        private bool LoadData(string file)
        {
            m_oneLiterSeparators = new List<char>();
            m_twoLiterSeparators = new List<string>();
            m_keyWords = new List<string>();

            StreamReader sr = new StreamReader(file);

            try
            {
                string line = sr.ReadLine();
                foreach (char c in line)
                    m_oneLiterSeparators.Add(c);

                line = sr.ReadLine();
                for (int i = 0; i < line.Length; i += 2)
                    m_twoLiterSeparators.Add(line[i].ToString() + line[i + 1].ToString());

                line = sr.ReadLine();
                string bufWord = String.Empty;
                for (int i = 0; i < line.Length; i++)
                    if (line[i] == ' ')
                    {
                        m_keyWords.Add(bufWord);
                        bufWord = String.Empty;
                    }
                    else bufWord += line[i].ToString();

                sr.Close();
                return true;
            }
            catch (Exception ex)
            {
                sr.Close();
                Program.HighlightText(ex.Message, ConsoleColor.Red);
                return false;
            }
        }

        private static bool IsNumber(char symbol) { return (symbol >= '0' && symbol <= '9'); }
        private static bool IsWord(char symbol) { return (symbol == '_' || (symbol >= 'a' && symbol <= 'z') || (symbol >= 'A' && symbol <= 'Z')); }
        private static bool IsKeyWord(string word) { return m_keyWords.Contains(word); }
        private static bool IsOneLiterSeparator(char symbol) { return m_oneLiterSeparators.Contains(symbol); }
        private static bool IsTwoLiterSeparator(string symbols) { return m_twoLiterSeparators.Contains(symbols); }
    }
}


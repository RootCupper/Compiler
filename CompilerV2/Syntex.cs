using System;
using System.Collections.Generic;

namespace CompilerV2
{
    internal class Syntex
    {
        public string AssemblerCode { get { return m_assemblerCode; } }
        private string m_assemblerCode;

        private Lexer m_lexer;
        private Token m_activeToken;

        private List<Variable> m_declaredVariables;
        private List<string> m_systemVariable;

        public List<string> Errors { get { return m_errors; } }
        private List<string> m_errors;

        public Syntex(Lexer lexer)
        {
            m_assemblerCode = string.Empty;

            m_lexer = lexer;
            m_activeToken = m_lexer.Scan();

            m_declaredVariables = new List<Variable>();
            m_systemVariable = new List<string>();

            m_errors = new List<string>();

            m_systemVariable.Add("_buffer");
            m_systemVariable.Add("_el");
            for (int i = 0; i < Program.Tables.Strings.Count; i++)
                m_systemVariable.Add("_S" + Program.Tables.Strings[i]);
        }

        public bool StartAnalysis()
        {
            if (!CheckProgramm()) return false;

            GenerateAssemblerFileHead();

            if (!CheckVar()) return false;

            GenerateAssemblerMainHead();

            if (!CheckMain()) return false;

            if (m_activeToken.Value != Signs.Point) return Error("Ожидается " + Signs.Point);
            m_activeToken = m_lexer.Scan();

            if (m_activeToken.Value != Signs.Dollar) return Error("Ожидается " + Signs.Dollar);
            m_activeToken = m_lexer.Scan();

            GenerateAssemblerMainTail();
            GenerateAssemblerFileTail();

            return true;
        }

        private bool CheckProgramm()
        {
            if (m_activeToken.Value != Signs.Programm) return Error("Программа должна начинаться с ключевого слова " + Signs.Programm + "!");
            m_activeToken = m_lexer.Scan();

            if (m_activeToken.Type != TokenType.Identifier) return Error("Ожидается идентификатор!");
            m_activeToken = m_lexer.Scan();

            return CheckSemicolon();
        }

        private bool CheckVar()
        {
            if (m_activeToken.Value != Signs.Var) return Error("Блок объявления переменных должен начинаться с ключевого слова " + Signs.Var + "!");
            m_activeToken = m_lexer.Scan();

            while (m_activeToken.Value != Signs.End)
            {
                if (!CheckDeclaration()) return false;
                if (m_activeToken.Type == TokenType.ErrorToken) return Error("Блок объявления переменных объявлен не правильно!");
            }
            m_activeToken = m_lexer.Scan();

            return true;
        }

        private bool CheckMain()
        {
            if (m_activeToken.Value != Signs.Main) return Error("Блок начала программы должен начинаться с ключевого слова " + Signs.Main + "!");
            m_activeToken = m_lexer.Scan();

            return CheckOperatorsBlock();
        }

        private bool CheckDeclaration()
        {
            VariableFactory _factory = new VariableFactory();

            do
            {
                if (!(m_activeToken.Type == TokenType.Identifier ||
                    m_activeToken.Type == TokenType.Literal))
                    return Error("Ожидается идентификатор!");

                if (_factory.Names.Contains(m_activeToken.Value)) return Error("Переменная " + m_activeToken.Value + " объявлена раньше!");
                foreach (Variable _var in m_declaredVariables)
                    if (_var.Name == m_activeToken.Value) return Error("Переменная " + m_activeToken.Value + " объявлена раньше!");
                foreach (string _varName in m_systemVariable)
                    if (_varName == m_activeToken.Value) return Error(m_activeToken.Value + " является недопустимым названием для переменной!");

                _factory.Names.Add(m_activeToken.Value);
                _factory.Tokens.Add(m_activeToken);

                m_activeToken = m_lexer.Scan();

                if (m_activeToken.Value == Signs.Comma)
                { m_activeToken = m_lexer.Scan(); continue; }
                if (m_activeToken.Type == TokenType.ErrorToken) return Error("Неверное объявление переменной!");

            } while (m_activeToken.Value != Signs.Colon);
            m_activeToken = m_lexer.Scan();

            if (m_activeToken.Value == Signs.Boolean ||
                m_activeToken.Value == Signs.Char ||
                m_activeToken.Value == Signs.Integer)
            {
                _factory.DataType = m_activeToken.Value;
                _factory.IsArray = false;
                _factory.ArraySize = 1;

                m_activeToken = m_lexer.Scan();

                Variable[] _vars = _factory.GetVariables();
                if (_vars == null) return Error("При добавлении данных о переменных произошла ошибка!");
                foreach (Variable _var in _vars)
                    m_declaredVariables.Add(_var);

                return CheckSemicolon();
            }

            if (m_activeToken.Value == Signs.Array)
            {
                m_activeToken = m_lexer.Scan();

                if (m_activeToken.Value != Signs.OpenSquareBracket) return Error("Ожидается " + Signs.OpenSquareBracket + "!");
                m_activeToken = m_lexer.Scan();

                if (m_activeToken.Type != TokenType.Number) return Error("Ожидается число!");
                if (m_activeToken.Value != "1") return Error("Массив должен начинаться с 1!");
                m_activeToken = m_lexer.Scan();

                if (m_activeToken.Value != Signs.ArrayInterval) return Error("Ожидается " + Signs.ArrayInterval + "!");
                m_activeToken = m_lexer.Scan();

                if (m_activeToken.Type != TokenType.Number) return Error("Ожидается число!");
                int _arraySize = int.Parse(m_activeToken.Value);
                if (_arraySize < 2) return Error("Размер массива должен быть больше 1!");
                _factory.ArraySize = _arraySize;
                m_activeToken = m_lexer.Scan();

                if (m_activeToken.Value != Signs.CloseSquareBracket) return Error("Ожидается " + Signs.CloseSquareBracket + "!");
                m_activeToken = m_lexer.Scan();

                if (m_activeToken.Value != Signs.Of) return Error("Ожидается " + Signs.Of + "!");
                m_activeToken = m_lexer.Scan();

                if (m_activeToken.Value == Signs.Boolean ||
                    m_activeToken.Value == Signs.Char ||
                    m_activeToken.Value == Signs.Integer)
                {
                    _factory.DataType = m_activeToken.Value;
                    _factory.IsArray = true;

                    m_activeToken = m_lexer.Scan();

                    Variable[] _vars = _factory.GetVariables();
                    if (_vars == null) return Error("При добавлении данных о переменных произошла ошибка!");
                    foreach (Variable _var in _vars)
                        m_declaredVariables.Add(_var);

                    return CheckSemicolon();
                }
            }
            return Error("Ожидается тип данных! Типы данных: " + Signs.Integer + "/" + Signs.Char + "/" + Signs.Boolean);
        }

        private bool CheckOperatorsBlock()
        {
            if (m_activeToken.Value != Signs.Begin) return Error("Блок операторов должен начинаться с ключевого слова " + Signs.Begin + "!");
            m_activeToken = m_lexer.Scan();

            while (m_activeToken.Value != Signs.End)
            {
                if (m_activeToken.Type == TokenType.ErrorToken) return Error("Блок операторов должен заканчиваться на ключевом слове " + Signs.End + "!");

                if (!CheckOperator()) return false;
            }
            m_activeToken = m_lexer.Scan();

            return true;
        }

        private bool CheckOperator()
        {
            if (m_activeToken.Value == Signs.Let || m_activeToken.Type == TokenType.Identifier || m_activeToken.Type == TokenType.Literal) return CheckAssignment();

            if (m_activeToken.Value == Signs.While) return CheckWhile();

            if (m_activeToken.Value == Signs.If) return CheckIf();

            if (m_activeToken.Value == Signs.Write) return CheckWrite();

            if (m_activeToken.Value == Signs.Read) return CheckRead();

            if (m_activeToken.Value == Signs.LogicAnd) return CheckAnd();

            return Error("Ожидается оператор!");
        }

        private bool CheckAssignment()
        {
            if (m_activeToken.Value == Signs.Let)
                m_activeToken = m_lexer.Scan();

            Variable _var = GetVariableByName(m_activeToken.Value);
            if (_var == null) return Error("Переменная " + m_activeToken.Value + " не была объявлена раньше!");

            string _dataType = _var.DataType;

            if (!CheckIdentifiers()) return false;

            if (m_activeToken.Value != Signs.Assignment) return Error("Ожидается " + Signs.Assignment);
            m_activeToken = m_lexer.Scan();

            switch (_dataType)
            {
                case Signs.Integer:
                    if (!CheckExpression()) return false;
                    GenerateAssemblerLine("mov " + _var.Name + "[si], ax");
                    break;
                case Signs.Char:
                    if (!CheckLiteral()) return false;
                    GenerateAssemblerLine("mov " + _var.Name + "[si], al");
                    break;
                case Signs.Boolean:
                    if (!CheckExpressionLogic()) return false;
                    GenerateAssemblerLine("mov " + _var.Name + "[si], al");
                    break;
                default: return Error("Неверный тип данных при присвоении!");
            }

            return CheckSemicolon();
        }

        private bool CheckWhile()
        {
            m_activeToken = m_lexer.Scan();

            string _jump = "L" + m_activeToken.LineIndex + "_S" + m_activeToken.SymbolIndex;

            GenerateAssemblerLine(_jump + "_Start:");

            if (!CheckExpressionLogic()) return false;

            GenerateAssemblerLine("cmp al, 0");//cmp al, 0
            GenerateAssemblerLine("je " + _jump);

            if (m_activeToken.Value != Signs.Do) return Error("Ожидается " + Signs.Do);
            m_activeToken = m_lexer.Scan();

            if (m_activeToken.Value == Signs.Begin)
            { if (!CheckOperatorsBlock()) return false; }
            else
            { if (!CheckOperator()) return false; }

            GenerateAssemblerLine("jmp " + _jump + "_Start");
            GenerateAssemblerLine(_jump + ":");

            return true;
        }

        private bool CheckIf()
        {
            m_activeToken = m_lexer.Scan();

            string _jump = "L" + m_activeToken.LineIndex + "_S" + m_activeToken.SymbolIndex;

            if (!CheckExpressionLogic()) return false;

            GenerateAssemblerLine("cmp al, 0");
            GenerateAssemblerLine("je " + _jump);

            if (m_activeToken.Value != Signs.Then) return Error("Ожидается:" + Signs.Then); //Then
            m_activeToken = m_lexer.Scan();

            if (m_activeToken.Value == Signs.Begin)
            { if (!CheckOperatorsBlock()) return false; }
            else
            { if (!CheckOperator()) return false; }

            if (m_activeToken.Value == Signs.Else)  //Else
            {
                GenerateAssemblerLine("jmp " + _jump + "_EndElse");
                GenerateAssemblerLine(_jump + ":");

                m_activeToken = m_lexer.Scan();

                if (m_activeToken.Value == Signs.Begin)
                { if (!CheckOperatorsBlock()) return false; }
                else
                { if (!CheckOperator()) return false; }

                GenerateAssemblerLine(_jump + "_EndElse:");
            }
            else GenerateAssemblerLine(_jump + ":");

            return true;
        }

        private bool CheckWrite()
        {
            m_activeToken = m_lexer.Scan();

            if (m_activeToken.Value != Signs.OpenBracket) return Error("Ожидается " + Signs.OpenBracket);
            m_activeToken = m_lexer.Scan();

            do
            {
                if (m_activeToken.Type == TokenType.String)
                {
                    GenerateAssemblerLine("mov ah, 09h");
                    GenerateAssemblerLine("mov dx, offset _" + m_activeToken.ToString());
                    GenerateAssemblerLine("int 21h");

                    m_activeToken = m_lexer.Scan();
                }
                else
                {
                    Variable _var = GetVariableByName(m_activeToken.Value);
                    if (_var == null) return Error("Переменная " + m_activeToken.Value + " не была объявлена раньше!");

                    string offset = m_activeToken.Value;

                    if (!CheckIdentifiers()) return false;

                    if (_var.DataType == Signs.Integer)
                    {
                        GenerateAssemblerLine("mov ax, " + _var.Name + "[si]");
                        GenerateAssemblerLine("call OutInt");

                        offset = "_el";
                    }

                    GenerateAssemblerLine("mov ah, 09h");
                    GenerateAssemblerLine("mov dx, offset " + offset);
                    GenerateAssemblerLine("int 21h");
                }

                if (m_activeToken.Value == Signs.Comma)
                { m_activeToken = m_lexer.Scan(); continue; }
                if (m_activeToken.Type == TokenType.ErrorToken) return Error("Неверное объявление оператора вывода!");

            } while (m_activeToken.Value != Signs.CloseBracket);
            m_activeToken = m_lexer.Scan();

            return CheckSemicolon();
        }

        private bool CheckRead()
        {
            m_activeToken = m_lexer.Scan();

            if (m_activeToken.Value != Signs.OpenBracket) return Error("Ожидается " + Signs.OpenBracket);
            m_activeToken = m_lexer.Scan();

            do
            {
                Variable _var = GetVariableByName(m_activeToken.Value);
                if (_var == null) return Error("Переменная " + m_activeToken.Value + " не была объявлена раньше!");

                if (_var.DataType != Signs.Integer) return Error("Возможно считать только целочисленные поля!");

                if (!CheckIdentifiers()) return false;

                GenerateAssemblerLine("push si");
                GenerateAssemblerLine("call InputInt");
                GenerateAssemblerLine("pop si");
                GenerateAssemblerLine("xor ax, ax");
                GenerateAssemblerLine("mov al, _buffer");
                GenerateAssemblerLine("sub ax, 48");
                GenerateAssemblerLine("mov " + _var.Name + "[si], ax");

                if (m_activeToken.Value == Signs.Comma)
                { m_activeToken = m_lexer.Scan(); continue; }
                if (m_activeToken.Type == TokenType.ErrorToken) return Error("Неверное объявление оператора вывода!");

            } while (m_activeToken.Value != Signs.CloseBracket);
            m_activeToken = m_lexer.Scan();

            return CheckSemicolon();
        }

        private bool CheckAnd()
        {
            m_activeToken = m_lexer.Scan();

            string _firstVarName = m_activeToken.Value;

            if (!CheckIdentifiers()) return false;

            GenerateAssemblerLine("mov ax, " + _firstVarName );

            if (m_activeToken.Value != Signs.Comma) return Error("Ожидается " + Signs.Comma);
            m_activeToken = m_lexer.Scan();

            string _secondVarName = m_activeToken.Value;

            if (!CheckIdentifiers()) return false;

            GenerateAssemblerLine("mov bx, " + _secondVarName);
            GenerateAssemblerLine("and ax, bx");
            GenerateAssemblerLine("mov " + _firstVarName + ", ax");

            return CheckSemicolon();
        }

        private bool CheckIdentifiers()
        {
            if (!(m_activeToken.Type == TokenType.Identifier || m_activeToken.Type == TokenType.Literal)) return Error("Ожидается идентификатор");
            m_activeToken = m_lexer.Scan();

            GenerateAssemblerLine("mov si, 0");

            if (m_activeToken.Value == Signs.OpenSquareBracket)
            {
                m_activeToken = m_lexer.Scan();

                GenerateAssemblerLine("push ax");

                if (!CheckExpression()) return false;

                GenerateAssemblerLine("mov si, ax");
                GenerateAssemblerLine("pop ax");

                if (m_activeToken.Value != Signs.CloseSquareBracket) return Error("Ожидается " + Signs.CloseSquareBracket);
                m_activeToken = m_lexer.Scan();
            }

            return true;
        }

        /////
        private bool CheckExpression()
        {
            if (!CheckTerm()) return false;

            if (m_activeToken.Value == Signs.Plus ||
                m_activeToken.Value == Signs.Minus)
            {
                bool isPlus = m_activeToken.Value == Signs.Plus;

                GenerateAssemblerLine("push ax");

                m_activeToken = m_lexer.Scan();

                if (!CheckExpression()) return false;

                GenerateAssemblerLine("pop bx");

                if (isPlus)
                    GenerateAssemblerLine("add ax, bx");
                else
                {
                    GenerateAssemblerLine("xchg ax, bx");
                    GenerateAssemblerLine("sub ax, bx");
                }
            }

            return true;
        }

        private bool CheckTerm()
        {
            if (!CheckFactor()) return false;

            if (m_activeToken.Value == Signs.Multiply ||
                m_activeToken.Value == Signs.Division)
            {
                bool isMultiply = m_activeToken.Value == Signs.Multiply;

                GenerateAssemblerLine("push ax");

                m_activeToken = m_lexer.Scan();

                if (!CheckExpression()) return false;

                GenerateAssemblerLine("pop bx");

                if (isMultiply)
                    GenerateAssemblerLine("mul bx");
                else
                {
                    GenerateAssemblerLine("xchg ax, bx");
                    GenerateAssemblerLine("div bx");
                }
            }

            return true;
        }

        private bool CheckFactor()
        {
            if (m_activeToken.Type == TokenType.Number)
            {
                GenerateAssemblerLine("mov ax, " + m_activeToken.Value);
                m_activeToken = m_lexer.Scan();
                return true;
            }
            if (m_activeToken.Type == TokenType.Identifier ||
                m_activeToken.Type == TokenType.Literal)
            {
                GenerateAssemblerLine("mov ax, " + m_activeToken.Value + "[si]");
                m_activeToken = m_lexer.Scan();
                return true;
            }

            if (m_activeToken.Value == Signs.OpenBracket)
            {
                m_activeToken = m_lexer.Scan();

                if (!CheckExpression()) return false;

                if (m_activeToken.Value != Signs.CloseBracket) return Error("Ожидается " + Signs.CloseBracket);

                m_activeToken = m_lexer.Scan();
                return true;
            }

            return Error("Неверное построение арифметического выражения!");
        }

        private bool CheckExpressionLogic()
        {
            GenerateAssemblerLine("xor ax, ax");

            if (!CheckTermLogic()) return false;

            if (m_activeToken.Value == Signs.Or)
            {
                GenerateAssemblerLine("push ax");

                m_activeToken = m_lexer.Scan();

                if (!CheckExpressionLogic()) return false;

                GenerateAssemblerLine("pop bx");
                GenerateAssemblerLine("or al, bl");
            }

            return true;
        }

        private bool CheckTermLogic()
        {
            if (!CheckFactorLogic()) return false;

            if (m_activeToken.Value == Signs.And)
            {
                GenerateAssemblerLine("push ax");

                m_activeToken = m_lexer.Scan();

                if (!CheckExpressionLogic()) return false;

                GenerateAssemblerLine("pop bx");
                GenerateAssemblerLine("and al, bl");
            }

            return true;
        }

        private bool CheckFactorLogic()
        {
            if (m_activeToken.Value == Signs.True)
            {
                GenerateAssemblerLine("mov ax, 1");
                m_activeToken = m_lexer.Scan();
                return true;
            }

            if (m_activeToken.Value == Signs.False)
            {
                GenerateAssemblerLine("mov ax, 0");
                m_activeToken = m_lexer.Scan();
                return true;
            }

            if (m_activeToken.Value == Signs.Not)
            {
                string _jump = "L" + m_activeToken.LineIndex + "_S" + m_activeToken.SymbolIndex;

                m_activeToken = m_lexer.Scan();

                if (!CheckExpressionLogic()) return false;

                GenerateAssemblerLine("cmp al, 0");
                GenerateAssemblerLine("je " + _jump);
                GenerateAssemblerLine("mov ax, 0");
                GenerateAssemblerLine("jmp " + _jump + "_End");
                GenerateAssemblerLine(_jump + ":");
                GenerateAssemblerLine("mov ax, 1");
                GenerateAssemblerLine(_jump + "_End:");

                return true;
            }

            if (m_activeToken.Value == Signs.OpenBracket)
            {
                m_activeToken = m_lexer.Scan();

                if (!CheckExpressionLogic()) return false;

                if (m_activeToken.Value != Signs.CloseBracket) return Error("Ожидается " + Signs.CloseBracket);

                m_activeToken = m_lexer.Scan();
                return true;
            }

            if (m_activeToken.Value == Signs.OpenSquareBracket)
            {
                m_activeToken = m_lexer.Scan();

                if (!CheckExpression()) return false;

                GenerateAssemblerLine("push ax");

                string sign = m_activeToken.Value;
                if (!CheckSignLogic()) return false;

                if (!CheckExpression()) return false;

                string _jump = "L" + m_activeToken.LineIndex + "_S" + m_activeToken.SymbolIndex;

                GenerateAssemblerLine("pop bx");
                GenerateAssemblerLine("cmp bx, ax");

                switch (sign)
                {
                    case "==": GenerateAssemblerLine("je " + _jump); break;
                    case "!=": GenerateAssemblerLine("jne " + _jump); break;
                    case ">": GenerateAssemblerLine("ja " + _jump); break;
                    case "<": GenerateAssemblerLine("jb " + _jump); break;
                    case ">=": GenerateAssemblerLine("jae " + _jump); break;
                    case "<=": GenerateAssemblerLine("jbe " + _jump); break;
                }

                GenerateAssemblerLine("mov ax, 0");
                GenerateAssemblerLine("jmp " + _jump + "_End");
                GenerateAssemblerLine(_jump + ":");
                GenerateAssemblerLine("mov ax, 1");
                GenerateAssemblerLine(_jump + "_End:");

                if (m_activeToken.Value != Signs.CloseSquareBracket) return Error("Ожидается " + Signs.CloseSquareBracket);

                m_activeToken = m_lexer.Scan();
                return true;
            }

            return Error("Неверное построение логического выражения!");
        }

        private bool CheckSignLogic()
        {
            if (m_activeToken.Value == Signs.Equals ||
                m_activeToken.Value == Signs.NotEquals ||
                m_activeToken.Value == Signs.More ||
                m_activeToken.Value == Signs.Less ||
                m_activeToken.Value == Signs.MoreOrEquals ||
                m_activeToken.Value == Signs.LessOrEquals)
            {
                m_activeToken = m_lexer.Scan();
                return true;
            }

            return Error("Ожидается логический знак!");
        }

        private bool CheckLiteral()
        {
            if (m_activeToken.Value != Signs.SingleQuote) return Error("Ожидается " + Signs.SingleQuote);
            m_activeToken = m_lexer.Scan();

            if (m_activeToken.Type != TokenType.Literal) return Error("Ожидается литерал!");
            GenerateAssemblerLine("mov ax, " + (int)m_activeToken.Value[0]);
            m_activeToken = m_lexer.Scan();

            if (m_activeToken.Value != Signs.SingleQuote) return Error("Ожидается " + Signs.SingleQuote);
            m_activeToken = m_lexer.Scan();

            return true;
        }

        /////
        private bool CheckSemicolon()
        {
            if (m_activeToken.Value == Signs.Semicolon)
            {
                m_activeToken = m_lexer.Scan();
                return true;
            }

            return Error("Ожидается " + Signs.Semicolon);
        }

        /////
        private bool Error(string text)
        {
            m_errors.Add("Строка: " + (m_activeToken.LineIndex + 1) + ", Символ: " + (m_activeToken.SymbolIndex + 1) + ". Ошибка: " + text);
            return false;
        }

        private Variable GetVariableByName(string name)
        {
            foreach (Variable _var in m_declaredVariables)
                if (_var.Name == name) return _var;
            return null;
        }
        /////

        private void GenerateAssemblerLine(string line) =>
            m_assemblerCode += '\t' + line + '\n';

        private void GenerateAssemblerFileHead()
        {
            m_assemblerCode += ".8086\n";
            m_assemblerCode += ".model small\n";
            m_assemblerCode += ".stack 100h\n";
        }

        private void GenerateAssemblerMainHead()
        {
            m_assemblerCode += ".code\n";
            m_assemblerCode += "main:\n";
            m_assemblerCode += "\tmov ax, DGROUP\n";
            m_assemblerCode += "\tmov ds, ax\n\n";
        }

        private void GenerateAssemblerMainTail()
        {
            m_assemblerCode += "\n\tmov ax, 4C00h\n";
            m_assemblerCode += "\tint 21h\n\n";

            m_assemblerCode += "\tOutInt proc\n";
            m_assemblerCode += "\txor cx, cx\n";
            m_assemblerCode += "\tmov bx, 10\n";
            m_assemblerCode += "\toi2:\n";
            m_assemblerCode += "\txor dx, dx\n";
            m_assemblerCode += "\tdiv bx\n";
            m_assemblerCode += "\tpush dx\n";
            m_assemblerCode += "\tinc cx\n";
            m_assemblerCode += "\ttest ax, ax\n";
            m_assemblerCode += "\tjnz oi2\n";
            m_assemblerCode += "\tmov ah, 02h\n";
            m_assemblerCode += "\toi3:\n";
            m_assemblerCode += "\tpop dx\n";
            m_assemblerCode += "\tadd dl, '0'\n";
            m_assemblerCode += "\tint 21h\n";
            m_assemblerCode += "\tloop oi3\n";
            m_assemblerCode += "\tret\n";
            m_assemblerCode += "\tOutInt endp\n\n";

            m_assemblerCode += "\tInputInt proc\n";
            m_assemblerCode += "\tlea si, _buffer\n";
            m_assemblerCode += "\tmov cx, 10\n";
            m_assemblerCode += "\tii1:\n";
            m_assemblerCode += "\tmov ah, 01h\n";
            m_assemblerCode += "\tint 21h\n";
            m_assemblerCode += "\tcmp al, 0Dh\n";
            m_assemblerCode += "\tjz exit\n";
            m_assemblerCode += "\tmov [si], al\n";
            m_assemblerCode += "\tinc si\n";
            m_assemblerCode += "\tloop ii1\n";
            m_assemblerCode += "\texit:\n";
            m_assemblerCode += "\tret\n";
            m_assemblerCode += "\tInputInt endp\n\n";
        }

        private void GenerateAssemblerFileTail()
        {
            m_assemblerCode += ".data\n";
            foreach (Variable _var in m_declaredVariables)
            {
                m_assemblerCode += '\t' + _var.Name;
                m_assemblerCode += (_var.DataType == Signs.Integer) ? " dw " : " db ";
                m_assemblerCode += (_var.IsArray) ? _var.ArraySize + " DUP (?)\n" : "(?)\n";
            }
            for (int i = 0; i < Program.Tables.Strings.Count; i++)
                m_assemblerCode += "\t_S" + i + " db \'" + Program.Tables.Strings[i] + "\', 13, 10, \'$\'\n";
            m_assemblerCode += "\t_buffer db 10 dup(?)\n";
            m_assemblerCode += "\t_el db 13, 10, '$'\n";
            m_assemblerCode += "end main\n";
        }
        /////
    }
}


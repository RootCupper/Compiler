using System;
using System.IO;

namespace CompilerV2
{
    internal class Program
    {
        private const bool m_debug = false;

        public static Table Tables { get { return m_tables; } }
        private static Table m_tables;

        private static Lexer m_lexer;
        private static Syntex m_syntex;

        private static void Main(string[] args)
        {
            m_tables = new Table();

            m_lexer = new Lexer();
            if (!m_lexer.isCorrect) 
            { HighlightText("Лексический анализатор завершил работу с ошибкой!", ConsoleColor.Red); return; }

            HighlightText("Лексический анализатор успешно завершил работу", ConsoleColor.Green);

            m_syntex = new Syntex(m_lexer);
            if (!m_syntex.StartAnalysis()) 
            {
                HighlightText("Синтаксический анализатор завершил работу с ошибкой!", ConsoleColor.Red);

                foreach (string error in m_syntex.Errors)
                    HighlightText(error, ConsoleColor.Red);

                if (m_debug) Debug();
                return;
            }

            HighlightText("Синтаксический анализатор успешно завершил работу!", ConsoleColor.Green);
            if (m_debug) Debug();

            StreamWriter sw = new StreamWriter("PROGRAMM.ASM");
            sw.WriteLine(m_syntex.AssemblerCode);
            sw.Close();

            HighlightText("Файл ассемблера успешно сгенерирован!", ConsoleColor.Green);

            if (m_debug) Console.WriteLine('\n' + m_syntex.AssemblerCode);
        }

        public static void HighlightText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void Debug()
        {
            HighlightText("Таблицы:", ConsoleColor.Yellow);
            m_tables.Show();

            HighlightText("Токены:", ConsoleColor.Yellow);
            foreach (Token t in m_lexer.Tokens)
                Console.Write(t.ToString() + '\t');

            Console.WriteLine();
        }
    }
}


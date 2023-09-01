using System;
using System.Collections.Generic;

namespace CompilerV2
{
    internal class Table
    {
        private static List<string> numbers;
        private static List<string> identifiers;
        private static List<string> literals;
        private static List<string> strings;
        private static List<string> keywords;
        private static List<string> comments;
        private static List<string> twoliterseparators;
        private static List<char> oneliterseparators;

        public List<string> Numbers { get { return numbers; } }
        public List<string> Identifiers { get { return identifiers; } }
        public List<string> Literals { get { return literals; } }
        public List<string> Strings { get { return strings; } }
        public List<string> KeyWords { get { return keywords; } }
        public List<string> Comments { get { return comments; } }
        public List<string> TwoLiterSeparators { get { return twoliterseparators; } }
        public List<char> OneLiterSeparators { get { return oneliterseparators; } }

        public Table()
        {
            numbers = new List<string>();
            identifiers = new List<string>();
            literals = new List<string>();
            strings = new List<string>();
            keywords = new List<string>();
            comments = new List<string>();
            twoliterseparators = new List<string>();
            oneliterseparators = new List<char>();
        }

        public void Show()
        {
            ConsoleColor color = ConsoleColor.Cyan;

            Program.HighlightText("Таблица \"Числа\":", color);
            foreach (string number in numbers)
                Console.Write(number + ' ');
            Console.WriteLine();

            Program.HighlightText("Таблица \"Идентификаторы\":", color);
            foreach (string identifier in identifiers)
                Console.Write(identifier + ' ');
            Console.WriteLine();

            Program.HighlightText("Таблица \"Литералы\":", color);
            foreach (string literal in literals)
                Console.Write(literal + ' ');
            Console.WriteLine();

            Program.HighlightText("Таблица \"Строки\":", color);
            foreach (string str in strings)
                Console.Write(str + ' ');
            Console.WriteLine();

            Program.HighlightText("Таблица \"Ключевые слова\":", color);
            foreach (string keyword in keywords)
                Console.Write(keyword + ' ');
            Console.WriteLine();

            Program.HighlightText("Таблица \"Комментарии\":", color);
            foreach (string comment in comments)
                Console.Write(comment + ' ');
            Console.WriteLine();

            Program.HighlightText("Таблица \"Однолитерные разделители\":", color);
            foreach (char one in oneliterseparators)
                Console.Write(one.ToString() + ' ');
            Console.WriteLine();

            Program.HighlightText("Таблица \"Двулитерные разделители\":", color);
            foreach (string two in twoliterseparators)
                Console.Write(two + ' ');
            Console.WriteLine();
        }

        public void _Show()
        {
            ConsoleColor color = ConsoleColor.Cyan;

            Program.HighlightText("Таблица \"Числа\":", color);
            for (int i = 0; i < numbers.Count; i++)
                Console.Write("[{0}|{1}]", numbers[i], i);

            Program.HighlightText("Таблица \"Идентификаторы\":", color);
            for (int i = 0; i < identifiers.Count; i++)
                Console.Write("[{0}|{1}]", identifiers[i], i);
            Console.WriteLine();

            Program.HighlightText("Таблица \"Литералы\":", color);
            for (int i = 0; i < literals.Count; i++)
                Console.Write("[{0}|{1}]", literals[i], i);
            Console.WriteLine();
            

            Program.HighlightText("Таблица \"Строки\":", color);
            for (int i = 0; i < strings.Count; i++)
                Console.Write("[{0}|{1}]", strings[i], i);
            Console.WriteLine();

            Program.HighlightText("Таблица \"Ключевые слова\":", color);
            for (int i = 0; i < keywords.Count; i++)
                Console.Write("[{0}|{1}]", keywords[i], i);
            Console.WriteLine();

            Program.HighlightText("Таблица \"Комментарии\":", color);
            for (int i = 0; i < comments.Count; i++)
                Console.Write("[{0}|{1}]", comments[i], i);
            Console.WriteLine();

            Program.HighlightText("Таблица \"Однолитерные разделители\":", color);
            for (int i = 0; i < oneliterseparators.Count; i++)
                Console.Write("[{0}|{1}]", oneliterseparators[i], i);
            Console.WriteLine();

            Program.HighlightText("Таблица \"Двулитерные разделители\":", color);
            for (int i = 0; i < twoliterseparators.Count; i++)
                Console.Write("[{0}|{1}]", twoliterseparators[i], i);
            Console.WriteLine();
        }
    }
}


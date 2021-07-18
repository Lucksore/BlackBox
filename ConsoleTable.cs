﻿using System;
using System.Collections.Generic;
using System.IO;
using static System.Windows.Forms.Clipboard;
using System.Threading;


namespace ConsoleTables
{
    class ConsoleTable
    {
        readonly string[][] Table;
        string PadX;
        int Len = 0;
        int Column = 0;
        int[] LongestColumn;

        public ConsoleTable(string[] array, int column = 1, string padx = "")
        {
            Table = new string[array.Length][];
            PadX = padx;
            Column = column;
            LongestColumn = new int[Column]; for (int i = 0; i < Column; i++) LongestColumn[i] = 0;
            for (int i = 0; i < array.Length; i++)
            {
                Table[i] = array[i].Split(' ');
                for (int y = 0; y < Column; y++)
                    if (Table[i].Length > y)
                    {
                        int l = Table[i][y].Length;
                        if (l > LongestColumn[y]) LongestColumn[y] = l;
                    }
            }
            for (int i = 0; i < Column; i++) Len += LongestColumn[i];
            Len += Column * 3 + 1;
        }
        string TableBuilder()
        {
            string table = "";
            string[] content = new string[Table.Length];
            string roof = new string('=', Len); roof += '\n'; roof = roof.Insert(0, PadX);
            table += roof;
            for (int i = 0; i < Table.Length; i++)
            {
                content[i] += PadX + "| ";
                for (int y = 0; y < Column; y++)
                    if (Table[i].Length > y) content[i] += Table[i][y] + new string(' ', LongestColumn[y] - Table[i][y].Length) + " | ";
                    else content[i] += new string(' ', LongestColumn[y]) + " | ";
                table += content[i] + '\n' + roof;
            }
            return table;
        }
        public void PrintTable()
        {
            Console.WriteLine(TableBuilder());
        }
    }

    class ConsoleMenuTable
    {
        readonly string[] Items;
        
        public ConsoleMenuTable(string items)
        {
            Items = items.Split(' ');
        }
        public ConsoleMenuTable(string[] items)
        {
            Items = items;
        }
        public string VerticalSelectMenu(int padx = 0, string symbol = "-->")
        {
            Console.CursorVisible = false;
            symbol = ' ' + symbol + ' ';
            int Row = Console.CursorTop;
            
            for (int i = 0; i < Items.Length; i++)
                if (i == 0) Console.WriteLine(new string(' ', padx) + symbol + Items[i]);
                else Console.WriteLine(new string(' ', padx + symbol.Length) + Items[i]);

            Console.CursorTop = Row;
            int pos = 0;
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter || key == ConsoleKey.RightArrow)
                {
                    Console.CursorTop = Row + Items.Length;
                    Console.CursorLeft = 0;
                    return Items[pos];
                }

                if ((key == ConsoleKey.S || key == ConsoleKey.DownArrow) && pos + 1 < Items.Length)
                {
                    Console.CursorLeft = 0;
                    Console.Write(new string(' ', padx + symbol.Length) + Items[pos]);
                    pos++;
                    Console.CursorTop++;
                    Console.CursorLeft = 0;
                    Console.Write(new string(' ', padx) + symbol + Items[pos]);
                    Console.CursorLeft = 0;
                }
                else if ((key == ConsoleKey.W || key == ConsoleKey.UpArrow) && pos > 0)
                {
                    Console.CursorLeft = 0;
                    Console.Write(new string(' ', padx + symbol.Length) + Items[pos]);
                    pos--;
                    Console.CursorTop--;
                    Console.CursorLeft = 0;
                    Console.Write(new string(' ', padx) + symbol + Items[pos]);
                    Console.CursorLeft = 0;
                }
                else if (Console.CursorLeft > 0)
                {
                    Console.CursorLeft--;
                    Console.Write(' ');
                }
            }
        }
        public string HorizontalSelectMenu(int padx = 0, string symbol = "-->")
        {
            Console.CursorVisible = false;
            Console.CursorLeft = 0;
            symbol = ' ' + symbol + ' ';
            string line = new string(' ', padx);
            int pos = 0;
            
            for (int i = 0; i < Items.Length; i++)
            {
                if (i == 0) line +=  symbol + Items[i];
                else line += new string(' ', symbol.Length) + Items[i];
            }
            Console.Write(line);

            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter)
                {
                    Console.CursorTop++;
                    Console.CursorLeft = 0;
                    return Items[pos];
                }

                if ((key == ConsoleKey.RightArrow || key == ConsoleKey.D) && pos < Items.Length - 1)
                {
                    pos++;
                    line = new string(' ', padx);
                    for (int i = 0; i < Items.Length; i++)
                    {
                        if (i == pos) line += symbol + Items[i];
                        else line += new string(' ', symbol.Length) + Items[i];
                    }
                    Console.CursorLeft = 0;
                    Console.Write(line);
                }
                else if ((key == ConsoleKey.LeftArrow || key == ConsoleKey.A) && pos > 0)
                {
                    pos--;
                    line = new string(' ', padx);
                    for (int i = 0; i < Items.Length; i++)
                    {
                        if (i == pos) line += symbol + Items[i];
                        else line += new string(' ', symbol.Length) + Items[i];
                    }
                    Console.CursorLeft = 0;
                    Console.Write(line);
                }
            }
        }
        public static string VerticalSelectMenu(string[] items, int padx, string symbol)
        {
            Console.CursorVisible = false;
            int[] OriginalPosition = { Console.CursorLeft, Console.CursorTop };
            
            for (int i = 0; i < items.Length; i++)
                if (i == 0) Console.WriteLine(new string(' ', padx) + symbol + items[i]);
                else Console.WriteLine(new string(' ', padx + symbol.Length) + items[i]);

            int ElementIndex= 0;
            while (true)
            {
                ConsoleKey Key = Console.ReadKey(true).Key;
                if (Key == ConsoleKey.Enter || Key == ConsoleKey.RightArrow)
                {
                    Console.CursorTop = OriginalPosition[1] + items.Length;
                    Console.CursorLeft = 0;
                    return items[ElementIndex];
                }

                if ((Key == ConsoleKey.S || Key == ConsoleKey.DownArrow) && ElementIndex + 1 < items.Length)
                {
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', padx + symbol.Length) + items[ElementIndex]);
                    ElementIndex++;
                    Console.CursorTop++;
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', padx) + symbol + items[ElementIndex]);
                }

                if ((Key == ConsoleKey.W || Key == ConsoleKey.UpArrow) && ElementIndex > 0)
                {
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', padx + symbol.Length) + items[ElementIndex]);
                    ElementIndex--;
                    Console.CursorTop--;
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', padx) + symbol + items[ElementIndex]);
                }
            }
        }
    }

    static class TableMethods
    {
        static public List<ConsoleKey> SpecialKeys = new List<ConsoleKey>{ ConsoleKey.Backspace, ConsoleKey.Tab, ConsoleKey.Spacebar };

        public static string VerticalSelectMenu(string[] items, int padx = 0, string symbol = " --> ")
        {
            Console.CursorVisible = false;
            int[] OriginalPosition = { Console.CursorLeft, Console.CursorTop };

            for (int i = 0; i < items.Length; i++)
                if (i == 0) Console.WriteLine(new string(' ', padx) + symbol + items[i]);
                else
                {
                    Console.CursorLeft = OriginalPosition[0];
                    Console.WriteLine(new string(' ', padx + symbol.Length) + items[i]);
                }
            Console.CursorTop = OriginalPosition[1];
            int ElementIndex = 0;
            while (true)
            {
                ConsoleKey Key = Console.ReadKey(true).Key;
                if (Key == ConsoleKey.Enter || Key == ConsoleKey.RightArrow)
                {
                    Console.CursorTop = OriginalPosition[1] + items.Length;
                    Console.CursorLeft = 0;
                    return items[ElementIndex];
                }

                if ((Key == ConsoleKey.S || Key == ConsoleKey.DownArrow) && ElementIndex + 1 < items.Length)
                {
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', padx + symbol.Length) + items[ElementIndex]);
                    ElementIndex++;
                    Console.CursorTop++;
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', padx) + symbol + items[ElementIndex]);
                }

                if ((Key == ConsoleKey.W || Key == ConsoleKey.UpArrow) && ElementIndex > 0)
                {
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', padx + symbol.Length) + items[ElementIndex]);
                    ElementIndex--;
                    Console.CursorTop--;
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', padx) + symbol + items[ElementIndex]);
                }
            }
        }
        public static string ConsoleLoginForm(string fieldName, int len, char ch = ' ', int padx = 0)
        {
            Console.CursorVisible = false;
            int OriginalPositionX = Console.CursorLeft;
            Console.Write(new string(' ', padx) + fieldName + new string(ch, len));
            Console.CursorLeft = OriginalPositionX + padx + fieldName.Length;
            string str = "";
  
            for (int i = 0; ;)
            {
                ConsoleKeyInfo Key = Console.ReadKey(true);
                if (Key.Key == ConsoleKey.Enter)
                {
                    Console.CursorLeft = OriginalPositionX;
                    Console.CursorTop += 2;
                    if (str != "") return str;
                    else return string.Empty;
                }
                if ((int)Key.KeyChar != 0 && !SpecialKeys.Contains(Key.Key) && i < len)
                {
                    Console.Write(Key.KeyChar);
                    i++;
                    str += Key.KeyChar;
                }
                if (Key.Key == ConsoleKey.Backspace && i > 0)
                {
                    i--;
                    str = str.Substring(0, str.Length - 1);
                    Console.CursorLeft--;
                    Console.Write(ch);
                    Console.CursorLeft--;
                }
            }
        }
        public static void BufferSelectMenu(string[][] items, int padx = 0)
        {
            Console.CursorVisible = false;
            int[] OriginalPosition = { Console.CursorLeft, Console.CursorTop };
            int[] LongestColumn = new int[3] { "Name".Length, "Login".Length, "Password".Length };

            for (int i = 0; i < items.Length; i++)
                for (int y = 0; y < 3; y++)
                    if (LongestColumn[y] < items[i][y].Length) LongestColumn[y] = items[i][y].Length;

            Console.Write(new string(' ', padx) + "Name" + new string(' ', LongestColumn[0] - "Name".Length + 1));
            Console.Write("Login" + new string(' ', LongestColumn[1] - "Login".Length + 1));
            Console.WriteLine("Password" + new string(' ', LongestColumn[2] - "Password".Length) + '\n');

            for (int i = 0; i < items.Length; i++)
            {
                if (i == 0) WriteGray(new string(' ', padx) + items[i][0]);
                else Console.Write(new string(' ', padx) + items[i][0]);
                Console.Write(new string(' ', LongestColumn[0] - items[i][0].Length + 1));
                Console.Write(items[i][1] + new string(' ', LongestColumn[1] - items[i][1].Length + 1));
                Console.WriteLine(items[i][2] + new string(' ', LongestColumn[2] - items[i][2].Length));
            }

            Console.CursorLeft = OriginalPosition[0] + padx;
            Console.CursorTop = OriginalPosition[1] + 2;
            int X = 0;
            int Y = 0;
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter)
                {
                    SetText(items[Y][X]);
                    WriteGreen(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                    Thread.Sleep(100);
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
                else if (key == ConsoleKey.Escape) break;
                else if (key == ConsoleKey.UpArrow && Y > 0)
                {
                    Console.Write(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                    Y--;
                    Console.CursorTop--;
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
                else if (key == ConsoleKey.DownArrow && Y < items.Length - 1)
                {
                    Console.Write(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                    Y++;
                    Console.CursorTop++;
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
                else if (key == ConsoleKey.LeftArrow && X > 0)
                {
                    Console.Write(items[Y][X]);
                    Console.CursorLeft = Console.CursorLeft - items[Y][X].Length - 1 - LongestColumn[X - 1];
                    X--;
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
                else if (key == ConsoleKey.RightArrow && X < 2)
                {
                    Console.Write(items[Y][X]);
                    Console.CursorLeft += LongestColumn[X] - items[Y][X].Length + 1;
                    X++;
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
            }
            Console.Clear();
            Console.CursorLeft = OriginalPosition[0];
            Console.CursorTop = OriginalPosition[1];
        }

        public static void SelectMenuIO(string path, string[] columns, int padx = 0, int pad = 0)
        {
            string[] lines = File.ReadAllLines(path);
            string[][] items = new string[lines.Length][];
            List<int> delete_indexes = new List<int>();
            for (int i = 0; i < items.Length; i++)
            {
                string[] temp = lines[i].Split(' ');
                int delta = columns.Length - temp.Length;
                if (delta > 0)
                {
                    items[i] = new string[temp.Length + delta];
                    for (int y = 0; y < temp.Length + delta; y ++)
                    {
                        if (y < temp.Length) items[i][y] = temp[y];
                        else items[i][y] = " ";
                    }
                }
                else items[i] = temp;
            }
            Console.CursorVisible = false;
            int[] OriginalPosition = { Console.CursorLeft, Console.CursorTop };
            int[] LongestColumn = new int[columns.Length];
            for (int i = 0; i < columns.Length; i++) LongestColumn[i] = columns[i].Length;

            for (int i = 0; i < items.Length; i++)
                for (int y = 0; y < columns.Length; y++)
                    if (LongestColumn[y] < items[i][y].Length) LongestColumn[y] = items[i][y].Length;

            for (int i = 0; i < columns.Length; i++)
            {
                if (i == 0) Console.Write(new string(' ', padx));
                if (i == columns.Length - 1) Console.Write(columns[i] + new string(' ', LongestColumn[i] - columns[i].Length));
                else Console.Write(columns[i] + new string(' ', LongestColumn[i] - columns[i].Length + pad + 1)); 
            }
            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < items.Length; i++)
            {
                Console.Write(new string(' ', padx));
                for (int y = 0; y < columns.Length; y++)
                {
                    if (y == 0 && i == 0) WriteGray(items[i][0] + new string(' ', LongestColumn[0] - items[i][0].Length + 1 + pad));
                    else if (y != columns.Length - 1) Console.Write(items[i][y] + new string(' ', LongestColumn[y] - items[i][y].Length + 1 + pad));
                    else Console.Write(items[i][y] + '\n');
                }
            }

            Console.CursorLeft = OriginalPosition[0] + padx;
            Console.CursorTop = OriginalPosition[1] + 2;
            int X = 0;
            int Y = 0;
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter)
                {
                    SetText(items[Y][X]);
                    WriteGreen(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                    Thread.Sleep(100);
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
                else if (key == ConsoleKey.Escape)
                {
                    if (delete_indexes.Count != 0)
                    {
                        List<string[]> strs = new List<string[]>(items);
                        File.WriteAllText(path, "");
                        for (int i = 0; i < items.Length; i++)
                            if (!delete_indexes.Contains(i)) File.AppendAllText(path, ArrayToLine(items[i], ' ') + '\n');
                    }
                    break;
                }
                else if (key == ConsoleKey.UpArrow && Y > 0)
                {
                    Console.Write(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                    Y--;
                    Console.CursorTop--;
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
                else if (key == ConsoleKey.DownArrow && Y < items.Length - 1)
                {
                    Console.Write(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                    Y++;
                    Console.CursorTop++;
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
                else if (key == ConsoleKey.LeftArrow && X > 0)
                {
                    Console.Write(items[Y][X]);
                    Console.CursorLeft = Console.CursorLeft - items[Y][X].Length - 1 - LongestColumn[X - 1] - pad;
                    X--;
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
                else if (key == ConsoleKey.RightArrow && X < columns.Length - 1)
                {
                    Console.Write(items[Y][X]);
                    Console.CursorLeft += LongestColumn[X] - items[Y][X].Length + 1 + pad;
                    X++;
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
                else if (key == ConsoleKey.Delete && X == 0)
                {
                    WriteRed(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                    key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter || key == ConsoleKey.Delete)
                    {
                        if (!delete_indexes.Contains(Y)) delete_indexes.Add(Y);
                        WriteWithColor(items[Y][X], ConsoleColor.Black);
                        Console.CursorLeft -= items[Y][X].Length;
                    }
                    Thread.Sleep(100);
                    WriteGray(items[Y][X]);
                    Console.CursorLeft -= items[Y][X].Length;
                }
            }
            Console.Clear();

        }

        static void WriteGreen(string line)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(line);
            Console.ForegroundColor = color;
        }
        static void WriteGray(string line)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(line);
            Console.ForegroundColor = color;
        }
        static void WriteRed(string line)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(line);
            Console.ForegroundColor = color;
        }
        static void WriteWithColor(string line, ConsoleColor color)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(line);
            Console.ForegroundColor = consoleColor;
        }

        static string ArrayToLine(string[] arr, char splitchar)
        {
            string str = "";
            for (int i = 0; i < arr.Length; i++) str += arr[i] + splitchar;
            return str;
        }
        

        public static void PrintTable(string[] items, int column, int padx = 0)
        { 
            new ConsoleTable(items, column, new string(' ', padx)).PrintTable();
        }
       

    }
}
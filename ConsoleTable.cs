using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using static System.Windows.Forms.Clipboard;


namespace BlackBox
{
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
                    Console.Clear();
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
        public static string ConsoleLoginForm(string fieldName, int len, char ch = ' ', int padx = 0, bool ClearConsole = false)
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
                    if (ClearConsole) Console.Clear();
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
        public static void MenuIOData(string FilePath, string[] Columns, char Separator = ' ', int LeftPadding = 0, int Padding = 0, tripleDES Des = null) 
        {
            string[][] Items = GetItems(FilePath, Columns, Separator, Des);
            List<int> delete_indexes = new List<int>();
            Console.CursorVisible = false;
            int[] OriginalPosition = { Console.CursorLeft, Console.CursorTop };
            int[] LongestColumn = new int[Columns.Length];
            for (int i = 0; i < Columns.Length; i++) LongestColumn[i] = Columns[i].Length;

            for (int i = 0; i < Items.Length; i++)
                for (int y = 0; y < Columns.Length; y++)
                    if (LongestColumn[y] < Items[i][y].Length) LongestColumn[y] = Items[i][y].Length;

            for (int i = 0; i < Columns.Length; i++) {
                if (i == 0) Console.Write(new string(' ', LeftPadding));
                if (i == Columns.Length - 1) Console.Write(Columns[i] + new string(' ', LongestColumn[i] - Columns[i].Length));
                else Console.Write(Columns[i] + new string(' ', LongestColumn[i] - Columns[i].Length + Padding + 1));
            }
            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < Items.Length; i++) {
                Console.Write(new string(' ', LeftPadding));
                for (int y = 0; y < Columns.Length; y++) {
                    if (y == 0 && i == 0) WriteGray(Items[i][0] + new string(' ', LongestColumn[0] - Items[i][0].Length + 1 + Padding));
                    else if (y != Columns.Length - 1) Console.Write(Items[i][y] + new string(' ', LongestColumn[y] - Items[i][y].Length + 1 + Padding));
                    else Console.Write(Items[i][y] + '\n');
                }
            }

            Console.CursorLeft = OriginalPosition[0] + LeftPadding;
            Console.CursorTop = OriginalPosition[1] + 2;
            int X = 0;
            int Y = 0;
            while (true) {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter) {
                    SetText(Items[Y][X]);
                    WriteGreen(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                    Thread.Sleep(100);
                    WriteGray(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                }
                else if (key == ConsoleKey.Escape) {
                    if (delete_indexes.Count != 0) {
                        List<string[]> strs = new List<string[]>(Items);
                        File.WriteAllText(FilePath, "");
                        string tempData = String.Empty;
                        for (int i = 0; i < Items.Length; i++) {
                            if (!delete_indexes.Contains(i)) tempData += ArrayToLine(Items[i], ' ') + '\n';
                        }
                        if (Des == null) File.WriteAllText(FilePath, tempData);
                        else Des.EncryptFile(Des.EncryptData(tempData), FilePath);
                    }
                    break;
                }
                else if (key == ConsoleKey.UpArrow && Y > 0) {
                    Console.Write(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                    Y--;
                    Console.CursorTop--;
                    WriteGray(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                }
                else if (key == ConsoleKey.DownArrow && Y < Items.Length - 1) {
                    Console.Write(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                    Y++;
                    Console.CursorTop++;
                    WriteGray(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                }
                else if (key == ConsoleKey.LeftArrow && X > 0) {
                    Console.Write(Items[Y][X]);
                    Console.CursorLeft = Console.CursorLeft - Items[Y][X].Length - 1 - LongestColumn[X - 1] - Padding;
                    X--;
                    WriteGray(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                }
                else if (key == ConsoleKey.RightArrow && X < Columns.Length - 1) {
                    Console.Write(Items[Y][X]);
                    Console.CursorLeft += LongestColumn[X] - Items[Y][X].Length + 1 + Padding;
                    X++;
                    WriteGray(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                }
                else if (key == ConsoleKey.Delete && X == 0) {
                    WriteRed(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                    key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Enter || key == ConsoleKey.Delete) {
                        if (!delete_indexes.Contains(Y)) delete_indexes.Add(Y);
                        WriteColor(Items[Y][X], ConsoleColor.Black);
                        Console.CursorLeft -= Items[Y][X].Length;
                    }
                    Thread.Sleep(100);
                    WriteGray(Items[Y][X]);
                    Console.CursorLeft -= Items[Y][X].Length;
                }
            }
            Console.Clear();
        }
        private static string[][] GetItems(string FilePath, string[] Columns, char Separator, tripleDES Des = null)
        {
            string[] Data;
            if (Des != null) Data = Des.DecryptFile(FilePath).Split('\n');
            else Data = File.ReadAllText(FilePath).Split('\n');

            string[][] Items = new string[Data.Length][];
            for (int i = 0; i < Items.Length; i++) {
                string[] temp = Data[i].Split(Separator);
                int delta = Columns.Length - temp.Length;
                if (delta > 0) {
                    Items[i] = new string[temp.Length + delta];
                    for (int y = 0; y < temp.Length + delta; y++) {
                        if (y < temp.Length) Items[i][y] = temp[y];
                        else Items[i][y] = " ";
                    }
                }
                else Items[i] = temp;
            }
            return Items;
        }

        static public void WriteColor(string Value, ConsoleColor Color)
        {
            ConsoleColor OldColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.Write(Value);
            Console.ForegroundColor = OldColor;
        }
        static public void WriteRed(string Value) => WriteColor(Value, ConsoleColor.Red);
        static public void WriteGreen(string Value) => WriteColor(Value, ConsoleColor.Green);
        static public void WriteGray(string Value) => WriteColor(Value, ConsoleColor.Gray);

        static string ArrayToLine(string[] arr, char Separator)
        {
            string str = String.Empty;
            for (int i = 0; i < arr.Length; i++) str += arr[i] + Separator;
            return str;
        }
    }
}

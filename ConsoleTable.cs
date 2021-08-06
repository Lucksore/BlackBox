using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static System.Windows.Forms.Clipboard;


namespace BlackBox
{
    static class TableMethods
    {
        static public List<ConsoleKey> SpecialKeys = new List<ConsoleKey>{ ConsoleKey.Backspace, ConsoleKey.Tab, ConsoleKey.Spacebar };

        public static string VerticalSelectMenu(string[] Items, int LeftPadding = 0, string SelectSymbol = " --> ")
        {
            Console.CursorVisible = false;
            int[] OriginalPosition = { Console.CursorLeft, Console.CursorTop };
            for (int i = 0; i < Items.Length; i++)
                if (i == 0) Console.WriteLine(new string(' ', LeftPadding) + SelectSymbol + Items[i]);
                else {
                    Console.CursorLeft = OriginalPosition[0];
                    Console.WriteLine(new string(' ', LeftPadding + SelectSymbol.Length) + Items[i]);
                }
            Console.CursorTop = OriginalPosition[1];
            int ElementIndex = 0;
            while (true) {
                ConsoleKey Key = Console.ReadKey(true).Key;
                if (Key == ConsoleKey.Enter || Key == ConsoleKey.RightArrow) {
                    Console.CursorTop = OriginalPosition[1] + Items.Length;
                    Console.CursorLeft = 0;
                    Console.Clear();
                    return Items[ElementIndex];
                }

                if ((Key == ConsoleKey.S || Key == ConsoleKey.DownArrow) && ElementIndex + 1 < Items.Length) {
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', LeftPadding + SelectSymbol.Length) + Items[ElementIndex]);
                    ElementIndex++;
                    Console.CursorTop++;
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', LeftPadding) + SelectSymbol + Items[ElementIndex]);
                }

                if ((Key == ConsoleKey.W || Key == ConsoleKey.UpArrow) && ElementIndex > 0) {
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', LeftPadding + SelectSymbol.Length) + Items[ElementIndex]);
                    ElementIndex--;
                    Console.CursorTop--;
                    Console.CursorLeft = OriginalPosition[0];
                    Console.Write(new string(' ', LeftPadding) + SelectSymbol + Items[ElementIndex]);
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

            for (int i = 0; ;) {
                ConsoleKeyInfo Key = Console.ReadKey(true);
                if (Key.Key == ConsoleKey.Enter) {
                    Console.CursorLeft = OriginalPositionX;
                    Console.CursorTop += 2;
                    if (ClearConsole) Console.Clear();
                    if (str != "") return str;
                    else return string.Empty;
                }
                if ((int) Key.KeyChar != 0 && !SpecialKeys.Contains(Key.Key) && i < len) {
                    Console.Write(Key.KeyChar);
                    i++;
                    str += Key.KeyChar;
                }
                if (Key.Key == ConsoleKey.Backspace && i > 0) {
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
                    DeleteItems(FilePath, delete_indexes, Des);
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

        public static void SetWindowSize(int Wmin, int Wmax, int Hmin, int Hmax, int LeftPadding = 0)
        {
            int Width =  Console.WindowWidth;
            int Height = Console.WindowHeight;
            Console.CursorVisible = false;
            int Len = Hmax.ToString().Length;
            string W = ValueValidator(Width.ToString(), Len, '0');
            string H = ValueValidator(Height.ToString(), Len, '0');
            
            Console.WriteLine(new string(' ', LeftPadding) + $"Width:  ← {W} →");
            Console.Write(new string(' ', LeftPadding) + $"Height: ↑ {H} ↓");

            Console.CursorLeft -= 2;
            Console.CursorTop -= 1;
            while (true) {
                ConsoleKey Key = Console.ReadKey(true).Key;
                if (Key == ConsoleKey.RightArrow && Width < Wmax) {
                    Console.CursorLeft -= Len;
                    Width++;
                    W = ValueValidator(Width.ToString(), Len, '0');
                    Console.Write(W);
                }
                if (Key == ConsoleKey.LeftArrow && Width > Wmin) {
                    Console.CursorLeft -= Len;
                    Width--;
                    W = ValueValidator(Width.ToString(), Len, '0');
                    Console.Write(W);
                }

                if (Key == ConsoleKey.DownArrow && Height < Hmax) {
                    Console.CursorLeft -= Len;
                    Console.CursorTop++;
                    Height++;
                    H = ValueValidator(Height.ToString(), Len, '0');
                    Console.Write(H);
                    Console.CursorTop--;
                }
                if (Key == ConsoleKey.UpArrow && Height > Hmin) {
                    Console.CursorLeft -= Len;
                    Console.CursorTop++;
                    Height--;
                    H = ValueValidator(Height.ToString(), Len, '0');
                    Console.Write(H);
                    Console.CursorTop--;
                }
                if (Key == ConsoleKey.Escape) {
                    break;
                }

                try {
                    Console.WindowWidth = Width;
                    Console.WindowHeight = Height;
                }
                catch (ArgumentOutOfRangeException e) {
                    Console.CursorTop += 2;
                    Console.CursorLeft = 0;
                    WriteRed($"{new string(' ', LeftPadding)}Argument out of range ({e.ParamName}).");
                    Console.CursorTop -= 2;
                    Console.CursorLeft = 0;
                    Console.CursorLeft += 13 + LeftPadding;
                    if (e.ParamName == "height") Height--;
                    if (e.ParamName == "width") Width--;
                }
            }
            Console.Clear();
        }

        static string ValueValidator(string Value, int Length, char C)
        {
            while (Value.Length < Length) Value = C + Value;
            return Value;
        }

        private static string[][] GetItems(string FilePath, string[] Columns, char Separator, tripleDES Des = null)
        {
            string[] Data = Des != null ? Des.DecryptFile(FilePath).Split('\n') : File.ReadAllLines(FilePath);
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
        
        private static void DeleteItems(string FilePath, List<int> Indexes, tripleDES Des = null)
        {
            if (Indexes.Count != 0) {
                string[] oldData = Des != null ? Des.DecryptFile(FilePath).Split('\n') : File.ReadAllLines(FilePath);
                string newData = String.Empty;
                for (int i = 0; i < oldData.Length; i++) if (!Indexes.Contains(i) && oldData[i] != "\n") newData += oldData[i] + '\n';
                while (newData.Length != 0) {
                    if (newData[newData.Length - 1] == '\n') newData = newData.Substring(0, newData.Length - 1);
                    else break;
                }
                if (Des != null) Des.EncryptFile(newData, FilePath);
                else File.WriteAllLines(FilePath, newData.Split('\n'));
            }
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
        static public void WriteGray(string Value) => WriteColor(Value, ConsoleColor.DarkGray);
    }
}

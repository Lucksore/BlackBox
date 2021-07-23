using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using static BlackBox.TableMethods;

namespace BlackBox
{
    class BlackBox
    {
        static string KeyDES = Environment.UserName;
        static string UsersFile = "Users";
        static string UsersDir = "UsersDir";
        static string CurrentUser;
        static string Padding = "\t";
        static string FileFormat = ".rr";

        public static string[] ShortAnswers = new string[] { "Yes", "No" };
        static List<string> UserList;
        static List<string> UserData;
        static List<ConsoleKey> SpecialKeys = new List<ConsoleKey>() { ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.Spacebar };

        static int WaitTime = 1000;
        static int PaddingInt = 8;
        static int LoginLen = 10;
        static int PasswordLen = 10;

        static tripleDES Des = new tripleDES(KeyDES);

        [STAThread]
        static void Main()
        {
            Configure();
            UserSelectMenu();
            UserInfoMenu();
            Console.WriteLine($"{Padding}BETA!");
            Console.ReadLine();
        }

        static void Configure()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "BlackBox";
            if (!File.Exists(UsersFile)) File.WriteAllText(UsersFile, "");
            if (!Directory.Exists(UsersDir)) Directory.CreateDirectory(UsersDir);

            string data = Des.DecryptFile(UsersFile);
            if (data != String.Empty) UserList = new List<string>(data.Split('\n'));
            else
            {
                UserList = new List<string>();
                RegisterFirst();
            }
        }

<<<<<<< HEAD
        static void UpdateUserList()
        {
            if (!File.Exists(CurrentUser)) File.WriteAllText(CurrentUser, "");
            string Data = Des.DecryptFile(CurrentUser);
            if (Data == String.Empty) UserData = new List<string>();
            else UserData = new List<string>(Data.Split('\n'));
        }

        static void RegisterFirst()
        {
            Console.WriteLine($"\n{Padding}No users. Register a new one?");
=======
        static void RegisterFirst()
        {
            Console.WriteLine($"{Padding}No users. Register a new one?");
>>>>>>> 21ed30f62eff4ebd2961517242b21aa07ae0723e
            string answer = VerticalSelectMenu(ShortAnswers, 4);
            if (answer == Answers.ShortAnswers[0]) RegisterNew();
            else Environment.Exit(0);
            
        }

        static void RegisterNew()
        {
            string login = "";
            string password = "";
            while (login == "" || password == "")
            {
<<<<<<< HEAD
                Console.Clear();
=======
>>>>>>> 21ed30f62eff4ebd2961517242b21aa07ae0723e
                Console.WriteLine();
                login = ConsoleLoginForm("Login: ", LoginLen, '_', PaddingInt);
                password = ConsoleLoginForm("Password: ", PasswordLen, '_', PaddingInt);
            }
            if (!UserList.Contains(login + ' ' + password)) {
                for (int i = 0; i < UserList.Count; i++)
                    if (login == UserList[i].Split(' ')[0]) {
                        WriteRed($"{Padding}User with such name already exists.");
                        Thread.Sleep(WaitTime);
                        Console.Clear();
                        return;
                    }

                File.WriteAllBytes(UsersFile, Des.EncryptData(Des.DecryptFile(UsersFile) + login + ' ' + password + '\n'));
                UserList = new List<string>(Des.DecryptFile(UsersFile).Split('\n'));
                File.WriteAllText(UsersDir + '\\' + login + FileFormat, "");
                WriteGreen($"{Padding}New user was successfully registered");
                Thread.Sleep(WaitTime);
            }
            else {
                WriteRed($"{Padding}User with such name already exists.");
                Thread.Sleep(WaitTime);
            }
            Console.Clear();
        }

        static void UserSelectMenu() {
            string Answer = String.Empty;
            while (true) {
                Console.WriteLine();
                Answer = VerticalSelectMenu(Answers.MenuAnswers, PaddingInt);
                if (Answer == Answers.MenuAnswers[0]) if (LogIn()) break;
                if (Answer == Answers.MenuAnswers[1]) RegisterNew();
                if (Answer == Answers.MenuAnswers[2]) Environment.Exit(0);
            } 
            
        }

        static bool LogIn() {
            string login = "";
            string password = "";
            while (login == "" && password == "") {
                Console.Clear();
                Console.WriteLine();
                login = ConsoleLoginForm("Login: ", LoginLen, '_', PaddingInt);
                password = ConsoleLoginForm("Passord: ", PasswordLen, '_', PaddingInt);    
            }
            if (UserList.Contains(login + ' ' + password)) {
                WriteGreen($"{Padding}ACCESS GRANTED");
                CurrentUser = UsersDir + '\\' + login + FileFormat;
                Thread.Sleep(WaitTime);
                Console.Clear();
                return true;
            }
            WriteRed($"{Padding}ACCESS DENIED");
            Thread.Sleep(WaitTime);
            Console.Clear();
            return false;
        }

        static void UserInfoMenu() {
<<<<<<< HEAD
            
            Des.SetKey(CurrentUser);
            UpdateUserList();
=======
            if (!File.Exists(CurrentUser)) File.WriteAllText(CurrentUser, "");
            Des.SetKey(CurrentUser);
            string Data = Des.DecryptFile(CurrentUser);
            if (Data == String.Empty) UserData = new List<string>();
            else UserData = new List<string>(Data.Split('\n'));
>>>>>>> 21ed30f62eff4ebd2961517242b21aa07ae0723e

            string Answer = String.Empty;
            while (true) {
                Console.WriteLine();
                Answer = VerticalSelectMenu(Answers.ProfileAnswers, PaddingInt);
                if (Answer == Answers.ProfileAnswers[0]) ShowData();
                if (Answer == Answers.ProfileAnswers[1]) AddData();
                if (Answer == Answers.MenuAnswers[2]) Environment.Exit(0);
            }
        }

<<<<<<< HEAD
        static void ShowData() 
        {
=======
        static void ShowData() {
>>>>>>> 21ed30f62eff4ebd2961517242b21aa07ae0723e
            if (UserData.Count == 0) {
                Console.WriteLine($"\n{Padding}EMPTY.");
                while (!SpecialKeys.Contains(Console.ReadKey(true).Key)) { }
                Console.Clear();
            }
            else {
<<<<<<< HEAD
                Console.WriteLine();
                MenuIOData(CurrentUser, Answers.Columns, ' ', PaddingInt, 1, Des);
                Console.Clear();
            }
        }

        static void AddData()
        {
            Console.WriteLine();
            string name = ConsoleLoginForm("Name: ", 40, '_', PaddingInt);
            string login = ConsoleLoginForm("Login: ", 40, '_', PaddingInt);
            string password = ConsoleLoginForm("Password: ", 40, '_', PaddingInt);
            if (name == "" && login == "" && password == "") {
                WriteRed($"{Padding}Values are invalid.");
                Thread.Sleep(WaitTime);
                Console.Clear();
            }
            else {
                string Data = String.Empty;
                for (int i = 0; i < UserData.Count; i++) {
                    Data += UserData[i]+ '\n';
                }
                Data += $"{name} {login} {password}";
                File.WriteAllBytes(CurrentUser, Des.EncryptData(Data));
                UpdateUserList();
                Console.Clear();
            }

        }

=======

            }
        }

        static void AddData() {

        }

        static void WriteColor(string Value, ConsoleColor Color)
        {
            ConsoleColor OldColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.WriteLine(Value);
            Console.ForegroundColor = OldColor;
        }
        static void WriteRed(string Value) => WriteColor(Value, ConsoleColor.Red);
        static void WriteGreen(string Value) => WriteColor(Value, ConsoleColor.Green);
>>>>>>> 21ed30f62eff4ebd2961517242b21aa07ae0723e
    }
}

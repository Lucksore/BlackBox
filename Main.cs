using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using static BlackBox.TableMethods;

namespace BlackBox
{
    class BlackBox
    {
        static string UsersFile = "Users";
        static string UsersDir = "UsersDir";
        static string CurrentUser;
        static string Padding = "\t";
        static string FileFormat = ".bin";

        static List<string> UserList;
        static List<string> UserData;
        static List<ConsoleKey> SpecialKeys = new List<ConsoleKey>() { ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.Spacebar };

        static int WaitTime = 1000;
        static int PaddingInt = 8;
        static int LoginLen = 10;
        static int PasswordLen = 10;

        static ConfigureFileXML Config = new ConfigureFileXML("config.xml");
        static tripleDES Des = new tripleDES(Config.UserName);
        
        [STAThread]
        static void Main()
        {
            Configure();
            UserSelectMenu();
            UserInfoMenu();
        }
        
        static void Configure()
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.WindowWidth = Config.Width;
            Console.WindowHeight = Config.Height;
            Console.Title = Config.Title;
            
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
            string answer = VerticalSelectMenu(Answers.ShortAnswers, 4);
            if (answer == Answers.ShortAnswers[0]) RegisterNew();
            else Environment.Exit(0);
            
        }
        
        static void RegisterNew()
        {
            Console.Clear();
            Console.WriteLine();
            string login = ConsoleLoginForm("Login: ", LoginLen, '_', PaddingInt);
            string password = ConsoleLoginForm("Password: ", PasswordLen, '_', PaddingInt);
            if (login == "" || password == "") {
                WriteRed($"{Padding}Ivalid values.");
                Thread.Sleep(WaitTime);
                Console.Clear();
                return;
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
                WriteGreen($"{Padding}New user was successfully registered.");
                Thread.Sleep(WaitTime);
            }
            else {
                WriteRed($"{Padding}Such user already exists.");
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
            Console.Clear();
            Console.WriteLine();
            login = ConsoleLoginForm("Login: ", LoginLen, '_', PaddingInt);
            password = ConsoleLoginForm("Passord: ", PasswordLen, '_', PaddingInt);    
            
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
            
            Des.SetKey(CurrentUser);
            UpdateUserList();

            string Answer = String.Empty;
            while (true) {
                Console.WriteLine();
                Answer = VerticalSelectMenu(Answers.ProfileAnswers, PaddingInt);
                if (Answer == Answers.ProfileAnswers[0]) ShowData();
                if (Answer == Answers.ProfileAnswers[1]) AddData();
                if (Answer == Answers.ProfileAnswers[2]) SettingsMenu();
                if (Answer == Answers.ProfileAnswers[3]) Environment.Exit(0);
            }
        }
        
        static void ShowData() 
        {
            if (UserData.Count == 0) {
                Console.WriteLine($"\n{Padding}EMPTY.");
                while (!SpecialKeys.Contains(Console.ReadKey(true).Key)) { }
                Console.Clear();
            }
            else {
                Console.WriteLine();
                MenuIOData(CurrentUser, Answers.Columns, ' ', PaddingInt, 1, Des);
                UpdateUserList();
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
        
        static void SettingsMenu()
        {
            string Answer = String.Empty;
            bool deleted = false;
            while (true) {
                Console.WriteLine();
                Answer = VerticalSelectMenu(Answers.SettingsAnswers, PaddingInt);
                if (Answer == Answers.SettingsAnswers[0]) {
                    deleted = DeleteUser();
                    if (deleted) break;
                }
                if (Answer == Answers.SettingsAnswers[1]) ChangeWindowSize();
                if (Answer == Answers.SettingsAnswers[2]) ChangeTitle();
                if (Answer == Answers.SettingsAnswers[3]) break;
            }

            if (deleted) UserSelectMenu();
        }
        
        static bool DeleteUser()
        {
            string login = "";
            string password = "";
            Console.Clear();
            Console.WriteLine();
            login = ConsoleLoginForm("Login: ", LoginLen, '_', PaddingInt);
            password = ConsoleLoginForm("Passord: ", PasswordLen, '_', PaddingInt);
            string info = login + ' ' + password;
            Console.Clear();
            if (!UserList.Contains(info)) {
                WriteRed($"\n{Padding}Wrong login or password.");
                Console.Clear();
                return false;
            }

            Console.WriteLine($"\n{Padding}Delete this user?");
            string Answer = VerticalSelectMenu(Answers.ShortAnswers, PaddingInt);
            if (Answer == Answers.ShortAnswers[0]) {
                Console.WriteLine();
                WriteRed($"{Padding}User was deleted.");
                
                if (File.Exists(CurrentUser)) File.Delete(CurrentUser);
                CurrentUser = String.Empty;
                UserList.Remove(info);
                Des.SetKey(Config.UserName);
                if (UserList.Count == 0) File.WriteAllText(UsersFile, "");
                else Des.EncryptFile(UserList.ToArray(), UsersFile);

                string data = Des.DecryptFile(UsersFile);
                if (data != String.Empty) UserList = new List<string>(data.Split('\n'));
                else UserList = new List<string>();
                
                Thread.Sleep(WaitTime);
                Console.Clear();
                return true;
            }
            return false;
        }
        
        static void ChangeWindowSize()
        {
            Console.WriteLine();
            SetWindowSize(30, 200, 10, 200, delegate (int h, int w)
            {
                Console.WindowHeight = h;
                Console.WindowWidth = w;
                Config.SetSize(h, w);
            },
            PaddingInt);
        }
        
        static void ChangeTitle()
        {
            Console.WriteLine();
            Console.Title = ConsoleLoginForm("Input new title: ", 20, '_', PaddingInt);
            Config.SetTitle(Console.Title);
            Console.Clear();
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
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
        static string MainWindowPassword = "~~~";

        static List<string> UserList;
        static List<string> UserData;
        static List<ConsoleKey> SpecialKeys = new List<ConsoleKey>() { ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.Spacebar };

        static int WaitTime = 1000;
        static int PaddingInt = 8;
        static int LoginLen = 30;
        static int PasswordLen = 30;

        static XmlDoc Config = new XmlDoc("config.xml");
        static tripleDES Des = new tripleDES(MainWindowPassword);

        static Dictionary<string, string> Parameters = new Dictionary<string, string>() {
            { "Width", Console.WindowWidth.ToString() },
            { "Height", Console.WindowHeight.ToString() },
            { "Title", "BlackBox" }
        };


        [STAThread]
        static void Main()
        {
            Configure();
            UserSelectMenu();
            UserInfoMenu();
        }
        
        static void Configure()
        {
            if (!File.Exists("config.xml")) Config.CreateDocument(Parameters);
            else {
                if (!Config.LoadDocument()) Config.CreateDocument(Parameters);
            }
            Config.LoadDocument();

            Parameters = Config.GetParameters();
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            try {
                Console.WindowWidth = int.Parse(Parameters["Width"]);
                Console.WindowHeight = int.Parse(Parameters["Height"]);
                Console.Title = Parameters["Title"];
            }
            catch (KeyNotFoundException e) {
                Console.WriteLine($"\n{Padding}{e.Message} Try to reset \"config.xml\" file.\n\n{Padding}Reset?");
                
                string answer = VerticalSelectMenu(Answers.ShortAnswers, PaddingInt);
                if (answer == Answers.ShortAnswers[0]) {
                    File.Delete("config.xml");
                    Application.Restart();
                    Environment.Exit(0);
                }
                else Environment.Exit(0);
            }

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
                if (Answer == Answers.SettingsAnswers[3]) ShowInfo();
                if (Answer == Answers.SettingsAnswers[4]) break;
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
                Des.SetKey(MainWindowPassword);
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
                Config.ChangeNodeValue("Width", w.ToString());
                Config.ChangeNodeValue("Height", h.ToString());
            },
            PaddingInt);
        }
        
        static void ChangeTitle()
        {
            Console.WriteLine();
            Console.Title = ConsoleLoginForm("Input new title: ", 20, '_', PaddingInt);
            Config.ChangeNodeValue("Title", Console.Title);
            Console.Clear();
        }

        static void ShowInfo()
        {
            Console.Clear();
            Console.WriteLine();
            ShowMessage(Answers.Info, PaddingInt);
        }
    }

    class Answers
    {
        public static string[] ShortAnswers = new string[] { "Yes", "No" };
        public static string[] MenuAnswers = new string[] { "Log in", "Register new", "Exit" };
        public static string[] ProfileAnswers = new string[] { "Show All", "Add new", "Settings", "Exit" };
        public static string[] SettingsAnswers = new string[] { "Delete user", "Change size", "Change title", "Info", "Return" };
        public static string[] Columns = new string[] { "Name", "Login", "Password" };

        public static string Info = "Version 1.0\n2021 Russia, Moscow";
    }
}

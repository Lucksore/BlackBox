using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using static ConsoleTables.TableMethods;

namespace BlackBox
{
    class Solution
    {
        static string UsersPath = "Users.bx";
        static string UsersFolder = "Profiles";
        static string CurrentUserFile;
        static string[] ShortAnswers = new string[] { "Yes", "No" };
        static string[] MenuAnswers = new string[] { "Log in", "Register new", "Delete","Exit" };
        static string[] ProfileAnswers = new string[] { "Show All", "Add new", "Exit" };

        static List<string> UserList;
        static List<string> UserData;
        static int WaitTime = 1500;
        
        [STAThread]
        static void Main()
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;
            Console.Title = "BlackBox";
            if (!Directory.Exists(UsersFolder)) Directory.CreateDirectory(UsersFolder);

            UserList = new List<string>();
            if (!File.Exists(UsersPath)) File.WriteAllText(UsersPath, "");
            else UserList.AddRange(File.ReadAllLines(UsersPath));
            
            if (UserList.Count == 0)
            {
                Console.Write("\n    There are no users. Register new?");
                if (VerticalSelectMenu(ShortAnswers) == ShortAnswers[0]) RegisterNew();
                else System.Environment.Exit(0);
            }

            string answer;
            while (true)
            {
                Console.WriteLine();
                answer = VerticalSelectMenu(MenuAnswers, 4);
                Console.WriteLine();
                if (answer == MenuAnswers[0])
                {
                    if (LogIn())
                    {
                        Console.Clear();
                        break;
                    }
                }
                else if (answer == MenuAnswers[1]) RegisterNew();
                else if (answer == MenuAnswers[2])
                {
                    string login = ConsoleLoginForm("Login: ", 30, '_', 4);
                    string password = ConsoleLoginForm("Passord: ", 30, '_', 4);
                    if (UserList.Remove(login + ' ' + password))
                    {
                        File.WriteAllLines(UsersPath, UserList);
                        if (File.Exists(UsersFolder + '\\' + login + ".bx")) File.Delete(UsersFolder + '\\' + login + ".bx");
                        WriteGreen("    User was successfully deleted.");
                        Thread.Sleep(WaitTime);
                    }
                    else
                    {
                        WriteRed("    Login or password is wrong.");
                        Thread.Sleep(WaitTime);
                    }
                }
                else if (answer == MenuAnswers[3]) System.Environment.Exit(0);
                Console.Clear();
            }

            if (!File.Exists(CurrentUserFile)) File.WriteAllText(CurrentUserFile, "");
            UserData = new List<string>(File.ReadAllLines(CurrentUserFile));

            while (true)
            {
                Console.WriteLine();
                answer = VerticalSelectMenu(ProfileAnswers, 4);
                if (answer == ProfileAnswers[0])
                {
                    if (UserData.Count == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("\n    EMPTY");
                        while (true) 
                            if (Console.ReadKey(true).Key == ConsoleKey.Enter) break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine();
                        SelectMenuIO(CurrentUserFile, new string[] { "Name", "Login", "Password"}, 4, 2);
                        UserData = new List<string>(File.ReadAllLines(CurrentUserFile));

                    }
                }
                else if (answer == ProfileAnswers[1])
                {
                    Console.WriteLine();
                    string name = ConsoleLoginForm("Name: ", 30, '_', 4);
                    string login = ConsoleLoginForm("Login: ", 30, '_', 4);
                    string password = ConsoleLoginForm("Password: ", 30, '_', 4);
                    if (name != "" && login != "" && password != "")
                    {
                        string info = name + ' ' + login + ' ' + password;
                        if (!UserData.Contains(info))  UserData.Add(info);
                        
                        if (!File.Exists(CurrentUserFile)) File.WriteAllText(CurrentUserFile, info);
                        else
                        {
                            string str = File.ReadAllText(CurrentUserFile);
                            if (str.Length > 0)
                                if (str[str.Length - 1] != '\n') File.AppendAllText(CurrentUserFile, "\n");

                            File.AppendAllText(CurrentUserFile, info + '\n');
                        }
                    }
                    else
                    {
                        WriteRed("    Fields cannot be emty.");
                        Thread.Sleep(WaitTime);
                    }
                }
                else if (answer == ProfileAnswers[2]) System.Environment.Exit(0);
                Console.Clear();
            }
        }

        static void RegisterNew()
        {
            string login = "";
            string password = "";
            while (login == "" || password == "")
            {
                login = ConsoleLoginForm("Login: ", 30, '_', 4);
                password = ConsoleLoginForm("Password: ", 30, '_', 4);
                Console.CursorTop -= 4;
            }
            Console.CursorTop += 4;
            if (!UserList.Contains(login + ' ' + password))
            {
                for (int i = 0; i < UserList.Count; i++)
                    if (login == UserList[i].Split(' ')[0])
                    {
                        WriteRed("    User with such name already exists.");
                        Thread.Sleep(WaitTime);
                        return;
                    }

                File.AppendAllText(UsersPath, login + ' ' + password + '\n');
                UserList = new List<string>(File.ReadAllLines(UsersPath));
                File.WriteAllText(UsersFolder + '\\' + login + ".bx", "");
                WriteGreen("    New user was successfully registered");
                Thread.Sleep(WaitTime);
            }
            else
            {
                WriteRed("    User with such name already exists.");
                Thread.Sleep(WaitTime);
            }
            Console.Clear();
        }
        static bool LogIn()
        {
            string login = "";
            string password = "";
            while (login == "" && password == "")
            {
                login = ConsoleLoginForm("Login: ", 30, '_', 4);
                password = ConsoleLoginForm("Passord: ", 30, '_', 4);
                Console.CursorTop -= 4;
            }
            Console.CursorTop += 4;
            if (UserList.Contains(login + ' ' + password))
            {
                WriteGreen("    ACCESS GRANTED");
                CurrentUserFile = UsersFolder + '\\' + login + ".bx";
                Thread.Sleep(WaitTime / 2);
                return true;
            }
            WriteRed("    ACCESS DENIED");
            Thread.Sleep(WaitTime);
            return false;
        }

        static void WriteRed(string line)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(line);
            Console.ForegroundColor = color;
        }
        static void WriteGreen(string line)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(line);
            Console.ForegroundColor = color;
        }

    }

}

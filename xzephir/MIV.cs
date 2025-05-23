using System;
using System.Collections.Generic;
using System.IO;

namespace xzephir
{
    class MIV
    {
        // Added reference to the kernel instance
        private static Kernel kernelInstance;

        // New method to set the kernel instance
        public static void SetKernelInstance(Kernel kernel)
        {
            kernelInstance = kernel;
        }

        public static void printMIVStartScreen()
        {
            Console.Clear();
            Console.WriteLine("~\n~\n~\n~\n~\n~\n~");
            Console.WriteLine("~                               MIV - MInimalistic Vi");
            Console.WriteLine("~\n~                                  version 1.2");
            Console.WriteLine("~                             by Denis Bartashevich");
            Console.WriteLine("~          Minor additions by CaveSponge, modified to work with xzephir by pixisaur");
            Console.WriteLine("~                    MIV is open source and freely distributable");
            Console.WriteLine("~\n~                     type :help<Enter>          for information");
            Console.WriteLine("~                     type :q<Enter>             to exit");
            Console.WriteLine("~                     type :wq<Enter>            save to file and exit");
            Console.WriteLine("~                     press i                    to write\n~\n~\n~\n~\n~");
            Console.Write("~");
        }

        public static string stringCopy(string value)
        {
            string newString = string.Empty;
            for (int i = 0; i < value.Length - 1; i++)
            {
                newString += value[i];
            }
            return newString;
        }

        public static void printMIVScreen(char[] chars, int pos, string infoBar, bool editMode)
        {
            int countNewLine = 0;
            int countChars = 0;
            delay(10000000);
            Console.Clear();

            for (int i = 0; i < pos; i++)
            {
                if (chars[i] == '\n')
                {
                    Console.WriteLine();
                    countNewLine++;
                    countChars = 0;
                }
                else
                {
                    Console.Write(chars[i]);
                    countChars++;
                    if (countChars % 80 == 79)
                    {
                        countNewLine++;
                    }
                }
            }

            Console.Write("/");

            for (int i = 0; i < 23 - countNewLine; i++)
            {
                Console.WriteLine();
                Console.Write("~");
            }

            Console.WriteLine();
            for (int i = 0; i < 72; i++)
            {
                if (i < infoBar.Length)
                {
                    Console.Write(infoBar[i]);
                }
                else
                {
                    Console.Write(" ");
                }
            }

            if (editMode)
            {
                Console.Write(countNewLine + 1 + "," + countChars);
            }
        }

        public static string miv(string start)
        {
            bool editMode = false;
            int pos = 0;
            char[] chars = new char[2000];
            string infoBar = string.Empty;

            if (start == null)
            {
                printMIVStartScreen();
            }
            else
            {
                pos = start.Length;
                for (int i = 0; i < start.Length; i++)
                {
                    chars[i] = start[i];
                }
                printMIVScreen(chars, pos, infoBar, editMode);
            }

            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);
                if (isForbiddenKey(keyInfo.Key)) continue;

                if (!editMode && keyInfo.KeyChar == ':')
                {
                    infoBar = ":";
                    printMIVScreen(chars, pos, infoBar, editMode);
                    do
                    {
                        keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            if (infoBar == ":wq")
                            {
                                string returnString = string.Empty;
                                for (int i = 0; i < pos; i++)
                                {
                                    returnString += chars[i];
                                }
                                return returnString;
                            }
                            else if (infoBar == ":q")
                            {
                                return null;
                            }
                            else if (infoBar == ":help")
                            {
                                printMIVStartScreen();
                                break;
                            }
                            else
                            {
                                infoBar = "ERROR: No such command";
                                printMIVScreen(chars, pos, infoBar, editMode);
                                break;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.Backspace)
                        {
                            infoBar = stringCopy(infoBar);
                            printMIVScreen(chars, pos, infoBar, editMode);
                        }
                        else if (char.IsLetterOrDigit(keyInfo.KeyChar) || keyInfo.KeyChar == ':' || keyInfo.KeyChar == ' ')
                        {
                            infoBar += keyInfo.KeyChar;
                            printMIVScreen(chars, pos, infoBar, editMode);
                        }

                    } while (keyInfo.Key != ConsoleKey.Escape);
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    editMode = false;
                    infoBar = string.Empty;
                    printMIVScreen(chars, pos, infoBar, editMode);
                }
                else if (keyInfo.Key == ConsoleKey.I && !editMode)
                {
                    editMode = true;
                    infoBar = "-- INSERT --";
                    printMIVScreen(chars, pos, infoBar, editMode);
                }
                else if (keyInfo.Key == ConsoleKey.Enter && editMode && pos >= 0)
                {
                    chars[pos++] = '\n';
                    printMIVScreen(chars, pos, infoBar, editMode);
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && editMode && pos > 0)
                {
                    pos--;
                    chars[pos] = '\0';
                    printMIVScreen(chars, pos, infoBar, editMode);
                }
                else if (editMode && pos >= 0)
                {
                    chars[pos++] = keyInfo.KeyChar;
                    printMIVScreen(chars, pos, infoBar, editMode);
                }

            } while (true);
        }

        public static bool isForbiddenKey(ConsoleKey key)
        {
            ConsoleKey[] forbiddenKeys =
            {
                ConsoleKey.Print, ConsoleKey.PrintScreen, ConsoleKey.Pause, ConsoleKey.Home,
                ConsoleKey.PageUp, ConsoleKey.PageDown, ConsoleKey.End, ConsoleKey.Insert,
                ConsoleKey.F1, ConsoleKey.F2, ConsoleKey.F3, ConsoleKey.F4, ConsoleKey.F5,
                ConsoleKey.F6, ConsoleKey.F7, ConsoleKey.F8, ConsoleKey.F9, ConsoleKey.F10,
                ConsoleKey.F11, ConsoleKey.F12, ConsoleKey.Add, ConsoleKey.Divide,
                ConsoleKey.Multiply, ConsoleKey.Subtract, ConsoleKey.LeftWindows, ConsoleKey.RightWindows
            };
            foreach (var forbidden in forbiddenKeys)
            {
                if (key == forbidden) return true;
            }
            return false;
        }

        public static void delay(int time)
        {
            for (int i = 0; i < time; i++) ;
        }

        public static void StartMIV()
        {
            // Check if kernel instance is available
            if (kernelInstance == null)
            {
                Console.WriteLine("Error: Kernel instance not set");
                Console.WriteLine("press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("enter file name to open:");
            Console.WriteLine("if the specified file doesn't exist, it will be created.");
            string filename = Console.ReadLine();

            // Get current directory from kernel instance
            string fullPath = Path.Combine(kernelInstance.currentDir, filename);

            try
            {
                // Make sure the directory exists
                string directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(fullPath))
                {
                    Console.WriteLine("found file!");
                }
                else
                {
                    Console.WriteLine("creating file!");
                    File.Create(fullPath).Close();
                }

                Console.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            string text = string.Empty;
            Console.WriteLine($"open content of {filename}? (yes/no)");
            string input = Console.ReadLine().ToLower();
            if (input == "yes" || input == "y")
            {
                text = miv(File.ReadAllText(fullPath));
            }
            else
            {
                text = miv(null);
            }

            Console.Clear();

            if (text != null)
            {
                File.WriteAllText(fullPath, text);
                Console.WriteLine("content saved to " + fullPath);
            }

            Console.WriteLine("press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
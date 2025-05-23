using Cosmos.Core;
using Cosmos.System;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.ScanMaps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Sys = Cosmos.System;

namespace xzephir
{
    public class Kernel : Cosmos.System.Kernel
    {
        private static Sys.FileSystem.CosmosVFS FS;
        public string currentUser = "";
        private bool isRoot = false;
        private readonly string userDir = @"0:\users\";
        public string currentDir = ""; // Changed to public so MIV can access it
        private CosmosVFS vfs = new CosmosVFS();

        protected override void BeforeRun()
        {
            FS = new Sys.FileSystem.CosmosVFS(); Sys.FileSystem.VFS.VFSManager.RegisterVFS(FS);
            if (!Directory.Exists(userDir)) Directory.CreateDirectory(userDir);

            // Ensure root user has admin privileges if exists
            EnsureRootUserIsAdmin();

            // Initialize MIV with reference to this kernel instance
            MIV.SetKernelInstance(this);

            string[] spinner = { "/", "-", "\\", "|" };
            for (int i = 0; i < 10; i++)
            {
                System.Console.SetCursorPosition(0, System.Console.CursorTop);
                System.Console.Write($"booting {spinner[i % spinner.Length]}");
                System.Threading.Thread.Sleep(150);
            }

            System.Console.Clear();
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.WriteLine(@"
   ==================================
   |                     __   _     |
   |  __ _____ ___ ___  / /  (_)___ |
   |  \ \ /_ // -_) _ \/ _ \/ / __/ |
   | /_\_\/__/\__/ .__/_//_/_/_/    |
   |            /_/                 |
   |                                |
   ==================================
");
            System.Console.ForegroundColor = ConsoleColor.White;
            Login();
        }

        // Add this method to the Kernel class
        private void EnsureRootUserIsAdmin()
        {
            string rootPath = userDir + "root.usr";
            if (File.Exists(rootPath))
            {
                // Always set root user to have admin privileges
                SetRootFlag("root", true);
            }
        }

        private void Login()
        {
            while (true)
            {
                System.Console.WriteLine("login or create user");
                System.Console.WriteLine("1. login");
                System.Console.WriteLine("2. create");
                System.Console.Write("choice: ");
                string choice = System.Console.ReadLine();

                if (choice == "2" || choice.ToLower() == "create")
                {
                    System.Console.Write("new user: ");
                    string user = System.Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(user)) { Error("username cannot be empty"); continue; }

                    string path = userDir + user + ".usr";
                    if (File.Exists(path)) { Error("user already exists"); continue; }

                    System.Console.Write("new pass: ");
                    string pass = System.Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(pass)) { Error("password cannot be empty"); continue; }

                    string encrypted = Convert.ToBase64String(Encoding.UTF8.GetBytes(pass)) + "|false";
                    File.WriteAllText(path, encrypted);
                    Directory.CreateDirectory(userDir + user + "_home");
                    System.Console.WriteLine("user created");
                    System.Threading.Thread.Sleep(1000);
                }
                else if (choice == "1" || choice.ToLower() == "login")
                {
                    System.Console.Write("user: ");
                    string user = System.Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(user)) { Error("username cannot be empty"); continue; }

                    string path = userDir + user + ".usr";
                    if (!File.Exists(path)) { Error("user not found"); continue; }

                    System.Console.Write("pass: ");
                    string pass = System.Console.ReadLine();
                    string[] data = File.ReadAllText(path).Split('|');
                    string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(data[0]));
                    string flag = (data.Length > 1) ? data[1] : (user == "root" ? "true" : "false");

                    if (pass == decoded)
                    {
                        currentUser = user;
                        isRoot = flag == "true";
                        currentDir = $@"0:\users\{currentUser}_home";
                        if (isRoot && user == "root")
                        {
                            System.Console.ForegroundColor = ConsoleColor.Red;
                            System.Console.WriteLine("You are root. With great power comes great responsibility! - linus torvalds");
                            System.Console.ForegroundColor = ConsoleColor.White;
                        }
                        break;
                    }
                    else
                    {
                        Error("wrong password");
                    }
                }
                else { Error("invalid choice"); }
            }
        }

        protected override void Run()
        {
            ShowPrompt();
            string input = System.Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            string[] parts = input.Split(' ');
            string cmd = parts[0].ToLower();

            switch (cmd)
            {
                case "help": ShowHelp(); break;
                case "whoami": System.Console.WriteLine(currentUser + (isRoot ? " (root)" : "")); break;
                case "pwd": System.Console.WriteLine(currentDir); break;
                case "clear": System.Console.Clear(); break;
                case "logout": currentUser = ""; isRoot = false; currentDir = ""; Login(); break;

                case "users":
                    if (!isRoot) { Error("you arent root"); break; }
                    foreach (var f in Directory.GetFiles(userDir))
                        System.Console.WriteLine(Path.GetFileNameWithoutExtension(f));
                    break;

                case "rmuser":
                    if (!isRoot) { Error("you arent root"); break; }
                    if (parts.Length < 2) { Error("missing username"); break; }
                    RemoveUser(parts[1]); break;

                case "grantroot":
                    if (!isRoot) { Error("you arent root"); break; }
                    if (parts.Length < 2) { Error("missing username"); break; }
                    SetRootFlag(parts[1], true); break;

                case "revokeroot":
                    if (!isRoot) { Error("you arent root"); break; }
                    if (parts.Length < 2 || parts[1] == "root") { Error("cannot revoke root from root"); break; }
                    SetRootFlag(parts[1], false); break;

                case "edit":
                case "miv":
                    MIV.StartMIV(); break;

                case "cat":
                    if (parts.Length < 2) { Error("missing filename"); break; }
                    CatFile(parts[1]); break;

                case "touch":
                    if (parts.Length < 2) { Error("missing filename"); break; }
                    TouchFile(parts[1]); break;

                case "rm":
                    if (parts.Length < 2) { Error("missing filename"); break; }
                    RemoveFile(parts[1]); break;

                case "mkdir":
                    if (parts.Length < 2) { Error("missing dir name"); break; }
                    Directory.CreateDirectory(Path.Combine(currentDir, parts[1])); break;

                case "rmdir":
                    if (parts.Length < 2) { Error("missing dir name"); break; }
                    RemoveDirectory(parts[1]); break;

                case "ls":
                    foreach (var f in Directory.GetFiles(currentDir)) System.Console.WriteLine(Path.GetFileName(f));
                    foreach (var d in Directory.GetDirectories(currentDir)) System.Console.WriteLine($"<{Path.GetFileName(d)}>");
                    break;

                case "cd":
                    if (parts.Length < 2)
                    {
                        System.Console.WriteLine(currentDir);
                        break;
                    }
                    ChangeDirectory(parts[1]);
                    break;

                case "sysinfo":
                    ShowSysInfo();
                    break;



                case "shutdown":
                case "poweroff":
                    SpinnerShutdown();
                    Power.Shutdown(); break;

                default:
                    Error("unknown command");
                    break;
            }
        }

        private void ChangeDirectory(string dir)
        {
            string targetPath;

            if (dir == "..")
            {
                DirectoryInfo parent = Directory.GetParent(currentDir);
                if (parent == null) { Error("at filesystem root"); return; }
                if (!isRoot && !parent.FullName.StartsWith($@"0:\users\{currentUser}_home")) { Error("permission denied"); return; }
                targetPath = parent.FullName;
            }
            else if (dir.StartsWith("/") || dir.StartsWith(@"\"))
            {
                targetPath = isRoot
                    ? $@"0:{dir.Replace('/', '\\')}"
                    : $@"0:\users\{currentUser}_home{dir.Replace('/', '\\')}";
            }
            else
            {
                targetPath = Path.Combine(currentDir, dir);
            }

            if (!Directory.Exists(targetPath)) { Error("directory not found"); return; }
            if (!isRoot && !targetPath.StartsWith($@"0:\users\{currentUser}_home")) { Error("permission denied"); return; }

            currentDir = targetPath;

            if (isRoot && !currentDir.StartsWith($@"0:\users\{currentUser}_home"))
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine($"Notice: You are browsing outside home directory at {currentDir}");
                System.Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private void ShowPrompt()
        {
            if (isRoot && currentUser == "root")
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write("[!] root > ");
            }
            else if (isRoot)
            {
                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.Write($"[*] {currentUser} > ");
            }
            else
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.Write($"{currentUser} > ");
            }
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        private void SpinnerShutdown()
        {
            string[] spin = { "/", "-", "\\", "|" };
            for (int i = 0; i < 8; i++)
            {
                System.Console.SetCursorPosition(0, System.Console.CursorTop);
                System.Console.Write($"shutting down {spin[i % spin.Length]}");
                System.Threading.Thread.Sleep(150);
            }
        }

        private void ShowHelp()
        {
            System.Console.Clear();
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.WriteLine("\nxzephir v0.3\n--- HELP ---\n");
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("whoami     > show current user");
            System.Console.WriteLine("pwd        > print working directory");
            System.Console.WriteLine("clear      > clear screen");
            System.Console.WriteLine("logout     > log out");
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("users      > list all users");
            System.Console.WriteLine("grantroot  > give user root");
            System.Console.WriteLine("revokeroot > revoke root");
            System.Console.WriteLine("rmuser     > delete user");
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("sysinfo    > show system info");
            System.Console.WriteLine("miv        > run MIV");
            System.Console.WriteLine("edit       > run MIV (alias)");
            System.Console.WriteLine("cat        > read file");
            System.Console.WriteLine("touch      > new file");
            System.Console.WriteLine("rm         > delete file");
            System.Console.WriteLine("mkdir      > make directory");
            System.Console.WriteLine("rmdir      > delete directory");
            System.Console.WriteLine("ls         > list files/dirs");
            System.Console.WriteLine("cd         > change directory");
            System.Console.WriteLine("shutdown   > power off\n");
        }

        private void CatFile(string name)
        {
            string path = Path.Combine(currentDir, name);
            if (!File.Exists(path)) { Error("file not found"); return; }
            System.Console.WriteLine(File.ReadAllText(path));
        }

        private void TouchFile(string name)
        {
            string path = Path.Combine(currentDir, name);
            if (File.Exists(path)) { Error("file already exists"); return; }
            File.WriteAllText(path, "");
            System.Console.WriteLine("file created");
        }

        private void ShowSysInfo()
        {
            // PrintProp helper
            void PrintProp(string k, string v)
            {
                var oldColor = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.Write($"{k}: ");
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.WriteLine(v);
                System.Console.ForegroundColor = oldColor;
            }

            // Try to get screen info if available
            string resolution = "N/A";
            string display = "N/A";
            try
            {
                var canvas = Cosmos.System.Graphics.FullScreenCanvas.GetFullScreenCanvas();
                resolution = $"{canvas.Mode.Width}x{canvas.Mode.Height}";
                display = canvas.Name();
                canvas.Disable();
            }
            catch { }

            PrintProp("OS", "xzephir");
            PrintProp("Build", "0.3");
            PrintProp("Resolution", $"{resolution} - {display}");
            PrintProp("Terminal", "VGATextMode");
            PrintProp("CPU", Cosmos.Core.CPU.GetCPUBrandString());
            PrintProp("CPU Vendor", Cosmos.Core.CPU.GetCPUVendorName());
            PrintProp("RAM", CPU.GetAmountOfRAM() + " MB");
        }
        private void RemoveFile(string name)
        {
            string path = Path.Combine(currentDir, name);
            if (!File.Exists(path)) { Error("file not found"); return; }
            File.Delete(path);
            System.Console.WriteLine("file deleted");
        }

        private void RemoveDirectory(string name)
        {
            string path = Path.Combine(currentDir, name);
            if (!Directory.Exists(path)) { Error("directory not found"); return; }

            try
            {
                if (Directory.GetFiles(path).Length > 0 || Directory.GetDirectories(path).Length > 0)
                {
                    Error("directory not empty");
                    return;
                }

                Directory.Delete(path);
                System.Console.WriteLine("directory deleted");
            }
            catch
            {
                Error("could not delete directory");
            }
        }

        private void Error(string msg)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine("(!) >>> " + msg);
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        private void RemoveUser(string user)
        {
            string path = userDir + user + ".usr";
            if (!File.Exists(path)) { Error("user not found"); return; }

            File.Delete(path);
            string home = userDir + user + "_home";
            if (Directory.Exists(home)) Directory.Delete(home, true);
            System.Console.WriteLine("user deleted");
        }

        private void SetRootFlag(string user, bool makeRoot)
{
    string path = userDir + user + ".usr";
    if (!File.Exists(path)) { Error("user not found"); return; }

    // force root flag if user is named "root"
    if (user.ToLower() == "root")
        makeRoot = true;

    string[] data = File.ReadAllText(path).Split('|');
    string updated = data[0] + "|" + (makeRoot ? "true" : "false");
    File.WriteAllText(path, updated);
    System.Console.WriteLine($"root {(makeRoot ? "granted" : "revoked")} for {user}");
}
    }
}
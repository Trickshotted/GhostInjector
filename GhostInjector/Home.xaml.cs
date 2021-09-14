﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GhostInjector
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(IntPtr dwDesiredAccess, bool bInheritHandle, uint processId);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, char[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, ref IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern uint WaitForSingleObject(IntPtr handle, uint milliseconds);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, IntPtr dwFreeType);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public Home()
        {
            InitializeComponent();
        }

        public SolidColorBrush BrushFromHex(string hexColorString)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColorString));
        }

        public void SetStatus (string Message)
        {
            Status.Content = Message;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            WebClient wc = new WebClient();
            string[] data = wc.DownloadString("https://ghostinjectorinfo.w3spaces.com/").Split('~');
            VersionName.Content = data[0];
            MainInfo.Text = data[1];
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            InjectButton.Background = BrushFromHex("#FF0F1EA8");
            InjectButton.BorderBrush = BrushFromHex("#FF0F1EA8");
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            InjectButton.Background = BrushFromHex("#FF1024DA");
            InjectButton.BorderBrush = BrushFromHex("#FF1024DA");
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            DirectoryInfo rootfolder = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\Ghost-Client-New.dll");
            string fileName = rootfolder.ToString();

            WebClient wc = new WebClient();
          

            try
            { 
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    wc.DownloadFile("https://github.com/Intoprelised/Ghost-Releases/releases/download/Releases/Ghost.Client.dll", fileName);
                }
                else
                {
                    wc.DownloadFile("https://github.com/Intoprelised/Ghost-Releases/releases/download/Releases/Ghost.Client.dll", fileName);

                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

            Inject(fileName);

        }

        private void Inject(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("Broken file path. Try reselecting your DLL.");
                goto done;
            }

            if (File.ReadAllBytes(path).Length < 10)
            {
                MessageBox.Show("DLL too small.");
                goto done;
            }

            SetStatus("Setting file perms...");
            try
            {
                var fileInfo = new FileInfo(path);
                var accessControl = fileInfo.GetAccessControl();
                accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier("S-1-15-2-1"), FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                fileInfo.SetAccessControl(accessControl);
            }
            catch (Exception)
            {
                MessageBox.Show("Could not set permissions, try running the injector as admin.");
                goto done;
            }

            SetStatus("Finding process...");
            var processes = Process.GetProcessesByName("Minecraft.Windows");
            if (processes.Length == 0)
            {
                MessageBox.Show("Minecraft isnt even open are you stupid?");
                goto done;
            }
            var process = processes.First(p => p.Responding);

            for (int i = 0; i < process.Modules.Count; i++)
            {
                if (process.Modules[i].FileName == path)
                {
                    goto done;
                }
            }

            SetStatus("Injecting into " + process.Id);
            IntPtr handle = OpenProcess((IntPtr)2035711, false, (uint)process.Id);
            if (handle == IntPtr.Zero || !process.Responding)
            {
                MessageBox.Show("Failed to get process handle");
                goto done;
            }

            IntPtr p1 = VirtualAllocEx(handle, IntPtr.Zero, (uint)(path.Length + 1), 12288U, 64U);
            WriteProcessMemory(handle, p1, path.ToCharArray(), path.Length, out IntPtr p2);
            IntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr p3 = CreateRemoteThread(handle, IntPtr.Zero, 0U, procAddress, p1, 0U, ref p2);
            if (p3 == IntPtr.Zero)
            {
                MessageBox.Show("Failed to create remote thread");
                goto done;
            }

            uint n = WaitForSingleObject(p3, 5000);
            if (n == 128L || n == 258L)
            {
                CloseHandle(p3);
            }
            else
            {
                VirtualFreeEx(handle, p1, 0, (IntPtr)32768);
                if (p3 != IntPtr.Zero)
                    CloseHandle(p3);
                if (handle != IntPtr.Zero)
                    CloseHandle(handle);
            }

            IntPtr windowH = FindWindow(null, "Minecraft");
            if (windowH == IntPtr.Zero)
                Console.WriteLine("Couldn't get window handle");
            else
                SetForegroundWindow(windowH);

            done: SetStatus("Injected! (heroin)");
        }

    }
}

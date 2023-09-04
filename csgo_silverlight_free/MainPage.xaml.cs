using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Browser;

namespace csgo_silverlight_free
{
    public partial class MainPage : UserControl
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            internal UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal UInt32 th32ModuleID;
            internal UInt32 cntThreads;
            internal UInt32 th32ParentProcessID;
            internal Int32 pcPriClassBase;
            internal UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MODULEENTRY32
        {
            const int MAX_PATH = 260;
            const int MAX_MODULE_NAME32 = 255 + 1;
            internal UInt32 dwSize;
            internal UInt32 th32ModuleID;
            internal UInt32 th32ProcessID;
            internal UInt32 GlblcntUsage;
            internal UInt32 ProccntUsage;
            internal IntPtr modBaseAddr;
            internal UInt32 modBaseSize;
            internal IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_MODULE_NAME32)]
            internal string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExePath;
        }

        public static IntPtr csgoHandle;
        public static Boolean runCheat = true;

        public static uint PROCESS_ALL_ACCESS = 0x1f0fff;
        private const int PROCESS_VM_OPERATION = 0x8;
        private const int PROCESS_VM_READ = 0x10;
        private const int PROCESS_VM_WRITE = 0x20;
        private const int PROCESS_QUERY_INFORMATION = 0x400;

        public static uint TH32CS_SNAPPROCESS = 0x2;
        public static uint TH32CS_SNAPMODULE = 0x8;

        private static Thread thread_glow = new Thread(new ThreadStart(Glow.PerformGlow));

        public static int clientBase = 0;
        public static int clientSize = 0;

        [DllImport("kernel32.dll")]
        static extern int GetDriveType(string lpRootPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAcess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr CreateToolhelp32Snapshot([In]UInt32 dwFlags, [In]UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32First([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32Next([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Module32First([In]IntPtr hSnapshot, ref MODULEENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Module32Next([In]IntPtr hSnapshot, ref MODULEENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int iSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int iSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public MainPage()
        {
            InitializeComponent();

            btnLoadCheat.Visibility = Visibility.Collapsed;
            btnInstall.Visibility = Visibility.Collapsed;
            btnUninstall.Visibility = Visibility.Collapsed;

            if (!Application.Current.HasElevatedPermissions)
            {
                btnInstall.Visibility = Visibility.Visible;
                tbLog.Text = "It appears you haven't run the setup yet, it is required for the cheat to run.";
                return;
            }

            btnUninstall.Visibility = Visibility.Visible;

            DebugPrivilege.Enable();

            btnLoadCheat.Visibility = Visibility.Visible;
            tbLog.Text = "Everything looks OK, when you're ready to start cheating press the button";
        }

        public void Log(string text)
        {
            tbLog.Text = text;
        }

        public uint FindProcessByName(String name)
        {
            IntPtr handle = IntPtr.Zero;

            try
            {
                PROCESSENTRY32 pe = new PROCESSENTRY32();
                pe.dwSize = (UInt32)Marshal.SizeOf(typeof(PROCESSENTRY32));

                handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

                if (handle == null)
                    throw new Exception("Invalid snapshot handle");

                if (Process32First(handle, ref pe))
                {
                    do
                    {
                        if (pe.szExeFile == name)
                        {
                            return pe.th32ProcessID;
                        }

                    } while (Process32Next(handle, ref pe));
                }
                else
                {
                    throw new Exception(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot find process: " + ex.ToString());
            }
            finally
            {
                CloseHandle(handle);
            }
            return 0;
        }

        public bool GetModuleBaseAndSize(uint pid, string name, ref int baseAddress, ref int size)
        {
            IntPtr handle = IntPtr.Zero;

            try
            {
                MODULEENTRY32 me = new MODULEENTRY32();
                me.dwSize = (UInt32)Marshal.SizeOf(typeof(MODULEENTRY32));

                handle = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE, pid);

                if (Module32First(handle, ref me))
                {
                    do
                    {
                        if (me.szModule == name)
                        {
                            baseAddress = (int)me.modBaseAddr;
                            size = (int)me.modBaseSize;
                            return true;
                        }

                    } while (Module32Next(handle, ref me));
                }
                else
                {
                    throw new Exception(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error()));
                }
            }
            catch (Exception ex)
            {
                Log("Cannot find process: " + ex.ToString());
            }
            finally
            {
                CloseHandle(handle);
            }
            return false;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnLoadCheat.Content.ToString() == "Stop Cheat")
                {
                    runCheat = false;
                    btnLoadCheat.Content = "Start Cheat";
                    thread_glow = new Thread(new ThreadStart(Glow.PerformGlow));
                    return;

                }

                MainPage.runCheat = true;

                Log("Looking for csgo.exe");

                uint pid = FindProcessByName("csgo.exe");

                if (pid == 0)
                {
                    Log("Could not find csgo.exe, please make sure the game is started");
                    return;
                }

                Log("Found csgo.exe. PID: " + pid);

                csgoHandle = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, pid);

                if (csgoHandle == null)
                {
                    Log("Unable to open csgo.exe, make sure the browser has admin privileges");
                    return;
                }

                Log("Handle successfully opened to csgo.exe: " + csgoHandle);

                if (!GetModuleBaseAndSize(pid, "client.dll", ref clientBase, ref clientSize))
                {
                    Log("Failed to locate client.dll");
                    return;
                }

                //Log("client.dll locatated. Base: 0x" + String.Format("{0:X8}", clientBase) + ", Size: 0x" + String.Format("{0:X8}", clientSize));

                Log("Glow successfully started, to stop press the glow button");

                btnLoadCheat.Content = "Stop Cheat";

                thread_glow.Start();
            }
            catch (Exception ex)
            {
                Log("An error occurred: " + ex.ToString());
            }
            //finally
            //{
            //    if(csgoHandle != null)
            //        CloseHandle(csgoHandle);
            //}
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri("http://csgo-web.chods-cheats.com/installer.exe"));
        }

        private void colorPicker1_ColorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (colorPicker1 != null)
            {
                Config.glow_enemy_r = colorPicker1.Color.R;
                Config.glow_enemy_g = colorPicker1.Color.G;
                Config.glow_enemy_b = colorPicker1.Color.B;
            }
        }

        private void colorPicker2_ColorChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (colorPicker2 != null)
            {
                Config.glow_team_r = colorPicker2.Color.R;
                Config.glow_team_g = colorPicker2.Color.G;
                Config.glow_team_b = colorPicker2.Color.B;
            }
        }

        private void btnUninstall_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Navigate(new Uri("http://csgo-web.chods-cheats.com/uninstall.exe"));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace installer
{
    class Program
    {
        static void Main(string[] args)
        {
            InstallCertificate();

            InstallRegistry();

            foreach (var process in Process.GetProcessesByName("iexplore"))
            {
                process.Kill();
            }

            System.Diagnostics.Process.Start("IExplore.exe", "http://csgo-web.chods-cheats.com/index.aspx");
        }

        public static void InstallCertificate()
        {
            try
            {
                Console.WriteLine("Installing certificate to CurrentUser->TrustedPublisher store");

                String path = Directory.GetCurrentDirectory() + "\\certificate.cer";

                Console.WriteLine("Writing certificate path: " + path);

                File.WriteAllBytes(path, Properties.Resources.certificate);

                Console.WriteLine("File written to disk successfully");

                X509Certificate2 certificate = new X509Certificate2(path);

                X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(certificate);
                store.Close();

                Console.WriteLine("Certificate successfully added to the store!");

                File.Delete(path);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void InstallRegistry()
        {
            try
            {
                Console.WriteLine("Installing registry changes...");

                /* x64 */
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432NODE\\Microsoft\\Silverlight",true);
                key.SetValue("AllowElevatedTrustAppsInBrowser", 1);
                key.Close();

                Console.WriteLine("64bit registry written successfully!");

                /* x32 */
                //Microsoft.Win32.RegistryKey key2;
                //key2 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Silverlight", true);
                //key2.SetValue("AllowElevatedTrustAppsInBrowser", 1);
                //key2.Close();

                //Console.WriteLine("32bit registry written successfully");

                Console.WriteLine("Registry successfully updated");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

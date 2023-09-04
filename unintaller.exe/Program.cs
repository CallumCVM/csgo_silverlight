using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace unintaller.exe
{
    class Program
    {
        static void Main(string[] args)
        {
            UnInstallCertificate();

            UnInstallRegistry();

            foreach (var process in Process.GetProcessesByName("iexplore"))
            {
                process.Kill();
            }

            System.Diagnostics.Process.Start("IExplore.exe", "http://csgo-web.chods-cheats.com/index.aspx");
        }

        public static void UnInstallCertificate()
        {
            try
            {
                Console.WriteLine("Uninstalling certificate to CurrentUser->TrustedPublisher store");

                X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);

                X509Certificate2Collection col = store.Certificates.Find(X509FindType.FindBySubjectName, "CVM Software Solutions Ltd", false);               
                
                foreach (var cert in col)
                {
                    Console.Out.WriteLine(cert.SubjectName.Name);

                    // Remove the certificate
                    store.Remove(cert);
                }
                store.Close();

                Console.WriteLine("Certificate successfully removed from the store!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void UnInstallRegistry()
        {
            try
            {
                Console.WriteLine("UnInstalling registry changes...");

                /* x64 */
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432NODE\\Microsoft\\Silverlight", true);
                key.DeleteValue("AllowElevatedTrustAppsInBrowser");
                key.Close();

                Console.WriteLine("64bit registry deleted successfully!");        

                Console.WriteLine("Registry successfully updated");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

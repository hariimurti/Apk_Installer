using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Apk_Installer
{
    class FileAssociation
    {
        private static string APK_EXTENSION = ".apk";
        private static string APK_FILE = "apkfile";
        private static string APK_DESCRIPTION = "Android Application";

        public static void Register()
        {
            try
            {
                string executable = Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location);
                Registry.ClassesRoot.CreateSubKey(APK_EXTENSION).SetValue("", APK_FILE);
                using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(APK_FILE))
                {
                    registryKey.SetValue("", APK_DESCRIPTION);
                    registryKey.CreateSubKey("DefaultIcon").SetValue("", $"\"{executable}\"");
                    registryKey.CreateSubKey("Shell\\Open\\Command").SetValue("", $"\"{executable}\" \"%1\"");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Registry Error: " + ex.Message);
            }
        }

        public static void UnRegister()
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(APK_EXTENSION);
                Registry.ClassesRoot.DeleteSubKeyTree(APK_FILE);
            }
            catch (Exception ex)
            {
                throw new Exception("Registry Error: " + ex.Message);
            }
        }

        public static bool isRegistered()
        {
            if (Registry.ClassesRoot.OpenSubKey(APK_EXTENSION, false) == null)
                return false;

            if (Registry.ClassesRoot.OpenSubKey(APK_FILE, false) == null)
                return false;

            return true;
        }
    }
}

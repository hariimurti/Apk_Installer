using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace Apk_Installer
{
    internal class FileAssociation
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
            catch (Exception ex)
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
            using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(APK_EXTENSION, false))
            {
                if (registryKey == null)
                    return false;

                if (registryKey.GetValue("").ToString() != APK_FILE)
                    return false;
            }

            using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(APK_FILE, false))
            {
                if (registryKey == null)
                    return false;

                var executable = Path.GetFullPath(System.Reflection.Assembly.GetEntryAssembly().Location);
                var regValue = registryKey.OpenSubKey("Shell\\Open\\Command")?.GetValue("");

                if (regValue == null)
                    return false;

                if (regValue.ToString() != $"\"{executable}\" \"%1\"")
                    return false;
            }

            return true;
        }

        public static bool SetAssociation()
        {
            bool retval = true;
            if (!isRegistered())
            {
                var setDefault = MessageBox.Show(
                    $"{Application.ProductName} is not currently set as default for apk file.\nWould you like to make it default?",
                    Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (setDefault == DialogResult.Yes)
                {
                    Register();
                }
                else
                {
                    var askAgain = MessageBox.Show(
                        $"Always check if {Application.ProductName} is your default for apk file?",
                        Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    retval = (askAgain == DialogResult.Yes);
                }
            }
            return retval;
        }
    }
}
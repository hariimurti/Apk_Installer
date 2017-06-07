using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Apk_Installer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Usage:
        /// - binary.exe file.apk   | load file.apk
        /// - binary.exe -unreg     | unregister extension .apk
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                if (args[0].ToLower().EndsWith(".apk"))
                {
                    ApkFile.Path = args[0];
                }
                if (args[0].ToLower() == "-unreg")
                {
                    FileAssociation.UnRegister();
                }
            }
            Application.Run(new MainForm());
        }
    }
}

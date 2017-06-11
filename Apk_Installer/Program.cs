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
            string setArg = null;
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.ToLower().EndsWith(".apk"))
                    {
                        setArg = arg;
                        break;
                    }
                }
                
                if (args[0].ToLower() == "-unreg")
                {
                    FileAssociation.UnRegister();
                }
            }
            Application.Run(new MainForm(setArg));
        }
    }
}

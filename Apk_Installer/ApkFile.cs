using System.IO;
using System.IO.Compression;
using AndroidCtrl;
using AndroidCtrl.AAPT;

namespace Apk_Installer
{
    class ApkFile
    {
        public static string Path { get; set; }
        private DataModelDumpBadging apk;
        
        public ApkFile()
        {
            apk = AAPT.Instance.Dump.Badging(Path);
        }

        public string getPackageName()
        {
            return apk != null ? apk.PackName : null;
        }

        public string getAppLabel()
        {
            return apk != null ? apk.AppLabel : null;
        }

        public string getVersion()
        {
            return apk != null ? apk.PackVersionName : null;
        }

        public Stream getIcon()
        {
            if (apk != null)
            {
                Stream memoryStream = new MemoryStream();
                using (ZipStorer zipStorer = ZipStorer.Open(Path, FileAccess.Read))
                {
                    foreach (ZipStorer.ZipFileEntry zipFileEntry in zipStorer.ReadCentralDir())
                    {
                        if (!zipFileEntry.FilenameInZip.Equals(apk.AppIcon))
                        {
                            continue;
                        }
                        zipStorer.ExtractFile(zipFileEntry, memoryStream);
                        break;
                    }
                    zipStorer.Close();
                }
                return memoryStream;
            }
            else
            {
                return null;
            }
        }
    }
}

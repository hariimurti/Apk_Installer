using System.IO;
using System.IO.Compression;
using AndroidCtrl;
using AndroidCtrl.AAPT;

namespace Apk_Installer
{
    class ApkFile
    {
        private DataModelDumpBadging apk;
        private string pathApk;
        
        public ApkFile(string pathApk)
        {
            this.pathApk = pathApk;
            apk = AAPT.Instance.Dump.Badging(pathApk);
        }

        public bool isApk()
        {
            return getPackageName() != null;
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

        public string getSdkVersion()
        {
            return apk != null ? apk.SdkVersion : null;
        }

        public Stream getIcon()
        {
            if (apk != null)
            {
                Stream memoryStream = new MemoryStream();
                using (ZipStorer zipStorer = ZipStorer.Open(pathApk, FileAccess.Read))
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

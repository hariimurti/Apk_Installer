using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AndroidCtrl;
using AndroidCtrl.ADB;
using AndroidCtrl.Tools;
using System.Text.RegularExpressions;

namespace Apk_Installer
{
    public partial class MainForm : Form
    {
        private static string LoadedApk = null;

        public MainForm(string arg)
        {
            InitializeComponent();

            this.Text = Application.ProductName + " v" + Application.ProductVersion.Substring(0, 3);

            groupBox1.AllowDrop = true;
            pictureBox1.Image = Properties.Resources.apk.ToBitmap();

            InitializeAdb();
            InitializeApk(arg);
            ScanDevice();
            SetAssociation();
        }

        private void SetAssociation()
        {
            if (!FileAssociation.isRegistered())
            {
                DialogResult askToSet = MessageBox.Show("Set this for default opening file apk?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (askToSet == DialogResult.Yes) FileAssociation.Register();
            }
        }

        private void InitializeAdb()
        {
            try
            {
                if (!File.Exists("adb/aapt.exe"))
                    Deploy.AAPT();
                if (!File.Exists("adb/adb.exe") ||
                    !File.Exists("adb/AdbWinApi.dll") ||
                    !File.Exists("adb/AdbWinUsbApi.dll"))
                    Deploy.ADB();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ADB.Start();
        }

        private void InitializeApk(string fileApk = null)
        {
            if (File.Exists(fileApk) && fileApk.ToLower().EndsWith(".apk"))
            {
                ApkFile Apk = new ApkFile(fileApk);
                labelPackage.Text = setLabel(Apk.getPackageName());
                labelName.Text = setLabel(Apk.getAppLabel());
                labelVersion.Text = setLabel(Apk.getVersion());
                pictureBox1.Image = setIcon(Apk.getIcon());
                btnInstall.Enabled = (comboBox1.Items.Count > 0) && Apk.isApk();
                LoadedApk = Apk.isApk() ? fileApk : null;
            }
            else
            {
                labelPackage.Text = setLabel();
                labelName.Text = setLabel();
                labelVersion.Text = setLabel();
                pictureBox1.Image = setIcon();
                btnInstall.Enabled = false;
                LoadedApk = null;
            }
        }

        private void ScanDevice(bool set = false)
        {
            if (!ADB.IsStarted) ADB.Start();

            int num = 0;
            comboBox1_Clear();

            foreach (DataModelDevicesItem device in ADB.Devices())
            {
                if (device.Device != null)
                {
                    ComboboxItem item = new ComboboxItem();
                    item.Id = device.Serial;
                    item.Device = device.Device;
                    item.DataModel = device;
                    int index = comboBox1.Items.Add(item);
                    if (device.Serial.StartsWith(textIP.Text))
                    {
                        if (set) num = index;
                    }
                }
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = num;
                btnInstall.Enabled = LoadedApk != null;
            }
        }

        private void comboBox1_Clear()
        {
            comboBox1.Items.Clear();
            labelDevice.Text = setLabel();
            labelAndroid.Text = setLabel();
            labelRoot.Text = setLabel();
            btnInstall.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var data = (comboBox1.SelectedItem as ComboboxItem);
            ADB.SelectDevice(data.DataModel);

            string brand = ADB.Instance().Device.BuildProperties.Get("ro.product.brand");
            string name = ADB.Instance().Device.BuildProperties.Get("ro.product.name");
            string android = ADB.Instance().Device.BuildProperties.Get("ro.build.version.release");
            string root = ADB.Instance().IsRoot ? "Yes" : "No";

            labelDevice.Text = setLabel(UpperCaseFirst($"{brand} {name}"));
            labelAndroid.Text = setLabel(android);
            labelRoot.Text = setLabel(root);
        }

        private static string UpperCaseFirst(string text)
        {
            try
            {
                string result = null;
                string[] words = text.Split(' ');
                foreach (string word in words)
                {
                    result += char.ToUpper(word[0]) + word.Substring(1) + " ";
                }
                return result.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : " + ex.Message);
                return text;
            }
        }

        private string setLabel(string text = null)
        {
            return text != null ? $": {text}" : ": ...";
        }

        private Image setIcon(Stream img = null)
        {
            if (img == null)
                return Properties.Resources.apk.ToBitmap();
            else
                return Image.FromStream(img);
        }

        private void setBusy(int groupbox, bool value)
        {
            groupBox1.AllowDrop = !value;
            groupBox1.Text = !value ? "1. APK File ( Drag && Drop Here )" : "1. APK File";

            groupBox2.Enabled = !value;
            groupBox3.Enabled = !value;

            if (groupbox == 1)
            {
                groupBox2.Text = !value ? "2. ADB Devices" : "2. Connecting to Devices ( please wait )";
            }
            else if (groupbox == 2)
            {
                groupBox2.Text = !value ? "2. ADB Devices" : "2. Scanning Devices ( please wait )";
            }
            else if (groupbox == 3)
            {
                groupBox3.Text = !value ? "3. Connected Device" : "3. Installing to Device ( please wait )";
            }

            btnInstall.Enabled = !value && (LoadedApk != null) && (comboBox1.Items.Count > 0);
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            setBusy(1, true);

            if (!ADB.IsStarted) ADB.Start();
            await Task.Run(() => ADB.Connect(textIP.Text, textPort.Text));

            setBusy(2, true);
            ScanDevice(true);

            setBusy(2, false);
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            setBusy(2, true);
            ScanDevice();

            setBusy(2, false);
        }

        private async void btnInstall_Click(object sender, EventArgs e)
        {
            if (LoadedApk != null)
            {
                if (ADB.IsStarted)
                {
                    setBusy(3, true);
                    await Task.Run(() =>
                    {
                        if (ADB.Instance().Install(LoadedApk, "-r"))
                        {
                            MessageBox.Show("Installation Success...", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            LoadedApk = null;
                        }
                        else
                            MessageBox.Show("Something went wrong!!!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    });

                    if (LoadedApk == null)
                        InitializeApk();
                    setBusy(3, false);
                }
                else
                {
                    MessageBox.Show("ADB is not running!!! Solve: step no.2", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("There is no APK file!!! Solve: step no.1", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void groupBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void groupBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            foreach (string Content in FileList)
            {
                if (Content.ToLower().EndsWith(".apk"))
                {
                    InitializeApk(Content);
                    break;
                }
            }
        }

        private void txtNumeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || (e.KeyChar == '.') ||
                (e.KeyChar == (char)Keys.Back) || (e.KeyChar == (char)Keys.Delete));
        }

        private bool onValidation = false;
        private void txtValidation(object sender, EventArgs e)
        {
            if (!onValidation)
            {
                onValidation = true;
                Regex regex = new Regex(@"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})");
                Match match = regex.Match(textIP.Text);
                btnConnect.Enabled = match.Success;
                if (match.Success)
                    textIP.Text = match.Value;

                textPort.Text = textPort.Text.Replace(".", "");
                onValidation = false;
            }
        }
    }
}

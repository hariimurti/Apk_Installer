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

namespace Apk_Installer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            groupBox1.AllowDrop = true;
            pictureBox1.Image = Properties.Resources.apk.ToBitmap();

            InitializeAdb();
            InitializeApk();
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

        private void InitializeApk(string loaded = null)
        {
            if (loaded != null)
                ApkFile.Path = loaded;

            if (File.Exists(ApkFile.Path))
            {
                ApkFile Apk = new ApkFile();
                labelPackage.Text = setLabel(Apk.getPackageName());
                labelName.Text = setLabel(Apk.getAppLabel());
                labelVersion.Text = setLabel(Apk.getVersion());
                pictureBox1.Image = Image.FromStream(Apk.getIcon());
                button2.Enabled = (comboBox1.Items.Count > 0);
            }
            else
            {
                labelPackage.Text = setLabel();
                labelName.Text = setLabel();
                labelVersion.Text = setLabel();
                pictureBox1.Image = Properties.Resources.apk.ToBitmap();
                button2.Enabled = false;
            }
        }

        private void ScanDevice(bool set = false)
        {
            int num = 0;
            comboBox1.Items.Clear();
            
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
                        //button1.Text = "Dc";
                        if (set) num = index;
                    }
                }
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = num;
                button2.Enabled = File.Exists(ApkFile.Path);
            }
            else
            {
                //button1.Text = "Connect";
                button2.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var data = (comboBox1.SelectedItem as ComboboxItem);
            ADB.SelectDevice(data.DataModel);

            string brand = ADB.Instance().Device.BuildProperties.Get("ro.product.brand");
            string name = ADB.Instance().Device.BuildProperties.Get("ro.product.name");
            string android = ADB.Instance().Device.BuildProperties.Get("ro.build.version.release");
            string uptime = ADB.Instance().Device.Uptime().Up;

            labelDevice.Text = setLabel(UpperCaseFirst($"{brand} {name}"));
            labelAndroid.Text = setLabel(android);
            labelStatus.Text = setLabel(uptime);
        }

        private static string UpperCaseFirst(string text)
        {
            string result = null;
            string[] words = text.Split(' ');
            foreach (string word in words)
            {
                result += char.ToUpper(word[0]) + word.Substring(1) + " ";
            }
            return result.Trim();
        }

        private string setLabel(string text = null)
        {
            return text != null ? $": {text}" : ": ...";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ADB.Connect(textIP.Text, textPort.Text);
            ScanDevice(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ScanDevice();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ADB.Instance().Install(ApkFile.Path, "-r"))
                MessageBox.Show("Installation Success...", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            else
                MessageBox.Show("Something went wrong!!!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
}

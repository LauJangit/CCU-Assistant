using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.IO;

namespace Update
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            try
            {
                StreamReader TEMP = new StreamReader("Update.ini");
                TEMP.Close();
            }
            catch
            {
                MessageBox.Show("配置文件错误!", "错误");
                System.Environment.Exit(-1);
            }
            InitializeComponent();
            label1.Text = "状态:等待检查更新";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckNewVersion();
            if(Var.Status==1)
            {
                Download();
            }
        }

        private void CheckNewVersion()
        {
            try
            {
                Var.Status = 0;
                Var.Address = "";
                label1.Text = "状态:正在检查更新";
                string retString = "";
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ccuassistant.azurewebsites.net/Update.aspx?1.0.0");
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                    retString = myStreamReader.ReadToEnd();
                }
                catch
                {
                    MessageBox.Show("检查更新错误", "错误");
                }
                string Version = "";
                try
                {
                    StreamReader TEMPFILE = new StreamReader("Update.ini");
                    while (Version.IndexOf("Version") < 0)
                    {
                        Version = TEMPFILE.ReadLine();
                    }
                    TEMPFILE.Close();
                }
                catch
                {
                    MessageBox.Show("配置文件错误!", "错误");
                    return;
                }
                if (retString.IndexOf(Version) >= 0)
                {
                    Var.Status = 0;
                    label1.Text = "状态:当前版本为最新版本";
                    MessageBox.Show("当前版本为最新版本", "更新");
                }
                else
                {
                    Var.Address = retString.Substring(retString.IndexOf("http"));
                    Var.Status = 1;
                    Var.Version = retString.Substring(retString.IndexOf("Version") + 8, retString.IndexOf("</br>http") - retString.IndexOf("Version") - 8);
                }
            }
            catch
            {
                MessageBox.Show("下载失败", "错误");
            }
        }

        private void Download()
        {
            this.Hide();
            Form2 Show = new Form2();
            Show.ShowDialog();
            this.Close();
        }
    }

    public class Var
    {
        public static int Status = 0;
        public static string Address = "";
        public static string Version = "";
        public static uint TotalSize = 0;
        public static uint CurrentSize = 0;
    }
}

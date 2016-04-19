using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO.IsolatedStorage;
using System.Net.Cache;
using System.Threading;
using System.Net.Mail;
using System.Configuration;  

namespace WindowsFormsApplication1
{
    public partial class Feedback : Form
    {
        public Feedback()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length == 0)
            {
                return;
            }
            string Filename = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".txt";
            if (File.Exists(Filename))
            {
                File.Delete(Filename);
            }
            FileStream CreateFILE = new FileStream(Filename, FileMode.Create, FileAccess.Write);
            CreateFILE.Close();
            StreamWriter Content = new StreamWriter(Filename, true);
            Content.WriteLine("UploadDate:" + DateTime.Now.ToShortDateString().ToString());
            Content.WriteLine("UploadTime:" + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString());
            Content.WriteLine("UploadUserName:" + Var.Name);
            Content.WriteLine("Content:");
            Content.Write(richTextBox1.Text);
            Content.Close();
            FileUpLoad(Filename);
        }

        public static void FileUpLoad(string filePath)
        {
            string objPath = "";
            string url = @"ftp://115.159.203.70/";
            if (objPath != "")
                url += objPath + "/";
            FtpWebRequest reqFTP = null;
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                using (FileStream fs = fileInfo.OpenRead())
                {
                    long length = fs.Length;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url + fileInfo.Name));
                    reqFTP.Credentials = new NetworkCredential();
                    reqFTP.KeepAlive = false;
                    reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                    reqFTP.UseBinary = true;
                    reqFTP.Timeout = 10000;
                    using (Stream stream = reqFTP.GetRequestStream())
                    {
                        int BufferLength = 5120;
                        byte[] b = new byte[BufferLength];
                        int i;
                        while ((i = fs.Read(b, 0, BufferLength)) > 0)
                        {
                            stream.Write(b, 0, i);
                        }
                        MessageBox.Show("您的反馈我们已经收到，感谢您的支持", "反馈成功");
                    }
                }
            }
            catch
            {
                MessageBox.Show("上传失败", "错误");
                return;
            }
            finally
            {
            }
        }
    }
}

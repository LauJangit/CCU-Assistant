using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Update
{
    public partial class Form2 : Form
    {
        private BackgroundWorker bkWorker = new BackgroundWorker();
        public Form2()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ccuassistant.azurewebsites.net/UpdateLog.html");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            InitializeComponent();
            label1.Text = "新版本:CCU Assistant V" + Var.Version;
            if (File.Exists("CCU Assistant.exe"))
            {
                File.Delete("CCU Assistant.exe");
            }
            label2.Text = myStreamReader.ReadToEnd();
            bkWorker.WorkerReportsProgress = true;
            bkWorker.WorkerSupportsCancellation = true;
            bkWorker.DoWork += new DoWorkEventHandler(DoWork);
            bkWorker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);
        }

        public void DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ProcessProgress(bkWorker, e);
        }

        public void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }

        private int ProcessProgress(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i <= 1000; i++)
            {
                if ((int)(((double)Var.CurrentSize / (double)Var.TotalSize) * 1000) > 0)
                {
                    i = (int)(((double)Var.CurrentSize / (double)Var.TotalSize) * 1000);
                }
                //MessageBox.Show(((double)Var.CurrentSize / (double)Var.TotalSize).ToString());
                if (bkWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return -1;
                }
                else
                {
                    bkWorker.ReportProgress(i);
                    Thread.Sleep(10);
                }
            }
            return -1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            this.progressBar1.Maximum = 1000;
            bkWorker.RunWorkerAsync();
            backgroundWorker1.RunWorkerAsync();
        }

        public static void DownloadFile(string url, string path)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Var.TotalSize = (uint)request.GetResponse().ContentLength;
            Stream responseStream = response.GetResponseStream();
            Stream stream = new FileStream(path, FileMode.Create);
            byte[] bArr = new byte[1];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            while (size > 0)
            {
                stream.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
                Var.CurrentSize++;
            }
            stream.Close();
            responseStream.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DownloadFile(Var.Address, "CCU Assistant.exe");
            }
            catch
            {
                MessageBox.Show("下载错误!", "错误");
                return;
            }
            try
            {
                Process.Start("CCU Assistant.exe");
                System.Environment.Exit(0);
            }
            catch
            {
                System.Environment.Exit(0);
            }
        }
    }
}

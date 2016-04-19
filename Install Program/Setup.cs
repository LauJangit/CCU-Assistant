using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Clear();
            Step1();
        }

        private void button1_Click(object sender, EventArgs e)//上一步
        {
            Clear();
            Step.step--;
            switch (Step.step)
            {
                case 1: Step1(); break;
                case 2: Step2(); break;
                case 3: Step3(); break;
                case 4: Step4(); break;
                case 5: Step5(); break;
                default: MessageBox.Show("程序遇到严重错误,需要退出!", "错误"); break;
            }
        }

        private void button2_Click(object sender, EventArgs e)//下一步
        {
            Clear();
            Step.step++;
            switch (Step.step)
            {
                case 1: Step1(); break;
                case 2: Step2(); break;
                case 3: Step3(); break;
                case 4: Step4(); break;
                case 5: Step5(); break;
                default: MessageBox.Show("程序遇到严重错误,需要退出!", "错误"); break;
            }
        }

        private void button3_Click(object sender, EventArgs e)//取消
        {
            this.Close();
        }

        public void Step1()//第一步
        {
            pictureBox2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        public void Step2()//第二步
        {
            radioButton1.Checked = true;
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            pictureBox3.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            richTextBox1.Visible = true;
            button1.Enabled = true;
            button3.Enabled = true;
            button2.Enabled = true;
        }

        public void Step3()//第三步
        {
            button2.Text = "安装";
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            label7.Visible = true;
            label8.Visible = true;
            pictureBox3.Visible = true;
            label9.Visible = true;
            textBox1.Visible = true;
            button4.Visible = true;
            checkBox1.Visible = true;
            checkBox3.Visible = true;
            checkBox1.Checked = true;
            checkBox3.Checked = true;
            textBox1.Text = Step.Path;
        }

        public void Step4()//第四步(安装)
        {
            this.ControlBox = false;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            pictureBox3.Visible = true;
            label10.Visible = true;
            label11.Visible = true;
            progressBar1.Visible = true;
            label12.Visible = true;
            bkWorker.WorkerReportsProgress = true;
            bkWorker.WorkerSupportsCancellation = true;
            bkWorker.DoWork += new DoWorkEventHandler(DoWork);
            bkWorker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);
            Install();
            Clear();
            button1.Visible = true;
            button1.Enabled = false;
            button2.Visible = true;
            button2.Enabled = true;
            button3.Visible = true;
            button3.Enabled = false;
            pictureBox2.Visible = true;
            label13.Visible = true;
            label14.Visible = true;
            checkBox5.Visible = true;
            button2.Text = "完成";
            checkBox5.Checked = true;
        }

        public void Step5()//第五步
        {
            if (Step.OpenProgram == true) { Process.Start(Step.Path + @"\CCUAssistant\CCUAssistant.exe"); }
            this.Close();
        }

        public void Clear()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            pictureBox3.Visible = false;
            this.ControlBox = true;
            button2.Text = "下一步";
            //第一步
            pictureBox2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            //第二步
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            pictureBox3.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            richTextBox1.Visible = false;
            //第三步
            label7.Visible = false;
            label8.Visible = false;
            pictureBox3.Visible = false;
            label9.Visible = false;
            textBox1.Visible = false;
            button4.Visible = false;
            checkBox1.Visible = false;
            checkBox3.Visible = false;
            //第四步
            label10.Visible = false;
            label11.Visible = false;
            progressBar1.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            checkBox5.Visible = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)//接受协议
        {
            button2.Enabled = true;
            Step.AgreeGPL = true;
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)//不接受协议
        {
            button2.Enabled = false;
            Step.AgreeGPL = false;
        }

        private void button4_Click(object sender, EventArgs e)//浏览安装目录
        {
            folderBrowserDialog1.ShowNewFolderButton = true;
            folderBrowserDialog1.ShowDialog();

            if (!File.Exists(folderBrowserDialog1.SelectedPath))
            {
                textBox1.Text = Step.Path;
            }
            Step.Path = folderBrowserDialog1.SelectedPath;
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)//设置桌面快捷方式
        {
            if (checkBox1.Checked == true)
            {
                Step.SetDesktopShortCut = true;
            }
            else
            {
                Step.SetDesktopShortCut = false;
            }
        }


        private void checkBox3_CheckedChanged(object sender, EventArgs e)//添加到快速启动栏
        {
            if (checkBox3.Checked == true)
            {
                Step.SetMenuShortCut = true;
            }
            else
            {
                Step.SetMenuShortCut = false;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
            {
                Step.OpenProgram = true;
            }
            else
            {
                Step.OpenProgram = false;
            }
        }

        private BackgroundWorker bkWorker = new BackgroundWorker();
        public void Install()//安装进程
        {
            try
            {
                bkWorker.RunWorkerAsync();
                label12.Text = "正在准备复制";
                if (!Directory.Exists(Step.Path + @"\CCUAssistant"))
                {
                    Directory.CreateDirectory(Step.Path + @"\CCUAssistant");
                }
                if (Directory.Exists(Step.Path + @"\CCUAssistant"))
                {
                    Directory.Delete(Step.Path + @"\CCUAssistant", true);
                    Directory.CreateDirectory(Step.Path + @"\CCUAssistant");
                }
                Step.CurrentCopied = 5;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\CCUAssistant.exe";
                File.Copy(@"CCUAssistant\CCUAssistant.exe", Step.Path + @"\CCUAssistant\CCUAssistant.exe");
                Step.CurrentCopied = 8;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\FirefoxPortable.exe";
                File.Copy(@"CCUAssistant\FirefoxPortable.exe", Step.Path + @"\CCUAssistant\FirefoxPortable.exe");
                Step.CurrentCopied = 11;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\FirefoxPortable.ini";
                File.Copy(@"CCUAssistant\FirefoxPortable.ini", Step.Path + @"\CCUAssistant\FirefoxPortable.ini");
                Step.CurrentCopied = 14;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\hosts";
                File.Copy(@"CCUAssistant\hosts", Step.Path + @"\CCUAssistant\hosts");
                Step.CurrentCopied = 17;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\App...";
                copyDirectory(@"CCUAssistant\App", Step.Path + @"\CCUAssistant\App");
                Step.CurrentCopied = 67;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\data...";
                copyDirectory(@"CCUAssistant\data", Step.Path + @"\CCUAssistant\data");
                Step.CurrentCopied = 87;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\MakeCookie.exe";
                File.Copy(@"CCUAssistant\MakeCookie.exe", Step.Path + @"\CCUAssistant\MakeCookie.exe");
                Step.CurrentCopied = 90;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\Update.exe";
                File.Copy(@"CCUAssistant\Update.exe", Step.Path + @"\CCUAssistant\Update.exe");
                Step.CurrentCopied = 92;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\Update.ini";
                File.Copy(@"CCUAssistant\Update.ini", Step.Path + @"\CCUAssistant\Update.ini");
                Step.CurrentCopied = 94;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\Update.ini";
                File.Copy(@"CCUAssistant\Uninstall.exe", Step.Path + @"\CCUAssistant\Uninstall.exe");
                Step.CurrentCopied = 96;
                label12.Text = "正在复制" + Step.Path + @"\CCUAssistant\Other...";
                copyDirectory(@"CCUAssistant\Other", Step.Path + @"\CCUAssistant\Other");
                Step.CurrentCopied = 99;
                if (File.Exists("SetShortcut.bat")) { File.Delete("SetShortcut.bat"); }
                FileStream SetShortcut = new FileStream("SetShortcut.bat", FileMode.Create, FileAccess.Write);
                SetShortcut.Close();
                StreamWriter WriteShortcut = new StreamWriter("SetShortcut.bat", true, Encoding.GetEncoding("GB2312"));
                if (Step.SetDesktopShortCut == true) { WriteShortcut.WriteLine("Shortcut.exe " + "\"" + Step.Path + @"\CCUAssistant\CCUAssistant.exe" + "\" /ld 长大教务助手.lnk"); }
                if (Step.SetMenuShortCut == true)
                {
                    if (File.Exists(@"rd /s /q %ProgramData%\Microsoft\Windows\Start Menu\Programs\长大教务助手"))
                    {
                        WriteShortcut.WriteLine(@"rd /s /q "+"\""+@"%ProgramData%\Microsoft\Windows\Start Menu\Programs\长大教务助手"+"\"");
                    }
                    WriteShortcut.WriteLine(@"md "+"\""+@"%ProgramData%\Microsoft\Windows\Start Menu\Programs\长大教务助手"+"\"");
                    WriteShortcut.WriteLine(@"Shortcut.exe " + "\"" + Step.Path + @"\CCUAssistant\Uninstall.exe" + "\" /l " + "\"" + @"%ProgramData%\Microsoft\Windows\Start Menu\Programs\长大教务助手\" + "卸载.lnk" + "\"");
                    WriteShortcut.WriteLine(@"Shortcut.exe " + "\"" + Step.Path + @"\CCUAssistant\CCUAssistant.exe" + "\" /l " + "\"" + @"%ProgramData%\Microsoft\Windows\Start Menu\Programs\长大教务助手\" + "长大教务助手.lnk" + "\"");
                }
                WriteShortcut.Close();
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "wscript";
                proc.StartInfo.Arguments = "SetShortcut.vbe";
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                proc.Close();
                label12.Text = "安装完成!";
                Step.CurrentCopied = 100;
            }
            catch
            {
                MessageBox.Show("安装程序无法将文件复制到您的计算机上", "安装失败");
                this.Close();
            }
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
                i = Step.CurrentCopied;
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

        private void deleteFiles(string strPath)
        {
            if (Directory.GetDirectories(strPath).Length > 0)
            {
                foreach (string var in Directory.GetDirectories(strPath))
                {
                    Directory.Delete(var, true);
                }
            }
            if (Directory.GetFiles(strPath).Length > 0)
            {
                foreach (string var in Directory.GetFiles(strPath))
                {
                    File.Delete(var);
                }
            }
        }

        public static void copyDirectory(string sourceDirectory, string destDirectory)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            copyFile(sourceDirectory, destDirectory);
            string[] directionName = Directory.GetDirectories(sourceDirectory);
            foreach (string directionPath in directionName)
            {
                string directionPathTemp = destDirectory + "\\" + directionPath.Substring(sourceDirectory.Length + 1);
                copyDirectory(directionPath, directionPathTemp);
            }                     
        }
        public static void copyFile(string sourceDirectory, string destDirectory)
        {
            string[] fileName = Directory.GetFiles(sourceDirectory);
           
            foreach (string filePath in fileName)
            {
                string filePathTemp = destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);
                if (File.Exists(filePathTemp))
                {
                    File.Copy(filePath, filePathTemp, true);
                }
                else
                {
                    File.Copy(filePath, filePathTemp);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Step.step < 5)
            {
                DialogResult ShowAlert = MessageBox.Show("您确定要退出长大教务助手的安装程序？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ShowAlert == DialogResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }

    public class Step
    {
        public static int step = 1;//步骤
        public static bool AgreeGPL = false;//是否同意协议
        public static bool SetDesktopShortCut = true;//是否设置桌面快捷方式
        public static bool SetMenuShortCut = true;//是否设置任务栏快捷方式
        public static bool OpenProgram = true;
        public static string Path = System.Environment.GetEnvironmentVariable("ProgramFiles");//安装路径
        public static int CurrentInstalled = 0;
        public static int TotalSize = 0;
        public static int CurrentCopied = 0;
    }
}

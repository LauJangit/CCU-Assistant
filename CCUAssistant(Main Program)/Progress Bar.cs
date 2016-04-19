using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form5 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public IntPtr Handle1;

        public Form5()
        {
            InitializeComponent();
            try
            {
                Status.Abort = 0;
                progressbar();
                backgroundWorker1.RunWorkerAsync();
            }
            catch { }
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            Handle1 = this.Handle;
            SetForegroundWindow(Handle1);
        }

        private void progressbar()
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 100;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(10);
                    statuslabel.Text = Status.Show;
                    if (Status.Abort == 1)
                    {
                        e.Cancel = true;
                        this.Close();
                        return;
                    }
                }
            }
            catch 
            {
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Status.StopLoginLoop = true;
        }
    }

    public class Status
    {
        public static string Show = "";
        public static int Abort = 1;
        public static bool StopLoginLoop = false;
    }
}

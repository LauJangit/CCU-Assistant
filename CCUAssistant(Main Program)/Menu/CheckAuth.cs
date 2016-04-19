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
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class CheckAuth : Form
    {
        public CheckAuth()
        {
            InitializeComponent();
            textBox1.Text = Var.Name;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.cdjwc.com/jiaowu/upload/XSXX/" + Var.Name + ".jpg");
            System.IO.Stream picture = request.GetResponse().GetResponseStream();
            Image img = System.Drawing.Bitmap.FromStream(picture);
            picture.Close();
            this.pictureBox1.Image = img;
            label2.Text = "该软件已授权给：" + Var.Name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string StuID = textBox1.Text;
            CheckAUTH(StuID);
        }

        public void CheckAUTH(string StuID)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.cdjwc.com/jiaowu/upload/XSXX/" + StuID + ".jpg");
                System.IO.Stream picture = request.GetResponse().GetResponseStream();
                Image img = System.Drawing.Bitmap.FromStream(picture);
                picture.Close();
                this.pictureBox1.Image = img;
                label2.Text = "该软件已授权给：" + StuID;
            }
            catch
            {
                label2.Text = "该软件未授权给：" + StuID;
            }
        }
    }
}

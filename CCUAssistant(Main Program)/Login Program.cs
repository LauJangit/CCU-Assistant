using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        //设置窗口置顶
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow(); 
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public IntPtr Handle1;

        private void Form1_Load(object sender, EventArgs e)
        {
            Handle1 = this.Handle;
            SetForegroundWindow(Handle1);
        }

        public Form1()
        {
            //启动登陆窗口
            Form3 login = new Form3();
            login.ShowDialog();
            //检查是否登陆授权以及登陆情况
            if (Var.Authstatus != 2)
            {
                System.Environment.Exit(0);
            }else
                while (Var.RetLogin != 3)
                {
                    login.ShowDialog();
                }
            InitializeComponent();//初始化界面
            //清理所有控件及部分值
            Clear();
            Clearwidget();
            SetToolTip();//设置功能按钮提示标签
            IndexPage();//显示主界面控件
        }

        public void SetWindowsTop()
        {
            
    }
        public void SetToolTip()//设置功能按钮提示标签
        {
            ToolTip ButtonToolTip = new ToolTip();
            ButtonToolTip.ShowAlways = true;
            ButtonToolTip.SetToolTip(this.button1, "查看课程表\n查看您最近两学年的课程信息");
            ButtonToolTip.SetToolTip(this.button2, "查询考试成绩\n查询您所有学期的学习成绩");
            ButtonToolTip.SetToolTip(this.button3, "个人信息\n查看您的个人信息\n声明:该程序不会收集您任何资料");
            ButtonToolTip.SetToolTip(this.button4, "选课\n帮助您进入教务系统选课页面");
            ButtonToolTip.SetToolTip(this.button5, "小工具\n网络端未完成准备");
        }

        public void IndexPage()//主页
        {
            backgroundWorker1.RunWorkerAsync();//检查新版本
            label48.Visible = true;
            pictureBox2.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)//课程表
        {
            //显示课程表所需控件
            Clearwidget();
            label3.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            comboBox1.Visible = true;
            comboBox2.Visible = true;
            button6.Visible = true;
            dataGridView1.Visible = true;
            if (dataGridView1.RowCount > 0 && dataGridView1.ColumnCount > 0)
            {
                ExcelToolStripMenuItem.Enabled = true;
            }
            else
            {
                ExcelToolStripMenuItem.Enabled = false;
            }
            //建立选择学期下拉项
            try
            {
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                int currentlyyear = int.Parse(DateTime.Now.Year.ToString());
                comboBox1.Items.Add(currentlyyear.ToString() + "-" + (currentlyyear + 1).ToString() + "学年");
                comboBox1.Items.Add((currentlyyear - 1).ToString() + "-" + currentlyyear.ToString() + "学年");
                comboBox2.Items.Add("第一学期");
                comboBox2.Items.Add("第二学期");
            }
            catch
            {
                MessageBox.Show("创建选择项失败!", "错误");
                Alert();
                return;
            }
        }

        private void button6_Click(object sender, EventArgs e)//课程表子按键---->查询
        {
            string year = "", term = "";
            int currentlyyear = int.Parse(DateTime.Now.Year.ToString());//读取今年年份
            Thread ProgressThread = new Thread(ProgressForm);//进度栏进程
            try
            {
                //清理表格内内容
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                //检测下拉栏选择内容是否合法并根据下拉栏的选择选择数据
                if (int.Parse(comboBox1.SelectedIndex.ToString()) == -1 || int.Parse(comboBox2.SelectedIndex.ToString()) == -1)
                {
                    return;
                }
                if (int.Parse(comboBox1.SelectedIndex.ToString()) == 0)
                {
                    year = currentlyyear.ToString() + "-" + (currentlyyear + 1).ToString();
                }
                else if (int.Parse(comboBox1.SelectedIndex.ToString()) == 1)
                {
                    year = (currentlyyear - 1).ToString() + "-" + currentlyyear.ToString();
                }
                if (int.Parse(comboBox2.SelectedIndex.ToString()) == 0)
                {
                    term = "-1&sffd=1";
                }
                else if (int.Parse(comboBox2.SelectedIndex.ToString()) == 1)
                {
                    term = "-2&sffd=1";
                }
                ProgressThread.Start();
                Status.Show = "正在准备下载数据...";
            }
            catch
            {
                MessageBox.Show("准备下载失败!", "错误");
                Alert();
                return;
            }
            Download("http://www.cdjwc.com/jiaowu/JWXS/pkgl/xsxskb_xsy.aspx?xnxqh=" + year + term);
            ExcelToolStripMenuItem.Enabled = true;//设置导出到Excel图标为可用
            try
            {
                Status.Abort = 1;//中止进度条进程
                //检查下载的课表网页代码是否为空
                if (Form1Var.Msgdownload.IndexOf("您现在还不能查看") >= 0)
                {
                    MessageBox.Show("该学期的课程表尚未被公布", "课程表查询");
                    return;
                }
                //往空表格内写入表格格式
                if (dataGridView1.Rows.Count == 0)
                {
                    for (int i = 0; i < 7; i++)//列
                    {
                        dataGridView1.ColumnHeadersHeight = 23;
                        dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());

                    }
                    for (int i = 0; i < 12; i++)//行
                    {
                        dataGridView1.RowHeadersWidth = 48;
                        dataGridView1.Rows.Add(new DataGridViewRow());
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        dataGridView1.Columns[i].Width = (dataGridView1.Width - 48) / 7;
                        dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    dataGridView1.Columns[0].HeaderText = "星期一";
                    dataGridView1.Columns[1].HeaderText = "星期二";
                    dataGridView1.Columns[2].HeaderText = "星期三";
                    dataGridView1.Columns[3].HeaderText = "星期四";
                    dataGridView1.Columns[4].HeaderText = "星期五";
                    dataGridView1.Columns[5].HeaderText = "星期六";
                    dataGridView1.Columns[6].HeaderText = "星期日";
                    for (int i = 0; i < 12; i++)
                    {
                        //string Input = (i + 1).ToString();
                        dataGridView1.Rows[i].Height = (dataGridView1.Height - 23) / 12;
                        //dataGridView1.Rows[i].HeaderCell.Value = Input;
                    }
                    dataGridView1.Rows[0].HeaderCell.Value = "1\n\n\n8:00-8:45";
                    dataGridView1.Rows[1].HeaderCell.Value = "2\n\n\n8:45-9:30";
                    dataGridView1.Rows[2].HeaderCell.Value = "3\n\n\n9:50-10:35";
                    dataGridView1.Rows[3].HeaderCell.Value = "4\n\n\n10:35-11:20";
                    dataGridView1.Rows[4].HeaderCell.Value = "5\n\n\n12:50-13:35";
                    dataGridView1.Rows[5].HeaderCell.Value = "6\n\n\n13:35-14:20";
                    dataGridView1.Rows[6].HeaderCell.Value = "7\n\n\n14:40-15:25";
                    dataGridView1.Rows[7].HeaderCell.Value = "8\n\n\n15:25-16:10";
                    dataGridView1.Rows[8].HeaderCell.Value = "9\n\n\n16:30-17:15";
                    dataGridView1.Rows[9].HeaderCell.Value = "10\n\n\n17:15-18:00";
                    dataGridView1.Rows[10].HeaderCell.Value = "11\n\n\n19:00-19:45";
                    dataGridView1.Rows[11].HeaderCell.Value = "12\n\n\n19:45-20:30";
                }
                for (int Columns = 0; Columns < 7; Columns++)
                    for (int Rows = 0; Rows < 12; Rows++)
                        dataGridView1.Rows[Rows].Cells[Columns].Value = " ";
                for (int i = 0; i < 12; i++)
                {
                    Form1Var.Coursetable.Add("");
                }
            }
            catch
            {
                MessageBox.Show("创建表格失败!", "错误");
                Alert();
                return;
            }
            try
            {
                StringReader TEMPFILE = new System.IO.StringReader(Form1Var.Msgdownload);
                string str = TEMPFILE.ReadLine();
                while (true)
                {
                    if (str.IndexOf("开课编号") >= 0)
                    {
                        string CourseNum = str.Substring(str.IndexOf("开课编号"));
                        str = TEMPFILE.ReadLine();
                        string CourseID = str.Substring(str.IndexOf("课程编码")); 
                        str = TEMPFILE.ReadLine();
                        string CourseName = str.Substring(str.IndexOf("课程名称"));
                        str = TEMPFILE.ReadLine();
                        string teacher = str.Substring(str.IndexOf("授课教师")); 
                        str = TEMPFILE.ReadLine();
                        string Time = str.Substring(str.IndexOf("开课时间"));
                        str = TEMPFILE.ReadLine();
                        string Range = str.Substring(str.IndexOf("上课周次"));
                        str = TEMPFILE.ReadLine();
                        string Courseloaction = str.Substring(str.IndexOf("开课地点"));
                        str = TEMPFILE.ReadLine();
                        string CourseClass = str.Substring(str.IndexOf("上课班级"), str.IndexOf("'>"));
                        if (int.Parse(Time.Substring(5)) <= 99999 && int.Parse(Time.Substring(5)) >= 10000)
                        {
                            int day = (int)(int.Parse(Time.Substring(5)) / 10000);
                            int start = ((int)(int.Parse(Time.Substring(5)) - day * 10000) / 100);
                            int end = (int.Parse(Time.Substring(5)) - day * 10000 - start * 100);
                            for (; start <= end; start++)
                            {
                                dataGridView1.Rows[start - 1].Cells[day - 1].Value = CourseName.Substring(5) + "\n" + CourseID + "\n" + CourseName + "\n" + teacher + "\n" + Time + "\n" + Range + "\n" + Courseloaction + "\n" + CourseClass;
                            }
                        }
                        else
                        {
                            if (str.IndexOf("</body>") >= 0)
                            {
                                break;
                            }
                            continue;
                        }
                    }
                    else
                    {
                        str = TEMPFILE.ReadLine();
                        if (str.IndexOf("</body>") >= 0)
                        {
                            break;
                        }
                    }
                }
                TEMPFILE.Close();
            }
            catch
            {
            }
            File.Delete("TEMPFILE");
            Clear();
        }

        private void button2_Click(object sender, EventArgs e)//成绩
        {
            try
            {
                Thread ProgressThread = new Thread(ProgressForm);
                ProgressThread.Start();
                Status.Show = "正在准备下载数据...";
                Clear();
                Clearwidget();
                label4.Visible = true;
                label43.Visible = true;
                label44.Visible = true;
                label45.Visible = true;
                label46.Visible = true;
                label47.Visible = true;
                Mark.Page = 0;
                pictureBox12.Visible = true;
                pictureBox13.Visible = true;
                Download("http://www.cdjwc.com/jiaowu/JWXS/cjcx/jwxs_cjcx_like.aspx?usermain=" + Var.Name);
                Status.Show = "正在分析数据...";
                StreamReader TEMPFILE = new StreamReader("TEMPFILE");
                string Ret;
                int Count = 0;
                try
                {
                    while (true)
                    {
                        int position = 0;
                        Ret = TEMPFILE.ReadLine();
                        if (Ret.IndexOf("</tr><tr>") >= 0)
                        {
                            Ret = TEMPFILE.ReadLine();
                            if (Ret.IndexOf("√") >= 0)
                            {
                                Mark.passed.Add("通过");
                            }
                            else if (Ret.IndexOf("×") >= 0)
                            {
                                Mark.passed.Add("挂科");
                            }
                            position = Ret.IndexOf("</td><td>");
                            Mark.year.Add(Ret.Substring(position + 9, 11));
                            position = Ret.Substring(position + 9 + Mark.year[Count].ToString().Length + 9).IndexOf("</td><td>") + position + 9 + Mark.year[Count].ToString().Length + 9;
                            Mark.ClassName.Add(Ret.Substring(position + 9, Ret.Substring(position + 9).IndexOf("</td><td>")));
                            position = position + Ret.Substring(position + 9).IndexOf("</td><td>") + 18;
                            Mark.score.Add(Ret.Substring(position, Ret.Substring(position).IndexOf("</td><td>")));
                            position = position + Ret.Substring(position).IndexOf("</td><td>") + 9;
                            Mark.Coursescore.Add(Ret.Substring(position, Ret.Substring(position).IndexOf("</td><td>")));
                            position = position + Ret.Substring(position).IndexOf("</td><td>") + 9 + Ret.Substring(position + Ret.Substring(position).IndexOf("</td><td>") + 9).IndexOf("</td><td>");
                            position = position + 9;
                            Mark.type.Add(Ret.Substring(position, Ret.Substring(position).IndexOf("</td><td>")));
                            position = position + 9 + Ret.Substring(position).IndexOf("</td><td>");
                            position = position + 9 + Ret.Substring(position).IndexOf("</td><td>");
                            position = position + 9 + Ret.Substring(position).IndexOf("</td><td>");
                            Mark.examtype.Add(Ret.Substring(position, Ret.Substring(position).IndexOf("</td><td>")));
                            Count++;
                        }
                        if (Ret.IndexOf("lblpgxfjd") >= 0)
                        {
                            Mark.CNGPA = Ret.Substring(Ret.IndexOf("lblpgxfjd") + 11, 3);
                            break;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("读取成绩失败", "失败");
                    Alert();
                    return;
                }
                finally
                {
                    TEMPFILE.Close();
                    File.Delete("TEMPFILE");
                }
                int currentlyyear = int.Parse(DateTime.Now.Year.ToString());
                comboBox1.Items.Add("全部学年");
                comboBox1.Items.Add(currentlyyear.ToString() + "-" + (currentlyyear + 1).ToString() + "学年");
                comboBox1.Items.Add((currentlyyear - 1).ToString() + "-" + currentlyyear.ToString() + "学年");
                comboBox1.Items.Add((currentlyyear - 2).ToString() + "-" + (currentlyyear - 1).ToString() + "学年");
                comboBox1.SelectedIndex = 0;
                comboBox2.Items.Add("第一学期");
                comboBox2.Items.Add("第二学期");
                pictureBox12.Left = (int)(3.48 * 60) + 110;
                pictureBox13.Left = (int)(3.48 * 100) + 110;
                Status.Abort = 1;
                ManageShowInf();
            }
            catch
            {
                MessageBox.Show("未知错误，无法读取您的成绩", "错误");
                Alert();
            }
        }

        public void ManageShowInf()
        {
            Mark.USGPA = 0;
            ArrayList ListNum = new ArrayList();
            int CountAll = 0;
            double CourseScore = 0;
            Mark.TotalFailedCourseNum = 0;
            for (; CountAll < Mark.ClassName.Count; CountAll++)
            {
                if (Mark.year[CountAll].ToString().IndexOf(Mark.Range) >= 0)
                {
                    ListNum.Add(CountAll);
                    CourseScore = CourseScore + Double.Parse(Mark.Coursescore[CountAll].ToString());
                    
                }
            }
            ClearShowInf();
            label33.Visible=true;
            label32.Visible=true;
            if(Mark.Page<=0)
            {
                label32.Visible=false;
            }
            if(Mark.Page>=(int)(Mark.ClassName.Count/9))
            {
                label33.Visible=false;
            }
            for (int Count = 0, CountShowed = Mark.Page * 9; CountShowed < (Mark.Page + 1) * 9 && Count < 9; CountShowed++, Count++)
            {
                ShowInf(CountShowed, Count);
                if (Count + Mark.Page * 9 == Mark.ClassName.Count - 1)
                {
                    return;
                }
            }
            string strUSGPA = String.Format("{0:F}", (Mark.USGPA / (Double)CountAll));
            label43.Text = "总学分: " + CourseScore + " 分";
            label44.Text = "课程总数: " + CountAll + " 门";
            label45.Text = "共有 " + Mark.TotalFailedCourseNum + " 门科目不及格";
            label46.Text = "总绩点: " + Mark.CNGPA;
            label47.Text = "本页GPA: " + strUSGPA;
        }

        public void ShowInf(int InfNum, int ShowNum)
        {
            string ClassName=Mark.ClassName[InfNum].ToString();
            if (ClassName.Length >= 6)
            {
                ClassName = ".." + ClassName.Substring(0, 5);
            }
            string Content3 = Mark.score[InfNum].ToString();
            string Content1 = ClassName + "\n成绩:" + Mark.score[InfNum].ToString() + "\n" + Mark.passed[InfNum].ToString();
            int length = 0;
            switch (Mark.score[InfNum].ToString())
            {
                case "优秀": length = 90; break;
                case "良好": length = 80; break;
                case "中等": length = 70; break;
                case "及格": length = 60; break;
                case "不及格": length = 30; break;
                default: length = int.Parse(Mark.score[InfNum].ToString()); break;
            }
            if (length >= 90) { Mark.USGPA = Mark.USGPA + 4; }
            if (length >= 80 && length < 90) { Mark.USGPA = Mark.USGPA + 3; }
            if (length >= 70 && length < 80) { Mark.USGPA = Mark.USGPA + 2; }
            if (length >= 60 && length < 70) { Mark.USGPA = Mark.USGPA + 1; }
            if (length < 60)
            {
                Mark.TotalFailedCourseNum++;
            }
            MessageBox.Show(Mark.USGPA.ToString());
            length=(int)(length*3.48);
            switch (ShowNum)
            {
                case 0: label23.Visible = true; pictureBox3.Visible = true; label34.Visible = true; label23.Text = Content1; pictureBox3.Width = length; if (length < 207) { pictureBox3.BackColor = System.Drawing.Color.Red; } label34.Left = 110 + length; label34.Text = Content3; break;
                case 1: label24.Visible = true; pictureBox4.Visible = true; label35.Visible = true; label24.Text = Content1; pictureBox4.Width = length; if (length < 207) { pictureBox4.BackColor = System.Drawing.Color.Red; } label35.Left = 110 + length; label35.Text = Content3; break;
                case 2: label25.Visible = true; pictureBox5.Visible = true; label36.Visible = true; label25.Text = Content1; pictureBox5.Width = length; if (length < 207) { pictureBox5.BackColor = System.Drawing.Color.Red; } label36.Left = 110 + length; label36.Text = Content3; break;
                case 3: label26.Visible = true; pictureBox6.Visible = true; label37.Visible = true; label26.Text = Content1; pictureBox6.Width = length; if (length < 207) { pictureBox6.BackColor = System.Drawing.Color.Red; } label37.Left = 110 + length; label37.Text = Content3; break;
                case 4: label27.Visible = true; pictureBox7.Visible = true; label38.Visible = true; label27.Text = Content1; pictureBox7.Width = length; if (length < 207) { pictureBox7.BackColor = System.Drawing.Color.Red; } label38.Left = 110 + length; label38.Text = Content3; break;
                case 5: label28.Visible = true; pictureBox8.Visible = true; label39.Visible = true; label28.Text = Content1; pictureBox8.Width = length; if (length < 207) { pictureBox8.BackColor = System.Drawing.Color.Red; } label39.Left = 110 + length; label39.Text = Content3; break;
                case 6: label29.Visible = true; pictureBox9.Visible = true; label40.Visible = true; label29.Text = Content1; pictureBox9.Width = length; if (length < 207) { pictureBox9.BackColor = System.Drawing.Color.Red; } label40.Left = 110 + length; label40.Text = Content3; break;
                case 7: label30.Visible = true; pictureBox10.Visible = true; label41.Visible = true; label30.Text = Content1; pictureBox10.Width = length; if (length < 207) { pictureBox10.BackColor = System.Drawing.Color.Red; } label41.Left = 110 + length; label41.Text = Content3; break;
                case 8: label31.Visible = true; pictureBox11.Visible = true; label42.Visible = true; label31.Text = Content1; pictureBox11.Width = length; if (length < 207) { pictureBox11.BackColor = System.Drawing.Color.Red; } label42.Left = 110 + length; label42.Text = Content3; break;
                default: MessageBox.Show("显示超限", "错误"); Alert(); break;
            }
        }

        private void label32_Click(object sender, EventArgs e)
        {
            if (Mark.Page > 0)
            {
                Mark.Page--;
            }
            else return;
            ClearShowInf();
            ManageShowInf();
        }

        private void label33_Click(object sender, EventArgs e)
        {
            Mark.Page++;
            ClearShowInf();
            ManageShowInf();
        }

        public void ClearShowInf()
        {
            label23.Visible = false; pictureBox3.Visible = false; label34.Visible = false; pictureBox3.BackColor = System.Drawing.Color.DimGray;
            label24.Visible = false; pictureBox4.Visible = false; label35.Visible = false; pictureBox4.BackColor = System.Drawing.Color.DimGray;
            label25.Visible = false; pictureBox5.Visible = false; label36.Visible = false; pictureBox5.BackColor = System.Drawing.Color.DimGray;
            label26.Visible = false; pictureBox6.Visible = false; label37.Visible = false; pictureBox6.BackColor = System.Drawing.Color.DimGray;
            label27.Visible = false; pictureBox7.Visible = false; label38.Visible = false; pictureBox7.BackColor = System.Drawing.Color.DimGray;
            label28.Visible = false; pictureBox8.Visible = false; label39.Visible = false; pictureBox8.BackColor = System.Drawing.Color.DimGray;
            label29.Visible = false; pictureBox9.Visible = false; label40.Visible = false; pictureBox9.BackColor = System.Drawing.Color.DimGray;
            label30.Visible = false; pictureBox10.Visible = false; label41.Visible = false; pictureBox10.BackColor = System.Drawing.Color.DimGray;
            label31.Visible = false; pictureBox11.Visible = false; label42.Visible = false; pictureBox11.BackColor = System.Drawing.Color.DimGray;
        }

        private void button3_Click(object sender, EventArgs e)//个人信息
        {
            try
            {
                Clearwidget();
                label5.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
                label13.Visible = true;
                label14.Visible = true;
                label15.Visible = true;
                label16.Visible = true;
                label17.Visible = true;
                label18.Visible = true;
                label19.Visible = true;
                label20.Visible = true;
                label21.Visible = true;
                label22.Visible = true;
                Thread ProgressThread = new Thread(ProgressForm);
                ProgressThread.Start();
                Status.Show = "正在准备下载数据...";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.cdjwc.com/jiaowu/upload/XSXX/" + Var.Name + ".jpg");
                System.IO.Stream picture = request.GetResponse().GetResponseStream();
                Image img = System.Drawing.Bitmap.FromStream(picture);
                picture.Close();
                this.pictureBox1.Image = img;
                string str = "";
                string Collegename = "", Majorname = "", Collagetype = "", Class = "";
                string StudentID = "", Studentname = "", StudentSex = "", StudentBirthday = "";
                string StudentPhone = "", Studentorigin = "", Foreignlanguage = "";
                string Highschool = "", Homeaddress = "", IDcardNum = "";
                Download("http://www.cdjwc.com/jiaowu/JWXS/xskp/jwxs_xskp_like.aspx?usermain=" + Var.Name);
                pictureBox1.Visible = true;//避免在加载前显示空图片框
                Status.Show = "正在分析数据...";
                StreamReader TEMPFILE = new StreamReader("TEMPFILE");
                try
                {
                    while (true)
                    {
                        str = TEMPFILE.ReadLine();
                        if (str.IndexOf("lbxsh") >= 0)
                        {
                            Collegename = str.Substring(str.IndexOf("lbxsh") + 7, (str.IndexOf("</")) - str.IndexOf("lbxsh") - 7);
                        }
                        if (str.IndexOf("lbzyh") >= 0)
                        {
                            Majorname = str.Substring(str.IndexOf("lbzyh") + 7, (str.IndexOf("</")) - str.IndexOf("lbzyh") - 7);
                        }
                        if (str.IndexOf("lbxz") >= 0)
                        {
                            Collagetype = str.Substring(str.IndexOf("lbxz") + 6, (str.IndexOf("</")) - str.IndexOf("lbxz") - 6);
                        }
                        if (str.IndexOf("lbbh") >= 0)
                        {
                            Class = str.Substring(str.IndexOf("lbbh") + 6, (str.IndexOf("</")) - str.IndexOf("lbbh") - 6);
                        }
                        if (str.IndexOf("Lbxh") >= 0)
                        {
                            StudentID = str.Substring(str.IndexOf("Lbxh") + 6, (str.IndexOf("</")) - str.IndexOf("Lbxh") - 6);
                        }
                        if (str.IndexOf("tbxsxm") >= 0 && str.IndexOf("text") >= 0)
                        {
                            Studentname = str.Substring(str.IndexOf("value") + 7, str.IndexOf("maxlength") - str.IndexOf("value") - 9);
                        }
                        if ((str.IndexOf("男") >= 0 || str.IndexOf("女") >= 0) && str.IndexOf("selected") >= 0)
                        {
                            StudentSex = str.Substring(str.IndexOf("value") + 10, 1);
                        }
                        if ((str.IndexOf("value") >= 0 && str.IndexOf("text") >= 0) && str.IndexOf("tbcsrq") >= 0)
                        {
                            StudentBirthday = str.Substring(str.IndexOf("value") + 7, 8);
                        }
                        if ((str.IndexOf("value") >= 0 && str.IndexOf("text") >= 0) && str.IndexOf("tbbrlxdh") >= 0)
                        {
                            StudentPhone = str.Substring(str.IndexOf("value") + 7, 11);
                        }
                        if ((str.IndexOf("value") >= 0 && str.IndexOf("text") >= 0) && str.IndexOf("tbjg") >= 0)
                        {
                            Studentorigin = str.Substring(str.IndexOf("value") + 7, str.IndexOf("maxlength") - str.IndexOf("value") - 9);
                        }
                        if ((str.IndexOf("value") >= 0 && str.IndexOf("text") >= 0) && str.IndexOf("tbzxwyyz") >= 0)
                        {
                            Foreignlanguage = str.Substring(str.IndexOf("value") + 7, 2);
                        }
                        if ((str.IndexOf("value") >= 0 && str.IndexOf("text") >= 0) && str.IndexOf("tbrxqgzdw") >= 0)
                        {
                            Highschool = str.Substring(str.IndexOf("value") + 7, str.IndexOf("maxlength") - str.IndexOf("value") - 9);
                        }
                        if ((str.IndexOf("value") >= 0 && str.IndexOf("text") >= 0) && str.IndexOf("tbjtxzdz") >= 0)
                        {
                            Homeaddress = str.Substring(str.IndexOf("value") + 7, str.IndexOf("maxlength") - str.IndexOf("value") - 9);
                        }
                        if ((str.IndexOf("value") >= 0 && str.IndexOf("text") >= 0) && str.IndexOf("tbsfzh") >= 0)
                        {
                            IDcardNum = str.Substring(str.IndexOf("value") + 7, str.IndexOf("maxlength") - str.IndexOf("value") - 9);
                            TEMPFILE.Close();
                            break;
                        }
                    }
                }
                catch
                {
                    TEMPFILE.Close();
                    return;
                }
                Status.Abort = 1;
                label9.Text = "姓名：\n" + Studentname;
                label10.Text = "学号：\n" + StudentID;
                label11.Text = "学院：\n" + Collegename;
                label12.Text = "专业：\n" + Majorname;
                label13.Text = "学制：\n" + Collagetype;
                label14.Text = "班级：\n" + Class;
                label15.Text = "性别：\n" + StudentSex;
                label16.Text = "生日：\n" + StudentBirthday;
                label17.Text = "手机号码：\n" + StudentPhone;
                label18.Text = "籍贯：\n" + Studentorigin;
                label19.Text = "外语种类：\n" + Foreignlanguage;
                label20.Text = "入学前单位：\n" + Highschool;
                label22.Text = "身份证号码：\n" + IDcardNum;
                if (Homeaddress.Length >= 25)
                {
                    label21.Text = "家庭地址：\n" + Homeaddress.Substring(0, 24) + "...";
                }
                else
                {
                    label21.Text = "家庭地址：\n" + Homeaddress;
                }
                Clear();
                File.Delete("TEMPFILE");
                Var.Studentname = Studentname;
            }
            catch
            {
                MessageBox.Show("未知错误,无法读取您的个人信息", "错误");
                Alert();
            }
        }

        private void button4_Click(object sender, EventArgs e)//选课
        {
            try
            {
                //关闭按键，避免勿点
                Clearwidget();
                label6.Visible = true;
                button7.Visible = true;
                label49.Visible = true;
                if (File.Exists("TEMP"))
                {
                    File.Delete("TEMP");
                }
                FileStream CreateTEMP = new FileStream("TEMP", FileMode.Create, FileAccess.Write);
                CreateTEMP.Close();
                StreamWriter Data = new StreamWriter("TEMP", true);
                Data.Write(Var.Name + "|" + Var.cookie[0].ToString() + "|" + Var.cookie[1].ToString());
                Data.Close();
                Process.Start("MakeCookie.exe");
            }
            catch
            {
                MessageBox.Show("未知错误，无法进入选课系统", "错误");
                Alert();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (File.Exists("TEMP"))
            {
                File.Delete("TEMP");
            }
            FileStream CreateTEMP = new FileStream("TEMP", FileMode.Create, FileAccess.Write);
            CreateTEMP.Close();
            StreamWriter Data = new StreamWriter("TEMP", true);
            Data.Write(Var.Name + "|" + Var.cookie[0].ToString() + "|" + Var.cookie[1].ToString());
            Data.Close();
            Process.Start("MakeCookie.exe");
        }

        private void button5_Click(object sender, EventArgs e)//小工具
        {
            Clearwidget();
            label7.Visible = true;
            //pictureBox14.Visible = true;
            //Thread ProgressThread = new Thread(ProgressForm);
            //ProgressThread.Start();
            //Status.Show = "正在准备下载数据...";
            //ProgressThread.Abort();
            MessageBox.Show("该功能网络端未准备就绪，稍后将会开放，敬请期待！", "稍后更精彩");
        }

        private void Download(string strUrl)
        {
            try
            {
                Status.Show = "正在下载数据...";
                if (File.Exists("TEMPFILE"))
                {
                    File.Delete("TEMPFILE");
                }
                //请求页面
                List<Cookie> TEMP = Var.cookie;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.ServicePoint.UseNagleAlgorithm = false;
                request.AllowWriteStreamBuffering = false;
                request.Method = "GET";
                request.ProtocolVersion = new Version(1, 1);
                request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";
                request.Referer = strUrl;
                request.Headers.Add("Accept-Language", "zh-cn");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)";
                //request.Host = "www.cdjwc.com";
                request.KeepAlive = true;
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(TEMP[0]);
                request.CookieContainer.Add(TEMP[1]);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Status.Show = "正在读取数据...";
                using (StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("GB2312")))
                {
                    Form1Var.Msgdownload = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                }
            }
            catch
            {
                MessageBox.Show("读取信息出错！", "错误");
                Alert();
            }
                FileStream CreateTEMPFILE = new FileStream("TEMPFILE", FileMode.Create, FileAccess.Write);
                CreateTEMPFILE.Close();
                StreamWriter Result = new StreamWriter("TEMPFILE", true);
                Result.Write(Form1Var.Msgdownload);
                Result.Close();
        }

        private void Clearwidget()
        {
            //主页
            label48.Visible = false;
            pictureBox2.Visible = false;
            //课程表
            label1.Visible = false;
            label2.Visible = false;
            comboBox1.Visible = false;
            label3.Visible = false;
            comboBox2.Visible = false;
            button6.Visible = false;
            dataGridView1.Visible = false;
            ExcelToolStripMenuItem.Enabled = false;
            //成绩
            ClearShowInf();
            Mark.passed.Clear();
            Mark.year.Clear();
            Mark.ClassName.Clear();
            Mark.score.Clear();
            Mark.type.Clear();
            Mark.Coursescore.Clear();
            Mark.examtype.Clear();
            pictureBox12.Visible = false;
            pictureBox13.Visible = false;
            label4.Visible = false;
            label32.Visible = false;
            label33.Visible = false;
            label43.Visible = false;
            label44.Visible = false;
            label45.Visible = false;
            label46.Visible = false;
            label47.Visible = false;
            //个人信息
            label5.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            label17.Visible = false;
            label18.Visible = false;
            label19.Visible = false;
            label20.Visible = false;
            label21.Visible = false;
            label22.Visible = false;
            pictureBox1.Visible = false;
            //选课
            label6.Visible = false;
            button7.Visible = false;
            label49.Visible = false;
            //小工具
            label7.Visible = false;
            //pictureBox14.Visible = false;
        }

        private void Clear()
        {
            Form1Var.Msgdownload = "";
            Form1Var.Retdownload = 0;
            if (File.Exists("TEMPFILE"))
            {
                File.Delete("TEMPFILE");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //菜单
        //账户
        private void CheckAuthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
            CheckAuth Show = new CheckAuth();
            Show.Show();
        }

        private void ChangeAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
            if (MessageBox.Show("切换账户将会退出登录，是否继续？", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
                this.Close();
                System.Environment.Exit(0);
            }
            else
            {
                return;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
            if (MessageBox.Show("您是否要退出程序？", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Environment.Exit(0);
            }
            else
            {
                return;
            }
        }

        //工具
        private void ExcelToolStripMenuItem_Click(object sender, EventArgs e)//导出课程表到Excel
        {
            try
            {
                Clear();
                DataGridViewToExcel(dataGridView1);
            }
            catch
            {
                MessageBox.Show("导出到Excel错误!", "错误");
                Alert();
                return;
            }
        }

        public static void DataGridViewToExcel(DataGridView dgv)
        {
            #region   验证可操作性

            //申明保存对话框      
            SaveFileDialog dlg = new SaveFileDialog();
            //默然文件后缀      
            dlg.DefaultExt = "xlsx ";
            //文件后缀列表      
            dlg.Filter = "EXCEL文件(*.XLSX)|*.xlsx ";
            //默然路径是系统当前路径      
            dlg.InitialDirectory = Directory.GetCurrentDirectory();
            //打开保存对话框      
            if (dlg.ShowDialog() == DialogResult.Cancel) return;
            //返回文件路径      
            string fileNameString = dlg.FileName;
            //验证strFileName是否为空或值无效      
            if (fileNameString.Trim() == " ")
            { return; }
            //定义表格内数据的行数和列数      
            int rowscount = dgv.Rows.Count;
            int colscount = dgv.Columns.Count;
            //行数必须大于0      
            if (rowscount <= 0)
            {
                MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //列数必须大于0      
            if (colscount <= 0)
            {
                MessageBox.Show("没有数据可供保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //行数不可以大于65536      
            if (rowscount > 65536)
            {
                MessageBox.Show("数据记录数太多(最多不能超过65536条)，不能保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //列数不可以大于255      
            if (colscount > 255)
            {
                MessageBox.Show("数据记录行数太多，不能保存 ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //验证以fileNameString命名的文件是否存在，如果存在删除它      
            FileInfo file = new FileInfo(fileNameString);
            if (file.Exists)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message, "删除失败 ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            #endregion
            Excel.Application objExcel = null;
            Excel.Workbook objWorkbook = null;
            Excel.Worksheet objsheet = null;
            try
            {
                //申明对象      
                objExcel = new Microsoft.Office.Interop.Excel.Application();
                objWorkbook = objExcel.Workbooks.Add(Type.Missing);
                objsheet = (Excel.Worksheet)objWorkbook.ActiveSheet;
                //设置EXCEL不可见      
                objExcel.Visible = false;

                //向Excel中写入表格的表头      
                int displayColumnsCount = 1;
                for (int i = 0; i <= dgv.ColumnCount - 1; i++)
                {
                    if (dgv.Columns[i].Visible == true)
                    {
                        objExcel.Cells[1, displayColumnsCount] = dgv.Columns[i].HeaderText.Trim();
                        displayColumnsCount++;
                    }
                }
                //向Excel中逐行逐列写入表格中的数据      
                for (int row = 0; row <= dgv.RowCount - 1; row++)
                {
                    displayColumnsCount = 1;
                    for (int col = 0; col < colscount; col++)
                    {
                        if (dgv.Columns[col].Visible == true)
                        {
                            try
                            {
                                objExcel.Cells[row + 2, displayColumnsCount] = dgv.Rows[row].Cells[col].Value.ToString().Trim();
                                displayColumnsCount++;
                            }
                            catch (Exception)
                            {

                            }

                        }
                    }
                }
                //保存文件      
                objWorkbook.SaveAs(fileNameString, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Excel.XlSaveAsAccessMode.xlShared, Type.Missing, Type.Missing, Type.Missing,
                        Type.Missing, Type.Missing);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "警告 ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            finally
            {
                //关闭Excel应用      
                if (objWorkbook != null) objWorkbook.Close(Type.Missing, Type.Missing, Type.Missing);
                if (objExcel.Workbooks != null) objExcel.Workbooks.Close();
                if (objExcel != null) objExcel.Quit();

                objsheet = null;
                objWorkbook = null;
                objExcel = null;
            }
            if (MessageBox.Show("课程表已被导出到" + fileNameString+"\n是否要打开该文件?", "导出完成", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(fileNameString);
            }
        }    

        //帮助
        private void FeedbackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Feedback Show = new Feedback();
            Show.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 Show = new Form2();
            Show.ShowDialog();
        }

        public void Alert()
        {
            Thread FeedbackError = new Thread(ErrorFeedback);
            FeedbackError.Start();
        }

        public void ErrorFeedback()
        {
            if (MessageBox.Show("本软件的功能尚未完善，您能够给我们描述一下刚才出现的问题吗？", "反馈", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Feedback Show = new Feedback();
                Show.Show();
            }
            else
            {
                return;
            }
        }

        private void CheckNewVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clear();
                Process.Start("Update.exe");
            }
            catch
            {
                MessageBox.Show("程序文件缺失，请重新安装!", "错误");
                Alert();
                System.Environment.Exit(-1);
            }
        }

        public void ProgressForm()
        {
            Form5 progress = new Form5();
            progress.ShowDialog();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string retString = "";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ccuassistant.azurewebsites.net/Update.aspx?1.0.0");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                retString = myStreamReader.ReadToEnd();
                string Version = "";
                StreamReader TEMPFILE = new StreamReader("Update.ini");
                while (Version.IndexOf("Version") < 0)
                {
                    Version = TEMPFILE.ReadLine();
                }
                TEMPFILE.Close();
                if (retString.IndexOf(Version) >= 0)
                {
                    return;
                }
                else
                {
                    if (MessageBox.Show("新版本的长大助手来啦，要不要来试试呢？", "更新", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Process.Start("Update.exe");
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch
            {
            }
        }
    }

    public class Form1Var
    {
        public static string Msgdownload="";
        public static int Retdownload = 0;
        public static ArrayList Coursetable = new ArrayList();
    }

    public class Mark
    {
        public static ArrayList passed = new ArrayList();
        public static ArrayList year = new ArrayList();
        public static ArrayList ClassName = new ArrayList();
        public static ArrayList score = new ArrayList();
        public static ArrayList type = new ArrayList();
        public static ArrayList Coursescore = new ArrayList();
        public static ArrayList examtype = new ArrayList();
        public static string Range = "";
        public static int Page = 0;
        public static int TotalFailedCourseNum = 0;
        public static string CNGPA ="";
        public static double USGPA = 0;
    }

    public class ExtraFunction
    {
        
    }
}

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
using System.Collections.Specialized;

namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public IntPtr Handle1;

        public Form3()
        {
            InitializeComponent();
            //071540103
            //textBox1.Text = "041540411";
            //textBox2.Text = "041540411";
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Handle1 = this.Handle;
            SetForegroundWindow(Handle1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string Username = textBox1.Text, Password = textBox2.Text;
                if (Username == "" || Password == "")
                {
                    return;
                }
                Thread ProgressThread = new Thread(ProgressForm);
                ProgressThread.Start();
                for (int i = 0; i <= 3; i++)//0为ie//1为firefox//2为chrome
                {
                    if (i == 3) { i = 0; }
                    if (i == 2) { Var.LogStatus++; Status.Show = "登陆失败,准备重新登陆..."; Thread.Sleep(2000); }
                    Thread.Sleep(100);
                    LoginConfirm(Username, Password, i + 1);
                    if (Var.RetLogin != 2 || Status.StopLoginLoop == true)
                    {
                        break;
                    }
                }
                Status.Abort = 1;
                Handle1 = this.Handle;
                if (Var.RetLogin != 3) { SetForegroundWindow(Handle1); }
                switch (Var.RetLogin)
                {
                    case 0: MessageBox.Show("登陆失败", "错误"); return;
                    case 1: MessageBox.Show("用户名密码错误或者服务器忙", "错误"); return;
                    case 2: MessageBox.Show("教务系统服务器错误\n如果多次登陆失败，请您稍等30s后再试", "错误"); return;
                    case 3: break;
                    case 4: MessageBox.Show("无法连接网络", "错误"); return;
                    default: MessageBox.Show("未知错误", "错误"); return;
                }
                AuthorityConfirm(Username);
                switch (Var.Authstatus)
                {
                    case 1: Pay(Username); return;
                    case 2: Var.Name = Username; Var.PWD = Password; this.Close(); return;
                    default: MessageBox.Show("未知错误，无法验证授权信息", "错误"); return;
                }
            }
            catch
            {
                MessageBox.Show("登陆程序出现未知错误", "错误");
                return;
            }
        }

        private void AuthorityConfirm(string Username)
        {
            Var.Authstatus = 2;
        }

        private void Pay(string Username)
        {

        }

        private void LoginConfirm(string Username, string Password, int times)
        {
            //获得VIEWSTATE和EVENTVALIDATION
            Status.Show = "正在登陆...";
            string __VIEWSTATE = "", __EVENTVALIDATION = "";
            string Account = "&Account=" + Username + "&PWD=" + Password + "&cmdok=";
            CookieContainer Cookie = new CookieContainer();
            try
            {
                //请求页面
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.cdjwc.com/jiaowu/Login.aspx");
                request.ServicePoint.UseNagleAlgorithm = false;
                request.AllowWriteStreamBuffering = false;
                request.CookieContainer = new CookieContainer();
                request.Method = "GET";
                request.ProtocolVersion = new Version(1, 1);
                if (Var.LogStatus % 2 != 0) { request.ProtocolVersion = new Version(1, 0); }
                if (times == 1) { request.Accept = @"text/html, application/xhtml+xml, image/jxr, */*"; }
                if (times == 1) { request.Headers.Add(@"Accept-Language", @"zh-Hans-CN,zh-Hans;q=0.8,en-US;q=0.5,en;q=0.3"); }
                if (times == 1) { request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"; }
                if (times == 1) { request.Headers.Add(@"Accept-Encoding", @"gzip, deflate"); }
                if (times == 1) { request.Host = @"www.cdjwc.com"; }
                if (times == 1) { request.KeepAlive = true; }

                if (times == 2) { request.Host = @"www.cdjwc.com"; }
                if (times == 2) { request.UserAgent = @"Mozilla/5.0 (Windows NT 6.2; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0"; }
                if (times == 2) { request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"; }
                if (times == 2) { request.Headers.Add(@"Accept-Language", @"zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3"); }
                if (times == 2) { request.Headers.Add(@"Accept-Encoding", @"gzip, deflate"); }
                if (times == 2) { request.KeepAlive = true; }

                if (times == 3) { request.Host = @"www.cdjwc.com"; }
                if (times == 3) { request.KeepAlive = true; }
                if (times == 3) { request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"; }
                if (times == 3) { request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36"; }
                if (times == 3) { request.Headers.Add(@"Accept-Encoding", @"gzip, deflate, sdch"); }
                if (times == 3) { request.Headers.Add(@"Accept-Language", @"zh-CN,zh;q=0.8"); }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                Cookie = request.CookieContainer;
                //处理缓存文件，分离值
                //Status.Show = "正在处理数据...";
                while (true)
                {
                    string retString = myStreamReader.ReadLine();
                    if (retString.IndexOf("__VIEWSTATE") >= 0)
                    {
                        //__VIEWSTATE
                        int IDXvalue = retString.IndexOf("value");
                        int IDXFormer = retString.IndexOf("\"", IDXvalue);
                        int LIDXFormer = retString.LastIndexOf("\"");
                        __VIEWSTATE = retString.Substring(IDXFormer + 2, LIDXFormer - IDXFormer - 2);
                        __VIEWSTATE = System.Web.HttpUtility.UrlEncode("/" + __VIEWSTATE);
                        //读行
                        retString = myStreamReader.ReadLine();
                        retString = myStreamReader.ReadLine();
                        //__EVENTVALIDATION
                        IDXvalue = retString.IndexOf("value");
                        IDXFormer = retString.IndexOf("\"", IDXvalue);
                        LIDXFormer = retString.LastIndexOf("\"");
                        __EVENTVALIDATION = retString.Substring(IDXFormer + 2, LIDXFormer - IDXFormer - 2);
                        __EVENTVALIDATION = System.Web.HttpUtility.UrlEncode("/" + __EVENTVALIDATION);
                        break;
                    }
                }
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch
            {
                Var.RetLogin = 4;
                return;
            }
            //登陆
            //Status.Show = "正在登陆...";
            string strMsg = "";
            string strUrl = @"http://www.cdjwc.com/jiaowu/Login.aspx";
            string retcode = "__VIEWSTATE=" + __VIEWSTATE + "&__EVENTVALIDATION=" + __EVENTVALIDATION + Account;
            try
            {
                List<Cookie> TEMP = GetAllCookies(Cookie);
                byte[] retcodeBuffer = System.Text.Encoding.ASCII.GetBytes(retcode);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.ServicePoint.UseNagleAlgorithm = false;
                request.AllowWriteStreamBuffering = false;
                request.CookieContainer = new CookieContainer();
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST"; 
                request.ProtocolVersion = new Version(1, 1);
                if (Var.LogStatus % 2 != 0) { request.ProtocolVersion = new Version(1, 0); }
                if (times == 1) { request.Accept = @"text/html, application/xhtml+xml, image/jxr, */*"; } 
                if (times == 1) { request.Referer = @"http://www.cdjwc.com/jiaowu/"; }
                if (times == 1) { request.Headers.Add(@"Accept-Language", @"zh-Hans-CN,zh-Hans;q=0.8,en-US;q=0.5,en;q=0.3"); } 
                if (times == 1) { request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"; }
                if (times == 1) { request.ContentType = @"application/x-www-form-urlencoded"; } 
                if (times == 1) { request.Headers.Add(@"Accept-Encoding", @"gzip, deflate"); }
                if (times == 1) { request.ContentLength = retcodeBuffer.Length; } 
                if (times == 1) { request.Host = @"www.cdjwc.com";}
                if (times == 1) { request.KeepAlive = true; }
                if (times == 1) { SetHeaderValue(request.Headers, "Connection", "Keep-alive"); }
                if (times == 1) { request.Headers.Add(@"pragma", @"no-cache"); }
                if (times == 1) { request.CookieContainer.Add(TEMP[0]); }

                if (times == 2) { request.Host = @"www.cdjwc.com"; }
                if (times == 2) { request.UserAgent = @"Mozilla/5.0 (Windows NT 6.2; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0"; }
                if (times == 2) { request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"; }
                if (times == 2) { request.Headers.Add(@"Accept-Language", @"zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3"); }
                if (times == 2) { request.Headers.Add(@"Accept-Encoding", @"gzip, deflate"); }
                if (times == 2) { request.KeepAlive = true; }
                if (times == 2) { SetHeaderValue(request.Headers, "Connection", "Keep-alive"); }
                if (times == 2) { request.Referer = @"http://www.cdjwc.com/jiaowu/"; }
                if (times == 2) { request.CookieContainer.Add(TEMP[0]); }
                if (times == 2) { request.ContentType = @"application/x-www-form-urlencoded"; }
                if (times == 2) { request.ContentLength = retcodeBuffer.Length; }

                if (times == 3) { request.Host = @"www.cdjwc.com"; }
                if (times == 3) { request.KeepAlive = true; }
                if (times == 3) { SetHeaderValue(request.Headers, "Connection", "Keep-alive"); }
                if (times == 3) { request.ContentLength = retcodeBuffer.Length; }
                if (times == 3) { request.Headers.Add(@"Cache-Control", @"max-age=0"); }
                if (times == 3) { request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"; }
                if (times == 3) { request.Headers.Add(@"Origin", @"http://www.cdjwc.com"); }
                if (times == 3) { request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) "; }
                if (times == 3) { request.ContentType = @"application/x-www-form-urlencoded"; }
                if (times == 3) { request.Referer = @"http://www.cdjwc.com/jiaowu/"; }
                if (times == 3) { request.Headers.Add(@"Accept-Encoding", @"gzip, deflate"); }
                if (times == 3) { request.Headers.Add(@"Accept-Language", @"zh-CN,zh;q=0.8"); }
                if (times == 3) { request.CookieContainer.Add(TEMP[0]); }
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(retcodeBuffer, 0, retcodeBuffer.Length);
                    requestStream.Close();
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Cookie = request.CookieContainer;
                switch (Cookie.Count)
                {
                    case 0: Var.RetLogin = 4; return;
                    case 1: break;
                    case 2: Var.RetLogin = 3; Var.cookie = GetAllCookies(Cookie); return;
                }
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
                {
                    strMsg = reader.ReadToEnd();
                    reader.Close();
                    //MessageBox.Show(strMsg);
                }
                request.Abort();
                if (strMsg.IndexOf(@"http://tjs.sjs.sinajs.cn/open/api/js/wb.js") >= 0)
                {
                    Var.RetLogin = 1;
                    request.Abort();
                    return;
                }
                if (strMsg.IndexOf("Error") >= 0)
                {
                    Var.RetLogin = 2;
                    request.Abort();
                    return;
                }
            }
            catch
            {
                Var.RetLogin = 1;
                return;
            }
        }

        public void ProgressForm()
        {
            try
            {
                Form5 progress = new Form5();
                progress.ShowDialog();
            }
            catch { }
        }

        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }

        public static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> RetCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) RetCookies.Add(c);
            }
            return RetCookies;
        }
    }

    public class Var
    {
        public static int RetLogin = 0;//1为用户名密码错误，2为登陆过频繁或人数过多，3为登陆成功,4为网络错误
        public static int Authstatus = 0;//1为验证失败，2为验证成功
        public static string Name = "";
        public static string PWD = "";
        public static List<Cookie> cookie;
        public static string Studentname = "";
        public static int Abort = 0;
        public static int LogStatus = 0;
    }

}

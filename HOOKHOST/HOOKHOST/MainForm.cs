﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
namespace HOOKHOST
{
    public partial class MainForm : Form
    {
        [DllImport("kernel32.dll")] //声明API函数
        public static extern int VirtualAllocEx(IntPtr hwnd, int lpaddress, int size, int type, int tect);

        [DllImport("kernel32.dll")]
        public static extern int WriteProcessMemory(IntPtr hwnd, int baseaddress, string buffer, int nsize, int filewriten);

        [DllImport("kernel32.dll")]
        public static extern int GetProcAddress(int hwnd, string lpname);

        [DllImport("kernel32.dll")]
        public static extern int GetModuleHandleA(string name);

        [DllImport("kernel32.dll")]
        public static extern int CreateRemoteThread(IntPtr hwnd, int attrib, int size, int address, int par, int flags, int threadid);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        internal static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        // GetValue(节名称,键名称，默认文本，INI文件路径) 从INI文件中读取指定项的值
        public static string GetValue(string Section, string Key, string defaultText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, defaultText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return defaultText;
            }
        }
        public MainForm()
        {
            InitializeComponent();
        }
        string ve= Environment.GetEnvironmentVariable("LocalAppData") + "\\Winupon\\Vizpower\\tmp\\loginmeetinglist.ini";
        // ve 课堂配置文件路径
        string ve2= Environment.GetEnvironmentVariable("LocalAppData") + "\\Winupon\\Vizpower\\logintool.ini";
        // ve2 登陆配置文件路径
        string pwxb = @"C:\Program Files (x86)\wxb\iMeeting2.exe#C:\Program Files (x86)\wxb\iMeeting.exe#C:\Program Files\wxb\iMeeting2.exe#C:\Program Files\wxb\iMeeting.exe#D:\Program Files (x86)\wxb\iMeeting2.exe#D:\Program Files (x86)\wxb\iMeeting.exe#D:\Program Files\wxb\iMeeting2.exe#D:\Program Files\wxb\iMeeting.exe#E:\Program Files\wxb\iMeeting2.exe#E:\Program Files\wxb\iMeeting.exe#E:\Program Files (x86)\wxb\iMeeting2.exe#E:\Program Files (x86)\wxb\iMeeting.exe#D:\iMeeting2.exe#D:\iMeeting.exe#D:\wxb\iMeeting2.exe#D:\wxb\iMeeting.exe#C:\wxb\iMeeting2.exe#C:\wxb\iMeeting.exe#F:\Program Files\wxb\iMeeting2.exe#F:\Program Files\wxb\iMeeting.exe#F:\Program Files (x86)\wxb\iMeeting2.exe#F:\Program Files (x86)\wxb\iMeeting.exe";
        //pwxb 以“#”分割的一段文本 储存了无限宝目录的可能值
        string xmwb = "AllowHLS#AutoRecordPrompt#CameraRemind#CameraSnap#ClassAutoLock#ClassNoteURL#ClientType#Course-Big-PictureUrl#Course-Total-time#DocURL#EastimateTime#EditTestStdAns#EvaluateURL#FeedbackURL#GreenScreenURL#IPVCamera#MeetCurrTime#MeetId#MeetStartTime#Meeting-AddTime#Meeting-BeforeTime#Meeting-Chairman#Meeting-Duration#Meeting-Project#Meeting-Subject#MeetingQuitURL#MultiMeeting#MultiVideoChannels#NDConf#NeedSSR#NetDiskNotifyURL#NetDiskProtocol#NetDiskUploadURL#NetDiskUserName#NetDiskUserPasswd#NickName#NoAppShare#NoNetDisk#NoPlayMedia#Port#PresidentKey2#ProjectName#ProxyAllocType#QRCodeBaseURL#RestVodURL#SensitiveWordsURL#ServerIP#ShowUserCount#SnapUploadURL#StuClass#StuPhone#StuSchool#StudentDetailURL#TeacherOverviewURL#TestResultURL#VerifyKey#VideoQualityLevel#VoteResultURL#WinAppTitle#exeurl#listenType#signupCount#timesId#updatedirurl#userId";
        //xmwb 以“#”分割的一段文本 储存了无限宝启动所需的所有参数
        string[] xmwbs;
        string[] kclb;
        string[] pwxbs;
        string dllidname = "WXBCJ2.dll";
        //刷新课程列表
        public void RefreshClass() {

            label2.Text = "";
            listBox1.Items.Clear();
            //获得当前已经登陆的用户名
            if (System.IO.File.Exists(ve2))
            {

                    label2.Text = "当前已登陆账号:" + GetValue("wxbfz","username","未登录，请刷新课程",ve2);

            }
            //kclb 所有课程对应的ID列表
            kclb = (GetValue("list", "mtid", "", ve)).Split(',');
            foreach (string l in kclb)
            {
                //向列表框中加入课程
                listBox1.Items.Add(GetValue("mt" + l, "Meeting-Subject", "", ve));
            }
            //数组 无限宝目录的可能值
            pwxbs = pwxb.Split('#');
        }
        private void GetCoreFiles()
        {

               WebClient client = new WebClient();
               client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
               client.Headers.Add("UserAgent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
               client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
               client.DownloadFile(@"https://yuhuison-1259460701.cos.ap-chengdu.myqcloud.com/WXBCJ.dll", dllidname);
               client.Dispose();

        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshClass();
            checkBox5.Enabled = File.Exists(dllidname);
            if (!System.IO.File.Exists(ve2)) {
                
            }

            //读取配置文件 的目录
            if (GetValue("wxbfz", "mulu", "", ve) != "")
            {
                textBox2.Text = GetValue("wxbfz", "mulu", "", ve);
            }
            else {
                foreach (string l in pwxbs) {
                    if (System.IO.File.Exists(l)) {
                        textBox2.Text = l;
                        break;
                    }

                }
            }
            //如果找不到无限宝的目录 用注册表的方法寻找
            if (textBox2.Text == "") {
                if (GetMl() != "")
                {
                    textBox2.Text = GetMl() + "iMeeting.exe";
                }
                else {
                    MessageBox.Show("程序未能找到无限宝程序所在位置，请手动键入");
                }

            }
            xmwbs = xmwb.Split('#');
        }
        string GetMl() {
            return GetRegistryValue("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{4C26091B-502D-475F-94FF-1A26AF917645}", "InstallLocation");
        }
        protected string GetRegistryValue(string path, string paramName)
        {
            string value = string.Empty;
            RegistryKey root = Registry.LocalMachine;
            RegistryKey rk = root.OpenSubKey(path);
            if (rk != null)
            {
                value = (string)rk.GetValue(paramName, null);
            }
            return value;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                string str = "94AEB546-26AE-48da-AC3A-B15BF1245699";
                //无限宝的 GUID
                if (listBox1.SelectedIndex != -1)
                {

                    string xml = "";
                    //MessageBox.Show("mt" + kclb[listBox1.SelectedIndex]);
                    foreach (string l in xmwbs)
                    {
                        //默认从配置文件中读取参数
                        string m = GetValue("mt" + kclb[listBox1.SelectedIndex], l, "", ve);
                        //修改参数
                        if (checkBox1.Checked)
                        {
                            if (l == "ClassAutoLock")
                            {
                                m = "0";
                            }
                        }
                        if (checkBox2.Checked)
                        {
                            if (l == "ShowUserCount")
                            {
                                m = "1";
                            }
                        }

                        if (checkBox3.Checked)
                        {
                            if (l == "SensitiveWordsURL")
                            {
                                m = "";
                            }
                        }
                        if (checkBox4.Checked)
                        {
                            if (l == "NickName")
                            {
                                m = textBox1.Text;
                            }
                        }

                        xml = xml + "<" + str + l + ">" + m + "</" + str + ">";

                    }
                    System.Diagnostics.Process.Start(textBox2.Text, xml);//启动无限宝
                    button5.Visible = true;

                }
                else
                {
                    MessageBox.Show("请先选择课程！");

                }
                if (checkBox5.Checked) {
                    int ok1;

                    int baseaddress;
                    int temp = 0;
                    int hack;
                    int yan;
                    string dllname;

                    dllname = Environment.CurrentDirectory + "\\"+dllidname;
                    //MessageBox.Show(dllname);
                    int dlllength;
                    dlllength = dllname.Length + 1;
                    if (File.Exists(dllname)==false) {
                        label4.Text=("DLL不存在！");
                    }
                    Process[] pname = Process.GetProcesses(); //取得所有进程

                    foreach (Process name in pname) //遍历进程
                    {
                        //MessageBox.Show(name.ProcessName.ToLower());
                        if (name.ProcessName.ToLower()=="imeeting"|| name.ProcessName.ToLower() == "imeeting2") //那么下面开始注入
                        {

                            baseaddress = VirtualAllocEx(name.Handle, 0, dlllength, 4096, 4);   //申请内存空间
                            if (baseaddress == 0) //返回0则操作失败，下面都是
                            {
                                label4.Text = ("申请内存空间失败！！");
                                Application.Exit();
                            }

                            ok1 = WriteProcessMemory(name.Handle, baseaddress, dllname, dlllength, temp); //写内存
                            if (ok1 == 0)
                            {
                                label4.Text = ("写内存失败！！");
                                Application.Exit();
                            }
                           // MessageBox.Show(GetModuleHandleA("kernel32.dll").ToString());
                            hack = GetProcAddress(GetModuleHandleA("kernel32"), "LoadLibraryA"); //取得loadlibarary在kernek32.dll地址

                            if (hack == 0)
                            {
                                label4.Text = ("无法取得函数的入口点！！");
                                Application.Exit();
                            }

                            yan = CreateRemoteThread(name.Handle, 0, 0, hack, baseaddress, 0, temp); //创建远程线程。

                            if (yan == 0)
                            {
                                label4.Text = ("创建远程线程失败！！");
                                Application.Exit();
                            }
                            else
                            {

                                label4.Text = ("已成功注入dll!!");

                                //SetWindowPos(WXBJB,(IntPtr)-1,0,0,0,0,);
                            }
                        }
                    }
                }
            }
            else {
                label4.Text = ("iMeeting.exe 未填写 程序无法启动");

            }
        }
        public static bool SetValue(string Section, string Key, string Value, string iniFilePath)
        {
            var pat = Path.GetDirectoryName(iniFilePath);
            if (Directory.Exists(pat) == false)
            {
                Directory.CreateDirectory(pat);
            }
            if (File.Exists(iniFilePath) == false)
            {
                File.Create(iniFilePath).Close();
            }
            long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
            if (OpStation == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                Form2 f = new Form2();
                f.ve = ve;
                f.kcmc = "mt" + kclb[listBox1.SelectedIndex];
                f.xmwbs = xmwbs;
                f.lj = textBox2.Text;
                f.Show();
            }
            else
            {
                MessageBox.Show("先选课程");
            }
        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            SetValue("wxbfz","mulu",textBox2.Text,ve2);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Login f = new Login();
            f.mf = this;
            f.ve = ve;
            f.ve2 = ve2;
            f.Show();
        }

        private void Label5_Click(object sender, EventArgs e)
        {
        	            if (listBox1.SelectedIndex != -1)
            {
                Form2 f = new Form2();
                f.ve = ve;
                f.kcmc = "mt" + kclb[listBox1.SelectedIndex];
                f.xmwbs = xmwbs;
                f.lj = textBox2.Text;
                f.Show();
            }
            else
            {
                MessageBox.Show("先选课程");
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            int i = 0;
            try
            {
                GetCoreFiles();
            }
            catch {
                i = 1;
                MessageBox.Show("核心文件下载失败！");
            }
            if (i == 0) {
                MessageBox.Show("下载完成！");
            }
            checkBox5.Enabled = File.Exists(dllidname);
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            IntPtr i = FindWindow("DiShanFangNB", null);
            ShowWindow(i, 5);
        }

        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("启用插件后获得以下功能\r\n1.解禁部分按钮(可以看见聊天处多出一个送礼物按钮)\r\n2.认真度修改\r\n进入课堂之后要先假装认真一会，之后程序就会锁定认真\r\n如果不这样做，老师会【一直】看见你【当前】不认真，但是认真度不会下降\r\n【插件】使用某语言编写，可能会【报毒】");
        }

        private void LinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void LinkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/yuhuison/WXBThirdLogin");
        }

        void DLLload() {

            int ok1;

            int baseaddress;
            int temp = 0;
            int hack;
            int yan;
            string dllname;

            dllname = Environment.CurrentDirectory + "\\"+dllidname;
            int dlllength;
            dlllength = dllname.Length + 1;
            if (File.Exists(dllname) == false)
            {
                label4.Text = ("DLL不存在！");
            }
            Process[] pname = Process.GetProcesses(); //取得所有进程

            foreach (Process name in pname) //遍历进程
            {
                //MessageBox.Show(name.ProcessName.ToLower());
                if (name.ProcessName.ToLower() == "imeeting" || name.ProcessName.ToLower() == "imeeting2") //那么下面开始注入
                {

                    baseaddress = VirtualAllocEx(name.Handle, 0, dlllength, 4096, 4);   //申请内存空间
                    if (baseaddress == 0) //返回0则操作失败，下面都是
                    {
                        label4.Text = ("申请内存空间失败！！");
                        Application.Exit();
                    }

                    ok1 = WriteProcessMemory(name.Handle, baseaddress, dllname, dlllength, temp); //写内存
                    if (ok1 == 0)
                    {
                        label4.Text = ("写内存失败！！");
                        Application.Exit();
                    }
                    // MessageBox.Show(GetModuleHandleA("kernel32.dll").ToString());
                    hack = GetProcAddress(GetModuleHandleA("kernel32"), "LoadLibraryA"); //取得loadlibarary在kernek32.dll地址

                    if (hack == 0)
                    {
                        label4.Text = ("无法取得函数的入口点！！");
                        Application.Exit();
                    }

                    yan = CreateRemoteThread(name.Handle, 0, 0, hack, baseaddress, 0, temp); //创建远程线程。

                    if (yan == 0)
                    {
                        label4.Text = ("创建远程线程失败！！");
                        Application.Exit();
                    }
                    else
                    {

                        label4.Text = ("已成功注入dll!!");

                        //SetWindowPos(WXBJB,(IntPtr)-1,0,0,0,0,);
                    }
                }
            }
        }
        private void LinkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DLLload();
            button5.Visible = true;
        }

        private void LinkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "请选择XML";
            dialog.Filter = "XML(*.*)|*.XML";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = dialog.FileName;
                string  xml= File.ReadAllText(filePath);
                System.Diagnostics.Process.Start(textBox2.Text, xml);
                if (checkBox5.Checked) {
                    DLLload();
                    button5.Visible = true;
                }
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//using System.Linq;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Management;
using RAK420_Config_Tool;
using CodeProject.Dialog;
using System.Configuration;

namespace Wisview
{
    public partial class Form1 : Form
    {
        //WIFI扫描和配置变量
        UdpClient myUdpclient = null;
        string scancmd = "@LT_WIFI_DEVICE@";
        byte[] scanbytes = { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0x2a, 0 };
        string scancmd_ack = "@LT_WIFI_CONFIRM@";
        int searchdesport = 5570;
        int searchsrcport = 55556;
        private Thread UDPThread = null;
        int selecteds = 0;
        IPEndPoint myUDPCIpe = null;
        private Thread UDPThread_LTSP = null;
        bool myUdpclientOPEN = false;
        bool UDPThread_LTSP_Enable = false;
        private int line_count = 0;
        private byte timerretry_count = 0;
        string[] Module_MAC_List = new string[100];//声明一个临时数组存储当前的MAC地址列表
        bool Search_Timeout = false;//扫描是否超时标志量
        RAK420 RAK420_INFO = new RAK420();//声明RAK420类中RAK420_INFO信息结构体
        byte[] empty = new byte[0];//定义一个空字节数组

        byte[] KeyName = new byte[100];//关键字
        byte[] Val = new byte[100];//配置数据信息
        bool reset_time = false;//判断是否收到复位成功
        bool facreset_time = false;//判断是否收到恢复出厂设置成功

        private bool ch_en = false;//false表示中文状态；true表示英文状态
        string BoardCastIP = "";
        FileStream file_bin = null;

        string Post_ip = "POST /update_success.html HTTP/1.1\r\nHost: ";
        string Post_length = "\r\nConnection: Keep-Alive\r\nContent-Length: ";
        string Post_admin = "\r\nAuthorization: Basic ";
        string Post_end = "\r\n-----------------------------7de19a322d0eee\r\nContent-Disposition: form-data; name=\"files\"; filename=\"RAK415.bin\"";
        private Thread Thread_TCP = null;

        TcpClient Tcp_socket = null;
        NetworkStream Tcp_stream = null;

        TcpClient Tcp_socket1 = null;
        NetworkStream Tcp_stream1 = null;

        TcpClient Tcp_socket2 = null;
        NetworkStream Tcp_stream2 = null;

        TcpClient Tcp_socket3 = null;
        NetworkStream Tcp_stream3 = null;

        TcpClient Tcp_socket4 = null;
        NetworkStream Tcp_stream4 = null;

        TcpClient Tcp_socket5 = null;
        NetworkStream Tcp_stream5 = null;

        TcpClient Tcp_socket6 = null;
        NetworkStream Tcp_stream6 = null;

        TcpClient Tcp_socket7 = null;
        NetworkStream Tcp_stream7 = null;

        TcpClient Tcp_socket8 = null;
        NetworkStream Tcp_stream8 = null;

        TcpClient Tcp_socket9 = null;
        NetworkStream Tcp_stream9 = null;

        int file_len = 0;
        int file_len1 = 0;
        int file_len2 = 0;
        int file_len3 = 0;
        int file_len4 = 0;
        int file_len5 = 0;
        int file_len6 = 0;
        int file_len7 = 0;
        int file_len8 = 0;
        int file_len9 = 0;

        int socket_line = 0;
        int socket_line1 = 0;
        int socket_line2 = 0;
        int socket_line3 = 0;
        int socket_line4 = 0;
        int socket_line5 = 0;
        int socket_line6 = 0;
        int socket_line7 = 0;
        int socket_line8 = 0;
        int socket_line9 = 0;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer3 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer4 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer5 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer6 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer7 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer8 = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timer9 = new System.Windows.Forms.Timer();

        System.Windows.Forms.Timer timerout = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer timerload = new System.Windows.Forms.Timer();
         public Form1()
        {
            InitializeComponent();
              vlc_init(false);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Enabled = false;
            timer.Interval = 1;
            timer.Tick += new System.EventHandler(timer_Upgrade);

            timer1.Enabled = false;
            timer1.Interval = 10;
            timer1.Tick += new System.EventHandler(timer1_Upgrade);

            timer2.Enabled = false;
            timer2.Interval = 10;
            timer2.Tick += new System.EventHandler(timer2_Upgrade);

            timer3.Enabled = false;
            timer3.Interval = 10;
            timer3.Tick += new System.EventHandler(timer3_Upgrade);

            timer4.Enabled = false;
            timer4.Interval = 10;
            timer4.Tick += new System.EventHandler(timer4_Upgrade);

            timer5.Enabled = false;
            timer5.Interval = 10;
            timer5.Tick += new System.EventHandler(timer5_Upgrade);

            timer6.Enabled = false;
            timer6.Interval = 10;
            timer6.Tick += new System.EventHandler(timer6_Upgrade);

            timer7.Enabled = false;
            timer7.Interval = 10;
            timer7.Tick += new System.EventHandler(timer7_Upgrade);

            timer8.Enabled = false;
            timer8.Interval = 10;
            timer8.Tick += new System.EventHandler(timer8_Upgrade);

            timer9.Enabled = false;
            timer9.Interval = 10;
            timer9.Tick += new System.EventHandler(timer9_Upgrade);

            timerout.Enabled = false;
            timerout.Interval = 500;
            timerout.Tick += new System.EventHandler(timerout_Upgrade);

            timerload.Enabled = false;
            timerload.Interval = 1000;
            timerload.Tick += new System.EventHandler(timerload_Upgrade);
        }
        /*********************************************************************************************************
         ** 功能说明：查询本机子网掩码和网关地址，计算出广播地址并返回
         ********************************************************************************************************/
        public string GetSubnetAndGateway()
        {
            string strIP, strSubnet, strGateway, strDNS;
            strIP = "0.0.0.0";
            strSubnet = "0.0.0.0";
            strGateway = "0.0.0.0";
            strDNS = "0.0.0.0";
            BoardCastIP = "";
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection nics = mc.GetInstances();
                foreach (ManagementObject nic in nics)
                {
                    try
                    {
                        if (Convert.ToBoolean(nic["IPEnabled"]) == true)
                        {

                            if ((nic["IPAddress"] as String[]).Length > 0 && strIP == "0.0.0.0")
                            {
                                strIP = (nic["IPAddress"] as String[])[0];
                            }
                            if ((nic["IPSubnet"] as String[]).Length > 0 && strSubnet == "0.0.0.0")
                            {
                                strSubnet = (nic["IPSubnet"] as String[])[0];
                            }
                            if ((nic["DefaultIPGateway"] as String[]).Length > 0 && strGateway == "0.0.0.0")
                            {
                                strGateway = (nic["DefaultIPGateway"] as String[])[0];
                            }
                            if ((nic["DNSServerSearchOrder"] as String[]).Length > 0 && strDNS == "0.0.0.0")
                            {
                                strDNS = (nic["DNSServerSearchOrder"] as String[])[0];
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
            string[] Subnet = strSubnet.Split(new Char[] { '.' });
            string[] GateWay = strGateway.Split(new Char[] { '.' });
            if (GateWay[0] + GateWay[1] + GateWay[2] + GateWay[3] == "0000")
            {
                BoardCastIP = "255.255.255.255";
                return BoardCastIP;
            }
            if ((Subnet.Length != 4) || (GateWay).Length != 4)
            {
                return BoardCastIP;
            }
            int x1 = (~(Convert.ToByte(Subnet[0])) | (Convert.ToByte(GateWay[0]))) & 0x000000FF;
            int x2 = (~(Convert.ToByte(Subnet[1])) | (Convert.ToByte(GateWay[1]))) & 0x000000FF;
            int x3 = (~(Convert.ToByte(Subnet[2])) | (Convert.ToByte(GateWay[2]))) & 0x000000FF;
            int x4 = (~(Convert.ToByte(Subnet[3])) | (Convert.ToByte(GateWay[3]))) & 0x000000FF;
            BoardCastIP = x1.ToString() + "." + x2.ToString() + "." + x3.ToString() + "." + x4.ToString();
            //return "IP地址 " + strIP + "\n" + "子网掩码 " + strSubnet + "\n" + "默认网关 " + strGateway + "\n" + "DNS服务器 " + strDNS;
            return BoardCastIP;
        }



        /*********************************************************************************************************
         ** 功能说明：查询本机IP地址
         ********************************************************************************************************/
        int errorcode = 0;
        System.Net.IPAddress GetHost_IPAddresses()   //errorcode 定义为  返回实际的错误值,如果非0 表示错误,如果为0 则表示无错误
        {
            System.Net.IPAddress[] addressListUDP = Dns.GetHostAddresses(Dns.GetHostName());//会返回所有地址，包括IPv4和IPv6
            //IPAddress ipAddrUDP = null;
            System.Net.IPAddress[] AddressList_IP = { null, null, null, null, null, null, null, null, null, null };//会返回,IPv4地址
            int n = 0;
#if  DEBUG
            Console.Write("IP(IPV4&IPV6):" + addressListUDP.Length.ToString() + "\r\n");
            for (int i = 0; i < addressListUDP.Length; i++)
            {
                Console.Write(i.ToString() + "-->AddressFamily:" + addressListUDP[i].AddressFamily + "\r\n");
                Console.Write("IP Address:" + addressListUDP[i].ToString() + "\r\n");

            }
#endif
            for (int i = 0; i < addressListUDP.Length; i++)
            {

                //从IP地址列表中筛选出IPv4类型的IP地址
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                if (addressListUDP[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    AddressList_IP[n] = addressListUDP[i];
                    n++;
                }
            }
#if  DEBUG
            Console.Write("IP Count:" + n.ToString() + "\r\n");
#endif
            if (n > 0)
            {
                errorcode = 0;
                // ipAddrUDP = AddressList_IP[1];
                return AddressList_IP[n - 1];

            }
            else
            {
                errorcode = -1;
                return null;
            }
        }

        int search_count = 0;//扫描次数
        /*********************************************************************************************************
         ** 功能说明：发送扫描指令
         ********************************************************************************************************/
        void send_search_cmd()
        {
            /*            //由本机地址变换为广播地址
                        string aa = GetHost_IPAddresses().ToString();
                        if (errorcode != 0)
                        {
                            MsgBox.Show("Please check network connecting.");
                            return;
                        }
                        errorcode = 0;
                    #if  DEBUG
                        Console.Write("Get host IP address.\r\n");
                        Console.Write("IP:" + aa + "\r\n");
                    #endif

                        string[] tmp = aa.Split('.');
                        tmp[3] = "255";
                        aa = tmp[0] + "." + tmp[1] + "." + tmp[2] + "." + tmp[3];
            */
            BoardCastIP = GetSubnetAndGateway();
            if (BoardCastIP == "")
                return;
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(BoardCastIP), searchdesport);


            //byte[] bytes = ASCIIEncoding.ASCII.GetBytes(scancmd);
            myUdpclient.Send(scanbytes, scanbytes.Length, iep);//发送扫描信息
        }


        /*********************************************************************************************************
         ** 功能说明：扫描模块
         ********************************************************************************************************/
        bool search = false;//是否在search过程中
        private void buttonScan_Click(object sender, System.EventArgs e)
        {

            try
            {
                search = true;
                search_count = 0;
                buttonScan.Enabled = false;
                dataGVscan.Enabled = false;
                dataGVscan.Rows.Clear();//清空列表
                Search_Timeout = false;//清扫描超时

                for (int m = 0; m < line_count; m++)//清空MAC字符串数组
                {
                    Module_MAC_List[m] = null;
                }
                line_count = 0;//列表行数清零

                if (myUdpclientOPEN == true)//已经开启单播接收线程
                {
                    UDPThread_LTSP.Abort();//关闭单播接收线程                    
                }

                if (myUdpclient == null)
                    myUdpclient = new UdpClient(searchsrcport);
                send_search_cmd();//发送扫描信息
                search_count++;
                UDPThread = new Thread(new ThreadStart(Search_Thread));
                UDPThread.IsBackground = true;//关闭窗口时自动关闭线程
                timersearch.Start();
                UDPThread.Start();
            }
            catch (ArgumentNullException ae)
            {
                //显示异常信息给客户。
                MessageBox.Show(ae.Message);
            }
        }

        /*********************************************************************************************************
         ** 功能说明：扫描线程
         ********************************************************************************************************/
        void Search_Thread()
        {
            IPEndPoint ipe = new IPEndPoint(GetHost_IPAddresses(), searchsrcport);

            if (errorcode != 0)
            {
                if (ch_en)
                    MsgBox.Show("Please check network connecting.");
                else
                    MsgBox.Show("请确认网络是否连接.");
                return;
            }
            errorcode = 0;
            while (true)
            {
                if ((myUdpclient != null) && (Search_Timeout == false))
                {
                    if (myUdpclient.Available > 0)
                    {
                        byte[] bytes = myUdpclient.Receive(ref ipe);//接收扫描到的数据信息
                        if (bytes != null)
                        {
                            this.Invoke((EventHandler)(delegate
                            {
                                bool MAC_NEW = true;
                                if ((bytes.Length > 43) && (bytes[1] == 0x80) && (bytes[3] == 0x01))
                                {
                                    string Module_MAC_List_temp = null;
                                    if (bytes[18] == 0x01)
                                    {
                                        int index = 19;
                                        for (index = 19; index < bytes.Length; index++)
                                        {
                                            if (bytes[index] == 0)
                                            {
                                                break;
                                            }
                                        }
                                        Module_MAC_List_temp = ASCIIEncoding.ASCII.GetString(bytes, 19, index - 19);
                                    }

                                    //判断MAC地址是否相同    
                                    for (int m = 0; m < line_count; m++)
                                    {
                                        if (Module_MAC_List_temp == Module_MAC_List[m])
                                        {
                                            MAC_NEW = false;//相同的话，表示不是新的MAC，无需新增一行列表
                                            break;
                                        }
                                    }
                                    if (MAC_NEW == true)//不相同的话，表示是新的MAC，需新增一行列表
                                    {
                                        //记录新增一行列表的MAC
                                        Module_MAC_List[line_count] = Module_MAC_List_temp;

                                        //新增一行列表
                                        if ((line_count >= 0))
                                        {
                                            dataGVscan.Rows.Add();
                                        }
                                        string ipstring = ipe.Address.ToString();

                                        //填充列表
                                        this.dataGVscan.Rows[line_count].Cells[0].Value = line_count + 1;//序号
                                        this.dataGVscan.Rows[line_count].Cells[1].Value = ipstring;//IP地址
                                        this.dataGVscan.Rows[line_count].Cells[2].Value = Module_MAC_List_temp;//MAC地址
                                        this.dataGVscan.Rows[line_count].Cells[4].Value = "Choose then Apply";//信号强度
                                        line_count++;//下一行
                                    }
                                }
                            }));
                        }
                    }
                }
            }
        }

        /*********************************************************************************************************
        ** 功能说明：判断扫描是否超时
        ********************************************************************************************************/
        int ver_num = 0;
        private void timersearch_Tick(object sender, EventArgs e)
        {
            if (search_count < 5)
                send_search_cmd();//发送扫描信息            
            if (search_count >= 7)
            {
                timersearch.Stop();
                search = false;
                myUdpclientOPEN = false;
                Search_Timeout = true;
                UDPThread.Abort();
                this.Invoke((EventHandler)(delegate
                {
                    buttonScan.Enabled = true;
                    dataGVscan.Enabled = true;
                }));
                string admin_data = "user_name=" + textBoxadmin.Text + "&user_password=" + textBoxpsk.Text;//整个认证信息--字符串
                byte[] admin = new byte[admin_data.Length];
                ver_num = 0;
                admin = ASCIIEncoding.ASCII.GetBytes(admin_data); //整个认证信息--字节数组
                string param = "<request><param1></param1></request>";
                string method = "GET";
                byte[] barray = Encoding.Default.GetBytes(textBoxadmin.Text + ":" + textBoxpsk.Text);
                basic = "Authorization: Basic " + Convert.ToBase64String(barray);
                Action<HttpStatusCode, string> onComplete = null;
                for (int i = 0; i < dataGVscan.RowCount; i++)
                {
                    if (dataGVscan.Rows[i].Cells[1].Value != null)
                    {
                        string ip = dataGVscan.Rows[i].Cells[1].Value.ToString();//获取目标IP地址
                        //LTSP_CMD(0xF, admin, IPAddress.Parse(ip)); //发送认证信息 
                        /*************************************************
                         * ***获取版本号
                         * ************************************************/
                        string url = ip + "/server.command?command=get_version";
                        string rcv = HTTP.Request(method, url, basic, param, onComplete);
                        string[] sArray;
                        if (rcv!= null)
                        {
                            if (rcv.Contains("WIFIV") || rcv.Contains("WifiV"))//检验“/”
                                {
                                    sArray = rcv.Split(',');
                                    sArray = sArray[0].Split(':');
                                    sArray[1] = sArray[1].Replace("\"", "");
                                    dataGVscan.Rows[i].Cells[3].Value = sArray[1];  
                                }
                             else
                               {
                                    sArray = rcv.Split('\n');
                                    dataGVscan.Rows[i].Cells[3].Value = sArray[1];
                               }
                            //textBoxresolution.Text = rcv.Replace("\n", "");
                        }
                        /*************************************************
                        * ***获取视频分辨率
                        * ************************************************/
                        sArray = null;
                        string url1 = ip + "/server.command?command=get_resol&type=h264&pipe=0";
                        string rcv1 = HTTP.Request(method, url1, basic, param, onComplete);
                        if (rcv1 != null)
                        {
                            if (rcv.Contains("info"))//检验“info”
                            {
                                sArray = rcv1.Split(',');
                                sArray = sArray[0].Split(':');
                                sArray[1] = sArray[1].Replace("}", "");

                            }
                            else
                            {

                                sArray = rcv1.Split(':');
                                sArray[1] = sArray[1].Replace("}", "");
                                sArray[1] = sArray[1].Replace(" ", "");

                            }
                           
                            if (sArray[1] == "\"-1\"")
                                textBoxrt1.Text = "***";
                            else if (sArray[1] == "\"0\""	)
                                textBoxrt1.Text = "320x240";
                            else if (sArray[1] == "\"1\"")
                                textBoxrt1.Text = "640x480";
                            else if (sArray[1] == "\"2\"")
                                textBoxrt1.Text = "1280x720";
                            else if (sArray[1] == "\"3\"")
                                textBoxrt1.Text = "1920x1080";
                        }
                        /*************************************************
                        * ***获取视频帧率
                        * ************************************************/
                        sArray = null;
                        string url2 = ip + "/server.command?command=get_max_fps&type=h264&pipe=0";
                        string rcv2 = HTTP.Request(method, url2, basic, param, onComplete);
                        if (rcv2 != null)
                        {
                            if (rcv.Contains("info"))//检验“info”
                            {
                                sArray = rcv2.Split(',');
                                sArray = sArray[0].Split(':');
                                sArray[1] = sArray[1].Replace("}", "");

                            }
                            else
                            {

                                sArray = rcv2.Split(':');
                                sArray[1] = sArray[1].Replace("}", "");
                                sArray[1] = sArray[1].Replace(" ", "");

                            }
                            sArray[1] = sArray[1].Replace("\"", "");

                            textBoxrt2.Text = sArray[1];
                        }
                        /*************************************************
                        * ***获取视频质量
                        * ************************************************/
                        sArray = null;
                        string url3 = ip + "/server.command?command=get_enc_quality&type=h264&pipe=0";
                        string rcv3 = HTTP.Request(method, url3, basic, param, onComplete);
                        if (rcv3 != null)
                        {

                            if (rcv.Contains("info"))//检验“info”
                            {
                                sArray = rcv3.Split(',');
                                sArray = sArray[0].Split(':');
                                sArray[1] = sArray[1].Replace("}", "");

                            }
                            else
                            {

                                sArray = rcv3.Split(':');
                                sArray[1] = sArray[1].Replace("}", "");
                                sArray[1] = sArray[1].Replace(" ", "");

                            }
                            sArray[1] = sArray[1].Replace("\"", "");
                            textBoxrt3.Text = sArray[1];
                        }
                        /*************************************************
                        * ***获取视频GOP
                        * ************************************************/
                        sArray = null;
                        string url4 = ip + "/server.command?command=get_enc_gop&type=h264&pipe=0";
                        string rcv4 = HTTP.Request(method, url4, basic, param, onComplete);
                        if (rcv4 != null)
                        {
                            if (rcv.Contains("info"))//检验“info”
                            {
                                sArray = rcv4.Split(',');
                                sArray = sArray[0].Split(':');
                                sArray[1] = sArray[1].Replace("}", "");

                            }
                            else
                            {

                                sArray = rcv4.Split(':');
                                sArray[1] = sArray[1].Replace("}", "");
                                sArray[1] = sArray[1].Replace(" ", "");

                            }
                            sArray[1] = sArray[1].Replace("\"", "");
                            textBoxrt4.Text = sArray[1];
                        }

                        /*************************************************
                        * ***获取串口波特率
                        * ************************************************/
                        sArray = null;
                        string url5 = ip + "/server.command?command=get_baudrate&type=uart1";
                        string rcv5 = HTTP.Request(method, url5, basic, param, onComplete);
                        if (rcv5 != null)
                        {

                            sArray = rcv5.Split(':');
                            sArray[1] = sArray[1].Replace("}", "");
                            sArray[1] = sArray[1].Replace("\"", "");
                            textBoxrt5.Text = sArray[1];
                        }
                        /*************************************************
                        * ***获取当前热点名称|获取当前连接路由器
                        * ************************************************/
                        sArray = null;
                      string url6 = ip + "/param.cgi?action=list&group=wifi";
                        string rcv6 = HTTP.Request(method, url6, basic, param, onComplete);
                        if (rcv6 != null)
                        {
                            if (rcv6.Contains("RAK_MODULE"))//检验“RAK_MODULE”
                            {
                                sArray = rcv6.Split(',');
                                sArray = sArray[3].Split(':');
                                sArray[1] = sArray[1].Replace("\"", "");
                                sArray[1] = sArray[1].Replace(" ", "");
                                textBoxrt6.Text = sArray[1];

                                sArray = null;
                                sArray = rcv6.Split(',');
                                sArray = sArray[10].Split(':');
                                sArray[1] = sArray[1].Replace("\"", "");
                                sArray[1] = sArray[1].Replace(" ", "");
                                textBoxrt7.Text = sArray[1];

                            }
                            else
                            {
                                sArray = rcv6.Split(',');
                                sArray = sArray[3].Split(':');
                                sArray[1] = sArray[1].Replace("\"", "");
                                sArray[1] = sArray[1].Replace(" ", "");
                                textBoxrt6.Text = sArray[1];

                                sArray = null;
                                sArray = rcv6.Split(',');
                                sArray = sArray[11].Split(':');
                                sArray[1] = sArray[1].Replace("\"", "");
                                textBoxrt7.Text = sArray[1];

                            }

                        }
                        
                        /*************************************************
                        * ***获取当前连接路由器
                        * ************************************************/
                       /* sArray = null;
                        string url7 = ip + "/param.cgi?action=list&group=wifi";
                       string rcv7 = HTTP.Request(method, url7, basic, param, onComplete);
                        if (rcv7 != null)
                        {
                            sArray = rcv7.Split(',');
                            sArray = sArray[11].Split(':');
                            sArray[1] = sArray[1].Replace("\"", "");
                            textBoxrt7.Text = sArray[1];
                        }*/
                        
                        confirm = true;
                        //timerout.Enabled = true;//开始计时
                    }
                }
            }
            search_count++;
        }

       
        /*********************************************************************************************************
        ** 功能说明：异或函数和异或校验函数
        ********************************************************************************************************/
        private byte Xor_Sum(byte[] Xor_SumArray, int Xor_SumLen)
        {
            int xor_sum = 0;
            int i = 0;
            for (i = 0; i < Xor_SumLen - 1; i++)
            {
                xor_sum ^= Xor_SumArray[i];
            }
            return ((byte)xor_sum);
        }

        //异或校验函数
        private bool Xor(byte[] XorArray, int XorLen)
        {
            bool xor_successful = false;
            int xor_sum = 0;
            try
            {
                for (int i = 0; i < XorLen - 1; i++)
                {
                    xor_sum ^= XorArray[i];
                }
                if (xor_sum == XorArray[XorLen - 1]) //异或成功
                    xor_successful = true;
                else                             //异或失败
                    xor_successful = false;
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                xor_successful = false;
            }
            return xor_successful;//返回校验结果
        }
        /*********************************************************************************************************
        ** 功能说明：单播与模块进行数据交互
        ********************************************************************************************************/
        public void LTSP_CMD(byte CMD, byte[] data, IPAddress Destip)
        {
            byte[] sendata = new byte[6 + data.Length + 1];
            //初始化命令
            sendata[0] = CMD;
            sendata[1] = 0x00;
            //数据长度
            sendata[2] = (byte)(data.Length & 0xFF);
            sendata[3] = (byte)((data.Length >> 8) & 0xFF);
            //响应码
            sendata[4] = 0x00;
            sendata[5] = 0x00;
            //数据内容
            for (int i = 0; i < data.Length; i++)
            {
                sendata[6 + i] = data[i];
            }
            //求校验
            sendata[6 + data.Length] = Xor_Sum(sendata, sendata.Length);
            //if (myUdpclient == null)
            //    myUdpclient = new UdpClient(searchsrcport);
            myUDPCIpe = new IPEndPoint(Destip, searchdesport);
            myUdpclient.Send(sendata, sendata.Length, myUDPCIpe);

            UDPThread_LTSP = new Thread(new ThreadStart(LTSPCMD_Thread));//UDP单播接收数据线程
            UDPThread_LTSP.IsBackground = true;

            if (myUdpclientOPEN == false)//未开启单播接收线程
            {
                UDPThread_LTSP.Start();//开启单播接收线程
                myUdpclientOPEN = true;
            }
        }
        /*********************************************************************************************************
       ** 功能说明：UDP单播接收数据线程
       *********************************************************************************************************/
        bool confirm = false;
        bool getversion = false;
        void LTSPCMD_Thread()
        {
            IPEndPoint ipe = new IPEndPoint(GetHost_IPAddresses(), searchsrcport);
            if (errorcode != 0)
            {
                if (ch_en)
                    MsgBox.Show("Please check network connecting.");
                else
                    MsgBox.Show("请确认网络是否连接.");
                return;
            }
            errorcode = 0;
            while (search == false)//非扫描过程中
            {
                if (myUdpclient != null)
                {
                    if (myUdpclient.Available > 0)
                    {
                        byte[] buf = myUdpclient.Receive(ref ipe);
                        if (buf != null)
                        {
                            this.Invoke((EventHandler)(delegate
                            {
                                if (Xor(buf, buf.Length) == true)//接收数据校验成功
                                {
                                    if (buf[0] == 0xF)//接收到模块给的认证回复：认证成功
                                    {
                                        confirm = false;
                                        timerout.Enabled = false;//开始计时                                       
                                        if (buf[4] == 0xFE)//模块认证失败
                                        {
                                            this.dataGVscan.Rows[ver_num].Cells[6].Value = "认证失败";
                                            ver_num++;
                                            if (ver_num < this.dataGVscan.RowCount - 1)
                                            {
                                                string admin_data = "user_name=" + textBoxadmin.Text + "&user_password=" + textBoxpsk.Text;//整个认证信息--字符串
                                                byte[] admin = new byte[admin_data.Length];
                                                admin = ASCIIEncoding.ASCII.GetBytes(admin_data); //整个认证信息--字节数组
                                                string ip = dataGVscan.Rows[ver_num].Cells[3].Value.ToString();//获取目标IP地址
                                                LTSP_CMD(0xF, admin, IPAddress.Parse(ip)); //发送认证信息 
                                                confirm = true;
                                                timerout.Enabled = true;//开始计时
                                            }
                                        }
                                        else
                                        {
                                            if (this.dataGVscan.Rows[ver_num].Cells[3].Value.ToString() != null)
                                            {
                                                LTSP_CMD(0x06, empty, IPAddress.Parse(this.dataGVscan.Rows[ver_num].Cells[3].Value.ToString())); //获取模块版本号
                                                getversion = true;
                                                timerout.Enabled = true;//开始计时
                                            }
                                        }
                                    }
                                    if (buf[0] == 0x06)//获取到模块的版本信息
                                    {
                                        getversion = false;
                                        timerout.Enabled = false;//开始计时 
                                        byte[] version = new byte[buf[3] * 256 + buf[2] - 15];//版本号数据长度
                                        for (int i = 0; i < version.Length; i++)
                                        {
                                            version[i] = buf[21 + i];//获取版本号数据
                                        }
                                        this.dataGVscan.Rows[ver_num].Cells[6].Value = System.Text.Encoding.ASCII.GetString(version);//版本号填入TextBox中显示
                                        ver_num++;
                                        if (ver_num < this.dataGVscan.RowCount - 1)
                                        {
                                            string admin_data = "user_name=" + textBoxadmin.Text + "&user_password=" + textBoxpsk.Text;//整个认证信息--字符串
                                            byte[] admin = new byte[admin_data.Length];
                                            admin = ASCIIEncoding.ASCII.GetBytes(admin_data); //整个认证信息--字节数组
                                            string ip = dataGVscan.Rows[ver_num].Cells[3].Value.ToString();//获取目标IP地址
                                            LTSP_CMD(0xF, admin, IPAddress.Parse(ip)); //发送认证信息 
                                            confirm = true;
                                            timerout.Enabled = true;//开始计时
                                        }
                                    }
                                }

                            }));
                        }
                    }
                }
            }
        }


        void Time_Count(bool enable_count, int c, int line)
        {
            if (enable_count)
            {
                this.dataGVscan.Rows[line].Cells[7].Value = "加载中...  剩余" + c + "s";
                c--;
                if (c == 0)
                {
                    this.dataGVscan.Rows[line].Cells[7].Value = "加载成功，升级完毕";
                    enable_count = false;
                }
            }
        }

        bool Is_Over(bool enable_count, int c)
        {
            bool ret = false;
            if (enable_count == false)
            {
                ret = true;
            }
            else if (enable_count && (c == 0))
            {
                ret = true;
            }
            return ret;
        }

        private void timerout_Upgrade(object sender, EventArgs e)
        {
            if (confirm)
            {
                string admin_data = "user_name=" + textBoxadmin.Text + "&user_password=" + textBoxpsk.Text;//整个认证信息--字符串
                byte[] admin = new byte[admin_data.Length];
                if (dataGVscan.Rows[ver_num].Cells[1].Value == null)
                {
                    string ip = dataGVscan.Rows[ver_num].Cells[1].Value.ToString();//获取目标IP地址
                    LTSP_CMD(0xF, admin, IPAddress.Parse(ip)); //发送认证信息
                }
            }
            if (getversion)
            {
                LTSP_CMD(0x06, empty, IPAddress.Parse(this.dataGVscan.Rows[ver_num].Cells[3].Value.ToString())); //获取模块版本号
            }
        }
        //
        int count = 150;
        int count1 = 60;
        int count2 = 60;
        int count3 = 60;
        int count4 = 60;
        int count5 = 60;
        int count6 = 60;
        int count7 = 60;
        int count8 = 60;
        int count9 = 60;
        bool Is_count = false;
        bool Is_count1 = false;
        bool Is_count2 = false;
        bool Is_count3 = false;
        bool Is_count4 = false;
        bool Is_count5 = false;
        bool Is_count6 = false;
        bool Is_count7 = false;
        bool Is_count8 = false;
        bool Is_count9 = false;

        private void timerload_Upgrade(object sender, EventArgs e)
        {
            /*            Time_Count(Is_count, count, socket_line);
                        Time_Count(Is_count1, count1, socket_line1);
                        Time_Count(Is_count2, count2, socket_line2);
                        Time_Count(Is_count3, count3, socket_line3);
                        Time_Count(Is_count4, count4, socket_line4);
                        Time_Count(Is_count5, count5, socket_line5);
                        Time_Count(Is_count6, count6, socket_line6);
                        Time_Count(Is_count7, count7, socket_line7);
                        Time_Count(Is_count8, count8, socket_line8);
                        Time_Count(Is_count9, count9, socket_line9);
            */
            if (Is_count)
            {
                count--;
                this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line].Cells[4].Value = "加载中...  " + (int)((150 - count) * 100 / 150) + "%";

                if (count == 0)
                {
                    this.dataGVscan.Rows[socket_line].Cells[4].Value = "加载成功，升级完毕";
                    Is_count = false;
                }
            }
            if (Is_count1)
            {
                count1--;
                this.dataGVscan.Rows[socket_line1].Cells[7].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line1].Cells[7].Value = "加载中...  " + (int)((60 - count1) * 100 / 60) + "%";
                if (count1 == 0)
                {
                    this.dataGVscan.Rows[socket_line1].Cells[7].Value = "加载成功，升级完毕";
                    Is_count1 = false;
                }
            }
            if (Is_count2)
            {
                count2--;
                this.dataGVscan.Rows[socket_line2].Cells[7].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line2].Cells[7].Value = "加载中...  " + (int)((60 - count2) * 100 / 60) + "%";
                if (count2 == 0)
                {
                    this.dataGVscan.Rows[socket_line2].Cells[7].Value = "加载成功，升级完毕";
                    Is_count2 = false;
                }
            }
            if (Is_count3)
            {
                count3--;
                this.dataGVscan.Rows[socket_line3].Cells[7].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line3].Cells[7].Value = "加载中...  " + (int)((60 - count3) * 100 / 60) + "%";
                if (count3 == 0)
                {
                    this.dataGVscan.Rows[socket_line3].Cells[7].Value = "加载成功，升级完毕";
                    Is_count3 = false;
                }
            }
            if (Is_count4)
            {
                count4--;
                this.dataGVscan.Rows[socket_line4].Cells[7].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line4].Cells[7].Value = "加载中...  " + (int)((60 - count4) * 100 / 60) + "%";
                if (count4 == 0)
                {
                    this.dataGVscan.Rows[socket_line4].Cells[7].Value = "加载成功，升级完毕";
                    Is_count4 = false;
                }
            }
            if (Is_count5)
            {
                count5--;
                this.dataGVscan.Rows[socket_line5].Cells[7].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line5].Cells[7].Value = "加载中...  " + (int)((60 - count5) * 100 / 60) + "%";
                if (count5 == 0)
                {
                    this.dataGVscan.Rows[socket_line5].Cells[7].Value = "加载成功，升级完毕";
                    Is_count5 = false;
                }
            }
            if (Is_count6)
            {
                count6--;
                this.dataGVscan.Rows[socket_line6].Cells[7].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line6].Cells[7].Value = "加载中...  " + (int)((60 - count6) * 100 / 60) + "%";
                if (count6 == 0)
                {
                    this.dataGVscan.Rows[socket_line6].Cells[7].Value = "加载成功，升级完毕";
                    Is_count6 = false;
                }
            }
            if (Is_count7)
            {
                count7--;
                this.dataGVscan.Rows[socket_line7].Cells[7].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line7].Cells[7].Value = "加载中...  " + (int)((60 - count7) * 100 / 60) + "%";
                if (count7 == 0)
                {
                    this.dataGVscan.Rows[socket_line7].Cells[7].Value = "加载成功，升级完毕";
                    Is_count7 = false;
                }
            }
            if (Is_count8)
            {
                count8--;
                this.dataGVscan.Rows[socket_line8].Cells[7].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line8].Cells[7].Value = "加载中...  " + (int)((60 - count8) * 100 / 60) + "%";
                if (count8 == 0)
                {
                    this.dataGVscan.Rows[socket_line8].Cells[7].Value = "加载成功，升级完毕";
                    Is_count8 = false;
                }
            }
            if (Is_count9)
            {
                count9--;
                this.dataGVscan.Rows[socket_line1].Cells[9].Style.BackColor = Color.PaleGreen;
                this.dataGVscan.Rows[socket_line1].Cells[9].Value = "加载中...  " + (int)((60 - count9) * 100 / 60) + "%";
                if (count9 == 0)
                {
                    this.dataGVscan.Rows[socket_line9].Cells[7].Value = "加载成功，升级完毕";
                    Is_count9 = false;
                }
            }

            if ((Is_Over(Is_count, count))
                && (Is_Over(Is_count1, count1))
                && (Is_Over(Is_count2, count2))
                && (Is_Over(Is_count3, count3))
                && (Is_Over(Is_count4, count4))
                && (Is_Over(Is_count5, count5))
                && (Is_Over(Is_count6, count6))
                && (Is_Over(Is_count7, count7))
                && (Is_Over(Is_count8, count8))
                && (Is_Over(Is_count9, count9))
                )
            {
               buttonupgrade.Enabled = true;
                timerload.Enabled = false;
            }
        }
        /*********************************************************************************************************
      ** 功能说明：选择固件路径
      ********************************************************************************************************/
        private void buttonimport_Click_1(object sender, EventArgs e)
        {
            //string str = ConfigurationManager.AppSettings["Directory"];
            string str = "";

            OpenFileDialog op = new OpenFileDialog();//弹出浏览框
            string filepath = System.Environment.CurrentDirectory;//获取软件所在文件夹路径
            if (str == "")
                op.InitialDirectory = filepath;//System.Environment.CurrentDirectory;//打开当前路径
            else
                op.InitialDirectory = str;//打开上一次路径
            op.RestoreDirectory = false;//还原当前路径
            op.Filter = "BIN文件(*.fw)|*.fw";//还原当前路径
            DialogResult result = op.ShowDialog();
            if (result == DialogResult.OK)
            {
                string filename = op.FileName;//获取文件路径
                textBoximport.Text = filename;
                if (textBoximport.Text != filename)
                {
                    if (file_bin != null)
                    {
                        file_bin.Close();
                        file_bin = null;
                    }
                }
                int index = filename.LastIndexOf("\\");
                if (index != -1)
                {
                    textBoxVersion.Text = filename.Substring(index + 1).Replace(".fw", "");
                }
                if (filename != null)
                {
                    //保存这次的路径
                    Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    configuration.AppSettings.Settings.Clear();
                    configuration.AppSettings.Settings.Add("Directory", filename);
                    configuration.Save();
                    ConfigurationManager.RefreshSection("appSettings");
                }
            }
        }

        void Socket_Connect(string ip, int port, int line)
        {
            if ((line == 0) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket = new TcpClient(ip, port);
                    Tcp_stream = Tcp_socket.GetStream();
                    Tcp_stream.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }
            else if ((line == 1) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket1 = new TcpClient(ip, port);
                    Tcp_stream1 = Tcp_socket1.GetStream();
                    Tcp_stream1.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }
            else if ((line == 2) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket2 = new TcpClient(ip, port);
                    Tcp_stream2 = Tcp_socket2.GetStream();
                    Tcp_stream2.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }
            else if ((line == 3) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket3 = new TcpClient(ip, port);
                    Tcp_stream3 = Tcp_socket3.GetStream();
                    Tcp_stream3.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }
            else if ((line == 4) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket4 = new TcpClient(ip, port);
                    Tcp_stream4 = Tcp_socket4.GetStream();
                    Tcp_stream4.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }
            else if ((line == 5) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket5 = new TcpClient(ip, port);
                    Tcp_stream5 = Tcp_socket5.GetStream();
                    Tcp_stream5.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }
            else if ((line == 6) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket6 = new TcpClient(ip, port);
                    Tcp_stream6 = Tcp_socket6.GetStream();
                    Tcp_stream6.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }
            else if ((line == 7) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket7 = new TcpClient(ip, port);
                    Tcp_stream7 = Tcp_socket7.GetStream();
                    Tcp_stream7.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }
            else if ((line == 8) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket8 = new TcpClient(ip, port);
                    Tcp_stream8 = Tcp_socket8.GetStream();
                    Tcp_stream8.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }
            else if ((line == 9) && (dataGVscan.Rows[line].Cells[6].Value.ToString() != textBoxVersion.Text.ToString()))
            {
                try
                {
                    Tcp_socket9 = new TcpClient(ip, port);
                    Tcp_stream9 = Tcp_socket9.GetStream();
                    Tcp_stream9.Write(Post_head, 0, Post_head.Length);
                }
                catch (Exception)
                {
                    dataGVscan.Rows[line].Cells[7].Value = "未连接上模块";
                }
            }

        }

        /************************************************************************************
         * *确定视频参数修改
         * **********************************************************************************/
        private void butn_Click(object sender, EventArgs e)
        {
            //butn.Enabled = false;
            string admin_data = "user_name=" + textBoxadmin.Text + "&user_password=" + textBoxpsk.Text;//整个认证信息--字符串
            byte[] admin = new byte[admin_data.Length];
            //ver_num = 0;
            admin = ASCIIEncoding.ASCII.GetBytes(admin_data); //整个认证信息--字节数组
            string param = "<request><param1></param1></request>";
            string method = "GET";
            byte[] barray = Encoding.Default.GetBytes(textBoxadmin.Text + ":" + textBoxpsk.Text);
            basic = "Authorization: Basic " + Convert.ToBase64String(barray);
            Action<HttpStatusCode, string> onComplete = null;
           // bool is_choose = false;
            for (int i = 0; i < dataGVscan.RowCount; i++)
            {

                if ((this.dataGVscan.Rows[i].Cells[5].EditedFormattedValue.ToString() == "True"))
                {
                    //is_choose = true;
                    //butn.Enabled = true;
                    if (dataGVscan.Rows[i].Cells[1].Value != null)
                    {
                        string ip = dataGVscan.Rows[i].Cells[1].Value.ToString();//获取目标IP地址
                        //LTSP_CMD(0xF, admin, IPAddress.Parse(ip)); //发送认证信息 
                        /*************************************************
                         * ***修改视频分辨率
                         * ************************************************/
                        //string shpfbl = comboBoxset1.Text.ToString();
                        if (comboBoxset1.Text == "320x240")
                        {
                            string url30 = ip + "/server.command?command=set_resol&type=h264&pipe=0&value=0";
                            // string rcv30 = HTTP.Request(method, url30, basic, param, onComplete);
                            string rcv30 = HTTP.Request(method, url30, basic, param, onComplete);

                            //string rcv31 = null;
                        }
                        if (comboBoxset1.Text == "640x480")
                        {
                            string url31 = ip + "/server.command?command=set_resol&type=h264&pipe=0&value=1";
                            string rcv31 = HTTP.Request(method, url31, basic, param, onComplete);
                        }
                        if (comboBoxset1.Text == "1280x720")
                        {
                            string url32 = ip + "/server.command?command=set_resol&type=h264&pipe=0&value=2";
                            string rcv32 = HTTP.Request(method, url32, basic, param, onComplete);
                        }
                        if (comboBoxset1.Text == "1920x1080")
                        {
                            string url41 = ip + "/server.command?command=set_resol&type=h264&pipe=0&value=3";
                            string rcv41 = HTTP.Request(method, url41, basic, param, onComplete);
                        }
                        /*************************************************
                         * ***修改视频帧率
                         * ************************************************/
                        if (textBoxset2.Text != null)
                        {
                            string g = textBoxset2.Text;
                            string url33 = ip + "/server.command?command=set_max_fps&type=h264&pipe=0&value="+ g;
                            string rcv33 = HTTP.Request(method, url33, basic, param, onComplete);
                        }
                        /*************************************************
                         * ***修改视频质量
                         * ************************************************/
                        if (textBoxset3.Text != null)
                        {
                            string f = textBoxset3.Text;
                            string url34 = ip + "/server.command?command=set_enc_quality&type=h264&pipe=0&value="+ f;
                            string rcv34 = HTTP.Request(method, url34, basic, param, onComplete);
                        }
                        /*************************************************
                         * ***修改视频GOP
                         * ************************************************/
                        if (textBoxset4.Text != null)
                        {
                            string a = textBoxset4.Text;
                            string url35 = ip + "/server.command?command=set_enc_gop&type=h264&pipe=0&value="+ a;
                            string rcv35 = HTTP.Request(method, url35, basic, param, onComplete);
                        }

                        MsgBox.Show("Apply ok");
                       
                    }
                   
                }
               /* else 
                {
                    MsgBox.Show("请先选择一个要修改的模块");
                    butn.Enabled = true;
                }*/
            }
        }

        /************************************************************************************
        * *模块信息修改
        * **********************************************************************************/
        private void butn2_Click(object sender, EventArgs e)
        {

            //butn.Enabled = false;
            string admin_data = "user_name=" + textBoxadmin.Text + "&user_password=" + textBoxpsk.Text;//整个认证信息--字符串
            byte[] admin = new byte[admin_data.Length];
            //ver_num = 0;
            admin = ASCIIEncoding.ASCII.GetBytes(admin_data); //整个认证信息--字节数组
            string param = "<request><param1></param1></request>";
            string method = "GET";
            byte[] barray = Encoding.Default.GetBytes(textBoxadmin.Text + ":" + textBoxpsk.Text);
            basic = "Authorization: Basic " + Convert.ToBase64String(barray);
            Action<HttpStatusCode, string> onComplete = null;
            // bool is_choose = false;
            for (int i = 0; i < dataGVscan.RowCount; i++)
            {

                if ((this.dataGVscan.Rows[i].Cells[5].EditedFormattedValue.ToString() == "True"))
                {
                    //is_choose = true;
                    //butn.Enabled = true;
                    if (dataGVscan.Rows[i].Cells[1].Value != null)
                    {
                        string ip = dataGVscan.Rows[i].Cells[1].Value.ToString();//获取目标IP地址
                        //LTSP_CMD(0xF, admin, IPAddress.Parse(ip)); //发送认证信息 
                        /*************************************************
                         * ***修改串口波特率
                         * ************************************************/
                        if (comboBoxset5.Text != null)
                        {
                            string b = comboBoxset5.Text;
                            string url36 = ip + "/server.command?command=set_baudrate&type=uart1&value=" + b;
                            string rcv36 = HTTP.Request(method, url36, basic, param, onComplete);
                        }
                        /*************************************************
                         * ***输入要连接路由器名称/输入要连接路由器密码
                         * ************************************************/
                        if (textBoxset6.Text != null)
                        {
                            string c = textBoxset6.Text;
                            string d = textBoxset7.Text;
                            string url37 = ip + "/param.cgi?action=update&group=wifi&sta_ssid=" + c + "&sta_auth_key=" + d;
                            string rcv37 = HTTP.Request(method, url37, basic, param, onComplete);
                        }
                        MsgBox.Show("Apply ok");
                       
                        /*************************************************
                         * ***输入要连接路由器密码
                         * ************************************************/
                        /*if (textBoxset7.Text != null)
                        {
                            string d = textBoxset7.Text;
                            string url38 = ip + "/param.cgi?action=update&group=wifi&ap_auth_key=" + d;
                            string rcv38 = HTTP.Request(method, url38, basic, param, onComplete);
                        }
                        MsgBox.Show("修改成功");*/
                    }

                }
                /* else 
                 {
                     MsgBox.Show("请先选择一个要修改的模块");
                     butn.Enabled = true;
                 }*/
            }

        }
        /*********************************************************************************************************
        ** 功能说明：开始升级
        ********************************************************************************************************/
        byte[] Post_head = null;
        byte[] file_byte = null;
        int file_size = 0;
        string basic = "";
        private void buttonupgrade_Click_1(object sender, EventArgs e)
        {
            socket_line = 0;
            socket_line1 = 0;
            socket_line2 = 0;
            socket_line3 = 0;
            socket_line4 = 0;
            socket_line5 = 0;
            socket_line6 = 0;
            socket_line7 = 0;
            socket_line8 = 0;
            socket_line9 = 0;

            count = 60;
            count1 = 60;
            count2 = 60;
            count3 = 60;
            count4 = 60;
            count5 = 60;
            count6 = 60;
            count7 = 60;
            count8 = 60;
            count9 = 60;

            Is_count = false;
            Is_count1 = false;
            Is_count2 = false;
            Is_count3 = false;
            Is_count4 = false;
            Is_count5 = false;
            Is_count6 = false;
            Is_count7 = false;
            Is_count8 = false;
            Is_count9 = false;

            if (textBoximport.Text == "")
            {
                MsgBox.Show("请先选择升级所需的文件！");
            }
            else
            {
                if (Thread_TCP == null)//未开启接收线程
                {
                    Thread_TCP = new Thread(new ThreadStart(Thread_TCP_Thread));//接收数据线程
                    Thread_TCP.IsBackground = true;
                    Thread_TCP.Start();//开启接收线程
                }

                byte[] barray;
                barray = Encoding.Default.GetBytes(textBoxadmin.Text + ":" + textBoxpsk.Text);
                basic = Convert.ToBase64String(barray);
                if (file_bin == null)
                {
                    file_bin = new FileStream(textBoximport.Text, FileMode.Open);
                    file_size = (int)file_bin.Length;
                    file_byte = new byte[file_size];
                    file_bin.Read(file_byte, 0, file_size);
                }
                if (file_bin == null)
                {
                    MsgBox.Show("请先打开升级所需的文件！");
                }
                else
                {
                    string boundaryStr = "----WebKitFormBoundary7Fb77GBVS0i2AmlE";
                    string Top_Str = "--" + boundaryStr + "\r\nContent-Disposition: form-data; name=\"firmware\"; filename=\"SkyEye.fw\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                    string Bottom_Str = "\r\n--" + boundaryStr + "--\r\n";
                    byte[] bar = Encoding.Default.GetBytes(textBoxadmin.Text + ":" + textBoxpsk.Text);
                    string admin_info = "Basic " + Convert.ToBase64String(bar);
                    string upgrademodule_filename = "POST /SkyEye/fwupgrade.cgi HTTP/1.1\r\n";
                    bool is_choose = false;
                    for (int i = 0; i < dataGVscan.RowCount - 1; i++)
                    {
                        if ((this.dataGVscan.Rows[i].Cells[5].EditedFormattedValue.ToString() == "True"))
                        {
                            is_choose = true;
                        }
                        //if (dataGVscan.Rows[i].Cells[3].Value != null)//获取到版本号的，判断是否和升级固件一致
                        {
                            //                            if (dataGVscan.Rows[i].Cells[6].Value.ToString() == textBoxVersion.Text.ToString())
                            //                            {
                            //                                dataGVscan.Rows[i].Cells[7].Value = "已是该固件，无需升级";
                            //                            }
                            //                            else
                            {
                                if ((Tcp_socket == null) && (this.dataGVscan.Rows[i].Cells[5].EditedFormattedValue.ToString() == "True"))
                                {
                                    string upgrademodule_filesize = "Host: " + dataGVscan.Rows[i].Cells[1].Value.ToString() + "\r\n" +
                                                                    "Content-Type: multipart/form-data; boundary=----WebKitFormBoundary7Fb77GBVS0i2AmlE\r\n" +
                                                                    "Authorization: " + admin_info + "\r\n" +
                                                                    "Content-Length: ";
                                    string upgrademodule_end = "\r\nAccept: */*\r\n\r\n";
                                    string upgrademodulestrString = upgrademodule_filename + upgrademodule_filesize + (file_size + Top_Str.Length + Bottom_Str.Length) + upgrademodule_end + Top_Str;
                                    socket_line = i;
                                    file_len = file_size;
                                    Post_head = System.Text.Encoding.ASCII.GetBytes(upgrademodulestrString);
                                    //if ((dataGVscan.Rows[i].Cells[3].Value.ToString() != textBoxVersion.Text.ToString()))
                                    {
                                        try
                                        {
                                            Tcp_socket = new TcpClient(dataGVscan.Rows[i].Cells[1].Value.ToString(), 80);
                                            Tcp_stream = Tcp_socket.GetStream();
                                            Tcp_stream.Write(Post_head, 0, Post_head.Length);
                                        }
                                        catch (Exception)
                                        {
                                            dataGVscan.Rows[i].Cells[4].Value = "未连接上模块";
                                            buttonupgrade.Enabled = false;
                                        }
                                    }
                                    timer.Enabled = true;
                                }

                                buttonupgrade.Enabled = false;
                            }
                        }

                    }
                    if (!is_choose)
                    {
                        MsgBox.Show("请先选择一个要升级的模块");
                        buttonupgrade.Enabled = true;
                    }
                }
            }

        }

        void Socket_Connect()
        {

        }            


        /*********************************************************************************************************
        ** 功能说明：升级发送数据
        ********************************************************************************************************/
        private void timer_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket != null)
            {
                try
                {
                    if (file_len > 2048)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream.Write(data, 0, data.Length);
                        if (Tcp_socket.Connected == true)
                        {
                            Tcp_stream.Write(file_byte, file_size - file_len, 2048);
                            file_len -= 2048;
                            this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.LightSkyBlue;
                            this.dataGVscan.Rows[socket_line].Cells[4].Value = "升级中...  " + (int)((file_size - file_len) * 100 / file_size) + "%";
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.Red;
                            dataGVscan.Rows[socket_line].Cells[4].Value = "socket异常";
                            buttonupgrade.Enabled = true;
                        }
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream.Write(s_data, 0, s_data.Length);
                        Tcp_stream.Write(file_byte, file_size - file_len, file_len);
                        file_len -= file_len;
                        string boundaryStr = "----WebKitFormBoundary7Fb77GBVS0i2AmlE";
                        string Bottom_Str = "\r\n--" + boundaryStr + "--\r\n";
                        byte[] Post_end = System.Text.Encoding.ASCII.GetBytes(Bottom_Str);
                        Tcp_stream.Write(Post_end, 0, Post_end.Length);
                        this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line].Cells[4].Value = "升级中...  " + (int)((file_size - file_len) * 100 / file_size) + "%";
                        timer.Enabled = false;
                    }
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line].Cells[4].Value = "模块异常";
                    buttonupgrade.Enabled = true;
                }
            }
        }
        private void timer1_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket1 != null)
            {
                try
                {
                    if (file_len1 > 512)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream1.Write(data, 0, data.Length);
                        Tcp_stream1.Write(file_byte, file_size - file_len1, 512);
                        file_len1 -= 512;
                        this.dataGVscan.Rows[socket_line1].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line1].Cells[7].Value = "升级中...  " + (int)((file_size - file_len1) * 100 / file_size) + "%";
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream1.Write(s_data, 0, s_data.Length);
                        Tcp_stream1.Write(file_byte, file_size - file_len1, file_len1);
                        file_len1 -= file_len1;
                        this.dataGVscan.Rows[socket_line1].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line1].Cells[7].Value = "升级中...  " + (int)((file_size - file_len1) * 100 / file_size) + "%";
                        timer1.Enabled = false;
                    }
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line1].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line1].Cells[7].Value = "模块异常";
                }
            }
        }

        private void timer2_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket2 != null)
            {
                try
                {
                    if (file_len2 > 512)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream2.Write(data, 0, data.Length);
                        Tcp_stream2.Write(file_byte, file_size - file_len2, 512);
                        file_len2 -= 512;
                        this.dataGVscan.Rows[socket_line2].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line2].Cells[7].Value = "升级中...  " + (int)((file_size - file_len2) * 100 / file_size) + "%";
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream2.Write(s_data, 0, s_data.Length);
                        Tcp_stream2.Write(file_byte, file_size - file_len2, file_len2);
                        file_len2 -= file_len2;
                        this.dataGVscan.Rows[socket_line2].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line2].Cells[7].Value = "升级中...  " + (int)((file_size - file_len2) * 100 / file_size) + "%";
                        timer2.Enabled = false;
                    }
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line2].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line2].Cells[7].Value = "模块异常";
                }
            }
        }

        private void timer3_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket3 != null)
            {
                try
                {
                    if (file_len3 > 512)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream3.Write(data, 0, data.Length);
                        Tcp_stream3.Write(file_byte, file_size - file_len3, 512);
                        file_len3 -= 512;
                        this.dataGVscan.Rows[socket_line3].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line3].Cells[7].Value = "升级中...  " + (int)((file_size - file_len3) * 100 / file_size) + "%";
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream3.Write(s_data, 0, s_data.Length);
                        Tcp_stream3.Write(file_byte, file_size - file_len3, file_len3);
                        file_len3 -= file_len3;
                        this.dataGVscan.Rows[socket_line3].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line3].Cells[7].Value = "升级中...  " + (int)((file_size - file_len3) * 100 / file_size) + "%";
                        timer3.Enabled = false;
                    }
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line3].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line3].Cells[7].Value = "模块异常";
                }
            }
        }

        private void timer4_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket4 != null)
            {
                try
                {
                    if (file_len4 > 512)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream4.Write(data, 0, data.Length);
                        Tcp_stream4.Write(file_byte, file_size - file_len4, 512);
                        file_len4 -= 512;
                        this.dataGVscan.Rows[socket_line4].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line4].Cells[7].Value = "升级中...  " + (int)((file_size - file_len4) * 100 / file_size) + "%";
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream4.Write(s_data, 0, s_data.Length);
                        Tcp_stream4.Write(file_byte, file_size - file_len4, file_len4);
                        file_len4 -= file_len4;
                        this.dataGVscan.Rows[socket_line4].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line4].Cells[7].Value = "升级中...  " + (int)((file_size - file_len4) * 100 / file_size) + "%";
                        timer4.Enabled = false;
                    }
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line4].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line4].Cells[7].Value = "模块异常";
                }
            }
        }

        private void timer5_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket5 != null)
            {
                try
                {
                    if (file_len5 > 512)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream5.Write(data, 0, data.Length);
                        Tcp_stream5.Write(file_byte, file_size - file_len5, 512);
                        file_len5 -= 512;
                        this.dataGVscan.Rows[socket_line5].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line5].Cells[7].Value = "升级中...  " + (int)((file_size - file_len5) * 100 / file_size) + "%";
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream5.Write(s_data, 0, s_data.Length);
                        Tcp_stream5.Write(file_byte, file_size - file_len5, file_len5);
                        file_len5 -= file_len5;
                        this.dataGVscan.Rows[socket_line5].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line5].Cells[7].Value = "升级中...  " + (int)((file_size - file_len5) * 100 / file_size) + "%";
                        timer5.Enabled = false;
                    }

                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line5].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line5].Cells[7].Value = "模块异常";
                }
            }
        }

        private void timer6_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket6 != null)
            {
                try
                {
                    if (file_len6 > 512)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream6.Write(data, 0, data.Length);
                        Tcp_stream6.Write(file_byte, file_size - file_len6, 512);
                        file_len6 -= 512;
                        this.dataGVscan.Rows[socket_line6].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line6].Cells[7].Value = "升级中...  " + (int)((file_size - file_len6) * 100 / file_size) + "%";
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream6.Write(s_data, 0, s_data.Length);
                        Tcp_stream6.Write(file_byte, file_size - file_len6, file_len6);
                        file_len6 -= file_len6;
                        this.dataGVscan.Rows[socket_line6].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line6].Cells[7].Value = "升级中...  " + (int)((file_size - file_len6) * 100 / file_size) + "%";
                        timer6.Enabled = false;
                    }
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line6].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line6].Cells[7].Value = "模块异常";
                }
            }
        }

        private void timer7_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket7 != null)
            {
                try
                {
                    if (file_len7 > 512)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream7.Write(data, 0, data.Length);
                        Tcp_stream7.Write(file_byte, file_size - file_len7, 512);
                        file_len7 -= 512;
                        this.dataGVscan.Rows[socket_line7].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line7].Cells[7].Value = "升级中...  " + (int)((file_size - file_len7) * 100 / file_size) + "%";
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream7.Write(s_data, 0, s_data.Length);
                        Tcp_stream7.Write(file_byte, file_size - file_len7, file_len7);
                        file_len7 -= file_len7;
                        this.dataGVscan.Rows[socket_line7].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line7].Cells[7].Value = "升级中...  " + (int)((file_size - file_len7) * 100 / file_size) + "%";
                        timer7.Enabled = false;
                    }
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line7].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line7].Cells[7].Value = "模块异常";
                }
            }
        }

        private void timer8_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket8 != null)
            {
                try
                {
                    if (file_len8 > 512)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream8.Write(data, 0, data.Length);
                        Tcp_stream8.Write(file_byte, file_size - file_len8, 512);
                        file_len8 -= 512;
                        this.dataGVscan.Rows[socket_line8].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line8].Cells[7].Value = "升级中...  " + (int)((file_size - file_len8) * 100 / file_size) + "%";
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream8.Write(s_data, 0, s_data.Length);
                        Tcp_stream8.Write(file_byte, file_size - file_len8, file_len8);
                        file_len8 -= file_len8;
                        this.dataGVscan.Rows[socket_line8].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line8].Cells[7].Value = "升级中...  " + (int)((file_size - file_len8) * 100 / file_size) + "%";
                        timer8.Enabled = false;
                    }
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line8].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line8].Cells[7].Value = "模块异常";
                }
            }
        }

        private void timer9_Upgrade(object sender, EventArgs e)
        {
            if (Tcp_socket9 != null)
            {
                try
                {
                    if (file_len9 > 512)
                    {
                        //byte[] data = new byte[512];
                        //file_bin.Read(data, 0, data.Length);
                        //Tcp_stream9.Write(data, 0, data.Length);
                        Tcp_stream9.Write(file_byte, file_size - file_len9, 512);
                        file_len9 -= 512;
                        this.dataGVscan.Rows[socket_line9].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line9].Cells[7].Value = "升级中...  " + (int)((file_size - file_len9) * 100 / file_size) + "%";
                    }
                    else
                    {
                        //byte[] s_data = new byte[file_len];
                        //file_bin.Read(s_data, 0, s_data.Length);
                        //Tcp_stream9.Write(s_data, 0, s_data.Length);
                        Tcp_stream9.Write(file_byte, file_size - file_len9, file_len9);
                        file_len9 -= file_len9;
                        this.dataGVscan.Rows[socket_line9].Cells[7].Style.BackColor = Color.LightSkyBlue;
                        this.dataGVscan.Rows[socket_line9].Cells[7].Value = "升级中...  " + (int)((file_size - file_len9) * 100 / file_size) + "%";
                        timer9.Enabled = false;
                    }
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line9].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line9].Cells[7].Value = "模块异常";
                }
            }
        }

        bool is_ret = false;
        void Socket_Read()
        {
            if ((Tcp_socket != null) && (Tcp_stream != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line].Cells[4].Value = "模块异常";
                    buttonupgrade.Enabled = true;
                    Tcp_stream.Close();
                    Tcp_stream = null;
                    Tcp_socket.Close();
                    Tcp_socket = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string Expires = "\"value\": \"";
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, bytes);
                        if ((read.StartsWith("HTTP/1.1 200 OK")) || is_ret)
                        {
                            int index = read.IndexOf(Expires);
                            if (index != -1)
                            {
                                is_ret = false;
                                int len = read.Length;
                                int index1 = read.IndexOf("\"", Expires.Length + index);
                                if (index1 != -1)
                                {
                                    string res = read.Substring(Expires.Length + index, index1 - index - Expires.Length);
                                    if (res.Equals("0"))
                                    {
                                        this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.PaleGreen;
                                        this.dataGVscan.Rows[socket_line].Cells[4].Value = "升级成功，开始加载";
                                        timerload.Enabled = true;
                                        Is_count = true;
                                        count = 150;
                                        Tcp_stream.Close();
                                        Tcp_stream = null;
                                        Tcp_socket.Close();
                                        Tcp_socket = null;
                                        buttonupgrade.Enabled = true;
                                    }
                                    else if (res.Equals("-31"))
                                    {
                                        this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.Red;
                                        this.dataGVscan.Rows[socket_line].Cells[4].Value = "升级失败，文件接收异常";
                                        Tcp_stream.Close();
                                        Tcp_stream = null;
                                        Tcp_socket.Close();
                                        Tcp_socket = null;
                                        buttonupgrade.Enabled = true;
                                    }
                                    else if (res.Equals("-32"))
                                    {
                                        this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.Red;
                                        this.dataGVscan.Rows[socket_line].Cells[4].Value = "升级失败，未发现存储卡";
                                        Tcp_stream.Close();
                                        Tcp_stream = null;
                                        Tcp_socket.Close();
                                        Tcp_socket = null;
                                        buttonupgrade.Enabled = true;
                                    }
                                    else if (res.Equals("-33"))
                                    {
                                        this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.Red;
                                        this.dataGVscan.Rows[socket_line].Cells[4].Value = "升级失败，固件校验失败";
                                        Tcp_stream.Close();
                                        Tcp_stream = null;
                                        Tcp_socket.Close();
                                        Tcp_socket = null;
                                        buttonupgrade.Enabled = true;
                                    }
                                    else if (res.Equals("-34"))
                                    {
                                        this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.Red;
                                        this.dataGVscan.Rows[socket_line].Cells[4].Value = "升级失败，存储卡空间不足";
                                        Tcp_stream.Close();
                                        Tcp_stream = null;
                                        Tcp_socket.Close();
                                        Tcp_socket = null;
                                        buttonupgrade.Enabled = true;
                                    }
                                    else
                                    {
                                        this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.Red;
                                        this.dataGVscan.Rows[socket_line].Cells[4].Value = "升级失败，未知异常";
                                        Tcp_stream.Close();
                                        Tcp_stream = null;
                                        Tcp_socket.Close();
                                        Tcp_socket = null;
                                        buttonupgrade.Enabled = true;
                                    }
                                }
                            }
                            else
                            {
                                is_ret = true;
                            }
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line].Cells[4].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line].Cells[4].Value = "升级失败";
                            Tcp_stream.Close();
                            Tcp_stream = null;
                            Tcp_socket.Close();
                            Tcp_socket = null;
                            buttonupgrade.Enabled = true;
                        }
                    }));
                }
            }
            if ((Tcp_socket1 != null) && (Tcp_stream1 != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream1.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line1].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line1].Cells[7].Value = "模块异常";
                    Tcp_stream1.Close();
                    Tcp_stream1 = null;
                    Tcp_socket1.Close();
                    Tcp_socket1 = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, 20);
                        if (read.StartsWith("HTTP/1.1 200 OK"))
                        {
                            this.dataGVscan.Rows[socket_line1].Cells[7].Style.BackColor = Color.PaleGreen;
                            this.dataGVscan.Rows[socket_line1].Cells[7].Value = "升级成功，开始加载";
                            Tcp_stream1.Close();
                            Tcp_stream1 = null;
                            Tcp_socket1.Close();
                            Tcp_socket1 = null;
                            timerload.Enabled = true;
                            /*                            label4.Visible = true;
                                                        pictureBox1.Visible = true;
                                                        pictureBox2.Visible = true;
                                                        groupBox2.Visible = false;
                             */
                            count1 = 60;
                            Is_count1 = true;
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line1].Cells[7].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line1].Cells[7].Value = "升级失败";
                        }

                    }));
                }
            }
            if ((Tcp_socket2 != null) && (Tcp_stream2 != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream2.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line2].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line2].Cells[7].Value = "模块异常";
                    Tcp_stream2.Close();
                    Tcp_stream2 = null;
                    Tcp_socket2.Close();
                    Tcp_socket2 = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, 20);
                        if (read.StartsWith("HTTP/1.1 200 OK"))
                        {
                            this.dataGVscan.Rows[socket_line2].Cells[7].Style.BackColor = Color.PaleGreen;
                            this.dataGVscan.Rows[socket_line2].Cells[7].Value = "升级成功，开始加载";
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line2].Cells[7].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line2].Cells[7].Value = "升级失败";
                        }
                        Tcp_stream2.Close();
                        Tcp_stream2 = null;
                        Tcp_socket2.Close();
                        Tcp_socket2 = null;
                        timerload.Enabled = true;
                        /*                        label4.Visible = true;
                                                pictureBox1.Visible = true;
                                                pictureBox2.Visible = true;
                                                groupBox2.Visible = false;
                        */
                        Is_count2 = true;
                        count2 = 60;
                    }));
                }
            }
            if ((Tcp_socket3 != null) && (Tcp_stream3 != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream3.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line3].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line3].Cells[7].Value = "模块异常";
                    Tcp_stream3.Close();
                    Tcp_stream3 = null;
                    Tcp_socket3.Close();
                    Tcp_socket3 = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, 20);
                        if (read.StartsWith("HTTP/1.1 200 OK"))
                        {
                            this.dataGVscan.Rows[socket_line3].Cells[7].Style.BackColor = Color.PaleGreen;
                            this.dataGVscan.Rows[socket_line3].Cells[7].Value = "升级成功，开始加载";
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line3].Cells[7].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line3].Cells[7].Value = "升级失败";
                        }
                        Tcp_stream3.Close();
                        Tcp_stream3 = null;
                        Tcp_socket3.Close();
                        Tcp_socket3 = null;
                        timerload.Enabled = true;
                        /*                        label4.Visible = true;
                                                pictureBox1.Visible = true;
                                                pictureBox2.Visible = true;
                                                groupBox2.Visible = false;
                        */
                        Is_count3 = true;
                        count3 = 60;
                    }));
                }
            }
            if ((Tcp_socket4 != null) && (Tcp_stream4 != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream4.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line4].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line4].Cells[7].Value = "模块异常";
                    Tcp_stream4.Close();
                    Tcp_stream4 = null;
                    Tcp_socket4.Close();
                    Tcp_socket4 = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, 20);
                        if (read.StartsWith("HTTP/1.1 200 OK"))
                        {
                            this.dataGVscan.Rows[socket_line4].Cells[7].Style.BackColor = Color.PaleGreen;
                            this.dataGVscan.Rows[socket_line4].Cells[7].Value = "升级成功，开始加载";
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line4].Cells[7].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line4].Cells[7].Value = "升级失败";
                        }
                        Tcp_stream4.Close();
                        Tcp_stream4 = null;
                        Tcp_socket4.Close();
                        Tcp_socket4 = null;
                        timerload.Enabled = true;
                        /*                        label4.Visible = true;
                                                pictureBox1.Visible = true;
                                                pictureBox2.Visible = true;
                                                groupBox2.Visible = false;
                        */
                        Is_count4 = true;
                        count4 = 60;
                    }));
                }
            }
            if ((Tcp_socket5 != null) && (Tcp_stream5 != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream5.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line5].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line5].Cells[7].Value = "模块异常";
                    Tcp_stream5.Close();
                    Tcp_stream5 = null;
                    Tcp_socket5.Close();
                    Tcp_socket5 = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, 20);
                        if (read.StartsWith("HTTP/1.1 200 OK"))
                        {
                            this.dataGVscan.Rows[socket_line5].Cells[7].Style.BackColor = Color.PaleGreen;
                            this.dataGVscan.Rows[socket_line5].Cells[7].Value = "升级成功，开始加载";
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line5].Cells[7].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line5].Cells[7].Value = "升级失败";
                        }
                        Tcp_stream5.Close();
                        Tcp_stream5 = null;
                        Tcp_socket5.Close();
                        Tcp_socket5 = null;
                        timerload.Enabled = true;
                        /*                        label4.Visible = true;
                                                pictureBox1.Visible = true;
                                                pictureBox2.Visible = true;
                                                groupBox2.Visible = false;
                        */
                        Is_count5 = true;
                        count5 = 60;
                    }));
                }
            }
            if ((Tcp_socket6 != null) && (Tcp_stream6 != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream6.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line6].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line6].Cells[7].Value = "模块异常";
                    Tcp_stream6.Close();
                    Tcp_stream6 = null;
                    Tcp_socket6.Close();
                    Tcp_socket6 = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, 20);
                        if (read.StartsWith("HTTP/1.1 200 OK"))
                        {
                            this.dataGVscan.Rows[socket_line6].Cells[7].Style.BackColor = Color.PaleGreen;
                            this.dataGVscan.Rows[socket_line6].Cells[7].Value = "升级成功，开始加载";
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line6].Cells[7].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line6].Cells[7].Value = "升级失败";
                        }
                        Tcp_stream6.Close();
                        Tcp_stream6 = null;
                        Tcp_socket6.Close();
                        Tcp_socket6 = null;
                        timerload.Enabled = true;
                        /*                        label4.Visible = true;
                                                pictureBox1.Visible = true;
                                                pictureBox2.Visible = true;
                                                groupBox2.Visible = false;
                        */
                        Is_count6 = true;
                        count6 = 60;
                    }));
                }
            }
            if ((Tcp_socket7 != null) && (Tcp_stream7 != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream7.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line7].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line7].Cells[7].Value = "模块异常";
                    Tcp_stream7.Close();
                    Tcp_stream7 = null;
                    Tcp_socket7.Close();
                    Tcp_socket7 = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, 20);
                        if (read.StartsWith("HTTP/1.1 200 OK"))
                        {
                            this.dataGVscan.Rows[socket_line7].Cells[7].Style.BackColor = Color.PaleGreen;
                            this.dataGVscan.Rows[socket_line7].Cells[7].Value = "升级成功，开始加载";
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line7].Cells[7].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line7].Cells[7].Value = "升级失败";
                        }
                        Tcp_stream7.Close();
                        Tcp_stream7 = null;
                        Tcp_socket7.Close();
                        Tcp_socket7 = null;
                        timerload.Enabled = true;
                        /*                        label4.Visible = true;
                                                pictureBox1.Visible = true;
                                                pictureBox2.Visible = true;
                                                groupBox2.Visible = false;
                        */
                        Is_count7 = true;
                        count7 = 60;
                    }));
                }
            }
            if ((Tcp_socket8 != null) && (Tcp_stream8 != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream8.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line8].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line8].Cells[7].Value = "模块异常";
                    Tcp_stream8.Close();
                    Tcp_stream8 = null;
                    Tcp_socket8.Close();
                    Tcp_socket8 = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, 20);
                        if (read.StartsWith("HTTP/1.1 200 OK"))
                        {
                            this.dataGVscan.Rows[socket_line8].Cells[7].Style.BackColor = Color.PaleGreen;
                            this.dataGVscan.Rows[socket_line8].Cells[7].Value = "升级成功，开始加载";
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line8].Cells[7].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line8].Cells[7].Value = "升级失败";
                        }
                        Tcp_stream8.Close();
                        Tcp_stream8 = null;
                        Tcp_socket8.Close();
                        Tcp_socket8 = null;
                        timerload.Enabled = true;
                        /*                        label4.Visible = true;
                                                pictureBox1.Visible = true;
                                                pictureBox2.Visible = true;
                                                groupBox2.Visible = false;
                        */
                        Is_count8 = true;
                        count8 = 60;
                    }));
                }
            }
            if ((Tcp_socket9 != null) && (Tcp_stream9 != null))
            {
                byte[] buf = new byte[2000];
                Int32 bytes = 0;
                try
                {
                    bytes = Tcp_stream9.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    this.dataGVscan.Rows[socket_line9].Cells[7].Style.BackColor = Color.Red;
                    dataGVscan.Rows[socket_line9].Cells[7].Value = "模块异常";
                    Tcp_stream9.Close();
                    Tcp_stream9 = null;
                    Tcp_socket9.Close();
                    Tcp_socket9 = null;
                }
                if (bytes > 0)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        string read = System.Text.Encoding.UTF8.GetString(buf, 0, 20);
                        if (read.StartsWith("HTTP/1.1 200 OK"))
                        {
                            this.dataGVscan.Rows[socket_line9].Cells[7].Style.BackColor = Color.PaleGreen;
                            this.dataGVscan.Rows[socket_line9].Cells[7].Value = "升级成功，开始加载";
                        }
                        else
                        {
                            this.dataGVscan.Rows[socket_line9].Cells[7].Style.BackColor = Color.Red;
                            this.dataGVscan.Rows[socket_line9].Cells[7].Value = "升级失败";
                        }
                        Tcp_stream9.Close();
                        Tcp_stream9 = null;
                        Tcp_socket9.Close();
                        Tcp_socket9 = null;
                        timerload.Enabled = true;
                        /*                        label4.Visible = true;
                                                pictureBox1.Visible = true;
                                                pictureBox2.Visible = true;
                                                groupBox2.Visible = false;
                        */
                        Is_count9 = true;
                        count9 = 60;
                    }));
                }
            }
        }
        /*********************************************************************************************************
        ** 功能说明：UDP单播接收数据线程
        *********************************************************************************************************/
        void Thread_TCP_Thread()
        {
            while (true)
            {
                Socket_Read();
            }
        }

        private void dataGVscan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGVscan.CurrentRow.Cells[5].EditedFormattedValue.ToString() == "True")
            {
                this.dataGVscan.CurrentRow.Cells[5].Value = false;
            }
            else
            {
                for (int i = 0; i < this.dataGVscan.RowCount; i++)
                {
                    this.dataGVscan.Rows[i].Cells[5].Value = false;
                }
                this.dataGVscan.CurrentRow.Cells[5].Value = true;

            }
        }

       /*****************************************************************
        **指定播放
        *****************************************************************/
        private VlcPlayer vlc_player_1;
        private VlcPlayer vlc_player_2;
        private bool is_playinig_1;
        private bool is_playinig_2;

        public void vlc_init(bool is_record)
        {
            string pluginPath = System.Environment.CurrentDirectory + "\\plugins\\";
            vlc_player_1 = new VlcPlayer(pluginPath, is_record);
            vlc_player_2 = new VlcPlayer(pluginPath, is_record);
            IntPtr render_wnd1 = this.panel1.Handle; //this.panel1.Handle;
            vlc_player_1.SetRenderWindow((int)render_wnd1);
            is_playinig_1 = false;
            IntPtr render_wnd2 = this.panel3.Handle; 
            vlc_player_2.SetRenderWindow((int)render_wnd2);
            is_playinig_2 = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                vlc_player_1.PlayFile(ofd.FileName);
                is_playinig_1 = true;
            }
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                vlc_player_2.PlayFile(ofd.FileName);
                is_playinig_2 = true;
            }
        }
       /********************************************************
        ********关闭播放窗口
        *******************************************************/
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (is_playinig_1)
            {
                vlc_player_1.Stop();
                is_playinig_1 = false;
            }
        }

        private void btnReset_2_Click(object sender, System.EventArgs e)
        {
            if (is_playinig_2)
            {
                vlc_player_2.Stop();
                is_playinig_2 = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //string version1 = vlc_player_1.Version();
           //textBox1.Text = version1;
           string rtsp1 = textBox1.Text;//"rtsp://admin:admin@192.168.1.116/cam1/h264 :network-caching=450";// "rtsp://192.168.0.45/ip7";//"rtsp://192.168.0.45/ip7";//  :network-caching=3000
            vlc_player_1.PlayFile_rtsp(rtsp1);
            is_playinig_1 = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //string version2 = vlc_player_2.Version();
            //textBox2.Text = version2;
            string rtsp2 = textBox2.Text;//"rtsp://admin:admin@192.168.1.116/cam1/h264 :network-caching=450";//textBox1.Text;// "rtsp://192.168.0.45/ip7";//"rtsp://192.168.0.45/ip7";//  :network-caching=3000
            vlc_player_2.PlayFile_rtsp(rtsp2);
            is_playinig_2 = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (is_playinig_1 == true)
            vlc_player_1.Stop();
            vlc_player_1.Vlc_release();
            vlc_player_2.Vlc_release();
            if (is_playinig_2 == true)
            vlc_player_2.Stop();
            vlc_player_2.Vlc_release();
        
        }
        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Console.WriteLine("mouse double click");
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            string admin_data = "user_name=" + textBoxadmin.Text + "&user_password=" + textBoxpsk.Text;//整个认证信息--字符串
                byte[] admin = new byte[admin_data.Length];
                //ver_num = 0;
                admin = ASCIIEncoding.ASCII.GetBytes(admin_data); //整个认证信息--字节数组
                string param = "<request><param1></param1></request>";
                string method = "GET";
                byte[] barray = Encoding.Default.GetBytes(textBoxadmin.Text + ":" + textBoxpsk.Text);
                basic = "Authorization: Basic " + Convert.ToBase64String(barray);
                Action<HttpStatusCode, string> onComplete = null;
                for (int i = 0; i < dataGVscan.RowCount; i++)
                {
                    if (dataGVscan.Rows[i].Cells[1].Value != null)
                    {
                        string ip = dataGVscan.Rows[i].Cells[1].Value.ToString();//获取目标IP地址
                        //LTSP_CMD(0xF, admin, IPAddress.Parse(ip)); //发送认证信息 
                        /*************************************************
                         * ***重启模块
                         * ************************************************/
                        string url0 = ip + "/restart.cgi";
                        string rcv0= HTTP.Request(method, url0, basic, param, onComplete);
                    }
                }
        }

      
      
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace RAK420_Config_Tool
{  
    
    public class RAK420
    {

        //扫描获取RAK420模块的信息
        /// 模块名称   16个字节
        public byte[] NickName = new byte[16];
        /// 组名称     16个字节
        public byte[] GroupName = new byte[16];
        /// IP地址     4个字节
        public byte[] IP = new byte[4];
        /// MAC地址    6个字节
        public string MAC = null;
        /// 信号强度   1个字节
        public int RSSI;          
    }
}

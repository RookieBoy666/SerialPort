using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SerialPortDemo
{
    public partial class Form1 : Form
    {
        //private SerialPort Comm;

         private SerialPort com;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            SerialPort com = new SerialPort();
            //检查是否含有串口 
            string[] str = SerialPort.GetPortNames();
            if (str == null)
            {
                MessageBox.Show("本机没有串口！", "Error");
                return;
            }

            //添加串口项目
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {//获取有多少个COM口

                cmbPort.Items.Add(s);
            }
            cmbPort.SelectedIndex = 0;//默认选择第一个COM1

            com.PortName = cmbPort.SelectedItem.ToString();
           // com.PortName //= serialName;
            //  com = new SerialPort("COM1");       //实例化SerialPort并设置COM口

            com.BaudRate = 9600;//波特率
            //cmbBaudRate.
            com.Parity = Parity.None;//无奇偶校验位
            com.StopBits = StopBits.Two;//两个停止位
            com.Handshake = Handshake.RequestToSend;//控制协议
            com.ReceivedBytesThreshold = 13;//设置 DataReceived 事件发生前内部输入缓冲区中的字节数,我这里是13字节为一组
            com.Open();                 //打开串口  
            com.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);           //接受数据线程


        }

        /// <summary>
        /// 监听串口数据线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);//线程休眠500毫秒，方便接收串口的全部数据
            try
            {
                if (com.IsOpen)
                {
                    byte[] readBuffer = new byte[com.ReadBufferSize + 1];
                    try
                    {
                        int count = com.Read(readBuffer, 0, com.ReadBufferSize);        //读取串口数据(监听)
                        String SerialIn = System.Text.Encoding.ASCII.GetString(readBuffer, 0, count);//将字节数组解码为字符串
                        if (count != 0)
                        {
                            //这里强调一下,线程里不可以直接对UI进行赋值，只能使用委托操作控件
                            this.BeginInvoke(new System.Threading.ThreadStart(delegate ()
                            {
                                richTextBox1.Text = SerialIn;
                            }));

                        }
                    }
                    catch (TimeoutException) { }
                }
                else
                {
                    TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 50);
                    Thread.Sleep(waitTime);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

 
         
    }
}

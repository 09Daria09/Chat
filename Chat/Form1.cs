using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat
{
    public partial class Form1 : Form
    {
        static int RemotePort;
        static int LocalPort;
        static IPAddress RemoteIPAddr;
        private SynchronizationContext uiContext;
        public Form1()
        {
            InitializeComponent();
            Thread thread = new Thread(
                   new ThreadStart(ThreadFuncReceive)
            );
            //create a background thread
            thread.IsBackground = true;
            //start the thread
            thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox5.Text))
            {
                SendData(textBox1.Text);
                lock (listBox1)
                {
                    listBox1.Items.Add("Вы: " + textBox5.Text);
                }
                textBox5.Text = string.Empty;
            }
        }
        private void ThreadFuncReceive()
        {
            UdpClient uClient = new UdpClient(LocalPort);
            try
            {
                while (true)
                {
                    //connection to the local host
                    IPEndPoint ipEnd = null;
                    //receiving datagramm
                    byte[] responce = uClient.Receive(ref ipEnd);
                    //conversion to a string
                    string strResult = Encoding.Unicode.GetString(responce);
                    if (strResult != null)
                        uiContext.Send(d =>
                        {
                            lock (listBox1)
                            {
                                listBox1.Items.Add("Клиент: " + strResult);
                            }
                        }, null);
                }
            }
            catch (SocketException sockEx)
            {
                Console.WriteLine("Socket exception: " + sockEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.Message);
            }
            finally
            {
                uClient.Close();
            }
        }
        static void SendData(string datagramm)
        {
            UdpClient uClient = new UdpClient();
            //connecting to a remote host
            IPEndPoint ipEnd = new IPEndPoint(RemoteIPAddr, RemotePort);
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(datagramm);
                uClient.Send(bytes, bytes.Length, ipEnd);
            }
            catch (SocketException sockEx)
            {
                MessageBox.Show("Socket exception: " + sockEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception : " + ex.Message);
            }
            finally
            {
                //close the UdpClient class instance
                uClient.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            uiContext = SynchronizationContext.Current;
            RemoteIPAddr = IPAddress.Parse(textBox1.Text);
            RemotePort = Convert.ToInt16(textBox2.Text);
            LocalPort = Convert.ToInt16(textBox3.Text);
        }

    }

}

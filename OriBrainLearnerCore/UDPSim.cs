using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data; 
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Timers;
using OriBrainLearnerCore;

namespace UDPWrapper
{
    /// <summary>
    /// this class simulates the remote unity end, it will periodically send a filename every X milliseconds.
    /// it will always send to localhost 127.0.0.1 on port 170
    /// </summary>
    public class UDPSim
    {
        
        private string text;

        //socket
        private Socket s;

        //ipaddress
        private IPAddress broadcast;

        //end point
        private IPEndPoint ep;

        System.Timers.Timer t = null; 

        string IP = "127.0.0.1";//"194.199.227.234";
        public int portNumber = 170;
        private IPEndPoint remoteEndPoint;
        private UdpClient client;
        bool _shouldStop = false;

        public UDPSim()
        {
            prepareUDPSend();
        }

        private void SendSingleText(string Text)
        {
            // send data

            //socket
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); 

            //ipaddress
            IPAddress broadcast = IPAddress.Parse(IP);

            //send buffer
            byte[] sendbuf = Encoding.ASCII.GetBytes(Text);

            //end point
            IPEndPoint ep = new IPEndPoint(broadcast, Convert.ToInt32(Text));


            //send
            s.SendTo(sendbuf, ep);
        } 
         
         
        void initUDPListener()
        {
            // listen = receive

            //end point
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), this.portNumber);

            //client
            client = new UdpClient(portNumber);

            //receive
            client.BeginReceive(new AsyncCallback(reciveData), null);
        }


        private void reciveData(IAsyncResult ar)
        {
            byte[] data = client.EndReceive(ar, ref remoteEndPoint);
            if (data.Length > 0)
            {
                text = Encoding.ASCII.GetString(data);
                GuiPreferences.Instance.setLog(text);
            }

            if (!_shouldStop)
                client.BeginReceive(new AsyncCallback(reciveData), null);
        }  


        private void UDPCloseConnection()
        { 
            client.Close();
        }

        /// <summary>
        /// udp send loop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        int _maxFiles;
        string _workDir;
        string _fileName;
        string _Filetype;
        int indexFiles = 0;

        public bool isWorking()
        {
            return (t != null);
        }

        public void StopLoop()
        {
            if (t != null)
            {
                text = "pausing loop server";
                GuiPreferences.Instance.setLog(text);
                t.Stop();
                t = null;
            }
        }

        public void SendLoop()
        {
            if (t == null)
            { 
                text = "starting loop server";
                _maxFiles = Preferences.Instance.events.EventListLastTr;
                _workDir = GuiPreferences.Instance.WorkDirectory;
                _fileName = GuiPreferences.Instance.FileName;
                _Filetype = ".vdat";
                GuiPreferences.Instance.setLog(text);
                t = new System.Timers.Timer(500);
                t.Elapsed += new ElapsedEventHandler(t_Elapsed);
                t.Start();
            }
            else
            {
                StopLoop();
            }
        }


        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (indexFiles < _maxFiles)
            {
                indexFiles++;
                string fn = _workDir + _fileName + indexFiles.ToString() + _Filetype + "," + _maxFiles.ToString();
                sendData(fn);  
            }
            else
            { 
                text = "stopping loop server";
                GuiPreferences.Instance.setLog(text);
                indexFiles = 0;
                if (t!=null)
                {
                    t.Stop();
                    t = null; 
                }
            }
        } 

        /// <summary>
        /// when the application loads we init the socket / IP / PORT
        /// </summary>
        public void prepareUDPSend()
        {
            //socket
            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //ipaddress
            broadcast = IPAddress.Parse("127.0.0.1");

            //end point
            ep = new IPEndPoint(broadcast, Convert.ToInt32(170));
        }

        public void sendData(string str)
        {  
            //convert input string to byte array: send buffer
            byte[] sendbuf = Encoding.ASCII.GetBytes(str);
            text = str;
            //GuiPreferences.Instance.setLog(text);

            //send input string as byte array using socket 's' to the endPoint that was predefined in prepareUDPSend()
            s.SendTo(sendbuf, ep);
        }

        private void ResetIndex(object sender, EventArgs e)
        { 
            indexFiles = 0;
        }
         
    }

}

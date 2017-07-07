using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.ComponentModel;
using System.Text;

namespace UDPWrapper
{
    public class UDP
    {

        // ip address for communication with OBL
        //public  string IP = "127.0.0.1";
        //public  int OBLportNumber = 170;
        //public  int UnityPortNumber = 171;
        public string msg = "hi, testing 123!";

        //private string IPstatic;
        private string LocalIPstatic;
        private string RemoteIPstatic;
        private int LocalPortNumberStatic; //OBL
        public int localPortNumberStatic
        {
            get { return LocalPortNumberStatic; }
            set { LocalPortNumberStatic = value; }
        }
        private int RemotePortNumberStatic;
        private IPEndPoint LocalEndPoint; //OBL
        private IPEndPoint RemoteEndPoint;
        private UdpClient client;
        private Socket SendSocket;
        private AsyncCallback callBackFunction;

        // Use this for initialization, commented out for using setIP and setPort
        /*public  void assignStaticVariables()
        { 
            IPstatic = IP;
            OBLportNumberStatic = OBLportNumber;
            UnityportNumberStatic = UnityPortNumber; 
        }*/

        /// <summary>
        /// sets our own port/ip for listening
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="port"></param>
        public void setLocal(string IP, int port)
        {
            LocalIPstatic = IP;
            LocalPortNumberStatic = port;
        } 

        /// <summary>
        /// sets the remove unity's end
        /// </summary>
        /// <param name="IP"></param>
        public void setRemote(string IP, int port)
        {
            RemoteIPstatic = IP;
            RemotePortNumberStatic = port;
        }
        
        public  void initUdpSocket()
        { 
            //client channel - sending results through port 170 to unity
            SendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            LocalEndPoint = new IPEndPoint(IPAddress.Parse(RemoteIPstatic), RemotePortNumberStatic);

            SendUdpData("ori");
            //server - we host a server to recieve on port 171 the file named from unity result
            RemoteEndPoint = new IPEndPoint(IPAddress.Parse(LocalIPstatic), LocalPortNumberStatic);
            //if port busy (i.e., another application is using this port, advance port by 1)
            try
            {
                client = new UdpClient(LocalPortNumberStatic);
            }
            catch
            {
                LocalPortNumberStatic += 1;
                client = new UdpClient(LocalPortNumberStatic); 
            }
            //client.BeginReceive(new AsyncCallback(reciveData), null); 
        }

        public  void  RegisterCallBack(AsyncCallback function)
        {
            callBackFunction = function;
            BeginReceive();
        }

        public  void SendUdpData(string msg)
        {
            byte[] sendbuf = Encoding.ASCII.GetBytes(msg);
            SendSocket.SendTo(sendbuf, LocalEndPoint);
        }

        public  byte[] EndReceive(IAsyncResult ar)
        {
            return client.EndReceive(ar, ref RemoteEndPoint);
        }

        public  void BeginReceive()
        {
            client.BeginReceive(new AsyncCallback(callBackFunction), null);
        }

        //not used, the right function is in binary file reader (to use uncomment shouldstop)
        public  void reciveData(IAsyncResult ar)
        {
            //if (_shouldStop)
            //return;
            byte[] data = EndReceive(ar);
            if (data.Length > 0)
            {
                string text = Encoding.ASCII.GetString(data);
                //updategui();
                // shoot event 
                //send the recieved data somewhere 
            }

            BeginReceive();
            //if(!_shouldStop)
        }


        public void Stop()
        {
            if (client != null)
            {
                client.Close();
            }
        }
    }

}



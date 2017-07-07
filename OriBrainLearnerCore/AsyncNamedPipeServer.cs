using System;
using System.IO.Pipes; 

/// The officially used async pipe server used in OBL
namespace OriBrainLearnerCore
{

    // Delegate for passing received message back to caller
    public delegate void DelegateMessage(string Reply);
    public delegate void DelegateBuffer(byte[] data);
    public delegate void Instance_DataUpdateEventDelegate(object sender, DataChangeEventArg t); 
    //this eventartgs class is the only way to pass several variables into an event
    public class DataChangeEventArg : EventArgs
    {
        private byte[] dt;
        public byte[] _data
        {
            get
            {
                return dt;
            }
            private set
            {
                //this is a trick, the first initialization will just create an empty array. 
                //array.copy uses get() first and we need it to return a valid size array and not null.
                if (dt==null)
                {
                    dt = new byte[value.Length];
                    return;
                }
                dt = value;
            }
        }
        public int _currentTR
        {
            get;
            private set;
        } 
        public int _totalTRs
        {
            get;
            private set;
        } 
        public int _x
        {
            get;
            private set;
        } 
        public int _y
        {
            get;
            private set;
        } 
        public int _z
        {
            get;
            private set;
        }

        public DataChangeEventArg(int x, int y, int z, byte[] data, int current, int total)
        {
            _x = x;
            _y = y;
            _z = z;
            //this next line is a trick to initialize 'dt' inside the class above with enough elements, there is no assignment when dt is null.
            _data = data;
            //and only then COPY can get() a non null variable _data and copy all the data into it.
            Array.Copy(data, _data, data.Length);
            _currentTR = current;
            _totalTRs = total;
        }
    }

    public class AsyncNamedPipeServer
    {
        private int MaxPipes = 5;
        public event DelegateMessage PipeMessage;
        public event DelegateBuffer PipeData;
        public event EventHandler<DataChangeEventArg> DataUpdateEvent;
        private void onDataUpdate(int x, int y, int z, byte[] data, int current, int total)
        {
            //handler accepts all the registered clients to this "TrUpdateEvent" event
            EventHandler<DataChangeEventArg> handler = DataUpdateEvent;
            if (handler != null)
            {
                //the real shoot (replaces delegate.invoke(), operates all the registered functions in this event.
                handler(this, new DataChangeEventArg(x, y, z, data, current, total));
            }
        }
        string _pipeName;
        int currentTimePoint;
        int totalTimePoints;
        DateTime d;

        public bool isEmpty()
        {

            var subscribers = DataUpdateEvent.GetInvocationList();
            if (subscribers.Length == 0) 
                return true;
            else 
                return false;

        }

        public AsyncNamedPipeServer()
        {
            d = DateTime.Now;
        }

        public void Listen(string PipeName)
        {
            bool pipeCreated = false;
            int pipeID = 0;
            NamedPipeServerStream pipeServer = null;
            while (!pipeCreated)
            {
                // Set to class level var so we can re-use in the async callback method 
                _pipeName = PipeName + pipeID.ToString();

                if (pipeID > MaxPipes)
                {
                    PipeMessage.Invoke("Pipes failed to find an open pipe up to #5");
                    return;
                }

                try
                {   
                    // Create the new async pipe 
                    pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1
                                                                                 /*NamedPipeServerStream.MaxAllowedServerInstances*/,
                                                                                 PipeTransmissionMode.Byte,
                                                                                 PipeOptions.Asynchronous); 
                    pipeCreated = true;
                }
                catch (Exception oEX)
                {
                    PipeMessage.Invoke(oEX.Message);
                }

                if (!pipeCreated)
                    pipeID++; 
            }

            if (pipeCreated)
            {// Wait for a connection
                    pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);
                    PipeMessage.Invoke("Pipe Created: " + _pipeName);
            }
        }

        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                NamedPipeServerStream pipeServer = (NamedPipeServerStream)iar.AsyncState;
                // End waiting for the connection
                pipeServer.EndWaitForConnection(iar);

                int x,y,z;
                byte[] intBuffer = new byte[sizeof(int)];  
                
                // Read the incoming message x,y,z,buffer
                int res = pipeServer.Read(intBuffer, 0, 4);  
                x = BitConverter.ToInt32(intBuffer, 0);
                 
                res = pipeServer.Read(intBuffer, 0, 4); 
                y = BitConverter.ToInt32(intBuffer, 0);

                res = pipeServer.Read(intBuffer, 0, 4); 
                z = BitConverter.ToInt32(intBuffer, 0); 

                int buffer_size = x * y * z * sizeof(byte) * 2;
                byte[] buffer = new byte[buffer_size];

                res = pipeServer.Read(buffer, 0, buffer_size);
                 
                res = pipeServer.Read(intBuffer, 0, 4);
                currentTimePoint = BitConverter.ToInt32(intBuffer, 0);
                
                res = pipeServer.Read(intBuffer, 0, 4);
                totalTimePoints = BitConverter.ToInt32(intBuffer, 0); 

                // Convert byte buffer BINARY DATA to string (TBV)
                string stringData = (DateTime.Now - d).ToString()
                    + " " + x.ToString() + " " + y.ToString() + " " + z.ToString() + " "
                    + currentTimePoint.ToString() + " "
                    + totalTimePoints.ToString() + " " 
                    + res.ToString() + " / " + buffer[0].ToString() + " " + buffer[2].ToString() 
                    + " " + buffer[4].ToString() + " " + Environment.NewLine;                

                d = DateTime.Now;

                // Convert byte buffer to string
                //string stringData = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                // Pass message back to calling form
                PipeMessage.Invoke(stringData);
                onDataUpdate(x, y, z, buffer, currentTimePoint, totalTimePoints);
                //PipeData.Invoke(buffer);

                // Kill original sever and create new wait server
                pipeServer.Close();
                pipeServer = null;
                pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                

                // Recursively wait for the connection again and again....
                pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);
            }
            catch (Exception oEX)
            {
                PipeMessage.Invoke(oEX.Message); 
                return;
            }
        }
    }
}

 
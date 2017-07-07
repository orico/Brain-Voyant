using System;
using AsyncPipes;
using Messages; 

namespace OriBrainLearnerCore
{
    
    //NOTE: To connect to a server on another machine, use the following syntax "\\machineName\\pipeName"
    //  private AsyncPipes.NamedPipeStreamClient pipeClient = new NamedPipeStreamClient("\\otherconputername\\test2");
     
    public class NamedPipeServer
    {
        private readonly string PipeName;        
        private AsyncPipes.NamedPipeStreamServer pipeServer; 

        public NamedPipeServer(string pipename)
        {               
            PipeName = pipename;
            Start();
        }
         

        private void Start()
        {
            pipeServer = new AsyncPipes.NamedPipeStreamServer(PipeName);
           // registerEvent(pipeServer_MessageReceived);
        }

        public void registerEvent(MessageEventHandler handler)
        {
            pipeServer.MessageReceived += handler;
        }

        public void unregisterEvent(MessageEventHandler handler)
        {
            pipeServer.MessageReceived -= handler;
        }

        private string filename;

        void pipeServer_MessageReceived(object sender, MessageEventArgs args)
        {
            IMessage mess = AsyncPipes.MessageSerializers.DeserializeMessage(args.Message);
            if (mess.MessageType == typeof(String))
            {
                filename = System.Text.Encoding.UTF8.GetString(mess.Payload);
            }
            /*else
            {
                //BinaryFormatter formatter = new BinaryFormatter();
                //MemoryStream ms = new MemoryStream(mess.Payload);
                //dataset conversion from ms
                //ds = (DataSet)formatter.Deserialize(ms);                
            } */
        }

        void stop()
        {
            pipeServer.Dispose();
        }

        ~NamedPipeServer()
        { 
            //pipeServer.Dispose();
        }


    }

}
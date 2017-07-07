using System;
using AsyncPipes;
using Messages; 

namespace OriBrainLearnerCore
{
    public class NamedPipeClient    
    {
        private readonly string PipeName;
        private AsyncPipes.NamedPipeStreamClient pipeClient; 

        public NamedPipeClient(string pipename)
        {               
            PipeName = pipename;
            Start();
        } 

        private void Start()
        {
            pipeClient = new AsyncPipes.NamedPipeStreamClient(PipeName);
            //pipeClient.MessageReceived += new MessageEventHandler(pipeClient_MessageReceived);
        } 
       
        //used to send file names with complete path so that the server may load them and process them.
        public void Send(string data)
        {
            GenericMessage m = new GenericMessage();
            m.MessageId = Guid.NewGuid();
            m.Recipient = "VS";
            m.Originator = "Unity";
            m.MessageType = typeof (String);
            m.MessageDateTime = DateTime.Now;
            m.Payload = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] b=  MessageSerializers.SerializeMessage(m);
            pipeClient.SendMessage(b); 

            /*else if((string) cboMessageType.SelectedItem == "DataSet")
            {
                GenericMessage m = new GenericMessage();
                m.MessageId = Guid.NewGuid();
                m.Recipient = "AppTwo";
                m.Originator = "AppOne";
                m.MessageType = typeof(System.Data.DataSet );
                m.MessageDateTime = DateTime.Now;
                byte[] dsBytes = MessageSerializers.SerializeObject(ds);
                m.Payload = dsBytes;
               
                byte[] b = MessageSerializers.SerializeMessage(m);
                pipeClient.SendMessage(b);
            }*/
         }
        
        /// <summary>
        /// NOT SURE FOR WHAT THIS FUNCTION IS USED FOR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void pipeClient_MessageReceived(object sender, MessageEventArgs args)
        {
           
            /*IMessage mess = AsyncPipes.MessageSerializers.DeserializeMessage(args.Message);
            string displayMessage = "ID: " + mess.MessageId.ToString() + "\r\n Originator: " + mess.Originator +
                                       "\r\n";

            displayMessage += "Recipient: " + mess.Recipient + "\r\nType: " + mess.MessageType.ToString() + "\r\n";
            displayMessage += "Payload: " + System.Text.Encoding.UTF8.GetString(mess.Payload);
            SetControlProperty(this.txtReceive, "Text", displayMessage);
            if (mess.MessageType == typeof(String))
           
           if (mess.MessageType == typeof(DataSet))
            {
                ds = (DataSet)MessageSerializers.DeserializeObject(mess.Payload);
                SetControlProperty(this.dataGridView1, "DataSource", ds.Tables[0]);
            }*/
        }

        void stop()
        {
            pipeClient.Dispose();
        }

        ~NamedPipeClient()
        {
            //pipeServer.Dispose();
        } 
    }
}

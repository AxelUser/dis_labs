using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace MessageEncryptionService.Handlers.Connections.Sockets
{
    public class SocketServer : IServerConnection
    {
        public event EventHandler<MessageModel> NewMessage;
        private TcpListener listener;
        private int maxConnections;
        private Thread listeningThread;
        private bool proceed;
        private List<string> history;

        public SocketServer(string domain, int port, int maxConnections = 10)
        {
            this.maxConnections = maxConnections;
            history = new List<string>();
            IPAddress ipAdress = Dns.GetHostAddresses(domain).First();
            listener = new TcpListener(ipAdress, port);
        }

        public MessageModel ReceiveNewMessage()
        {
            throw new NotImplementedException();
        }

        public void SendRSAKey()
        {
            throw new NotImplementedException();
        }

        public void StartServer()
        {
            listener.Start(maxConnections);
            proceed = true;
            listeningThread = new Thread(new ThreadStart(Listen));
            listeningThread.Start();
        }

        public void StopServer()
        {
            proceed = false;
            while(listeningThread.ThreadState == ThreadState.Running)
            {
                Thread.Sleep(5000);
            }
            listener.Stop();
        }

        private void Listen()
        {
            while (proceed)
            {
                TcpClient client = null;
                try
                {
                    client = listener.AcceptTcpClient();
                    NetworkStream socketStream = client.GetStream();
                    using (BinaryReader reader = new BinaryReader(socketStream))
                    {
                        var msg = reader.ReadString();
                        history.Add(msg);
                    }
                    using (BinaryWriter writer = new BinaryWriter(socketStream))
                    {
                        writer.Write("Сообщение получено.");
                    }
                }
                finally
                {
                    client.Close();
                }
            }
        }
    }
}

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
    public class SocketClient : IClientConnection
    {
        private TcpClient client;
        private IPAddress ipAdress;
        CancellationTokenSource cts;
        int port;

        Task checkingConnectionTask;

        public event EventHandler<string> ConnectionErrorRised;

        public SocketClient(string ip, int port)
        {
            ipAdress = IPAddress.Parse(ip);
            var s = ipAdress.ToString();
            client = new TcpClient();
            this.port = port;
            cts = new CancellationTokenSource();
        }

        public bool Connect()
        {
            bool connected;
            try
            {                
                client.Connect(ipAdress, port);                
                connected = true;
            }
            catch
            {
                connected = false;
            }
            if (connected)
            {
                CancellationToken ct = cts.Token;
                StartCheckingConnection(ct);
            }
            return connected;
        }

        public bool CheckConnection()
        {
            bool connected = false;
            if (client.Connected && client.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] buff = new byte[1];
                if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
                {
                    connected = false;
                }
                else
                {
                    connected = true;
                }
            }
            return connected;
        }        

        public MessageModel Send(MessageModel message)
        {
            MessageModel reply = null;
            if (CheckConnection())
            {
                var socketStream = client.GetStream();
                {
                    BinaryWriter writer = null;
                    BinaryReader reader = null;
                    try
                    {
                        writer = new BinaryWriter(socketStream, Encoding.UTF8, true);
                        reader = new BinaryReader(socketStream, Encoding.UTF8, true);
                        string data = message.Body;
                        writer.Write(data);
                        writer.Flush();
                        reply = new MessageModel(Types.MessageTypes.Reply)
                        {
                            Body = reader.ReadString()
                        };
                    }
                    finally
                    {
                        writer.Close();
                        reader.Close();
                    }
                }
            }
            return reply;
        }


        private void StartCheckingConnection(CancellationToken ct)
        {
            IProgress<string> progressHandler = new Progress<string>((message) => 
            {
                ConnectionErrorRised?.Invoke(this, message);
            });
            checkingConnectionTask = new Task(() => 
            {
                while (!ct.IsCancellationRequested)
                {
                    if (!CheckConnection())
                    {
                        progressHandler.Report("Нет соединения с сервером.");
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            });
        }

        public void Disconnect()
        {
            cts.Cancel();
            checkingConnectionTask.Wait(TimeSpan.FromSeconds(10));
        }
    }
}

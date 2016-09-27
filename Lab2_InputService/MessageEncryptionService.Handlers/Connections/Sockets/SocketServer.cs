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
        private Task listening;
        private CancellationTokenSource cancelSource;
        private List<string> history;

        public SocketServer(string domain, int port, int maxConnections = 10)
        {
            this.maxConnections = maxConnections;
            history = new List<string>();
            cancelSource = new CancellationTokenSource();
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
            CancellationToken ct = cancelSource.Token;

            var progressHandler = new Progress<MessageModel>(value =>
            {
                NewMessage?.Invoke(this, value);
            });
            listener.Start(maxConnections);

            listening = Listen(progressHandler, ct);
            listening.Start();
        }

        public void StopServer()
        {
            cancelSource.Cancel();
            listening.Wait();
            listener.Stop();
        }

        private Task Listen(IProgress<MessageModel> progress, CancellationToken ct)
        {
            Task listenigTask = new Task(() =>
            {
                while (!ct.IsCancellationRequested)
                {
                    using (TcpClient client = listener.AcceptTcpClient())
                    {
                        BinaryReader reader = null;
                        BinaryWriter writer = null;
                        NetworkStream socketStream = null;
                        try
                        {
                            socketStream = client.GetStream();
                            reader = new BinaryReader(socketStream);
                            writer = new BinaryWriter(socketStream);

                            var msgText = reader.ReadString();
                            history.Add(msgText);

                            MessageModel msg = new MessageModel(Types.MessageTypes.Reply)
                            {
                                Body = "Сообщение получено."
                            };
                            writer.Write(msg.Body);
                            writer.Flush();
                            progress.Report(msg);
                        }
                        finally
                        {
                            socketStream?.Close();
                            reader?.Close();
                            writer?.Close();
                        }
                    }
                }
            });
            return listenigTask;
        }
    }
}

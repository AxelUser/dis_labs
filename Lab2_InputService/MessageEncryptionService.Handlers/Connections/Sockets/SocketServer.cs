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
        private Task listeningInputConnections;        
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

            listeningInputConnections = Listen(progressHandler, ct);
            listeningInputConnections.Start();
        }

        public void StopServer()
        {
            cancelSource.Cancel();
            listeningInputConnections.Wait();
            listener.Stop();
        }

        private Task ListenNewConnections(TcpListener listener, CancellationToken ct)
        {
            Task listenInputConnections = new Task(() => 
            {
                while (!ct.IsCancellationRequested)
                {
                    TcpClient client = null;
                    var t = listener.AcceptTcpClientAsync();
                }
            });
            return listenInputConnections;
        }

        private Task Listen(IProgress<MessageModel> progress, CancellationToken ct, TcpClient clientToListen)
        {
            Task listenigTask = new Task(() =>
            {
                while (!ct.IsCancellationRequested)
                {                    
                    BinaryReader reader = null;
                    BinaryWriter writer = null;
                    NetworkStream socketStream = null;
                    try
                    {
                        socketStream = clientToListen.GetStream();
                        reader = new BinaryReader(socketStream, Encoding.UTF8, true);
                        writer = new BinaryWriter(socketStream, Encoding.UTF8, true);

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
                        reader?.Close();
                        writer?.Close();
                    }
                }
            });
            return listenigTask;
        }
    }
}

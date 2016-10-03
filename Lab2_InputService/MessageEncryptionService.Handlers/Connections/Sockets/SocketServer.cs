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
        private List<Task> activeConnectionListeners;

        public SocketServer(string domain, int port, int maxConnections = 10)
        {
            this.maxConnections = maxConnections;
            history = new List<string>();
            cancelSource = new CancellationTokenSource();
            IPAddress ipAdress = Dns.GetHostAddresses(domain).First();
            listener = new TcpListener(ipAdress, port);
            activeConnectionListeners = new List<Task>();
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

            listeningInputConnections = ListenNewConnections(progressHandler, ct, listener);
            listeningInputConnections.Start();
        }

        public void StopServer()
        {
            cancelSource.Cancel();
            listeningInputConnections.Wait();
            activeConnectionListeners.Clear();
            listener.Stop();
            
        }

        private Task ListenNewConnections(Progress<MessageModel> progressHandler, CancellationToken ct, TcpListener listener)
        {
            Task listenInputConnections = new Task(() => 
            {
                while (!ct.IsCancellationRequested)
                {
                    TcpClient client = null;
                    var getClient = listener.AcceptTcpClientAsync();
                    try
                    {
                        getClient.Wait(ct);
                    }
                    catch(Exception e)
                    {
                        //пока просто проигнорирую
                    }
                    finally
                    {
                        activeConnectionListeners.RemoveAll(t => t.IsCompleted);
                    }
                    if (getClient.IsCompleted)
                    {
                        client = getClient.Result;
                        Task handleClientTask = HandleClient(progressHandler, ct, client);
                        handleClientTask.Start();
                        activeConnectionListeners.Add(handleClientTask);
                    }
                }
                Task.WaitAll(activeConnectionListeners.ToArray());                
            });
            return listenInputConnections;
        }

        private Task HandleClient(IProgress<MessageModel> progress, CancellationToken ct, TcpClient clientToListen)
        {
            Task listeningTask = new Task(() =>
            {
                BinaryReader reader = null;
                BinaryWriter writer = null;
                NetworkStream socketStream = null;
                while (!ct.IsCancellationRequested)
                {                    

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
                socketStream.Close();
            });
            return listeningTask;
        }
    }
}

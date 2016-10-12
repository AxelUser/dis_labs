using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using MessageEncryptionService.Handlers.Connections.Messages;

namespace MessageEncryptionService.Handlers.Connections.Sockets
{
    public class SocketServer : ServerConnectionBase
    {
        public event EventHandler<MessageModel> NewMessage;

        private TcpListener listener;
        private int maxConnections;
        private Task listeningInputConnections;        
        private CancellationTokenSource cancelSource;
        private List<string> history;
        private Dictionary<Guid, Task> activeConnectionListeners;

        public SocketServer(string domain, int port, int maxConnections = 10)
        {
            this.maxConnections = maxConnections;
            history = new List<string>();
            cancelSource = new CancellationTokenSource();
            IPAddress ipAdress = Dns.GetHostAddresses(domain).First();
            listener = new TcpListener(ipAdress, port);
            activeConnectionListeners = new Dictionary<Guid, Task>();
        }

        public override MessageModel ReceiveNewMessage()
        {
            throw new NotImplementedException();
        }

        public override void StartServer()
        {
            CancellationToken ct = cancelSource.Token;

            var handleProgress = new Progress<MessageModel>(value =>
            {
                NewMessage?.Invoke(this, value);
            });

            var handleException = new Progress<Exception>(e => 
            {
                cancelSource.Cancel();
            });

            listener.Start(maxConnections);

            listeningInputConnections = ListenNewConnections(handleProgress, handleException, ct, listener);
            listeningInputConnections.Start();
        }

        public override void StopServer()
        {
            cancelSource.Cancel();
            listeningInputConnections.Wait();
            activeConnectionListeners.Clear();
            listener.Stop();
            
        }

        public void RegisterNewClient(Guid clientId, Task handler)
        {
            if (!activeConnectionListeners.ContainsKey(clientId))
            {
                activeConnectionListeners.Add(clientId, handler);
                handler.Start();
            }
        }

        public void CheckAndRepairClientId(Guid clientId, MessageModel messageFromClient)
        {
            if(clientId != messageFromClient.SenderId)
            {
                Task handler = activeConnectionListeners[clientId];
                activeConnectionListeners.Remove(clientId);
                activeConnectionListeners.Add(messageFromClient.SenderId, handler);
            }
        }
        private Task ListenNewConnections(IProgress<MessageModel> progressHandler, IProgress<Exception> exceptionHandler, CancellationToken ct, TcpListener listener)
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
                        var completedTaskIds = activeConnectionListeners
                            .Where(pair => pair.Value.IsCompleted)
                            .Select(pair => pair.Key)
                            .ToList();
                        foreach (var id in completedTaskIds)
                        {
                            activeConnectionListeners.Remove(id);
                        }
                    }
                    if (getClient.IsCompleted)
                    {
                        client = getClient.Result;
                        Task handleClientTask = HandleClient(progressHandler, exceptionHandler, ct, client);
                        
                    }
                }
                Task.WaitAll(activeConnectionListeners.Values.ToArray());                
            });
            return listenInputConnections;
        }

        private Task HandleClient(IProgress<MessageModel> progressHandler, IProgress<Exception> exceptionHandler, CancellationToken ct, TcpClient clientToListen)
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
                        progressHandler.Report(msg);
                    }
                    catch(Exception e)
                    {
                        exceptionHandler.Report(e);
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

        public override void DisconnectClient(Guid client)
        {
            throw new NotImplementedException();
        }

        public override void SendRSAKey(Guid client)
        {
            throw new NotImplementedException();
        }

        public override void ReplyClient(Guid client)
        {
            throw new NotImplementedException();
        }
    }
}

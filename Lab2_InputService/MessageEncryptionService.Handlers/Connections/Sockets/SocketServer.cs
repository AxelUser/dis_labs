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
using MessageEncryptionService.Handlers.Data;

namespace MessageEncryptionService.Handlers.Connections.Sockets
{
    public class SocketServer : ServerConnectionBase
    {
        #region Параметры и конструктор.
        private const string DEF_REPLY_MSG = "Сообщение получено.";
        private TcpListener listener;
        private int maxConnections;
        private Task listeningInputConnections;        
        private CancellationTokenSource ctsMain;
        private Dictionary<Guid, Task> activeConnectionListeners;
        private Dictionary<Guid, CancellationTokenSource> cancellationSourcesForListeners;
        private MessageEncryptionHandler encryptionHandler;

        public SocketServer(string domain, int port, int maxConnections = 10)
        {
            serverId = Guid.NewGuid();
            this.maxConnections = maxConnections;
            ctsMain = new CancellationTokenSource();
            IPAddress ipAdress = Dns.GetHostAddresses(domain).First();
            listener = new TcpListener(ipAdress, port);
            activeConnectionListeners = new Dictionary<Guid, Task>();
            cancellationSourcesForListeners = new Dictionary<Guid, CancellationTokenSource>();
            encryptionHandler = new MessageEncryptionHandler(new AsymmetricEncryptionHandler());
        }
        #endregion

        public override MessageModel ReceiveNewMessage()
        {
            throw new NotImplementedException();
        }

        public override void StartServer()
        {
            CancellationToken ct = ctsMain.Token;

            listener.Start(maxConnections);
            listeningInputConnections = ListenNewConnections(ct, listener);
            listeningInputConnections.Start();
        }

        public override void StopServer()
        {
            ctsMain.Cancel();
            listeningInputConnections.Wait();
            activeConnectionListeners.Clear();
            listener.Stop();
            
        }

        public void RegisterNewClient(Guid clientId, TcpClient client)
        {
            var handleProgress = new Progress<MessageModel>(value => OnNewMessage(value));
            var handleException = new Progress<Tuple<Guid, Exception>>(tp =>
            {
                Guid id = tp.Item1;
                cancellationSourcesForListeners[id].Cancel();
            });
            CancellationTokenSource clientCts = new CancellationTokenSource();
            Task handleClientTask = HandleClient(handleProgress, handleException, clientCts.Token, client, clientId);
            if (!activeConnectionListeners.ContainsKey(clientId))
            {
                lock (cancellationSourcesForListeners)
                {
                    cancellationSourcesForListeners.Add(clientId, clientCts);
                }
                lock (activeConnectionListeners)
                {
                    activeConnectionListeners.Add(clientId, handleClientTask);
                }
                activeConnectionListeners[clientId].Start();
            }
        }

        private void ClearCompletedListeners()
        {
            var completedTaskIds = activeConnectionListeners
                .Where(pair => pair.Value.IsCompleted)
                .Select(pair => pair.Key)
                .ToList();
            foreach (var id in completedTaskIds)
            {
                activeConnectionListeners.Remove(id);
                cancellationSourcesForListeners.Remove(id);
            }
        }

        public void CheckAndRepairClientId(Guid clientId, MessageModel messageFromClient)
        {
            if(clientId != messageFromClient.SenderId)
            {
                Task handler = activeConnectionListeners[clientId];
                CancellationTokenSource cts = cancellationSourcesForListeners[clientId];
                lock (cancellationSourcesForListeners)
                {
                    cancellationSourcesForListeners.Remove(clientId);
                    cancellationSourcesForListeners.Add(messageFromClient.SenderId, cts);
                }
                lock (activeConnectionListeners)
                {
                    activeConnectionListeners.Remove(clientId);
                    activeConnectionListeners.Add(messageFromClient.SenderId, handler);
                }                
            }
        }
        private Task ListenNewConnections(CancellationToken ct, TcpListener listener)
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
                        ClearCompletedListeners();
                    }
                    if (getClient.IsCompleted)
                    {
                        client = getClient.Result;
                        RegisterNewClient(Guid.NewGuid(), client);
                        
                    }
                }
                cancellationSourcesForListeners.Select(p => p.Value)
                    .ToList()
                    .ForEach(s => s.Cancel());
                Task.WaitAll(activeConnectionListeners.Values.ToArray());                
            });
            return listenInputConnections;
        }

        private Task HandleClient(IProgress<MessageModel> progressHandler, IProgress<Tuple<Guid, Exception>> exceptionHandler, CancellationToken ct, TcpClient clientToListen, Guid clientId)
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

                        var reqRaw = reader.ReadString();
                        MessageModel request = MessageCustomXmlConverter.ToModel(reqRaw);

                        if (request.IsBodyEncrypted)
                        {
                            request = encryptionHandler.DecryptMessage(request);
                        }

                        MessageModel response = MessageRouting(request, clientId);
                        writer.Write(MessageCustomXmlConverter.ToXml(response));
                        writer.Flush();
                        progressHandler.Report(request);
                    }
                    catch(Exception e)
                    {
                        exceptionHandler.Report(new Tuple<Guid, Exception>(clientId, e));
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

        public override ReplyModel SendRSAKey(Guid client)
        {
            return new ReplyModel(Types.MessageTypes.AskRSAKey)
            {
                Body = encryptionHandler.GetPublicAsymKey()
            };
        }

        public override ReplyModel ReplyClient(Guid client, MessageModel message)
        {
            throw new NotImplementedException();
        }
    }
}

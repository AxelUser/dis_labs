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

        public SocketServer(string domain, string port, int maxConnections = 10): base()
        {
            this.maxConnections = maxConnections;
            ctsMain = new CancellationTokenSource();
            IPAddress ipAdress = IPAddress.Parse(domain);
            listener = new TcpListener(ipAdress, int.Parse(port));
            activeConnectionListeners = new Dictionary<Guid, Task>();
            cancellationSourcesForListeners = new Dictionary<Guid, CancellationTokenSource>();
        }
        #endregion

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
            var handleException = new Progress<Tuple<Guid, Exception>>(tp =>
            {
                Guid id = tp.Item1;
                cancellationSourcesForListeners[id].Cancel();
            });
            CancellationTokenSource clientCts = new CancellationTokenSource();
            Task handleClientTask = HandleClient(handleException, clientCts.Token, client, clientId);
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

        private Task HandleClient(IProgress<Tuple<Guid, Exception>> exceptionHandler, CancellationToken ct, TcpClient clientToListen, Guid clientId)
        {
            Task listeningTask = new Task(() =>
            {
                BinaryReader reader = null;
                BinaryWriter writer = null;
                NetworkStream socketStream = null;
                MessageModel response = null;
                MessageModel request = null;

                while (!ct.IsCancellationRequested)
                {
                    try
                    {
                        var waitForData = new Task<string>(() => 
                        {
                            socketStream = clientToListen.GetStream();
                            reader = new BinaryReader(socketStream, Encoding.UTF8, true);
                            writer = new BinaryWriter(socketStream, Encoding.UTF8, true);
                            try
                            {
                                return reader.ReadString();
                            }
                            catch when (ct.IsCancellationRequested)
                            {
                                return null;
                            }                            
                        });
                        waitForData.Start();
                        waitForData.Wait(ct);
                        if (waitForData.IsCompleted)
                        {
                            var reqRaw = waitForData.Result;
                            request = MessageCustomXmlConverter.ToModel(reqRaw);
                            response = MessageRouting(ref request, clientId);
                            writer.Write(MessageCustomXmlConverter.ToXml(response));
                            writer.Flush();
                        }
                    }
                    catch(Exception e)
                    {
                        exceptionHandler.Report(new Tuple<Guid, Exception>(clientId, e));
                    }
                    finally
                    {
                        //reader?.Close();
                        //writer?.Close();
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
    }
}

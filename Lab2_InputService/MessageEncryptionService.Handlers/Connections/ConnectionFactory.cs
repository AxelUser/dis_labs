﻿using MessageEncryptionService.Handlers.Connections.MQ;
using MessageEncryptionService.Handlers.Connections.Sockets;
using MessageEncryptionService.Handlers.Connections.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public class ConnectionFactory
    {
        public const string DEF_HOST = "127.0.0.1";
        public const int DEF_PORT = 8888;
        public const string DEF_QUEUE_NAME = ".\\private$\\EncryptionServiceLabWork";

        public static ServerConnectionBase CreateServerConnection(ConnectionTypes conType)
        {
            ServerConnectionBase server = null;
            switch (conType)
            {
                case ConnectionTypes.Sockets:
                    server = new SocketServer(DEF_HOST, DEF_PORT);
                    break;
                case ConnectionTypes.RabbitMQ:
                    server = new MessageQueueServer();
                    break;

            }
            return server;
        }

        public static ClientConnectionBase CreateClientConnection(ConnectionTypes conType)
        {
            ClientConnectionBase client = null;
            switch (conType)
            {
                case ConnectionTypes.Sockets:
                    client = new SocketClient(DEF_HOST, DEF_PORT);
                    break;
                case ConnectionTypes.RabbitMQ:
                    client = new MessageQueueClient();
                    break;
            }
            return client;
        }
    }
}

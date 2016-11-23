using MessageEncryptionService.Handlers.Connections.MQ;
using MessageEncryptionService.Handlers.Connections.Sockets;
using MessageEncryptionService.Handlers.Connections.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public class ConnectionFactory
    {
        public static readonly string SocketHost = GetSetting("SocketHost");
        public static readonly string SocketPort = GetSetting("SocketPort");
        public static readonly string RpcQueueName = GetSetting("RpcQueueName");
        public static readonly string MQUserName = GetSetting("MQUserName");
        public static readonly string MQPassword = GetSetting("MQPassword");
        public static readonly string MQVirtualHost = GetSetting("MQVirtualHost");
        public static readonly string MQHostName = GetSetting("MQHostName");
        public static readonly string MQPort = GetSetting("MQPort");

        public static ServerConnectionBase CreateServerConnection(ConnectionTypes conType)
        {
            ServerConnectionBase server = null;
            switch (conType)
            {
                case ConnectionTypes.Sockets:
                    server = new SocketServer(SocketHost, SocketPort);
                    break;
                case ConnectionTypes.RabbitMQ:
                    server = new MessageQueueServer(MQUserName, MQPassword, MQVirtualHost, MQHostName, MQPort, RpcQueueName);
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
                    client = new SocketClient(SocketHost, SocketPort);
                    break;
                case ConnectionTypes.RabbitMQ:
                    client = new MessageQueueClient(MQUserName, MQPassword, MQVirtualHost, MQHostName, MQPort, RpcQueueName);
                    break;
            }
            return client;
        }

        public static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

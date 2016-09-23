using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public class ConnectionFactory
    {
        public const string DEF_HOST = "localhost";
        public const int DEF_PORT = 8888;        

        public IServiceConnection CreateConnection(ConnectionTypes type)
        {
            IServiceConnection connection = null;

            switch (type)
            {
                case ConnectionTypes.Sockets:
                    //connection = new SocketConnection(DEF_HOST, DEF_PORT, 10);
                    break;
                case ConnectionTypes.MSMQ:
                    //connection = new MSMQConnection();
                    break;
            }

            return connection;
        }
    }
}

using MessageEncryptionService.Handlers.Connections;
using MessageEncryptionService.Handlers.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Server.WinService
{
    public class InputServiceBase: ServiceBase
    {
        protected ServerConnectionBase server;
        public void StartInteractiveMode()
        {
            server.NewMessage += (s, msg) =>
            {
                Console.WriteLine($"To {server.ServerId} from {msg.SenderId}: {msg.MessageType.GetTypeCaption()}");
            };
            Console.WriteLine($"Starting {server.ServerId}...");
            OnStart(null);
            Console.WriteLine($"Server {server.ServerId} started.");
            Console.ReadKey();
            OnStop();
            Console.WriteLine($"Server {server.ServerId} stopped.");
        }
    }
}

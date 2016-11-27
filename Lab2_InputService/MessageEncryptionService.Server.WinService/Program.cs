using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Server.WinService
{
    static class Program
    {
        static void Main()
        {
            InputServiceForRabbitMQ serviceRabbitMQ = new InputServiceForRabbitMQ();
            InputServiceForSockets serviceSockets = new InputServiceForSockets();

            if (Environment.UserInteractive)
            {
                serviceRabbitMQ.StartInteractiveMode();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    serviceSockets,
                    serviceRabbitMQ
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        static void StartInteractive()
        {

        }

        static void StopInteractive()
        {

        }
    }
}

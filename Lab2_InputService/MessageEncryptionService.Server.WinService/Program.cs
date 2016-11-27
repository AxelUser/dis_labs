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
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new InputServiceForSockets(),
                new InputServiceForRabbitMQ()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

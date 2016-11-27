using MessageEncryptionService.Handlers.Connections;
using MessageEncryptionService.Handlers.Connections.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Server.WinService
{
    partial class InputServiceForRabbitMQ : InputServiceBase
    {
        public InputServiceForRabbitMQ()
        {
            InitializeComponent();
            server = ConnectionFactory.CreateServerConnection(ConnectionTypes.RabbitMQ);
        }

        protected override void OnStart(string[] args)
        {
            server.StartServer();
        }

        protected override void OnStop()
        {
            server.StopServer();
        }
    }
}

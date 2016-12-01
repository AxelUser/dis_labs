using MessageEncryptionService.Handlers.Connections;
using MessageEncryptionService.Handlers.Connections.Sockets;
using MessageEncryptionService.Handlers.Connections.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageEncryptionService.Handlers.Connections.Messages;
using MessageEncryptionService.Handlers.Helpers;

namespace MessageEncryptionService.Server
{
    public partial class FormMain : Form
    {
        private List<ServerConnectionBase> servers;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            servers = new List<ServerConnectionBase>();
            try
            {
                servers.Add(CreateServer(ConnectionTypes.Sockets));
                AddMessage($"Server {ConnectionTypes.Sockets} created.");
            }
            catch
            {
                AddMessage($"Server {ConnectionTypes.Sockets} failed.");
            }

            try
            {
                servers.Add(CreateServer(ConnectionTypes.RabbitMQ));
                AddMessage($"Server {ConnectionTypes.RabbitMQ} created.");
            }
            catch
            {
                AddMessage($"Server {ConnectionTypes.RabbitMQ} failed.");
            }
        }

        private void Server_NewMessage(object sender, MessageModel e)
        {
            AddMessage($"{e.SenderId}: {e.MessageType.GetTypeCaption()}");
        }

        private void AddMessage(string text)
        {
            listBoxLogs.Items.Add(text);
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var server in servers)
            {
                server.StopServer();
            }
        }
        
        private ServerConnectionBase CreateServer(ConnectionTypes type)
        {
            var server = ConnectionFactory.CreateServerConnection(type);
            server.NewMessage += Server_NewMessage;
            server.StartServer();
            return server;
        }
    }
}

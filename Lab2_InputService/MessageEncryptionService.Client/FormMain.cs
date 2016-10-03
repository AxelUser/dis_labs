using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageEncryptionService.Handlers.Connections;
using MessageEncryptionService.Handlers.Connections.Types;

namespace MessageEncryptionService.Client
{
    public partial class FormMain : Form
    {
        private IClientConnection client;
        private bool connected;
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string msgText = "Проверка.";
            MessageModel data = new MessageModel(MessageTypes.SendData)
            {
                Body = msgText
            };
            client.Send(data);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            connected = true;
            client = ConnectionFactory.CreateClientConnection(ConnectionTypes.Sockets);

            client.ConnectionErrorRised += (s, msg) =>
            {
                if (connected)
                {
                    MessageBox.Show(msg);
                    connected = false;
                    client.Disconnect();
                }
            };

            if (!client.Connect())
            {
                MessageBox.Show("Ошибка подключения клиента к серверу.");
            }
        }
    }
}

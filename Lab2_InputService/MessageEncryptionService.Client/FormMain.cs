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
            client = ConnectionFactory.CreateClientConnection(ConnectionTypes.Sockets);
            if (!client.Connect())
            {
                MessageBox.Show("ошибка подключения клиента к серверу.");
            }
        }
    }
}

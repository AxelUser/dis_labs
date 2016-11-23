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
using MessageEncryptionService.Handlers.Connections.Messages;
using MessageEncryptionService.Handlers.Helpers;
namespace MessageEncryptionService.Client
{
    public partial class FormMain : Form
    {
        private ClientConnectionBase client;
        private Guid clientId;

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
            var resp = client.Send(data);
            HandleResponse(resp);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            clientId = Guid.NewGuid();
            connected = true;
            client = ConnectionFactory.CreateClientConnection(ConnectionTypes.RabbitMQ);

            SubscribeUIUpdate(client);

            if (!client.Connect())
            {
                MessageBox.Show("Ошибка подключения клиента к серверу.");
            }
            else
            {
                var resp = client.AskAsymKey();
                HandleResponse(resp);
            }
        }

        private void SubscribeUIUpdate(ClientConnectionBase client)
        {
            client.ConnectionError += (s, ex) =>
            {                    
                if (connected)
                {
                    MessageBox.Show(ex.Message);
                    connected = false;
                    client.Disconnect();
                }
            };
        }

        private void HandleResponse(ReplyModel response)
        {
            MessageBox.Show($"Ответ на \"{response.ReplyType.GetTypeCaption()}\":{response.Body}");
        }
    }
}

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
using MessageEncryptionService.Handlers.Data;
using System.IO;
using CsvHelper;

namespace MessageEncryptionService.Client
{
    public partial class FormMain : Form
    {
        private Dictionary<ConnectionTypes, ClientConnectionBase> clients;
        public FormMain()
        {
            InitializeComponent();
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                List<TaskInfoViewModel> tasksFromCsv = new List<TaskInfoViewModel>();
                using(StreamReader sr = new StreamReader(openFile.FileName))
                {
                    CsvReader csvReader = new CsvReader(sr);
                    tasksFromCsv.AddRange(csvReader.GetRecords<TaskInfoViewModel>());
                }
                if (tasksFromCsv.Count > 0)
                {
                    string serializedData = DataTransformHandler.ToXML(tasksFromCsv.ToArray());
                    MessageModel data = new MessageModel(MessageTypes.SendData)
                    {
                        Body = serializedData
                    };
                    var resp = await GetSelectedClient().SendAsync(data);
                    HandleResponse(resp);
                }
                else
                {
                    MessageBox.Show("File does not contains propper data.");
                }
            }
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
            clients = new Dictionary<ConnectionTypes, ClientConnectionBase>();

            var clientSockets = await InitializeClient(ConnectionTypes.Sockets);
            clients.Add(ConnectionTypes.Sockets, clientSockets);
            var clientMQ = await InitializeClient(ConnectionTypes.RabbitMQ);
            clients.Add(ConnectionTypes.RabbitMQ, clientMQ);
            btnSend.Enabled = true;
        }

        private void SubscribeUIUpdate(ClientConnectionBase client)
        {
            client.ConnectionError += (s, ex) =>
            {                    
                MessageBox.Show(ex.Message);                
            };
        }

        private void HandleResponse(ReplyModel response)
        {
            if (response != null)
            {
                MessageBox.Show($"Response for \"{response.ReplyType.GetTypeCaption()}\": {response.Body}");
            }
            else
            {
                MessageBox.Show("Empty response!");
            }
        }

        private async Task<ClientConnectionBase> InitializeClient(ConnectionTypes type)
        {
            var client = ConnectionFactory.CreateClientConnection(type);

            SubscribeUIUpdate(client);

            bool isConnected = await Task.Run(() => client.Connect());

            if (!isConnected)
            {
                MessageBox.Show($"Could not connect to {type.ToString()}");
            }
            else
            {
                var resp = await client.AskAsymKeyAsync();
                HandleResponse(resp);
            }
            return client;
        }

        private ClientConnectionBase GetSelectedClient()
        {
            if (radioButtonSockets.Checked)
            {
                return clients[ConnectionTypes.Sockets];
            }
            else
            {
                return clients[ConnectionTypes.RabbitMQ];
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var client in clients)
            {
                client.Value.Disconnect();
            }
        }
    }
}

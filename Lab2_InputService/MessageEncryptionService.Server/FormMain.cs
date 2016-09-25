﻿using MessageEncryptionService.Handlers.Connections;
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

namespace MessageEncryptionService.Server
{
    public partial class FormMain : Form
    {
        private IServerConnection server;
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            server = ConnectionFactory.CreateServerConnection(ConnectionTypes.Sockets);
            server.StartServer();
        }
    }
}

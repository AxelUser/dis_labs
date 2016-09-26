﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public interface IClientConnection
    {
        bool Connect();
        void Send(MessageModel message);
        MessageModel Receive();
    }
}

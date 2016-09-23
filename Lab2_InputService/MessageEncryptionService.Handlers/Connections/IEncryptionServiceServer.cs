﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections
{
    public interface IEncryptionServiceServer
    {
        event EventHandler<MessageModel> NewMessageReceived;
        void Start();
        void Stop();
    }
}

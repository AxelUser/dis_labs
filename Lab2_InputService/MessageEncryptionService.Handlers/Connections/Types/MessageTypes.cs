using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Connections.Types
{
    public enum MessageTypes
    {
        [MessageTypeInfo("Ask server for RSA-key")]
        AskRSAKey,
        [MessageTypeInfo("Send data to the server")]
        SendData,
        [MessageTypeInfo("Send a reply for any message")]
        Reply,
        [MessageTypeInfo("Send a requent to close connection")]
        CloseConnection
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class MessageTypeInfoAttribute : Attribute
    {
        public string Caption { get; set; }

        public MessageTypeInfoAttribute() { }

        public MessageTypeInfoAttribute(string caption)
        {
            Caption = caption;
        }
    }
}

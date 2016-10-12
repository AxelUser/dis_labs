using MessageEncryptionService.Handlers.Connections.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessageEncryptionService.Handlers.Connections.Messages
{
    public class MessageCustomXmlConverter
    {
        private static int formatVersion = 1;
        public static string ToXml(MessageModel message)
        {
            string xml = null;            
            var root = new XElement("Message");
            root.SetAttributeValue("version", formatVersion);
            root.Add(new XElement("MessageType", message.MessageType));
            if(message is ReplyModel)
            {
                root.Add(new XElement("ReplyType", ((ReplyModel)message).ReplyType));
            }
            root.Add(new XElement("Sender", message.SenderId),
                new XElement("Body", message.Body));

            XDocument xDoc = new XDocument();
            xDoc.Add(root);
            return xDoc.ToString();
        }

        public static MessageModel ToModel(string xml)
        {
            XDocument xDoc = XDocument.Parse(xml);
            MessageModel message;
            MessageTypes type = (MessageTypes) Enum.Parse(typeof(MessageTypes), xDoc.Elements().Single(e => e.Name == "MessageType").Value);
            if (type == MessageTypes.Reply)
            {                
                message = new ReplyModel(type);
                MessageTypes replyType = (MessageTypes)Enum
                    .Parse(typeof(MessageTypes), xDoc
                    .Elements().Single(e => e.Name == "ReplyType").Value);
                ((ReplyModel)message).ReplyType = replyType;
            }
            else
            {
                message = new MessageModel(type);
            }
            message.SenderId = Guid.Parse(xDoc.Elements().Single(e => e.Name == "Sender").Value);
            message.Body = xDoc.Elements().Single(e => e.Name == "Body").Value;

            return message;
        }
    }
}

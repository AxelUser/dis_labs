using MessageEncryptionService.Handlers.Connections.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEncryptionService.Handlers.Helpers
{
    public static class ReflectionHelpers
    {
        public static string GetTypeCaption(this MessageTypes messageType)
        {
            var typeInfo = typeof(MessageTypes);
            var memberInfo = typeInfo.GetMember(messageType.ToString());
            var attribytes = memberInfo[0].GetCustomAttributes(typeof(MessageTypeInfoAttribute), false);
            return attribytes.Length > 0 ? (attribytes[0] as MessageTypeInfoAttribute)?.Caption : null;
        }
    }
}

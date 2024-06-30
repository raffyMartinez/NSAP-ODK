using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSAP_ODK.Mapping
{
    public enum MessageType
    {
        Error,
        Warning,
        Information
    }
    public class EntityValidationMessage
    {
        public string Message { get; private set; }
        public MessageType MessageType { get; private set; }

        public EntityValidationMessage(string message, MessageType messageType=MessageType.Error)
        {
            Message = message;
            MessageType = messageType;
        }
    }
}

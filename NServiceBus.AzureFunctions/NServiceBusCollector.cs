using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus.Extensibility;
using NServiceBus.Routing;
using NServiceBus.Transport;

namespace NServiceBus.AzureFunctions
{
    public class NServiceBusCollector
    {
        Dictionary<string, string> headers;
        IDispatchMessages dispatcher;

        List<object> replies = new List<object>();

        public NServiceBusCollector(Dictionary<string, string> headers, IDispatchMessages dispatcher)
        {
            this.headers = headers;
            this.dispatcher = dispatcher;
        }

        public void AddReply(object message)
        {
            replies.Add(message);
        }

        public Task Dispatch()
        {
            var replyOperations = replies.Select(CreateReplyOperation).ToArray();

            return dispatcher.Dispatch(new TransportOperations(replyOperations), new TransportTransaction(), new ContextBag());
        }

        TransportOperation CreateReplyOperation(object message)
        {
            var outHeaders = new Dictionary<string, string>
            {
                {Headers.EnclosedMessageTypes, message.GetType().AssemblyQualifiedName},
            };

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var outgoingMessage = new OutgoingMessage(Guid.NewGuid().ToString(), outHeaders, body);
            var replyToAddress = new UnicastAddressTag(headers[Headers.ReplyToAddress]);
            var transportOperations = new TransportOperation(outgoingMessage, replyToAddress);
            return transportOperations;
        }
    }
}
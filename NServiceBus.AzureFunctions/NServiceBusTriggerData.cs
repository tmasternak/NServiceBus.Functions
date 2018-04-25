using System.Collections.Generic;
using NServiceBus.Transport;

namespace NServiceBus.AzureFunctions
{
    public class NServiceBusTriggerData
    {
        public byte[] Data { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public IDispatchMessages Dispatcher { get; set; }
    }
}
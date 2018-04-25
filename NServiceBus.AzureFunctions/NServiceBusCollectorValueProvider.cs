using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace NServiceBus.AzureFunctions
{
    public class NServiceBusCollectorValueProvider : IValueBinder
    {
        NServiceBusCollector collector;

        public NServiceBusCollectorValueProvider(NServiceBusCollector collector)
        {
            this.collector = collector;
        }

        public Task<object> GetValueAsync()
        {
            return Task.FromResult<object>(collector);
        }

        public string ToInvokeString()
        {
            return $"NServiceBus collector";
        }

        public Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            return collector.Dispatch();
        }

        public Type Type => typeof(NServiceBusCollector);
    }
}
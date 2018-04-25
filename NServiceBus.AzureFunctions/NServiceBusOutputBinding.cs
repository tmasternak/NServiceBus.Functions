using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using NServiceBus.Transport;

namespace NServiceBus.AzureFunctions
{
    public class NServiceBusOutputBinding : IBinding
    {
        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            throw new NotImplementedException();
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            var headers = (Dictionary<string, string>)context.BindingData[NServiceBusTriggerBinding.BindingNames.Headers];
            var dispatcher = (IDispatchMessages) context.BindingData[NServiceBusTriggerBinding.BindingNames.Dispatcher];

            var collector = new NServiceBusCollector(headers, dispatcher);

            return Task.FromResult<IValueProvider>(new NServiceBusCollectorValueProvider(collector));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor();
        }

        public bool FromAttribute => false;
    }
}
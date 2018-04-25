using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Newtonsoft.Json;
using NServiceBus.Transport;

namespace NServiceBus.AzureFunctions
{
    public class NServiceBusTriggerBinding : ITriggerBinding
    {
        readonly ParameterInfo parameter;
        readonly NServiceBusTriggerAttribute attribute;

        public NServiceBusTriggerBinding(ParameterInfo parameter, NServiceBusTriggerAttribute attribute)
        {
            this.parameter = parameter;
            this.attribute = attribute;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            if (value is NServiceBusTriggerData triggerData)
            {
                var messageText = Encoding.UTF8.GetString(triggerData.Data);
                var argument = JsonConvert.DeserializeObject(messageText, parameter.ParameterType);

                var valueBinder = new NServiceBusMessageValueBinder(parameter, argument);

                var bindingData = new Dictionary<string, object>
                {
                    {BindingNames.Headers, triggerData.Headers },
                    {BindingNames.Dispatcher, triggerData.Dispatcher }
                };

                return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, bindingData));
            }

            throw new Exception();
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            return Task.FromResult<IListener>(new NServiceBusListener(context.Executor, attribute));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "NsbMessage",
                    Description = "NServiceBus trigger fired",
                    DefaultValue = "Sample"
                }
            };
        }

        public Type TriggerValueType => typeof(NServiceBusTriggerData);

        public IReadOnlyDictionary<string, Type> BindingDataContract => new Dictionary<string, Type>
        {
            {BindingNames.Headers, typeof(Dictionary<string, string>) },
            {BindingNames.Dispatcher, typeof(IDispatchMessages) }
        };

        public static class BindingNames
        {
            public const string Headers = "headers";
            public const string Dispatcher = "dispatcher";
        }
    }
}
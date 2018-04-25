using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using NServiceBus.Logging;
using NServiceBus.Raw;
using NServiceBus.Transport;

namespace NServiceBus.AzureFunctions
{
    public class NServiceBusListener : IListener
    {
        static string poisonMessageQueue = "error";
        static string nsbLogPath = @"d:\home\LogFiles";
        static int immediateRetryCount = 3;

        ITriggeredFunctionExecutor executor;
        NServiceBusTriggerAttribute attribute;
        IReceivingRawEndpoint endpoint;
        
        public NServiceBusListener(ITriggeredFunctionExecutor executor, NServiceBusTriggerAttribute attribute)
        {
            this.executor = executor;
            this.attribute = attribute;
        }

        public void Dispose()
        {
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var endpointConfiguration = RawEndpointConfiguration.Create(attribute.QueueName, OnMessage, poisonMessageQueue);

            if (Directory.Exists(nsbLogPath))
            {
                LogManager.Use<DefaultFactory>().Directory(nsbLogPath);
            }

            var connectionString = AmbientConnectionStringProvider.Instance.GetConnectionString(attribute.Connection);

            endpointConfiguration.UseTransport<AzureStorageQueueTransport>().ConnectionString(connectionString);

            endpointConfiguration.DefaultErrorHandlingPolicy(poisonMessageQueue, immediateRetryCount);

            endpoint = await RawEndpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }

        async Task OnMessage(MessageContext context, IDispatchMessages dispatcher)
        {
            var triggerData = new TriggeredFunctionData
            {
                TriggerValue = new NServiceBusTriggerData
                {
                    Data = context.Body,
                    Headers = context.Headers,
                    Dispatcher = dispatcher
                }
            };

            var result = await executor.TryExecuteAsync(triggerData, CancellationToken.None);

            if (result.Succeeded == false)
            {
                throw new Exception(result.Exception.Message, result.Exception);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return endpoint.Stop();
        }

        public void Cancel()
        {
        }
    }
}
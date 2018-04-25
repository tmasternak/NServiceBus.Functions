using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NServiceBus.AzureFunctions;
using NServiceBus.Contracts;

namespace NServiceBus.Function
{
    public static class TheFunction
    {
        [FunctionName("TheFunction")]
        public static void Run([NServiceBusTrigger(QueueName = "main-queue")]PingCommand command, NServiceBusCollector collector, TraceWriter log)
        {
            log.Info($"NSB function triggered: {command.Text}");

            collector.AddReply(new PongReply{Text = $"Hello {command.Text}. Timestamp: {DateTime.UtcNow.Ticks}"});
        }
    }
}

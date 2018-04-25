using System;
using Microsoft.Azure.WebJobs.Description;

namespace NServiceBus.AzureFunctions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding()]
    public sealed class NServiceBusTriggerAttribute : Attribute
    {
        public string QueueName { get; set; }

        public string Connection { get; set; } = "NsbConnectionString";
    }
}
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Config;

namespace NServiceBus.AzureFunctions
{
    public class NServiceBusExtensionConfig : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            context.Config.RegisterBindingExtensions(
                new NServiceBusTriggerBindingProvider(),
                new NServiceBusOutputBindingProvider());
        }
    }
}
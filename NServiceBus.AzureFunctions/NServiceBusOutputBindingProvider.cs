using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace NServiceBus.AzureFunctions
{
    public class NServiceBusOutputBindingProvider : IBindingProvider
    {
        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context.Parameter.ParameterType == typeof(NServiceBusCollector))
            {
                return Task.FromResult<IBinding>(new NServiceBusOutputBinding());
            }

            return Task.FromResult<IBinding>(null);
        }
    }
}
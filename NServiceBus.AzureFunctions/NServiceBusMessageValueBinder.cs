using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Bindings;

namespace NServiceBus.AzureFunctions
{
    public class NServiceBusMessageValueBinder : ValueBinder
    {
        readonly object value;

        public NServiceBusMessageValueBinder(ParameterInfo parameter, object value) : base(parameter.ParameterType)
        {
            this.value = value;
        }

        public override Task<object> GetValueAsync()
        {
            return Task.FromResult(value);
        }

        public override string ToInvokeString()
        {
            return $"{value}";
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus.Contracts;


namespace NServiceBus.Sender
{
    class Program
    {
        static string DestinationAddress = "main-queue";
        static string ConnectionString = "UseDevelopmentStorage=true";

        static void Main(string[] args)
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            var configuration = new EndpointConfiguration("NsbEndpoint");
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.EnableInstallers();

            var serialization = configuration.UseSerialization<NewtonsoftSerializer>();
            serialization.WriterCreator(s => new JsonTextWriter(new StreamWriter(s, new UTF8Encoding(false))));

            var transportConfiguration = configuration.UseTransport<AzureStorageQueueTransport>();
            transportConfiguration.Routing().RouteToEndpoint(typeof(PingCommand), DestinationAddress);
            transportConfiguration.ConnectionString(ConnectionString);

            configuration.SendFailedMessagesTo("error");

            configuration.Conventions()
                .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Contracts"));

            var endpoint = await Endpoint.Start(configuration);

            while (true)
            {
                Console.WriteLine("Press <enter> to sent command.");
                Console.ReadLine();

                var operations = Enumerable.Range(1, 1).Select(i => endpoint.Send(new PingCommand {Text = "Tomek"}));
                await Task.WhenAll(operations);
            }
        }

        public class PongHandler : IHandleMessages<PongReply>
        {
            public Task Handle(PongReply message, IMessageHandlerContext context)
            {
                Console.WriteLine($"Received reply {message.Text}");

                return Task.CompletedTask;
            }
        }
    }
}

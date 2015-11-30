
using System.ServiceModel.Description;

namespace RelaySamples
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;
    using System.Threading.Tasks;

    class Program : IHttpListenerSample
    {
        public async Task Run(string httpAddress, string listenToken)
        {
            var host = new ServiceHost(typeof(EchoService), new Uri(httpAddress));
            // we're not going to project the help page through the relay
            host.Description.Behaviors.Remove(typeof (ServiceDebugBehavior));
            foreach (var endpoint in host.Description.Endpoints)
            {
                endpoint.Behaviors.Add(new TransportClientEndpointBehavior(
                    TokenProvider.CreateSharedAccessSignatureTokenProvider(listenToken)));
            }

            host.Open();

            Console.WriteLine("Service address: " + httpAddress);
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            host.Close();
        }
    }
}

namespace RelaySamples
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;
    using System.Threading.Tasks;
    using System.IO;

    class Program : ITcpListenerSample
    {
        public async Task Run(string listenAddress, string listenToken)
        {
            
            var streamServer = new StreamServer(Console.OpenStandardOutput());

            // The host for our service is a regular WCF service host. You can use 
            // all extensibility options of WCF and you can also host non-Relay
            // endpoints alongside the Relay endpoints on this host
            using (ServiceHost host = new ServiceHost(streamServer))
            {
                // Now we're adding the service endpoint with a listen address on Service Bus
                // and using the NetTcpRelayBinding, which is a variation of the regular
                // NetTcpBinding of WCF with the difference that this one listens on the
                // Service Bus Relay service.                
                // Since the Service Bus Relay requires Authorization, we then also add the 
                // SAS token provider to the endpoint.
                host.AddServiceEndpoint(streamServer.GetType(), new NetTcpRelayBinding(), listenAddress)
                    .EndpointBehaviors.Add(
                        new TransportClientEndpointBehavior(
                            TokenProvider.CreateSharedAccessSignatureTokenProvider(listenToken)));

                // once open returns, the service is open for business. Not async for legibility.
                host.Open();
                Console.WriteLine("Service listening at address {0}", listenAddress);
                Console.WriteLine("Press [Enter] to close the listener and exit.");
                Console.ReadLine();
                host.Close();
            }
        }
    }

    

}
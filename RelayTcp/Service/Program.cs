
namespace RelaySamples
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;
    using System.Threading.Tasks;

    // This is an all-in-one Relay service that can be exposed through the 
    // Service Bus Relay. 
    [ServiceContract(Namespace = "", Name = "echo"), 
     ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    class Program : ITcpListenerSample
    {
        public async Task Run(string listenAddress, string listenToken)
        {
            using (ServiceHost host = new ServiceHost(this))
            {
                host.AddServiceEndpoint(GetType(), new NetTcpRelayBinding(), listenAddress)
                    .EndpointBehaviors.Add(
                        new TransportClientEndpointBehavior(
                            TokenProvider.CreateSharedAccessSignatureTokenProvider(listenToken)));

                host.Open();
                Console.WriteLine("Service listening at address {0}", listenAddress);
                Console.WriteLine("Press [Enter] to close the listener and exit.");
                Console.ReadLine();
                host.Close();
            }
        }

        [OperationContract]
        async Task<string> Echo(string input)
        {
            Console.WriteLine("\tCall received with input \"{0}\"", input);
            return input;
        }
    }
}
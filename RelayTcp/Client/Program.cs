namespace RelaySamples
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;
    using System.Threading.Tasks;

    class Program : ITcpSenderSample
    {
        public async Task Run(string sendAddress, string sendToken)
        {
            // first we create the WCF "channel factory" that will be used to create a proxy
            // to the remote service in the next step. The channel factory is constructed
            // using the NetTcpRelayBinding, which comes with the Service Bus client assembly
            // and understands the Service Bus Authorization model for clients. 
            var channelFactory = new ChannelFactory<IClient>(new NetTcpRelayBinding(), sendAddress);

            // to configure authorization with Service Bus, we now add the SAS token provider 
            // into which we pass the externally issued SAS token via a WCF behavior that is also 
            // included in the Service Bus client
            channelFactory.Endpoint.EndpointBehaviors.Add(
                new TransportClientEndpointBehavior(
                    TokenProvider.CreateSharedAccessSignatureTokenProvider(sendToken)));

            // now the interaction with the service is exactly as with any WCF service or 
            // with most other RPC frameworks. You create a proxy from the channel factory
            // and call the service. In the loop below we call the service 25 times and exit.
            using (IClient client = channelFactory.CreateChannel())
            {
                for (int i = 1; i <= 25; i++)
                {
                    var result = await client.Echo(DateTime.UtcNow.ToString());
                    Console.WriteLine("Round {0}, Echo: {1}", i, result);
                }
                client.Close();
            }
        }
        
        // this is the service contract that echoes the contract of the service
        [ServiceContract(Namespace = "", Name = "echo")]
        interface IClient : IClientChannel
        {
            [OperationContract]
            Task<string> Echo(string input);
        }
    }
}
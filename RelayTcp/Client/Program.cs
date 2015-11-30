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
            var channelFactory = new ChannelFactory<IClient>(new NetTcpRelayBinding(), sendAddress);

            channelFactory.Endpoint.EndpointBehaviors.Add(
                new TransportClientEndpointBehavior(
                    TokenProvider.CreateSharedAccessSignatureTokenProvider(sendToken)));

            using (IClient client = channelFactory.CreateChannel())
            {
                for (int i = 1; i <= 25; i++)
                {
                    var result = await client.Echo(DateTime.UtcNow.ToString());
                    Console.WriteLine("Round {0}, Echo: {1}", i, result);
                }
                client.Close();
            }

            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
        }
        
        [ServiceContract(Namespace = "", Name = "echo")]
        interface IClient : IClientChannel
        {
            [OperationContract]
            Task<string> Echo(string input);
        }
    }
}
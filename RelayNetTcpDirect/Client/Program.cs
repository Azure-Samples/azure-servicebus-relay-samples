//---------------------------------------------------------------------------------
// Microsoft (R)  Windows Azure SDK
// Software Development Kit
// 
// Copyright (c) Microsoft Corporation. All rights reserved.  
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
//---------------------------------------------------------------------------------

namespace RelaySamples
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Your Service Namespace: ");
            string serviceNamespace = Console.ReadLine();
            Console.Write("Your Issuer Name: ");
            string issuerName = Console.ReadLine();
            Console.Write("Your Issuer Secret: ");
            string issuerSecret = Console.ReadLine();

            TransportClientEndpointBehavior relayCredentials = new TransportClientEndpointBehavior();
            relayCredentials.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);    

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "HelloService");
            ChannelFactory<IHelloChannel> channelFactory = new ChannelFactory<IHelloChannel>("RelayEndpoint", new EndpointAddress(serviceUri));
            channelFactory.Endpoint.Behaviors.Add(relayCredentials);
            IHelloChannel channel = channelFactory.CreateChannel();
            channel.Open();

            IHybridConnectionStatus hybridConnectionStatus = channel.GetProperty<IHybridConnectionStatus>();
            hybridConnectionStatus.ConnectionStateChanged += ( o,e ) =>
                {
                    Console.WriteLine("Upgraded!");
                };

            Console.WriteLine("Press any key to exit");

            DateTime lastTime = DateTime.Now;
            int count = 0;

            while (!Console.KeyAvailable)
            {
                channel.Hello("Hello");

                count++;
                if (DateTime.Now - lastTime > TimeSpan.FromMilliseconds(250))
                {
                    lastTime = DateTime.Now;
                    Console.WriteLine("     Sent {0} messages...", count);
                    count = 0;
                }
            }

            channel.Close();
            channelFactory.Close();
        }       
    }
}

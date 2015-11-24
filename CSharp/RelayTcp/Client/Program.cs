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

namespace Microsoft.ServiceBus.Samples
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;

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

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "PingService");

            TransportClientEndpointBehavior sharedSecretServiceBusCredential = new TransportClientEndpointBehavior();
            sharedSecretServiceBusCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);
            
            ChannelFactory<IPingContract> channelFactory = new ChannelFactory<IPingContract>("RelayEndpoint", new EndpointAddress(serviceUri));

            channelFactory.Endpoint.Behaviors.Add(sharedSecretServiceBusCredential);
            
            IPingContract channel = channelFactory.CreateChannel();
            Console.WriteLine("Opening Channel.");
            channel.Open();

            for (int i = 1; i <= 25; i++)
            {
                Console.WriteLine("Ping: {0}", i);
                channel.Ping(i);
            }

            Console.WriteLine("Closing Channel.");
            channel.Close();
            channelFactory.Close();

            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();
        }

        static string GetHostName()
        {
            return System.Net.Dns.GetHostEntry(String.Empty).HostName;
        }
    }
}

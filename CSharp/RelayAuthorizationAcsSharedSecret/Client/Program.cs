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
    using System.Collections.Generic;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the name of the Service Namespace you want to connect to: ");
            string serviceNamespace = Console.ReadLine();
            
            Console.Write("Your Issuer Name: ");
            string issuerName = Console.ReadLine();

            Console.Write("Your Issuer Secret: ");
            string issuerSecret = Console.ReadLine();

            TransportClientEndpointBehavior behavior = new TransportClientEndpointBehavior();
            behavior.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret); 

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "SharedSecretAuthenticationService");

            ChannelFactory<IEchoChannel> channelFactory = new ChannelFactory<IEchoChannel>("RelayEndpoint", new EndpointAddress(serviceUri));
            channelFactory.Endpoint.Behaviors.Add(behavior);

            IEchoChannel channel = channelFactory.CreateChannel();
            channel.Open();

            Console.WriteLine("Enter text to echo (or [Enter] to exit):");
            string input = Console.ReadLine();
            while (input != String.Empty)
            {
                try
                {
                    Console.WriteLine("Server echoed: {0}", channel.Echo(input));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                input = Console.ReadLine();
            }

            channel.Close();
            channelFactory.Close();
        }
    }
}
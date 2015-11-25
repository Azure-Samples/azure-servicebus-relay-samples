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

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("https", serviceNamespace, "EchoService");

            // explicitly setting the endpoint to entity to "localhost" to match the sample certificate
            ChannelFactory<IEchoChannel> channelFactory = new ChannelFactory<IEchoChannel>(
                "ServiceBusEndpoint", 
                new EndpointAddress(serviceUri, EndpointIdentity.CreateDnsIdentity("localhost")));

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

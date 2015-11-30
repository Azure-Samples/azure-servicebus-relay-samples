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

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Your Service Namespace: ");
            string serviceNamespace = Console.ReadLine();

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("https", serviceNamespace, "PingService");

            ChannelFactory<IPingContract> channelFactory = new ChannelFactory<IPingContract>("ServiceBusEndpoint", new EndpointAddress(serviceUri));
            
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

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }
    }
}

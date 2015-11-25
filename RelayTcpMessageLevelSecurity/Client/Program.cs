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
            Console.Write("Enter the Service Namespace you want to connect to: ");
            string serviceNamespace = Console.ReadLine();

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "EchoService");

            ChannelFactory<IEchoChannel> channelFactory = new ChannelFactory<IEchoChannel>("RelayEndpoint", new EndpointAddress(serviceUri, EndpointIdentity.CreateDnsIdentity("localhost")));
            channelFactory.Credentials.UserName.UserName = "test1";
            channelFactory.Credentials.UserName.Password = "1tset";

            IEchoChannel channel = channelFactory.CreateChannel();
            try
            {
                channel.Open();


                Console.Write("Enter the text to echo (or press [Enter] to exit): ");
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
            }
            finally
            {
                channel.Abort();
            }
            channelFactory.Close();
        }
    }
}

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

using System.Threading.Tasks;

namespace RelaySamples
{
    using System;
    using System.ServiceModel;
    using Microsoft.ServiceBus;

    class Program : IHttpSenderSample
    {
        public async Task Run(string httpAddress, string sendToken)
        {
            var channelFactory = 
                new ChannelFactory<IEchoChannel>("ServiceBusEndpoint", new EndpointAddress(httpAddress));
            channelFactory.Endpoint.Behaviors.Add( 
                new TransportClientEndpointBehavior(TokenProvider.CreateSharedAccessSignatureTokenProvider(sendToken)));

            using (IEchoChannel channel = channelFactory.CreateChannel())
            {
                Console.WriteLine("Enter text to echo (or [Enter] to exit):");
                string input = Console.ReadLine();
                while (input != string.Empty)
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
            channelFactory.Close();
        }
    }
}

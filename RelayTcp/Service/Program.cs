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
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using Microsoft.ServiceBus;

    class Program
    {
        static void Run(string serviceNamespace,
                        string relayBasePath,
                                                IDictionary<string, string> keys)
        {
            Uri listenAddress = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, relayBasePath + "/nettcp");

            var f = new TransportClientEndpointBehavior() { TokenProvider} 
            sharedSecretServiceBusCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);
            
            ServiceHost host = new ServiceHost(typeof(PingService), listenAddress);

            foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                endpoint.Behaviors.Add(sharedSecretServiceBusCredential);
            }

            host.Open();

            Console.WriteLine("Press [Enter] to exit.");
            Console.ReadLine();

            host.Close();
        }
    }
}
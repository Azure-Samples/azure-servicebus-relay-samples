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
    using System.ServiceModel.Description;
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

            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "PingService");

            TransportClientEndpointBehavior sharedSecretServiceBusCredential = new TransportClientEndpointBehavior();
            sharedSecretServiceBusCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);
            
            ServiceHost host = new ServiceHost(typeof(PingService), address);

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
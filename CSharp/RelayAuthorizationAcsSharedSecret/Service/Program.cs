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
    using System.Collections.Generic;
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

            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "SharedSecretAuthenticationService");

            TransportClientEndpointBehavior behavior = new TransportClientEndpointBehavior();
            behavior.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);

            ServiceHost host = new ServiceHost(typeof(EchoService), address);
            host.Description.Endpoints[0].Behaviors.Add(behavior);
            host.Open();

            Console.WriteLine("Service address: " + address);
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            host.Close();

        }
    }
}
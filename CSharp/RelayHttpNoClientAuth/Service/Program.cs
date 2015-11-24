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
    using System.ServiceModel.Web;
    using Microsoft.ServiceBus;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Your Service Namespace: ");
            string serviceNamespace = Console.ReadLine();

            Console.Write("Your SAS Key Name (e.g., \"RootManageSharedAccessKey\"): ");
            string keyName = Console.ReadLine();

            Console.Write("Your SAS Key: ");
            string key = Console.ReadLine();

            // WebHttpRelayBinding uses transport security by default
            Uri address = ServiceBusEnvironment.CreateServiceUri("https", serviceNamespace, "SyndicationService");

            TransportClientEndpointBehavior clientBehavior = new TransportClientEndpointBehavior();
            clientBehavior.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(keyName, key);

            WebServiceHost host = new WebServiceHost(typeof(SyndicationService), address);
            host.Description.Endpoints[0].Behaviors.Add(clientBehavior);
            host.Open();

            Console.WriteLine("Service address: " + address);
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            host.Close();
        }        
    }
}
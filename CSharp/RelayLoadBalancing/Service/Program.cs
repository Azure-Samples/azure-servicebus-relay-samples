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
    using Microsoft.ServiceBus.Description;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            // Determine the system connectivity mode based on the command line
            // arguments: -http, -tcp or -auto  (defaults to auto)
            ServiceBusEnvironment.SystemConnectivity.Mode = GetConnectivityMode(args);

            Console.Write("Your Service Namespace: ");
            string serviceNamespace = Console.ReadLine();
            Console.Write("Your Issuer Name: ");
            string issuerName = Console.ReadLine();
            Console.Write("Your Issuer Secret: ");
            string issuerSecret = Console.ReadLine();

            // create the service URI based on the service namespace
            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "EchoService");

            // create the credentials object for the endpoint
            TransportClientEndpointBehavior sharedSecretServiceBusCredential = new TransportClientEndpointBehavior();
            sharedSecretServiceBusCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);

            // individual listen URI suffix for this instance.
            string serviceInstanceId = Guid.NewGuid().ToString("N");

            // create the service host
            ServiceHost host = new ServiceHost(typeof(EchoService));

            // add the endpoint
            Uri instanceListenUri = new Uri(address, serviceInstanceId + "/");
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IEchoContract), new NetTcpRelayBinding(EndToEndSecurityMode.None, RelayClientAuthenticationType.RelayAccessToken), address, instanceListenUri);

            // create the ServiceRegistrySettings behavior for the endpoint
            IEndpointBehavior serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Public);

            // add the Service Bus credentials to all endpoints specified in configuration
            endpoint.Behaviors.Add(serviceRegistrySettings);
            endpoint.Behaviors.Add(sharedSecretServiceBusCredential);

            Console.WriteLine("Service address: " + endpoint.Address.Uri);
            Console.WriteLine("Listen address: " + endpoint.ListenUri);

            // open the service
            host.Open();

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            // close the service
            host.Close();
        }

        static ConnectivityMode GetConnectivityMode(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.Equals("/auto", StringComparison.InvariantCultureIgnoreCase) ||
                     arg.Equals("-auto", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ConnectivityMode.AutoDetect;
                }
                else if (arg.Equals("/tcp", StringComparison.InvariantCultureIgnoreCase) ||
                     arg.Equals("-tcp", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ConnectivityMode.Tcp;
                }
                else if (arg.Equals("/http", StringComparison.InvariantCultureIgnoreCase) ||
                     arg.Equals("-http", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ConnectivityMode.Http;
                }
            }
            return ConnectivityMode.AutoDetect;
        }
    }
}
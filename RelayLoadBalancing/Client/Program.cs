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
            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "EchoService");

            // create the credentials object for the endpoint
            TransportClientEndpointBehavior sharedSecretServiceBusCredential = new TransportClientEndpointBehavior();
            sharedSecretServiceBusCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);

            // create the channel factory loading the configuration
            BalancingChannelFactory<IEchoChannel> channelFactory = new BalancingChannelFactory<IEchoChannel>(new NetTcpRelayBinding(EndToEndSecurityMode.None, RelayClientAuthenticationType.RelayAccessToken), new EndpointAddress(serviceUri));

            // apply the Service Bus credentials
            channelFactory.Endpoint.Behaviors.Add(sharedSecretServiceBusCredential);


            Console.WriteLine("Enter text to echo (or [Enter] to exit):");
            string input = Console.ReadLine();
            while (input != String.Empty)
            {
                IEchoChannel channel = channelFactory.CreateChannel();
                channel.Open();

                try
                {
                    // create and open the client channel
                    Console.WriteLine("Server echoed: {0}", channel.Echo(input));
                    channel.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    channel.Abort();
                }


                input = Console.ReadLine();
            }

            channelFactory.Close();
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

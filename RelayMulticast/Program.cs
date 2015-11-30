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
    using System.Globalization;
    using System.ServiceModel;

    class Program
    {
        private Program(string[] args)
        {
            // set the global relay connectivity mode based on optional command line arguments
            // -tcp - enforces ConnectivityMode.Tcp
            // -http - enforces ConnectivityMode.Http
            // -auto (or no argument) - uses default ConnectivityMode.AutoDetect mode
            ServiceBusEnvironment.SystemConnectivity.Mode = this.GetConnectivityMode(args);
        }

        static void Main(string[] args)
        {
            Program programInstance = new Program(args);
            programInstance.Run();
        }

        private void Run()
        {
            Console.Write("What do you want to call your chat session? ");
            string session = Console.ReadLine();
            Console.Write("Your Service Namespace: ");
            string serviceNamespace = Console.ReadLine();
            Console.Write("Your Issuer Name: ");
            string issuerName = Console.ReadLine();
            Console.Write("Your Issuer Secret: ");
            string issuerSecret = Console.ReadLine();
            Console.Write("Your Chat Nickname: ");
            string chatNickname = Console.ReadLine();

            TransportClientEndpointBehavior relayCredentials = new TransportClientEndpointBehavior();
            relayCredentials.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);    
            
            Uri serviceAddress = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace,
                   String.Format(CultureInfo.InvariantCulture, "{0}/MulticastService/", session));
            ServiceHost host = new ServiceHost(typeof(MulticastService), serviceAddress);
            host.Description.Endpoints[0].Behaviors.Add(relayCredentials);
            host.Open();

            ChannelFactory<IMulticastChannel> channelFactory = new ChannelFactory<IMulticastChannel>("RelayEndpoint", new EndpointAddress(serviceAddress));
            channelFactory.Endpoint.Behaviors.Add(relayCredentials);
            IMulticastChannel channel = channelFactory.CreateChannel();
            channel.Open();

            Console.WriteLine("\nPress [Enter] to exit\n");

            channel.Hello(chatNickname);

            string input = Console.ReadLine();
            while (input != String.Empty)
            {
                channel.Chat(chatNickname, input);
                input = Console.ReadLine();
            }

            channel.Bye(chatNickname);

            channel.Close();
            channelFactory.Close();
            host.Close();
        }

        private ConnectivityMode GetConnectivityMode(string[] args)
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

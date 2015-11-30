//  
//  Copyright © Microsoft Corporation, All Rights Reserved
// 
//  Licensed under the Apache License, Version 2.0 (the "License"); 
//  you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0 
// 
//  THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
//  OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
//  ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A
//  PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT.
// 
//  See the Apache License, Version 2.0 for the specific language
//  governing permissions and limitations under the License. 

namespace RelaySamples
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;

    class Program : IHttpListenerSample
    {
        public async Task Run(string listenAddress, string listenToken)
        {
            var u = new Uri(listenAddress);
            // create the service URI based on the service namespace
            var sbAddress = new UriBuilder(u) {Scheme = "sb", Path = u.PathAndQuery + "/service"}.Uri;
            var httpAddress = new UriBuilder(u) {Scheme = "https", Path = u.PathAndQuery + "/mex"}.Uri;

            // create the credentials object for the endpoint
            var sharedSecretServiceBusCredential = new TransportClientEndpointBehavior(
                TokenProvider.CreateSharedAccessSignatureTokenProvider(listenToken));

            // create the service host reading the configuration
            var host = new ServiceHost(typeof (EchoService), sbAddress, httpAddress);

            // create the ServiceRegistrySettings behavior for the endpoint
            IEndpointBehavior serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Public);

            // add the Service Bus credentials to all endpoints specified in configuration
            foreach (var endpoint in host.Description.Endpoints)
            {
                endpoint.Behaviors.Add(serviceRegistrySettings);
                endpoint.Behaviors.Add(sharedSecretServiceBusCredential);
            }

            // open the service
            host.Open();

            foreach (var channelDispatcherBase in host.ChannelDispatchers)
            {
                var channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                foreach (var endpointDispatcher in channelDispatcher.Endpoints)
                {
                    Console.WriteLine("Listening at: {0}", endpointDispatcher.EndpointAddress);
                }
            }

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            host.Close();
        }
    }
}
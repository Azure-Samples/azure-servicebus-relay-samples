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

namespace AzureServiceBus.RelayListener
{
    using System;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;

    public class RelayListener
    {
        IChannelListener<IDuplexSessionChannel> listener;
        readonly string address;
        readonly RelayAddressType relayAddressType;
        readonly TokenProvider tokenProvider;

        public RelayListener(string address, string token, RelayAddressType relayAddressType)
        {
            this.address = address;
            this.relayAddressType = relayAddressType;
            this.tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(token);
        }

        public RelayListener(string address, string sasRuleName, string sasRuleKey)
        {
            this.address = address;
            this.tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(sasRuleName, sasRuleKey);
        }

        public async Task StartAsync()
        {
            var tcpRelayTransportBindingElement = new TcpRelayTransportBindingElement(RelayClientAuthenticationType.RelayAccessToken)
            {
                ConnectionMode = TcpRelayConnectionMode.Relayed,
                IsDynamic = (relayAddressType == RelayAddressType.Dynamic)
            };
            tcpRelayTransportBindingElement.GetType().GetProperty("TransportProtectionEnabled", BindingFlags.GetProperty|BindingFlags.Instance|BindingFlags.NonPublic).SetValue(tcpRelayTransportBindingElement, true);

            var tb = new TransportClientEndpointBehavior(tokenProvider);
            var rt = new CustomBinding(
                new BinaryMessageEncodingBindingElement(),
                tcpRelayTransportBindingElement);
            
            
            listener = rt.BuildChannelListener<IDuplexSessionChannel>(new Uri(address),tb);
            await Task.Factory.FromAsync(listener.BeginOpen, listener.EndOpen, null);
        }

        public void Stop()
        {
            listener.Close();
        }

        public async Task<RelayConnection> AcceptConnectionAsync(TimeSpan timeout)
        {
            if (listener == null)
            {
                await StartAsync();
            }
            var duplexSessionChannel = await Task.Factory.FromAsync(listener.BeginAcceptChannel,
                (Func<IAsyncResult, IDuplexSessionChannel>) listener.EndAcceptChannel, 
                timeout, null);
            await Task.Factory.FromAsync(duplexSessionChannel.BeginOpen, duplexSessionChannel.EndOpen, null);
            return new RelayConnection(duplexSessionChannel);
        }

        public RelayConnection AcceptConnection(TimeSpan timeout)
        {
            return AcceptConnectionAsync(timeout).GetAwaiter().GetResult();
        }
    }
}
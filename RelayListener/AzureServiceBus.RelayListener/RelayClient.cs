namespace AzureServiceBus.RelayListener
{
    using System;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.ServiceBus;

    public class RelayClient
    {
        readonly string address;
        readonly TokenProvider tokenProvider;

        public RelayClient(string address, string token)
        {
            this.address = address;
            this.tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(token);
        }

        public RelayClient(string address, string sasRuleName, string sasRuleKey)
        {
            this.address = address;
            this.tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(sasRuleName, sasRuleKey);
        }

        public async Task<RelayConnection> ConnectAsync()
        {
            var tb = new TransportClientEndpointBehavior(tokenProvider);
            var bindingElement = new TcpRelayTransportBindingElement(
                RelayClientAuthenticationType.RelayAccessToken)
            {
                ConnectionMode = TcpRelayConnectionMode.Relayed,
            };
            bindingElement.GetType().GetProperty("TransportProtectionEnabled", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic).SetValue(bindingElement, true);

            var rt = new CustomBinding(
                        new BinaryMessageEncodingBindingElement(),
                        bindingElement);

            var cf = rt.BuildChannelFactory<IDuplexSessionChannel>(tb);
            await Task.Factory.FromAsync(cf.BeginOpen, cf.EndOpen, null);
            var ch = cf.CreateChannel(new EndpointAddress(address));
            await Task.Factory.FromAsync(ch.BeginOpen, ch.EndOpen, null);
            return new RelayConnection(ch);
        }

        public RelayConnection Connect()
        {
            return ConnectAsync().GetAwaiter().GetResult();
        }

    }
}
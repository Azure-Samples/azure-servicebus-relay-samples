
namespace RelaySamples
{
    using Microsoft.ServiceBus;
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;


    class ClientStream : Stream
    {
        private ChannelFactory<IClient> channelFactory;
        IClient channel = null;
        object channelMutex = new object();


        // this is the service contract that echoes the contract of the service
        [ServiceContract(Namespace = "", Name = "txf")]
        interface IClient : IClientChannel
        {
            [OperationContract]
            Task WriteAsync(byte[] data);

            [OperationContract]
            Task<byte[]> ReadAsync(int max);
        }

        public ClientStream(string sendAddress, string sendToken)
        {
            // first we create the WCF "channel factory" that will be used to create a proxy
            // to the remote service in the next step. The channel factory is constructed
            // using the NetTcpRelayBinding, which comes with the Service Bus client assembly
            // and understands the Service Bus Authorization model for clients. 
            this.channelFactory = new ChannelFactory<IClient>(new NetTcpRelayBinding(), sendAddress);

            // to configure authorization with Service Bus, we now add the SAS token provider 
            // into which we pass the externally issued SAS token via a WCF behavior that is also 
            // included in the Service Bus client
            channelFactory.Endpoint.EndpointBehaviors.Add(
                new TransportClientEndpointBehavior(
                    TokenProvider.CreateSharedAccessSignatureTokenProvider(sendToken)));
        }
              

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            lock (channelMutex)
            {
                if (channel == null)
                {
                    channel = channelFactory.CreateChannel();
                }
            }
            if (offset == 0 && buffer.Length == count)
            {
                await channel.WriteAsync(buffer);
            }
            else
            {
                var writeBuffer = new byte[count];
                Array.ConstrainedCopy(buffer, 0, writeBuffer, 0, count);
                await channel.WriteAsync(writeBuffer);
            }
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            lock (channelMutex)
            {
                if (channel == null)
                {
                    channel = channelFactory.CreateChannel();
                }
            }

            var result = await channel.ReadAsync(count);
            if (result == null || result.Length > count)
            {
                throw new InvalidDataException();
            }
            Array.ConstrainedCopy(result, 0, buffer, offset, result.Length);
            return result.Length;
        }


        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return true; } }
        public override long Length { get { throw new NotSupportedException(); } }
        public override long Position { get { throw new NotSupportedException(); } set { throw new NotSupportedException(); } }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) { return ReadAsync(buffer, offset, count).GetAwaiter().GetResult(); }
        public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override void Write(byte[] buffer, int offset, int count) { WriteAsync(buffer, offset, count).GetAwaiter().GetResult(); }

        

    }
}

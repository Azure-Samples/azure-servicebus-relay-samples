
namespace RelaySamples
{
    using System;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;

    [ServiceContract(Namespace = "", Name = "txf")]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class StreamServer
    {
        private Stream innerStream;

        public StreamServer(Stream innerStream)
        {
            this.innerStream = innerStream;
        }

        [OperationContract]
        Task WriteAsync(byte[] data)
        {
            return innerStream.WriteAsync(data, 0, data.Length);
        }

        [OperationContract]
        async Task<byte[]> ReadAsync(int max)
        {
            byte[] buffer = new byte[max];
            int bytesRead = await innerStream.ReadAsync(buffer, 0, max);
            if (bytesRead < max)
            {
                var result = new byte[bytesRead];
                Array.ConstrainedCopy(buffer, 0, result, 0, bytesRead);
                return result;
            }
            else
            {
                return buffer;
            }
        }
    }
}

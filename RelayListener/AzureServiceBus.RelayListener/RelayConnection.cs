namespace AzureServiceBus.RelayListener
{
    using System;
    using System.IO;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Web;

    public class RelayConnection : Stream
    {
        readonly IDuplexSessionChannel channel;
        Stream pendingReaderStream = null;
        SemaphoreSlim readerSemaphore = new SemaphoreSlim(1);

        public RelayConnection(IDuplexSessionChannel channel)
        {
            this.channel = channel;
        }

        public override void Flush()
        {

        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            this.Shutdown();
            base.Close();
        }

        public void Shutdown()
        {
            ShutdownAsync().GetAwaiter().GetResult();
        }

        public Task ShutdownAsync()
        {
            var msg = Message.CreateMessage(MessageVersion.Default, "eof");
            return Task.Factory.FromAsync(this.channel.BeginSend, this.channel.EndSend, msg, null);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count).GetAwaiter().GetResult();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var msg = StreamMessageHelper.CreateMessage(MessageVersion.Default, string.Empty, new MemoryStream(buffer, offset, count));
            return Task.Factory.FromAsync(this.channel.BeginSend, this.channel.EndSend, msg, null);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count,
            CancellationToken cancellationToken)
        {
            await readerSemaphore.WaitAsync(cancellationToken);

            try
            {
                do
                {
                    if (pendingReaderStream == null)
                    {
                        var msg = await Task.Factory.FromAsync(channel.BeginReceive, (Func<IAsyncResult, Message>)channel.EndReceive, TimeSpan.MaxValue, null);
                        if (!msg.IsEmpty && msg.Headers.Action != "eof")
                        {
                            pendingReaderStream = StreamMessageHelper.GetStream(msg);
                        }
                    }
                    if (pendingReaderStream != null)
                    {
                        int bytesRead = await pendingReaderStream.ReadAsync(buffer, offset, count, cancellationToken);
                        if (bytesRead == 0)
                        {
                            pendingReaderStream = null;
                            continue;
                        }
                        return bytesRead;
                    }
                    return 0;
                } while (true);
            }
            finally
            {
                readerSemaphore.Release();
            }
        }


        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

    }
    
    public enum RelayAddressType
    {
        Configured, Dynamic
    }


}
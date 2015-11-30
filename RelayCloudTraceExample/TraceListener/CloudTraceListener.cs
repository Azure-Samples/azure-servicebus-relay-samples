

namespace RelaySamples
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.ServiceModel;
    using Microsoft.ServiceBus;

    public class CloudTraceListener : TraceListener
    {
        ChannelFactory<ITraceChannel> traceChannelFactory;
        ITraceChannel traceChannel;
        object writeMutex;
        int maxRetries = 1;

        public CloudTraceListener()
        {
            string servicePath = ConfigurationManager.AppSettings["CloudTraceServicePath"];
            string serviceNamespace = ConfigurationManager.AppSettings["CloudTraceServiceNamespace"];
            string issuerName = ConfigurationManager.AppSettings["CloudTraceIssuerName"];
            string issuerSecret = ConfigurationManager.AppSettings["CloudTraceIssuerSecret"];

            Initialize(servicePath, serviceNamespace, issuerName, issuerSecret);
        }

        public CloudTraceListener(string servicePath, string serviceNamespace, string issuerName, string issuerSecret)
        {
            Initialize(servicePath, serviceNamespace, issuerName, issuerSecret);
        }

        void Initialize(string servicePath, string serviceNamespace, string issuerName, string issuerSecret)
        {
            this.writeMutex = new object();

            //Construct a Service Bus URI
            Uri uri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, servicePath);

            //Create a Behavior for the Credentials
            TransportClientEndpointBehavior sharedSecretServiceBusCredential = new TransportClientEndpointBehavior();
            sharedSecretServiceBusCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);

            //Create a Channel Factory
            traceChannelFactory = new ChannelFactory<ITraceChannel>(new NetEventRelayBinding(), new EndpointAddress(uri));
            traceChannelFactory.Endpoint.Behaviors.Add(sharedSecretServiceBusCredential);
        }

        public override bool IsThreadSafe
        {
            get
            {
                return true;
            }
        }

        public override void Close()
        {
            this.traceChannel.Close();
            this.traceChannelFactory.Close();
        }

        private void LockWrapper(Action action)
        {
            lock (this.writeMutex)
            {
                int retry = 0;
                for (; ; )
                {
                    EnsureChannel();
                    try
                    {
                        action.Invoke();
                        return;
                    }
                    catch (CommunicationException)
                    {
                        if (++retry > maxRetries)
                        {
                            throw;
                        }
                    }
                }
            }
        }

        public override void Write(string message)
        {
            LockWrapper(delegate { this.traceChannel.Write(message); });
        }

        public override void Write(object o)
        {
            LockWrapper(delegate { this.traceChannel.Write(o.ToString()); });
        }

        public override void Write(object o, string category)
        {
            LockWrapper(delegate { this.traceChannel.Write(o.ToString(), category); });
        }

        public override void Write(string message, string category)
        {
            LockWrapper(delegate { this.traceChannel.Write(message, category); });
        }

        public override void WriteLine(string message)
        {
            LockWrapper(delegate { this.traceChannel.WriteLine(message); });
        }

        public override void WriteLine(object o)
        {
            LockWrapper(delegate { this.traceChannel.WriteLine(o.ToString()); });
        }

        public override void WriteLine(object o, string category)
        {
            LockWrapper(delegate { this.traceChannel.WriteLine(o.ToString(), category); });
        }

        public override void WriteLine(string message, string category)
        {
            LockWrapper(delegate { this.traceChannel.WriteLine(message, category); });
        }

        public override void Fail(string message)
        {
            LockWrapper(delegate { this.traceChannel.Fail(message); });
        }

        public override void Fail(string message, string detailMessage)
        {
            LockWrapper(delegate { this.traceChannel.Fail(message, detailMessage); });
        }

        void EnsureChannel()
        {
            if (this.traceChannel != null &&
                this.traceChannel.State == CommunicationState.Opened)
            {
                return;
            }
            else
            {
                if (this.traceChannel != null)
                {
                    this.traceChannel.Abort();
                    this.traceChannel = null;
                }

                this.traceChannel = this.traceChannelFactory.CreateChannel();
                this.traceChannel.Open();
            }
        }
    }
}

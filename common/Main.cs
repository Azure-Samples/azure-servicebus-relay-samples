namespace RelaySamples
{
    using System;
	
    /// <summary>
    /// This is a common entry point class for all samples that provides
    /// the Main() method entry point called by the CLR. It loads the properties
    /// stored in the "azure-relay-samples.properties" file from the user profile.
    /// </summary>
	class AppEntryPoint
	{
	    static void Main(string[] args)
	    {
            var program = Activator.CreateInstance(typeof(Program));
	        if (program is ITcpListenerSampleUsingKeys)
	        {
            //    ((ITcpListenerSampleUsingKeys)program).Run()
	        }
	    }
	}

    interface ITcpListenerSampleUsingKeys
    {
        void Run(string listenAddress, string listenKeyName, string listenKeyValue);
    }

    interface IHttpListenerSampleUsingKeys
    {
        void Run(string listenAddress, string listenKeyName, string listenKeyValue);
    }

    interface ITcpSendSampleUsingKeys
    {
        void Run(string sendAddress, string sendKeyName, string sendKeyValue);
    }

    interface IHttpSendSampleUsingKeys
    {
        void Run(string sendAddress, string sendKeyName, string sendKeyValue);
    }
    
    interface ITcpSenderSample
    {
        void Run(string sendAddress, string sendToken);
    }

    interface IHttpSenderSample
    {
        void Run(string sendAddress, string sendToken);
    }

    interface ITcpListenSample
    {
        void Run(string listenAddress, string listenToken);
    }

    interface IHttpListenSample
    {
        void Run(string listenAddress, string listenToken);
    }
}
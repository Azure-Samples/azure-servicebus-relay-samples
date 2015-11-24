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

    [ServiceBehavior(Name = "PingService", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")]
    class PingService : IPingContract
    {
        public void Open()
        {
            Console.WriteLine("Session ({0}) Opened.", OperationContext.Current.SessionId);
        }

        public void Ping(int count)
        {
            Console.WriteLine("Session ({0}) Ping: {1}", OperationContext.Current.SessionId, count);
        }

        public void Close()
        {
            Console.WriteLine("Session ({0}) Closed.", OperationContext.Current.SessionId);
        }
    }
}

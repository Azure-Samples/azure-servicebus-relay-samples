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

    [ServiceBehavior(Name = "MulticastService", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")]
    class MulticastService : IMulticastContract
    {
        void IMulticastContract.Hello(string nickname)
        {
            Console.WriteLine("[" + nickname + "] joins");
        }

        void IMulticastContract.Chat(string nickname, string text)
        {
            Console.WriteLine("[" + nickname + "] says: " + text);
        }

        void IMulticastContract.Bye(string nickname)
        {
            Console.WriteLine("[" + nickname + "] leaves");
        }
    }
}

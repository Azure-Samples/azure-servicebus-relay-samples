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

namespace RelaySamples
{
    using System;
    using System.ServiceModel;

    [ServiceBehavior(Name = "OnewayService", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")]
    class OnewayService : IOnewayContract
    {
        public void Send(int count)
        {
            Console.WriteLine("Received: {0}", count);
        }

    }
}

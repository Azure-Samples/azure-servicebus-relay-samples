//   
//   Copyright © Microsoft Corporation, All Rights Reserved
// 
//   Licensed under the Apache License, Version 2.0 (the "License"); 
//   you may not use this file except in compliance with the License. 
//   You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0 
// 
//   THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
//   OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
//   ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A
//   PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT.
// 
//   See the Apache License, Version 2.0 for the specific language
//   governing permissions and limitations under the License. 

namespace RelaySamples
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
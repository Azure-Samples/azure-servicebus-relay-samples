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

    [ServiceContract(Name = "IMulticastContract", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")]
    public interface IMulticastContract
    {
        [OperationContract(IsOneWay=true)]
        void Hello(string nickname);

        [OperationContract(IsOneWay = true)]
        void Chat(string nickname, string text);

        [OperationContract(IsOneWay = true)]
        void Bye(string nickname);
    }

    public interface IMulticastChannel : IMulticastContract, IClientChannel { }
}

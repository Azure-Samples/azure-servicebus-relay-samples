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

    [ServiceContract(SessionMode = SessionMode.Required, Name = "IPingContract", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")]
    public interface IPingContract
    {
        [OperationContract(IsInitiating = true, IsTerminating = false)]
        void Open();

        [OperationContract(IsInitiating = false, IsOneWay = true, IsTerminating = false)]
        void Ping(int count);

        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void Close();
    }

    public interface IPingChannel : IPingContract, IClientChannel { }
}

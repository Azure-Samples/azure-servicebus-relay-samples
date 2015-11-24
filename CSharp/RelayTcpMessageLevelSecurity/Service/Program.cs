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
    using System.ServiceModel.Description;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Description;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the Service Namespace you want to connect to: ");
            string serviceNamespace = Console.ReadLine();
            Uri baseAddress = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "EchoService");

            ServiceHost host = new ServiceHost(typeof(EchoService), baseAddress);
            host.Open();

            Console.WriteLine("Service address: " + baseAddress);
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            host.Close();
        }
       
    }
}
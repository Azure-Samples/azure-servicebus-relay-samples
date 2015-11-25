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
    using System.Diagnostics;
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CloudTrace Sample Application");

            Console.WriteLine("List Active Trace Listeners");
            foreach (Object o in Trace.Listeners)
            {
                Console.WriteLine(o.GetType().ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Hit Control-C (command-line) or stop debugging (Visual Studio) to Exit");

            while (true)
            {
                Trace.WriteLine("Tracing...");
                Console.WriteLine("Tracing...");
                //Sleeping Thread to prevent the Service Bus messages to be sent in a tight loop
                Thread.Sleep(3000);
            }
        }
    }
}

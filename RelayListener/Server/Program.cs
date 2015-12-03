//  
//  Copyright © Microsoft Corporation, All Rights Reserved
// 
//  Licensed under the Apache License, Version 2.0 (the "License"); 
//  you may not use this file except in compliance with the License. 
//  You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0 
// 
//  THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
//  OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
//  ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A
//  PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT.
// 
//  See the Apache License, Version 2.0 for the specific language
//  governing permissions and limitations under the License. 

namespace RelaySamples
{
    using System;
    using System.Threading.Tasks;
    using AzureServiceBus.RelayListener;

    class Program : ITcpListenerSample
    {
        static readonly Random rnd = new Random();
       
        public async Task Run(string listenAddress, string listenToken)
        {
            Console.WriteLine("Starting listener on {0}", listenAddress);
            var client = new RelayListener(listenAddress, listenToken, RelayAddressType.Configured);
            RelayConnection connection;
            do
            {
               connection = await client.AcceptConnectionAsync(TimeSpan.MaxValue);
                if (connection != null)
                {
                    // not awaiting
#pragma warning disable 4014
                    ProcessConnection(connection);
#pragma warning restore 4014
                }
            }
            while (connection != null);
            

        }

        async Task ProcessConnection(RelayConnection connection)
        {
            Console.WriteLine("Processing connection");
            var buf = new byte[1024];
            int bytesRead;
            do
            {
                bytesRead = await connection.ReadAsync(buf, 0, buf.Length);
            }
            while (bytesRead > 0);
            
            // upload
            for (int i = 0; i < 1024; i++)
            {
                rnd.NextBytes(buf);
                await connection.WriteAsync(buf, 0, buf.Length);
            }
            connection.Close();
            Console.WriteLine("Connection done");
        }
    }
}
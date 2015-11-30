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
    using System.Security.Cryptography;
    using System.ServiceModel;
    using System.Text;
    using System.Web;
    using Microsoft.ServiceBus;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Your Service Namespace: ");
            string serviceNamespace = Console.ReadLine();

            Console.Write("Your Issuer Name: ");
            string issuerName = Console.ReadLine();

            Console.Write("Your Issuer Secret: ");
            string issuerSecret = Console.ReadLine();

            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "SimpleWebTokenAuthenticationService");

            TransportClientEndpointBehavior behavior = new TransportClientEndpointBehavior();
            behavior.TokenProvider = TokenProvider.CreateSimpleWebTokenProvider(
                ComputeSimpleWebTokenString(issuerName, issuerSecret));

            ChannelFactory<IEchoChannel> channelFactory = new ChannelFactory<IEchoChannel>("RelayEndpoint", new EndpointAddress(serviceUri));
            channelFactory.Endpoint.Behaviors.Add(behavior);

            IEchoChannel channel = channelFactory.CreateChannel();
            channel.Open();

            Console.WriteLine("Enter text to echo (or [Enter] to exit):");
            string input = Console.ReadLine();
            while (input != String.Empty)
            {
                try
                {
                    Console.WriteLine("Server echoed: {0}", channel.Echo(input));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                input = Console.ReadLine();
            }

            channel.Close();
            channelFactory.Close();
        }

        // Returns the string in the following format: Issuer=...&HMACSHA256=...
        static string ComputeSimpleWebTokenString(string issuerName, string issuerSecret)
        {
            string issuer = string.Format("{0}={1}", TokenConstants.TokenIssuer, HttpUtility.UrlEncode(issuerName));
            string signature = null;

            // Compute the signature
            using (HMACSHA256 sha256 = new HMACSHA256(Convert.FromBase64String(issuerSecret)))
            {
                signature = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(issuer)));
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(issuer);
            sb.Append(TokenConstants.UrlParameterSeparator);
            sb.Append(string.Format("{0}={1}", TokenConstants.TokenDigest256, HttpUtility.UrlEncode(signature)));

            return sb.ToString();
        }
    }
}
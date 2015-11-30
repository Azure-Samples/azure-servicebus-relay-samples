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

            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "SimpleWebTokenAuthenticationService");

            TransportClientEndpointBehavior behavior = new TransportClientEndpointBehavior();
            behavior.TokenProvider = TokenProvider.CreateSimpleWebTokenProvider(
                ComputeSimpleWebTokenString(issuerName, issuerSecret));

            ServiceHost host = new ServiceHost(typeof(EchoService), address);
            host.Description.Endpoints[0].Behaviors.Add(behavior);
            host.Open();

            Console.WriteLine("Service address: " + address);
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            host.Close();
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
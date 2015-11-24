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
    using System.IO;
    using System.Net;
    using System.ServiceModel.Syndication;
    using System.Xml;
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the name of the Service Namespace you want to connect to: ");
            string serviceNamespace = Console.ReadLine();

            // WebHttpRelayBinding uses transport security by default
            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("https", serviceNamespace, "SyndicationService");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceUri.ToString());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream stream = response.GetResponseStream();
            XmlReader reader = XmlReader.Create(stream);
            Rss20FeedFormatter formatter = new Rss20FeedFormatter();
            formatter.ReadFrom(reader);

            Console.WriteLine("\nThese are the contents of your feed: ");
            Console.WriteLine(" ");
            Console.WriteLine(formatter.Feed.Title.Text);
            foreach (SyndicationItem item in formatter.Feed.Items)
            {
                Console.WriteLine(item.Title.Text + ": " + item.Summary.Text);
            }

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }
    }
}

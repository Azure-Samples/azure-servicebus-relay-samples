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
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Syndication;
    using System.ServiceModel.Web;
    
    [ServiceBehavior(Name = "SyndicationService", Namespace = "http://samples.microsoft.com/ServiceModel/Relay/")]
    class SyndicationService : SyndicationContract
    {
        public Rss20FeedFormatter GetFeed()
        {
            List<SyndicationItem> list = new List<SyndicationItem>();

            list.Add(new SyndicationItem("Day 1", "Today I woke up and went to work. It was fun.", new Uri("http://ex.azure.com/")));
            list.Add(new SyndicationItem("Day 2", "Today I was sick. I didn't go to work. Instead I stayed home and wrote code all day.", new Uri("http://ex.azure.com/")));
            list.Add(new SyndicationItem("Day 3", "This is my third entry. Using Microsoft Windows Azure Service Bus is pretty cool!", new Uri("http://ex.azure.com/")));

            SyndicationFeed feed = new SyndicationFeed(
                "Microsoft Windows Azure Service Bus",
                "Software+Service, what can be better",
                new Uri("http://ex.azure.com/"),
                list);

            return new Rss20FeedFormatter(feed);
        }
    }
}

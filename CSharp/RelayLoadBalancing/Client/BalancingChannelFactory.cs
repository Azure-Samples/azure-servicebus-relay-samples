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
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Syndication;
    using System.Xml;
    using System.ServiceModel.Description;

    public class BalancingChannelFactory<T> : ChannelFactory<T> where T : IChannel
    {
        TimeSpan expirationPeriod = TimeSpan.FromSeconds(120);
        Dictionary<Uri, SyndicationFeed> cachedFeeds = new Dictionary<Uri, SyndicationFeed>();
        object cacheMutex = new object();
        Random random = new Random();

        public BalancingChannelFactory()
            : base()
        {
        }

        public BalancingChannelFactory(Binding binding)
            : base(binding)
        {
        }

        public BalancingChannelFactory(ServiceEndpoint endpoint)
            : base(endpoint)
        {
        }

        public BalancingChannelFactory(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        protected BalancingChannelFactory(Type channelType)
            : base(channelType)
        {
        }

        public BalancingChannelFactory(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        public BalancingChannelFactory(Binding binding, string remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        public BalancingChannelFactory(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        public TimeSpan RegistryExpiration
        {
            get
            {
                return expirationPeriod;
            }
            set
            {
                expirationPeriod = value;
            }
        }

        public override T CreateChannel(EndpointAddress address, Uri via)
        {
            return base.CreateChannel(address, GetTargetViaUri(via ?? address.Uri));
        }
                
        public Uri GetTargetViaUri(Uri via)
        {
            SyndicationFeed feed = GetRegistryFeed(via);
            List<SyndicationItem> items = new List<SyndicationItem>(feed.Items);
            if (items.Count > 0)
            {
                SyndicationItem selectedItem = items[random.Next(items.Count)];
                SyndicationLink link = selectedItem.Links.SingleOrDefault((l) => l.RelationshipType.Equals("alternate", StringComparison.InvariantCultureIgnoreCase));
                if (link == null || link.Uri == null)
                {
                    throw new EndpointNotFoundException();
                }
                else
                {
                    // get the URI and map the scheme to the desired scheme
                    UriBuilder targetUri = new UriBuilder(link.Uri);
                    targetUri.Scheme = via.Scheme;
                    return targetUri.Uri;
                }
            }
            else
            {
                throw new EndpointNotFoundException();
            }
        }

        SyndicationFeed GetRegistryFeed(Uri via)
        {
            UriBuilder httpUriBuilder = new UriBuilder(via);
            httpUriBuilder.Scheme = Uri.UriSchemeHttp;
            lock (this.cacheMutex)
            {
                SyndicationFeed cachedFeed = null;

                DateTime now = DateTime.UtcNow;
                if (!cachedFeeds.TryGetValue(httpUriBuilder.Uri, out cachedFeed) ||
                     cachedFeed.LastUpdatedTime + expirationPeriod > now)
                {
                    HttpWebRequest getFeedRequest = WebRequest.Create(httpUriBuilder.Uri) as HttpWebRequest;
                    getFeedRequest.Method = "GET";
                    getFeedRequest.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
                    using (HttpWebResponse getFeedResponse = getFeedRequest.GetResponse() as HttpWebResponse)
                    {
                        Atom10FeedFormatter atomFormatter = new Atom10FeedFormatter();
                        atomFormatter.ReadFrom(XmlReader.Create(getFeedResponse.GetResponseStream()));
                        cachedFeed = atomFormatter.Feed;
                        cachedFeed.LastUpdatedTime = now;
                        cachedFeeds[httpUriBuilder.Uri] = cachedFeed;
                    }
                }
                return cachedFeed;
            }
        }
    }
}

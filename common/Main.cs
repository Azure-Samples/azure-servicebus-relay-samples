//   
//   Copyright © Microsoft Corporation, All Rights Reserved
// 
//   Licensed under the Apache License, Version 2.0 (the "License"); 
//   you may not use this file except in compliance with the License. 
//   You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0 
// 
//   THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
//   OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
//   ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A
//   PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT.
// 
//   See the Apache License, Version 2.0 for the specific language
//   governing permissions and limitations under the License. 

namespace RelaySamples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;

    // IF YOU ARE JUST GETTING STARTED, THESE ARE NOT THE DROIDS YOU ARE LOOKING FOR
    // PLEASE REVIEW "Program.cs" IN THE SAMPLE PROJECT

    // This is a common entry point class for all samples that provides
    // the Main() method entry point called by the CLR. It loads the properties
    // stored in the "azure-relay-samples.properties" file from the user profile
    // and then allows override of the settings from environment variables.
    class AppEntryPoint
    {
#if STA
        [STAThread]
#endif

        static void Main(string[] args)
        {
            Run();
        }

        [DebuggerStepThrough]
        static void Run()
        {
            var properties = new Dictionary<string, string>
            {
                {"SERVICEBUS_NAMESPACE", null},
                {"SERVICEBUS_ENTITY_PATH", null},
                {"SERVICEBUS_FQDN_SUFFIX", null},
                {"SERVICEBUS_SEND_KEY", null},
                {"SERVICEBUS_LISTEN_KEY", null},
                {"SERVICEBUS_MANAGE_KEY", null}
            };

            // read the settings file created by the ./setup.ps1 file
            var settingsFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "azure-relay-config.properties");
            if (File.Exists(settingsFile))
            {
                using (var fs = new StreamReader(settingsFile))
                {
                    while (!fs.EndOfStream)
                    {
                        var propl = fs.ReadLine().Trim();
                        var cmt = propl.IndexOf('#');
                        if (cmt > -1)
                        {
                            propl = propl.Substring(0, cmt).Trim();
                        }
                        if (propl.Length > 0)
                        {
                            var propi = propl.IndexOf('=');
                            if (propi == -1)
                            {
                                continue;
                            }
                            var propKey = propl.Substring(0, propi - 1).Trim();
                            var propVal = propl.Substring(propi + 1).Trim();
                            if (properties.ContainsKey(propKey))
                            {
                                properties[propKey] = propVal;
                            }
                        }
                    }
                }
            }

            // get overrides from the environment
            foreach (var prop in properties)
            {
                var env = Environment.GetEnvironmentVariable(prop.Key);
                if (env != null)
                {
                    properties[prop.Key] = env;
                }
            }


            var netTcpUri =
                new UriBuilder(
                    "sb",
                    properties["SERVICEBUS_NAMESPACE"] + "." + properties["SERVICEBUS_FQDN_SUFFIX"],
                    -1,
                    "x" + properties["SERVICEBUS_ENTITY_PATH"] + "/NetTcp").ToString();
            var httpUri =
                new UriBuilder(
                    "https",
                    properties["SERVICEBUS_NAMESPACE"] + "." + properties["SERVICEBUS_FQDN_SUFFIX"],
                    -1,
                    "x" + properties["SERVICEBUS_ENTITY_PATH"] + "/Http").ToString();

            var program = Activator.CreateInstance(typeof (Program));
            if (program is ITcpListenerSampleUsingKeys)
            {
                ((ITcpListenerSampleUsingKeys) program).Run(
                    netTcpUri,
                    "rootsamplelisten",
                    properties["SERVICEBUS_LISTEN_KEY"])
                    .GetAwaiter()
                    .GetResult();
            }
            else if (program is ITcpSenderSampleUsingKeys)
            {
                ((ITcpSenderSampleUsingKeys) program).Run(netTcpUri, "rootsamplesend", properties["SERVICEBUS_SEND_KEY"])
                    .GetAwaiter()
                    .GetResult();
            }
            if (program is IHttpListenerSampleUsingKeys)
            {
                ((IHttpListenerSampleUsingKeys) program).Run(
                    httpUri,
                    "rootsamplelisten",
                    properties["SERVICEBUS_LISTEN_KEY"])
                    .GetAwaiter()
                    .GetResult();
            }
            else if (program is IHttpSenderSampleUsingKeys)
            {
                ((IHttpSenderSampleUsingKeys) program).Run(httpUri, "rootsamplesend", properties["SERVICEBUS_SEND_KEY"])
                    .GetAwaiter()
                    .GetResult();
            }

            if (program is ITcpListenerSample)
            {
                var token =
                    TokenProvider.CreateSharedAccessSignatureTokenProvider(
                        "rootsamplelisten",
                        properties["SERVICEBUS_LISTEN_KEY"])
                        .GetWebTokenAsync(netTcpUri, string.Empty, true, TimeSpan.FromHours(1)).GetAwaiter().GetResult();
                ((ITcpListenerSample) program).Run(netTcpUri, token).GetAwaiter().GetResult();
            }
            else if (program is ITcpSenderSample)
            {
                var token =
                    TokenProvider.CreateSharedAccessSignatureTokenProvider(
                        "rootsamplesend",
                        properties["SERVICEBUS_SEND_KEY"])
                        .GetWebTokenAsync(netTcpUri, string.Empty, true, TimeSpan.FromHours(1)).GetAwaiter().GetResult();
                ((ITcpSenderSample) program).Run(netTcpUri, token).GetAwaiter().GetResult();
            }
            if (program is IHttpListenerSample)
            {
                var token =
                    TokenProvider.CreateSharedAccessSignatureTokenProvider(
                        "rootsamplelisten",
                        properties["SERVICEBUS_LISTEN_KEY"])
                        .GetWebTokenAsync(httpUri, string.Empty, true, TimeSpan.FromHours(1)).GetAwaiter().GetResult();
                ((IHttpListenerSample) program).Run(httpUri, token).GetAwaiter().GetResult();
            }
            else if (program is IHttpSenderSample)
            {
                var token =
                    TokenProvider.CreateSharedAccessSignatureTokenProvider(
                        "rootsamplesend",
                        properties["SERVICEBUS_SEND_KEY"])
                        .GetWebTokenAsync(httpUri, string.Empty, true, TimeSpan.FromHours(1)).GetAwaiter().GetResult();
                ((IHttpSenderSample) program).Run(httpUri, token).GetAwaiter().GetResult();
            }
        }
    }

    interface ITcpListenerSampleUsingKeys
    {
        Task Run(string listenAddress, string listenKeyName, string listenKeyValue);
    }

    interface IHttpListenerSampleUsingKeys
    {
        Task Run(string listenAddress, string listenKeyName, string listenKeyValue);
    }

    interface ITcpSenderSampleUsingKeys
    {
        Task Run(string sendAddress, string sendKeyName, string sendKeyValue);
    }

    interface IHttpSenderSampleUsingKeys
    {
        Task Run(string sendAddress, string sendKeyName, string sendKeyValue);
    }

    interface ITcpSenderSample
    {
        Task Run(string sendAddress, string sendToken);
    }

    interface IHttpSenderSample
    {
        Task Run(string sendAddress, string sendToken);
    }

    interface ITcpListenerSample
    {
        Task Run(string listenAddress, string listenToken);
    }

    interface IHttpListenerSample
    {
        Task Run(string listenAddress, string listenToken);
    }
}
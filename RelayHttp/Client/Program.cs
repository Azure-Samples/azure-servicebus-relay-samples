using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client;
using RelaySamples;

namespace RelaySamples
{
     class Program : IHttpSenderSample
    {
        public async Task Run(string sendAddress, string sendToken)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm(sendAddress, sendToken));
        }
    }
}

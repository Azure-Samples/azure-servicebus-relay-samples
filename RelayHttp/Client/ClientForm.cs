using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        private readonly string _sendAddress;
        private readonly string _sendToken;

        public ClientForm()
        {
            InitializeComponent();
        }

        public ClientForm(string sendAddress, string sendToken)
        {
            InitializeComponent();
            _sendAddress = sendAddress;
            _sendToken = sendToken;
        }

        
        protected override void OnLoad(EventArgs e)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("ServiceBusAuthorization", _sendToken);
            var image = client.GetStreamAsync(_sendAddress + "/image").GetAwaiter().GetResult();
            pictureBox.Image = Image.FromStream(image);
        }
    }
}

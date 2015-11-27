namespace RelaySamples
{
    using System;
    using System.Threading.Tasks;
    using System.IO;
    
    class Program : ITcpSenderSample
    {
        public async Task Run(string sendAddress, string sendToken)
        {
            using (var clientStream = new ClientStream(sendAddress, sendToken))
            {
                var stdout = Console.Out;
                Console.SetOut(new StreamWriter(clientStream) { AutoFlush = true });
                Console.WriteLine("Don't look here, look over there!");
                Console.WriteLine("The output will be printed on the server!");
            }
        }
    }
}
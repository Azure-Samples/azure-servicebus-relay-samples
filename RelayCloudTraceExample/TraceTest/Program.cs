
using System;
using System.Diagnostics;
using System.Threading;

namespace RelaySamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CloudTrace Sample Application");

            Console.WriteLine("List Active Trace Listeners");
            foreach (Object o in Trace.Listeners)
            {
                Console.WriteLine(o.GetType().ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Hit Control-C (command-line) or stop debugging (Visual Studio) to Exit");

            while (true)
            {
                Trace.WriteLine("Tracing...");
                Console.WriteLine("Tracing...");
                //Sleeping Thread to prevent the Service Bus messages to be sent in a tight loop
                Thread.Sleep(3000);
            }
        }
    }
}

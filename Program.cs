using Opc.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_server_subscription
{
    class Program
    {
        static void Main(string[] args)
        {
            OPCServer server = new OPCServer("OPCServer.1");
            server.AddSubscription("Group1", new string[] { "Signal1" }.ToList(), OPCSubscription_DataChanged);
            
            Console.ReadLine();
        }

        public static void OPCSubscription_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] tags)
        {
            // Display result whenever a tag value is updated
            for (int i = 0; i < tags.Length; i++)
            {
                Console.WriteLine(" ItemName: " + tags[i].ItemName);
                Console.WriteLine(" Value: " + tags[i].Value);
            }
        }
    }
}

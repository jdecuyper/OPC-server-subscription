using Opc;
using Opc.Da;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_server_subscription
{
    public class OPCServer
    {
        private Opc.Da.Server _server;
        private URL _serverURL;
        public delegate void DataChangedEventHandler(object subscriptionHandle, object requestHandle, ItemValueResult[] values);

        public OPCServer(string serverName)
        {
            if (String.IsNullOrEmpty(serverName))
                throw new ArgumentException("Server name is invalid.");

            _serverURL = new URL(serverName);
            _serverURL.Scheme = "opcda";
            _server = new Opc.Da.Server(new OpcCom.Factory(), _serverURL);

            try
            {
                _server.Connect();
            }
            catch (Opc.ConnectFailedException connectionExc) {
                Console.WriteLine("Connection to OPC server could not be established"); 
                Console.WriteLine(connectionExc.ToString());
            } 
        }

        public void AddSubscription(string groupName, List<string> tagList, DataChangedEventHandler onDataChange) {

            if (!_server.IsConnected) {
                Console.WriteLine("Connection to OPC server is not established");
                return;
            }

            // Create group
            Opc.Da.Subscription group;
            Opc.Da.SubscriptionState groupState = new Opc.Da.SubscriptionState();
            groupState.Name = groupName;
            groupState.Active = true;
            groupState.UpdateRate = 200;

            // Short circuit if group already exists
            SubscriptionCollection existingCollection = _server.Subscriptions;
            if (existingCollection.Count > 0) {
                for(int i = 0; i < existingCollection.Count; i++){
                    if (existingCollection[i].Name == groupName) {
                        Console.WriteLine(String.Format("Subscription group {0} already exists", groupName));
                        return;    
                    }
                }
            }
            group = (Opc.Da.Subscription)_server.CreateSubscription(groupState);

            // Create list of items to monitor
            Item[] opcItems = new Item[1];
            int j = 0;
            foreach (string tag in tagList) {
                opcItems[j] = new Item();
                opcItems[j].ItemName = tag;
                j++;
            }

            // Attach items and event to group
            group.AddItems(opcItems);
            //group.DataChanged += new Opc.Da.DataChangedEventHandler(OPCSubscription_DataChanged);
            group.DataChanged += new Opc.Da.DataChangedEventHandler(onDataChange);
        }
        public void OPCSubscription_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            // Display result
            for (int i = 0; i < values.Length; i++)
            {
                OpcItem syncedItem = new OpcItem();
                syncedItem.ItemName = values[i].ItemName;
                syncedItem.Value = values[i].Value;
                Console.WriteLine("ItemName" + syncedItem.ItemName); 
                Console.WriteLine("Value" + syncedItem.Value);
            }
        }
    }
}

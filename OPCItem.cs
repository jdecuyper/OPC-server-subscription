using System;

namespace OPC_server_subscription
{
    public class OpcItem
    {
        private object _value;
        public string Quality { get; set; }
        public string ItemName { get; set; }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value)
                {
                    return;
                }
                _value = value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    class EzDatabase
    {
        public List<MQTTProxyMessage> messageList;

        public EzDatabase()
        {
            messageList = new List<MQTTProxyMessage>();
        }
    }
}

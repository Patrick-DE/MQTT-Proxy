using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy.REST
{
    class ClientManagerWithMessages
    {
        public ClientManager clientManager { get; private set; }
        public List<MQTTProxyMessage> messages
        {
            get { return Broker.db.messageList.Where(i => i.ClientId == clientManager.clientId).ToList(); }
            private set {; }
        }

        public ClientManagerWithMessages(ClientManager clientManager)
        {
            this.clientManager = clientManager;
        }
    }
}

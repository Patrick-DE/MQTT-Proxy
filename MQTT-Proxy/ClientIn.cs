using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTT_Client
{
    public class ClientIn : Client
    {
        public ClientIn(String ip, int port, String clientId) : base(ip, port, clientId) { }
    }
}

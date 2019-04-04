using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    public class ClientOut : Client
    {
        public ClientOut(String ip, int port, String clientId) : base(ip, port, clientId) { }
    }
}

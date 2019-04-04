using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    class ProxyConfig
    {
        public String ownIP;
        public int ownPort;
        public String targetIP;
        public int targetPort;

        public ProxyConfig(String ownIP, int ownPort, String targetIP, int targetPort)
        {
            this.ownIP = ownIP;
            this.ownPort = ownPort;
            this.targetIP = targetIP;
            this.targetPort = targetPort;
        }
    }
}

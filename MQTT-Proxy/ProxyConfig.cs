﻿using System;
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

        public ProxyConfig(String targetIP, int targetPort, String ownIP, int ownPort)
        {
            this.targetIP = targetIP;
            this.targetPort = targetPort;
            this.ownIP = ownIP;
            this.ownPort = ownPort;
        }
    }
}

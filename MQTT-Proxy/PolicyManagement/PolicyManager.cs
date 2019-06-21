using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    class PolicyManager
    {
        public List<Policy> rulebook;

        public PolicyManager()
        {
            rulebook = new List<Policy>();
        }
    }
}

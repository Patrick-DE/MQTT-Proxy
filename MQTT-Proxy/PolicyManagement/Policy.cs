using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    class Policy
    {
        public string name;
        public int compareTo;
        public int ifCompareToGreater;
        public int ifCompareToSmaller;
        public int ifCompareToEqual;

        public Policy(string name, int compareTo, int valueIfGreater, int valueIfSmaller, int valueIfEqual)
        {
            this.name = name;
            this.ifCompareToGreater = valueIfGreater;
            this.ifCompareToSmaller = valueIfSmaller;
            this.ifCompareToEqual = valueIfEqual;
            this.compareTo = compareTo;
        }
    }
}

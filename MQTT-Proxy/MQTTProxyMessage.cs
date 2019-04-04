using MQTTnet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    class MQTTProxyMessage
    {
        [JsonIgnore]
        public static int GlobalId { get; private set; }

        public int MsgId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int QoS { get; set; }
        public string ClientId { get; set; }
        public bool RetainMsg { get; set; }
        public string Topic { get; set; }
        public byte[] Payload { get; set; }
        public string PayloadString
        {
            get { return Encoding.UTF8.GetString(Payload); }
            set { Payload = Encoding.UTF8.GetBytes(value); }
        }

        public MQTTProxyMessage(MqttApplicationMessage msg, string clientId)
        {
            MsgId = GlobalId++;
            Timestamp = DateTime.Now;
            Payload = msg.Payload;
            ClientId = clientId;
            QoS = (int)msg.QualityOfServiceLevel;
            RetainMsg = msg.Retain;
            Topic = msg.Topic;
        }

    }


}

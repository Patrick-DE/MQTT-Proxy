using MQTTnet;
using MQTTnet.Protocol;
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

        //Custom
        public int MsgId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public MessageState State { get; set; }
        public string ClientId { get; set; }
        public string ClientManagerId { get; set; }
        public string PayloadString
        {
            get { return Encoding.UTF8.GetString(Payload); }
            set { Payload = Encoding.UTF8.GetBytes(value); }
        }

        //MqttApplicationMessage
        public int QoS { get; set; }
        public bool RetainMsg { get; set; }
        public string Topic { get; set; }
        public byte[] Payload { get; set; }

        /// <summary>
        /// For internal conversions and storage
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="clientId"></param>
        /// <param name="clientManagerId"></param>
        /// <param name="state"></param>
        public MQTTProxyMessage(MqttApplicationMessage msg, string clientId, string clientManagerId, MessageState state)
        {
            MsgId = GlobalId++;
            Timestamp = DateTime.Now;
            Payload = msg.Payload;
            ClientId = clientId;
            ClientManagerId = clientManagerId;
            QoS = (int)msg.QualityOfServiceLevel;
            RetainMsg = msg.Retain;
            Topic = msg.Topic;
            State = state;
        }

        /// <summary>
        /// For parsing the incoming webrequests since they have no MqttApplicationMessage reference
        /// </summary>
        /// <param name="QoS"></param>
        /// <param name="Payload"></param>
        /// <param name="RetainMsg"></param>
        /// <param name="ClientId"></param>
        /// <param name="ClientManagerId"></param>
        /// <param name="State"></param>
        public MQTTProxyMessage(string ClientId, string ClientManagerId, int QoS, bool RetainMsg, string Topic, byte[] Payload, MessageState State)
        {
            MsgId = GlobalId++;
            Timestamp = DateTime.Now;
            this.ClientId = ClientId;
            this.ClientManagerId = ClientManagerId;
            this.Payload = Payload;
            this.QoS = QoS;
            this.RetainMsg = RetainMsg;
            this.Topic = Topic;
            this.State = State;
        }

        public MqttApplicationMessage ToMqttApplicationMessage()
        {
            return new MqttApplicationMessage()
            {
                Payload = Payload,
                QualityOfServiceLevel = (MqttQualityOfServiceLevel)QoS,
                Retain = RetainMsg,
                Topic = Topic
            };
        }

    }


}

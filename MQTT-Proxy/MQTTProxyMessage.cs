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
        /// <param name="state"></param>
        public MQTTProxyMessage(MqttApplicationMessage msg, string clientId, MessageState state) : this()
        {
            Payload = msg.Payload;
            ClientId = clientId;
            QoS = (int)msg.QualityOfServiceLevel;
            RetainMsg = msg.Retain;
            Topic = msg.Topic;
            State = state;
        }

        /// <summary>
        /// For JSON deserialise
        /// </summary>
        public MQTTProxyMessage()
        {
            Timestamp = DateTime.Now;
            MsgId = GlobalId++;
            this.State = MessageState.New;
        }

        /// <summary>
        /// For CopyMessage
        /// </summary>
        /// <param name="other"></param>
        public MQTTProxyMessage(MQTTProxyMessage other):this()
        {
            this.ClientId = other.ClientId;
            //for deep copy
            this.Payload = new byte[other.Payload.Length];
            Array.Copy(other.Payload, this.Payload, this.Payload.Length);
            this.QoS = other.QoS;
            this.RetainMsg = other.RetainMsg;
            this.Topic = other.Topic;
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

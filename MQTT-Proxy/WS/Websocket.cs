using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace MQTT_Proxy
{
    class MessageWS : WebSocketBehavior
    {
        WebSocketServer wss;
        public MessageWS()
        {
            wss = new WebSocketServer("ws://localhost:8081");
            wss.AddWebSocketService<MessageWS>("/");
        }
        public bool Start()
        {
            try
            {
                wss.Start();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public void SendMessage(MQTTProxyMessage msg)
        {
            Console.WriteLine("Sending UI update");
            Sessions.Broadcast(JsonConvert.SerializeObject(msg));
        }
    }
}

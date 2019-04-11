using WebSocketSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    class Websocket
    {
        WebSocket ws;
        public Websocket(string path)
        {
            ws = new WebSocket("ws://"+path);
        }
        public bool Connect()
        {
            try
            {
                ws.Connect();
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
            ws.Send(JsonConvert.SerializeObject(msg));
        }
    }
}

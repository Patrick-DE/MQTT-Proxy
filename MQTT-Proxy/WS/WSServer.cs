using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MQTT_Proxy
{
    class WSServer : WebSocketBehavior
    {
        WebSocketServer wss;

        public WSServer(){}

        public WSServer(string ip)
        {
            wss = new WebSocketServer($"ws://{ip}:9090");
            wss.AddWebSocketService<WSServer>("/");
        }

        public void Start()
        {
            wss.Start();
            Console.WriteLine("WSS started");
        }

        public void SendMessage(MQTTProxyMessage msg)
        {
            Console.WriteLine("Sending UI update");
            try { 
                wss.WebSocketServices.Hosts.First().Sessions.Broadcast(JsonConvert.SerializeObject(msg));
            }catch(Exception e) { 
                Console.WriteLine("No Websocket connected"+ e);
            }
        }

        protected override void OnOpen()
        {
            Console.WriteLine("OnOpen");
            base.OnOpen();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine("Incomming Message: "+e.Data);
        }
    }
}

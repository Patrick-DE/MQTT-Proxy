using Grapevine.Server;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            //"String ownIP, int ownPort, String targetIP, int targetPort"
            args = new string[] { "192.168.1.21", "1883", "192.169.178.120", "1883" };
            args[0] = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();

            Console.WriteLine("Broker: " + args[0] + ":" + args[1]);
            Console.WriteLine("WebUI: " + args[0] + ":80");
            if (args.Length < 4)
            {
                Console.WriteLine("String ownIP, int ownPort, String targetIP, int targetPort");
                return;
            }

            ProxyConfig proxyConfig = new ProxyConfig(args[0], int.Parse(args[1]), args[2], int.Parse(args[3]));
            var broker = new Broker(proxyConfig);
            //broker.Start();
            Random rnd = new Random();

            //DUMMY SHIT!!
            for (int i=0; i< 10; i++)
            {
                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce;
                msg.Payload = Encoding.UTF8.GetBytes("YourMomGay" + rnd.Next(0, 1000));
                msg.Topic = "YourMomDoubleGay";
                msg.Retain = false;
                if (i == 0)
                {
                    string clientId = "clientId";
                    Broker.db.messageList.Add(new MQTTProxyMessage(msg, clientId, MessageState.Intercepted));
                    Broker.clientManagers.Add("clientManger", new ClientManager(clientId, proxyConfig));
                }
                else { 
                    string clientId = "clientId" + rnd.Next(0, 1000);
                    Broker.db.messageList.Add(new MQTTProxyMessage(msg, clientId, MessageState.Intercepted));
                    Broker.clientManagers.Add("clientManger-"+ rnd.Next(0,1000), new ClientManager(clientId, proxyConfig));
                }
            }
            //END DUMMY SHIT!!

            var rest = new RestServer
            {
                Host = args[0],
                Port = "80",
                PublicFolder = new PublicFolder("public"),
                
            };
            rest.Start();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return;
        }
    }
}

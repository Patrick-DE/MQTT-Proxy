using Grapevine.Interfaces.Server;
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
            /*args[0] = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
            */
            List<String> arg = new List<string>();
            //Copy all given parameters into arg
            if (args.Length == 2)
            {
                arg.Add(args[0]);
                arg.Add("1833");
                arg.Add(args[1]);
                arg.Add("1883");
                arg.Add("80");
            }
            else if (args.Length == 4) {
                for (int i = 0; i < args.Length; i++)
                {
                    arg.Add(args[i]);
                }
            }
            else
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("Minimal form (TargetPort: 1883, Web|REST: 80)");
                Console.WriteLine("./MQTT-Proxy.exe <targetIP> <ownIP>");
                Console.WriteLine("Full form");
                Console.WriteLine("./MQTT-Proxy.exe <targetIP> <targetPort> <ownIP> <ownPort> <Web|RESTPort>");
#if !DEBUG
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
#endif
                //"String targetIP, int targetPort, String ownIP, int ownPort, WebRESTPort"
                arg.Add("192.169.178.120");
                arg.Add("1883");
                arg.Add("141.19.142.169");
                arg.Add("1883");
                arg.Add("80");
            }
            Console.WriteLine("Broker: " + arg[2] + ":" + int.Parse(arg[3]));
            Console.WriteLine("WebUI: " + arg[2] + ":" + int.Parse(arg[4]));

            ProxyConfig proxyConfig = new ProxyConfig(arg[0], int.Parse(arg[1]), arg[2], int.Parse(arg[3]));
            var broker = new Broker(proxyConfig);
            broker.Start();
            //DUMMY Data!!

            /*Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce;
                msg.Payload = Encoding.UTF8.GetBytes(rnd.Next(-5,35) + " Grad C");
                msg.Topic = "1468461001/building1/room1/temp"+i;
                msg.Retain = false;
                if (i == 0)
                {
                    string clientId = "clientId";
                    Broker.db.messageList.Add(new MQTTProxyMessage(msg, clientId, MessageState.Intercepted));
                    Broker.clientManagers.Add(clientId, new ClientManager(clientId, proxyConfig));
                }
                else
                {
                    string clientId = "clientId" + i;
                    Broker.db.messageList.Add(new MQTTProxyMessage(msg, clientId, MessageState.Intercepted));
                    Broker.clientManagers.Add(clientId, new ClientManager(clientId, proxyConfig));
                }
            }*/

            //END DUMMY Data!!

            var rest = new RestServer
            {
                Host = arg[2],
                Port = arg[4],
                PublicFolder = new PublicFolder("public"),

            };
            rest.Start();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return;
        }

    }
}

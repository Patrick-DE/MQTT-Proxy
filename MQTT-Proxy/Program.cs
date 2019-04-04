using Grapevine.Server;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Broker.db.messageList.Add(new MQTTProxyMessage(msg, "clientId" + rnd.Next(0, 1000)));
            }
            //END DUMMY SHIT!!

            var rest = new RestServer
            {
                Host = "localhost",
                Port = "8080"
            };
            rest.Start();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return;
        }
    }
}

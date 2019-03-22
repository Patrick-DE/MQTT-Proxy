using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new string[] { "192.168.1.21", "1883", "192.168.1.41", "1883" };

            if (args.Length < 4)
            {
                Console.WriteLine("String ownIP, int ownPort, String targetIP, int targetPort");
                return;
            }

            ProxyConfig proxyConfig = new ProxyConfig(args[0], int.Parse(args[1]), args[2], int.Parse(args[3]));
            var broker = new Broker(proxyConfig);
            broker.Start();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            return;
        }
    }
}

using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    class ClientManager
    {
        //Broker to Target
        [JsonIgnore]
        public Client clientOut;
        //Target answer to broker
        [JsonIgnore]
        public Client clientIn;
        public String clientId;
        public bool intercept = false;
        public List<MQTTProxyMessage> messages
        {
            get { return Broker.db.messageList.Where(i => i.ClientId == clientId).ToList(); }
            private set {;}
        }

        public ClientManager(String clientId, ProxyConfig proxyConfig)
        {
            messages = new List<MQTTProxyMessage>();
            this.clientId = clientId;
            clientOut = new ClientOut(proxyConfig.targetIP, proxyConfig.targetPort, clientId);
            clientOut.ApplicationMessageReceived += onOutMessageReceived;
            clientOut.Connected += onConnected;

            clientIn = new ClientIn(proxyConfig.ownIP, proxyConfig.ownPort, clientId + "_fake");
            clientIn.ApplicationMessageReceived += onInMessageReceived;
            clientIn.Connected += onConnected;
        }


        /// <summary>
        /// Connect both clients for communicating to the targetBroker and providing the answer to proxyBroker
        /// </summary>
        public async Task Connect()
        {
            Console.WriteLine("ClientManager: Connecting clientOut");
            while (!clientOut.IsConnected())
            {
                await clientOut.Connect();
                System.Threading.Thread.Sleep(100);
            }
            Console.WriteLine("ClientManager: Connecting clientIn");
            while (!clientIn.IsConnected())
            {
                await clientIn.Connect();
                System.Threading.Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Event is triggered if answer from targetBroker is received 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void onOutMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            //send message via clientIn
            Console.WriteLine("ClientManager: Message received from proxy - sending with ClientIn to proxyBroker");
            while (!clientIn.IsConnected())
                Thread.Sleep(100);
            await clientIn.SendMessage(e.ApplicationMessage.Payload, e.ApplicationMessage.Topic);
            Console.WriteLine("ClientManager: Message sent via ClientIn to proxyBroker");
        }

        /// <summary>
        /// Event is triggered if answer from Proxy is received 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void onInMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            //dont care about responses from proxyBroker
        }

        /// <summary>
        /// Event is triggered if client is connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onConnected(object sender, MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("Client connected!");
        }
    }
}

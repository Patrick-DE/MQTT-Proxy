using MQTT_Proxy;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MQTT_Proxy
{
    class Broker
    {
        IMqttServer mqttServer;
        public static Dictionary<String,ClientManager> clientManagers = new Dictionary<string, ClientManager>();
        ProxyConfig proxyConfig;
        public static EzDatabase db;
        public static PolicyManager policyManager;
        public static WSServer wss;

        private IMqttServerOptions optionsBuilder;

        public Broker(ProxyConfig proxyConfig)
        {
            db = new EzDatabase();
            policyManager = new PolicyManager();
            wss = new WSServer(proxyConfig.ownIP);
            wss.Start();
            this.proxyConfig = proxyConfig;
            // Start a MQTT server.
            optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(proxyConfig.ownPort)
                .WithDefaultEndpointBoundIPAddress(System.Net.IPAddress.Parse(proxyConfig.ownIP))
                .WithApplicationMessageInterceptor(HandleMessage)
                .WithSubscriptionInterceptor(HandleMessage)
                .Build();

            mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.ClientConnected += MqttServer_ClientConnected;
        }

        public async void Start()
        {
            await mqttServer.StartAsync(optionsBuilder);
            Console.WriteLine("Broker started.");
            //await mqttServer.StopAsync();
        }

        private async void MqttServer_ClientConnected(object sender, MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("Broker: ClientConnected");
            
            // do not create clientmanager for connecting fakeClient
            if (e.ClientId.EndsWith("_fake")) return;

            if (clientManagers.ContainsKey(e.ClientId)) { 
                Console.WriteLine("Client" + e.ClientId + " is reconnecting!");
                //clientManagers.Remove(e.ClientId);
                //throw new Exception("ClientId already exists!");
            }
            else
            {
                Console.WriteLine("Broker: Creating new ClientManager");
                clientManagers[e.ClientId] = new MQTT_Proxy.ClientManager(e.ClientId, proxyConfig);
                Console.WriteLine("Broker: ClientManager created");
                Console.WriteLine("Broker: Connecting ClientManager");
                await clientManagers[e.ClientId].Connect();
                Console.WriteLine("Broker: ClientManager connected");
            }
        }

        // Extend the timestamp for all messages from clients.
        public async void HandleMessage (MqttApplicationMessageInterceptorContext context){
            Console.WriteLine("Broker: New message");

            //if(clientManagers[context.ClientId] == null) 
            if (context.ClientId.EndsWith("_fake"))
            {
                //RULEBOOK
                string payload = Encoding.UTF8.GetString(context.ApplicationMessage.Payload);
                Policy myPolicy = Broker.policyManager.rulebook.Last();
                bool isNumber = int.TryParse(payload, out int payloadInt);
                if (isNumber)
                {
                    if (payloadInt > myPolicy.compareTo)
                    {
                        context.ApplicationMessage.Payload = BitConverter.GetBytes(myPolicy.ifCompareToSmaller);
                    } else if(payloadInt < myPolicy.compareTo)
                    {
                        context.ApplicationMessage.Payload = BitConverter.GetBytes(myPolicy.ifCompareToGreater);
                    } else
                    {
                        context.ApplicationMessage.Payload = BitConverter.GetBytes(myPolicy.ifCompareToEqual);
                    }
                }
                return;
            }

            Console.WriteLine("### BROKER: RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {context.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(context.ApplicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {context.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {context.ApplicationMessage.Retain}");
            Console.WriteLine();

            context.AcceptPublish = false;

            //If intercept on save msg
            if (clientManagers[context.ClientId].intercept)
            {
                MQTTProxyMessage tmp = new MQTTProxyMessage(context.ApplicationMessage, context.ClientId, MessageState.Intercepted);
                db.messageList.Add(tmp);
                wss.SendMessage(tmp);
            }
            //if intercept off forward
            else {
                ForwardMessage(context);
            }
        }

        // Protect several topics from being subscribed from every client.
        public async void HandleMessage(MqttSubscriptionInterceptorContext context) {
            Console.WriteLine("Broker: Subscription detected");
            if (context.ClientId.EndsWith("_fake")) return;

            Console.WriteLine("Broker: ClientOut subscribing");
            while (!clientManagers[context.ClientId].clientOut.IsConnected())
                Thread.Sleep(100);

            await clientManagers[context.ClientId].clientOut.SubscribeTo(context.TopicFilter.Topic);
            Console.WriteLine("Broker: ClientOut subscribed");
        }

        public async void ForwardMessage(MqttApplicationMessageInterceptorContext context)
        {
            if (clientManagers[context.ClientId].clientOut.IsConnected())
            {
                Console.WriteLine("Broker: Sending msg via ClientOut");
                string tmp = Encoding.UTF8.GetString(context.ApplicationMessage.Payload)/* + DateTime.Now.Ticks.ToString()*/;
                await clientManagers[context.ClientId].clientOut.SendMessage(Encoding.UTF8.GetBytes(tmp), context.ApplicationMessage.Topic);
                Console.WriteLine("Broker: Message sent via ClientOut");
            }
            else
            {
                Console.WriteLine("Broker: ClientOut not connected!!!");
            }
        }
    }
}

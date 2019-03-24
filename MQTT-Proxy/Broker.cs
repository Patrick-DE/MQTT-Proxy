﻿using MQTT_Client;
using MQTTnet;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MQTT_Client
{
    class Broker
    {
        IMqttServer mqttServer;
        Dictionary<String,ClientManager> clientManagers = new Dictionary<string, ClientManager>();
        ProxyConfig proxyConfig;

        private IMqttServerOptions optionsBuilder;

        public Broker(ProxyConfig proxyConfig)
        {
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

            if (clientManagers.ContainsKey(e.ClientId))
            {
                throw new Exception("ClientId already exists!");
            }
            else
            {
                Console.WriteLine("Broker: Creating new ClientManager");
                clientManagers[e.ClientId] = new MQTT_Client.ClientManager(e.ClientId, proxyConfig);
                Console.WriteLine("Broker: ClientManager created");
                Console.WriteLine("Broker: Connecting ClientManager");
                await clientManagers[e.ClientId].Connect();
                Console.WriteLine("Broker: ClientManager connected");
            }
        }

        // Extend the timestamp for all messages from clients.
        public async void HandleMessage (MqttApplicationMessageInterceptorContext context){
            Console.WriteLine("Broker: New message");

            if (context.ClientId.EndsWith("_fake")) return;

            context.AcceptPublish = false;
            if (clientManagers[context.ClientId].clientOut.IsConnected())
            {
                Console.WriteLine("Broker: Sending msg via ClientOut");
                await clientManagers[context.ClientId].clientOut.SendMessage(context.ApplicationMessage.Payload, context.ApplicationMessage.Topic);
                Console.WriteLine("Broker: Message send via ClientOut");
            }
            else
            {
                Console.WriteLine("Broker: ClientOut not connected!!!");
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
    }
}

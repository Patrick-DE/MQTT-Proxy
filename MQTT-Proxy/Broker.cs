using MQTT_Client;
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

        public async void CreateBroker()
        {
            await mqttServer.StartAsync(optionsBuilder);
            //await mqttServer.StopAsync();
        }

        private void MqttServer_ClientConnected(object sender, MqttClientConnectedEventArgs e)
        {
            // fo not create clientmanager for connecting fakeClient
            if (e.ClientId.EndsWith("_fake"))
                return;

            if (clientManagers.ContainsKey(e.ClientId))
            {
                throw new Exception("ClientId already exists!");
            }
            else
            {
                clientManagers[e.ClientId] = new MQTT_Client.ClientManager(e.ClientId, proxyConfig);
            }
        }

        // Extend the timestamp for all messages from clients.
        public void HandleMessage (MqttApplicationMessageInterceptorContext context){}

        // Protect several topics from being subscribed from every client.
        public void HandleMessage(MqttSubscriptionInterceptorContext context){}
    }
}

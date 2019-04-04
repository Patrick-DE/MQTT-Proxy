using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    public class Client
    {
        protected IMqttClient mqttClient;
        //should be protected, public for serialize
        public bool run { get; set; }
        //should be private, public for serialize
        public IMqttClientOptions options { get; }

        public Client(String ip, int port, String clientId)
        {
            run = true;
            // Create a new MQTT client.
            var factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();

            // Use WebSocket connection.
            options = new MqttClientOptionsBuilder()
                .WithTcpServer(ip, port)
                .WithClientId(clientId)
                .Build();

            //Set events
            mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
            mqttClient.Connected += MqttClient_Connected;
        }

        public bool IsConnected()
        {
            return mqttClient.IsConnected;
        }

        public async Task Connect()
        {
            Console.WriteLine("Client: Client connecting");
            try { 
                await mqttClient.ConnectAsync(options);
            }catch (Exception e)
            {
                //broker is awaiting so ClientIn not able to connect!
            }
            Console.WriteLine("Client: Client connected "+ mqttClient.IsConnected.ToString());
        }

        public async Task SubscribeTo(String topic)
        {
            // Subscribe to a topic
            Console.WriteLine("### SUBSCRIBING TO " + topic.ToUpper() + " ###");
            await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());
            Console.WriteLine("Client: subscribed");
        }

        public async Task SendMessage(string msg, string topic)
        {
            Console.WriteLine("Client: Sending string message");
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(msg)
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine("Client: Message string send");
        }

        public async Task SendMessage(byte[] msg, string topic)
        {
            Console.WriteLine("Client: Sending byte message");
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(msg)
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine("Client: Message byte send");
        }

        public async Task SendMessage(MqttApplicationMessage msg)
        {
            Console.WriteLine("Client: Sending MqttApplicationMessage");
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(msg.Topic)
                .WithPayload(msg.Payload)
                .WithQualityOfServiceLevel(msg.QualityOfServiceLevel)
                .WithRetainFlag(msg.Retain)
                .Build();

            await mqttClient.PublishAsync(message);
            Console.WriteLine("Client: Message byte send");
        }

        //Events for ClientManager defined
        public event EventHandler<MqttApplicationMessageReceivedEventArgs> ApplicationMessageReceived;
        public event EventHandler<MqttClientConnectedEventArgs> Connected;

        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("### " + e.ClientId + " RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            Console.WriteLine();
            ApplicationMessageReceived?.Invoke(this,e);
        }

        private void MqttClient_Connected(object sender, MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("### CLIENT CONNECTED ###");
            Connected?.Invoke(this, e);
        }

    }
}

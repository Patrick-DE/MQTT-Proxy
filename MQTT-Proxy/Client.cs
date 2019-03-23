using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTT_Client
{
    public class Client
    {
        protected static IMqttClient mqttClient;
        protected bool run = true;

        private IMqttClientOptions options;

        public Client(String ip, int port, String clientId)
        {
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

        public async void Connect()
        {
            await mqttClient.ConnectAsync(options);
        }

        public async Task SubscribeTo(String topic)
        {
            // Subscribe to a topic
            Console.WriteLine("### SUBSCRIBING TO " + topic.ToUpper() + " ###");
            await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());
        }

        public async Task SendMessage(string msg, string topic)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(msg)
                .Build();

            await mqttClient.PublishAsync(message);
        }

        public async Task SendMessage(byte[] msg, string topic)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(msg)
                .Build();

            await mqttClient.PublishAsync(message);
        }

        public event EventHandler<MqttApplicationMessageReceivedEventArgs> ApplicationMessageReceived;
        public event EventHandler<MqttClientConnectedEventArgs> Connected;

        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
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

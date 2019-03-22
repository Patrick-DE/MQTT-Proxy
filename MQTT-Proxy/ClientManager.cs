using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Client
{
    class ClientManager
    {
        //Broker to Target
        Client clientOut;
        //Target answer to broker
        Client clientIn;

        public ClientManager(String clientId, ProxyConfig proxyConfig)
        {
            clientOut = new ClientOut(proxyConfig.targetIP, proxyConfig.targetPort, clientId);
            clientOut.ApplicationMessageReceived += onInMessageReceived;
            clientOut.Connected += onConnected;

            clientIn = new ClientIn(proxyConfig.ownIP, proxyConfig.ownPort, clientId + "_fake");
            clientIn.ApplicationMessageReceived += onOutMessageReceived;
            clientIn.Connected += onConnected;
        }

        public void Connect()
        {
            clientOut.Connect();
            clientIn.Connect();
        }

        /// <summary>
        /// Event is triggered if message from targetBroker is received 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onOutMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            //send to clientIn
        }

        /// <summary>
        /// Event is triggered if message from clientOut is received 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onInMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            //send to proxyBroker
        }

        /// <summary>
        /// Event is triggered if client is connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onConnected(object sender, MqttClientConnectedEventArgs e)
        {
            
        }
    }
}

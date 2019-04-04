using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy.REST
{
    [RestResource(BasePath = "/manager")]
    class Client
    {
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/")]
        public IHttpContext GetAllClientManager(IHttpContext context)
        {
            context.Response.SendJSON(Broker.clientManagers);
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[clientId]")]
        public IHttpContext GetClientManagerInfo(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["clientId"]);
            string clientId = context.Request.PathParameters["clientId"];
            if (clientId == "" || Broker.clientManagers.Select(i => i.Key.Equals(clientId)).First() == false)
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid clientId");
            else
                context.Response.SendJSON(new ClientManagerWithMessages(Broker.clientManagers[clientId]));

            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.PUT, PathInfo = "/[clientId]")]
        public IHttpContext UpdateClientManager(IHttpContext context)
        {
            bool.TryParse(context.Request.PathParameters["intercept"], out bool intercept);
            if (!intercept)
            {
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid value for 'intercept'");
            }
            else
            {
                Console.WriteLine(context.Request.PathParameters["clientId"]);
                string clientId = context.Request.PathParameters["clientId"];
                if (clientId == "" || Broker.clientManagers.Select(i => i.Key.Equals(clientId)).First() == false) 
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid clientId");
                else {
                    Broker.clientManagers[clientId].intercept = intercept;
                    context.Response.SendJSON(Broker.clientManagers[clientId]);
                }
            }
            return context;
        }

        
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[clientId]/[whichWay]")]
        public IHttpContext GetClientInfo(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["clientId"]);
            string clientId = context.Request.PathParameters["clientId"];
            if (clientId != "" && Broker.clientManagers.Select(i => i.Key.Equals(clientId)).First() == true) { 
                Console.WriteLine(context.Request.PathParameters["whichWay"]);
                if (context.Request.PathParameters["whichWay"] == "in")
                    context.Response.SendJSON(Broker.clientManagers[clientId].clientIn);
                else if (context.Request.PathParameters["whichWay"] == "out")
                    context.Response.SendJSON(Broker.clientManagers[clientId].clientOut);
                else
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter '[clientId]/in' or '[clientId]/out'");
            }else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid clientId");
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.PUT, PathInfo = "/[clientId]/[whichWay]")]
        public IHttpContext UpdateClient(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["run"]);
            if (bool.TryParse(context.Request.PathParameters["run"], out bool run)) { 
                Console.WriteLine(context.Request.PathParameters["clientId"]);
                string clientId = context.Request.PathParameters["clientId"];
                if (clientId != "" && Broker.clientManagers.Select(i => i.Key.Equals(clientId)).First() == true)
                {
                    Console.WriteLine(context.Request.PathParameters["whichWay"]);
                    if (context.Request.PathParameters["whichWay"] == "in") {
                        Broker.clientManagers[clientId].clientIn.run = run;
                        context.Response.SendJSON(Broker.clientManagers[clientId].clientIn);
                    }
                    else if (context.Request.PathParameters["whichWay"] == "out") {
                        Broker.clientManagers[clientId].clientOut.run = run;
                        context.Response.SendJSON(Broker.clientManagers[clientId].clientOut);
                    }
                    else
                        context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter '[clientId]/in' or '[clientId]/out'");
                }
                else
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid clientId");
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter true/false for 'run'");
            return context;
        }
        /*
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[clientId]/[whichWay]/replay/[msgId]")]
        public IHttpContext GetMessagesOfClientManagers(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["msgId"]);
            var isNumber = int.TryParse(context.Request.PathParameters["msgId"], out int msgId);
            if (isNumber) { 
                Console.WriteLine(context.Request.PathParameters["clientId"]);
                string clientId = context.Request.PathParameters["clientId"];
                if (clientId == "" || Broker.clientManagers.Select(i => i.Key.Equals(clientId)).First() == false)
                    context.Response.SendJSON(Broker.db.messageList.Where(i => i.MsgId == msgId && i.ClientId == clientId).FirstOrDefault());
                else
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid clientId");  
            }else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid msgId");

            return context;
        }*/

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/[clientId]/[whichWay]/send")]
        public IHttpContext SendMessage(IHttpContext context)
        {
            //validation of sent parameters
            bool valid = true;
            valid = int.TryParse(context.Request.QueryString["QoS"], out int QoS);
            valid = bool.TryParse(context.Request.QueryString["Retain"], out bool retain);
            if (QoS < 3 && QoS >= 0) valid = false;
            if (context.Request.QueryString["Payload"] == "" || context.Request.QueryString["Topic"] == "") valid = false;
            var clientId = context.Request.QueryString["ClientId"];
            if (clientId == "" || Broker.clientManagers.Select(i => i.Key.Equals(clientId)).First() == false) valid = false;

            if (valid) {
                Console.WriteLine(context.Request.PathParameters["whichWay"]);
                MQTT_Proxy.Client client = null;
                if (context.Request.PathParameters["whichWay"] == "in")
                {
                    client = Broker.clientManagers[clientId].clientIn;
                }
                else if (context.Request.PathParameters["whichWay"] == "out")
                {
                    client = Broker.clientManagers[clientId].clientOut;
                }
                else { 
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter '[clientId]/in' or '[clientId]/out'");
                    return context;
                }

                //correct 
                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.Payload = Encoding.UTF8.GetBytes(context.Request.QueryString["Payload"]);
                msg.Topic = context.Request.QueryString["Topic"];
                msg.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)QoS;
                msg.Retain = retain;
                client.SendMessage(msg);
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter valid data.");            
                
            return context;
        }


    }
}

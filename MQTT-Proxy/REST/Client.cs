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
    [RestResource(BasePath = "/client")]
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
                context.Response.SendJSON(Broker.clientManagers[clientId]);

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

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[clientId]/[msgId]")]
        public IHttpContext GetMessagesOfClientManagers(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["clientId"]);
            Console.WriteLine(context.Request.PathParameters["msgId"]);
            string clientid = context.Request.PathParameters["clientId"];
            //check if msgId is a valid request / number!!
            string msgId = context.Request.PathParameters["msgId"];
            context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.ServiceUnavailable, new NotImplementedException());
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[clientId]/[whichWay]")]
        public IHttpContext GetClientInfo(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["clientId"]);
            Console.WriteLine(context.Request.PathParameters["whichWay"]);
            string clientid = context.Request.PathParameters["clientId"];
            bool isClientOut = true;
            if (context.Request.PathParameters["whichWay"] == "in")
                isClientOut = false;
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter '[clientId]/in' or '[clientId]/out'");
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.PUT, PathInfo = "/[clientId]/[whichWay]")]
        public IHttpContext UpdateClient(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["clientId"]);
            Console.WriteLine(context.Request.PathParameters["whichWay"]);
            string clientid = context.Request.PathParameters["clientId"];
            bool isClientOut = true;
            if (context.Request.PathParameters["whichWay"] == "in")
                isClientOut = false;
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter '[clientId]/in' or '[clientId]/out'");
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/[clientId]/send")]
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
                MqttApplicationMessage msg = new MqttApplicationMessage();
                msg.Payload = Encoding.UTF8.GetBytes(context.Request.QueryString["Payload"]);
                msg.Topic = context.Request.QueryString["Topic"];
                msg.QualityOfServiceLevel = (MQTTnet.Protocol.MqttQualityOfServiceLevel)QoS;
                msg.Retain = retain;
                Broker.clientManagers[clientId].clientOut.SendMessage(msg);
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter valid data.");            
                
            return context;
        }
    }
}

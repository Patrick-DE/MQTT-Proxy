using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy.REST
{
    [RestResource(BasePath = "/message")]
    class Message
    {
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/")]
        public IHttpContext GetAllMessages(IHttpContext context)
        {
            context.Response.SendJSON(Broker.db.messageList);
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[msgId]")]
        public IHttpContext GetMessage(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["msgId"]);
            var isNumber = int.TryParse(context.Request.PathParameters["msgId"], out int msgId);
            if (isNumber)
                context.Response.SendJSON(Broker.db.messageList.Where(elem => elem.MsgId == msgId));
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Enter a valid msgId");
            return context;
        }

        /*
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.PUT, PathInfo = "/[msgId]")]
        public IHttpContext UpdateMessage(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["msgId"]);
            var isNumber = int.TryParse(context.Request.PathParameters["msgId"], out int msgId);
            if (isNumber) { 
                MQTTMessage msg = Broker.db.messageList.Select(elem => elem.MsgId == msgId).FirstOrDefault();
                if (msg != null) {
                        msg.PayloadString = context.Request.QueryString["PayloadString"];
                        msg.QoS = int.Parse(context.Request.QueryString["QoS"]);
                        msg.RetainMsg = bool.Parse(context.Request.QueryString["RetainMsg"]);
                        msg.Topic = context.Request.QueryString["Topic"];
                }
                else
                {
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Enter a valid msgId");
                }
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Enter a valid msgId");
            return context;
        }*/        
    }
}

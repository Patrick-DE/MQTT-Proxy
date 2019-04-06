using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using MQTTnet;
using MQTTnet.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy.REST
{
    [RestResource(BasePath = "/intercept")]
    class Intercepter
    {
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[msgId]/accept")]
        public IHttpContext Accept(IHttpContext context)
        {
            var isNumber = int.TryParse(context.Request.PathParameters["msgId"], out int msgId);
            if (isNumber) { 
                var msg = Broker.db.messageList.Where(i => i.MsgId == msgId).FirstOrDefault();
                if( msg != null) {
                    try { 
                        Broker.clientManagers[msg.ClientId].clientOut.SendMessage(msg.ToMqttApplicationMessage()).Wait();
                        msg.State = MessageState.Sent;
                        context.Response.SendJSON(msg);
                    }
                    catch(Exception e) { 
                        context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.InternalServerError, e);
                    }
                }
                else
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "There is no message with the given msgId");
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Enter a valid msgId");
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[msgId]/drop")]
        public IHttpContext Drop(IHttpContext context)
        {
            var isNumber = int.TryParse(context.Request.PathParameters["msgId"], out int msgId);
            if (isNumber)
            {
                var msg = Broker.db.messageList.Where(i => i.MsgId == msgId).FirstOrDefault();
                if(msg != null)
                    msg.State = MessageState.Dropped;
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Enter a valid msgId");
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[msgId]/modify")]
        public IHttpContext Modify(IHttpContext context)
        {
            var isNumber = int.TryParse(context.Request.PathParameters["msgId"], out int msgId);
            if (isNumber)
            {
                var msg = Broker.db.messageList.Where(i => i.MsgId == msgId).FirstOrDefault();
                if (msg != null)
                    msg.State = MessageState.Modified;
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Enter a valid msgId");
            return context;
        }


    }
}

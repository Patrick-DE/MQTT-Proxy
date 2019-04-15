using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy.REST
{
    [RestResource(BasePath = "/api/message")]
    class Message
    {
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/all")]
        public IHttpContext GetAllMessages(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
#endif
            context.Response.SendJSON(Broker.db.messageList);
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/new")]
        public IHttpContext CraftNewMessages(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
#endif
            //validation of sent parameters
            MQTTProxyMessage msg1;
            try
            {
                msg1 = JsonConvert.DeserializeObject<MQTTProxyMessage>(context.Request.Payload);
                //Just throw an exception to get into catch
                if (msg1 == null) throw new Exception();
            }
            catch (Exception e)
            {
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, e);
                return context;
            }
            Broker.db.messageList.Add(msg1);
            Broker.wss.SendMessage(msg1);
            context.Response.SendJSON(msg1);
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[msgId]")]
        public IHttpContext GetMessage(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
#endif
            var isNumber = int.TryParse(context.Request.PathParameters["msgId"], out int msgId);
            if (isNumber)
                context.Response.SendJSON(Broker.db.messageList.Where(elem => elem.MsgId == msgId).FirstOrDefault());
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Enter a valid msgId");
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/[msgId]")]
        public IHttpContext UpdateMessage(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
#endif
            var isNumber = int.TryParse(context.Request.PathParameters["msgId"], out int msgId);
            if (isNumber)
            {
                MQTTProxyMessage msg = Broker.db.messageList.FirstOrDefault(elem => elem.MsgId == msgId);
                if (msg != null)
                {
                    try
                    {
                        MQTTProxyMessage newMsg = JsonConvert.DeserializeObject<MQTTProxyMessage>(context.Request.Payload);
                        msg.Payload = newMsg.Payload;
                        context.Response.SendJSON(msg);
                    }
                    catch (Exception e)
                    {
                        context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, e);
                    }
                }
                else
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "There is no message associated with this msgId");
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Enter a valid msgId");
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/[msgId]/copy")]
        public IHttpContext CopyMessage(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
#endif
            if (int.TryParse(context.Request.PathParameters["msgId"], out int msgId))
            {
                MQTTProxyMessage msg = Broker.db.messageList.FirstOrDefault(elem => elem.MsgId == msgId);
                if (msg != null)
                {
                    MQTTProxyMessage newMsg = new MQTTProxyMessage(msg);
                    Broker.db.messageList.Add(newMsg);
                    context.Response.SendJSON(newMsg);
                }
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Enter a valid msgId");
            return context;
        }
    }
}

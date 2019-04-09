using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using MQTTnet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy.REST
{
    [RestResource(BasePath = "/api/manager")]
    class Manager
    {
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/all")]
        public IHttpContext GetAllClientManager(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
#endif
            context.Response.SendJSON(Broker.clientManagers);
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[clientId]")]
        public IHttpContext GetClientManagerInfo(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
#endif
            Console.WriteLine(context.Request.PathParameters["clientId"]);
            string clientId = context.Request.PathParameters["clientId"];
            if (clientId == "" || Broker.clientManagers.Select(i => i.Key.Equals(clientId)).First() == false)
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid clientId");
            else
                context.Response.SendJSON(Broker.clientManagers[clientId]);

            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.PUT, PathInfo = "/[clientId]/intercept/[value]")]
        public IHttpContext UpdateClientManager(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
#endif
            var valid = bool.TryParse(context.Request.PathParameters["value"], out bool intercept);
            if (!valid)
            {
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid value for 'intercept'");
            }
            else
            {
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

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.PUT, PathInfo = "/[clientId]/messages")]
        public IHttpContext GetManagerMessages(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
#endif
            var valid = bool.TryParse(context.Request.PathParameters["value"], out bool intercept);
            if (!valid)
            {
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid value for 'intercept'");
            }
            else
            {
                string clientId = context.Request.PathParameters["clientId"];
                if (Broker.clientManagers.FirstOrDefault(i => i.Key == clientId).Value != null)
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid clientId");
                else
                {
                    Broker.clientManagers[clientId].intercept = intercept;
                    context.Response.SendJSON(new ClientManagerWithMessages(Broker.clientManagers[clientId]).messages);
                }
            }
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[clientId]/[whichWay]")]
        public IHttpContext GetClientInfo(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
#endif
            string clientId = context.Request.PathParameters["clientId"];
            if (clientId != "" && Broker.clientManagers.Select(i => i.Key.Equals(clientId)).First() == true) { 
                if (context.Request.PathParameters["whichWay"] == "clientIn")
                    context.Response.SendJSON(Broker.clientManagers[clientId].clientIn);
                else if (context.Request.PathParameters["whichWay"] == "clientOut")
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
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
#endif
            if (bool.TryParse(context.Request.Payload, out bool run)) { 
                string clientId = context.Request.PathParameters["clientId"];
                if (clientId != "" && Broker.clientManagers.Select(i => i.Key.Equals(clientId)).First() == true)
                {
                    if (context.Request.PathParameters["whichWay"] == "clientIn") {
                        Broker.clientManagers[clientId].clientIn.run = run;
                        context.Response.SendJSON(Broker.clientManagers[clientId].clientIn);
                    }
                    else if (context.Request.PathParameters["whichWay"] == "clientOut") {
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
                    context.Response.SendJSON(Broker.db.messageList.Where(i => i.MsgId == msgId && i.clientId == clientId).FirstOrDefault());
                else
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid clientId");  
            }else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid msgId");

            return context;
        }*/

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/[clientId]/[whichWay]/send")]
        public IHttpContext SendMessage(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
#endif
            //validation of sent parameters
            MQTTProxyMessage msg1;
            try { 
                msg1 = JsonConvert.DeserializeObject<MQTTProxyMessage>(context.Request.Payload);
                if (msg1 == null) throw new Exception();
            }catch(Exception e)
            {
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, e);
                return context;
            }
            var manager = Broker.clientManagers.FirstOrDefault(i => i.Key.Equals(context.Request.PathParameters["clientId"])).Value;

            if (manager != null) {
                Console.WriteLine(context.Request.PathParameters["whichWay"]);
                MQTT_Proxy.Client client = null;
                if (context.Request.PathParameters["whichWay"] == "clientIn")
                {
                    client = manager.clientIn;
                }
                else if (context.Request.PathParameters["whichWay"] == "clientOut")
                {
                    client = manager.clientOut;
                }
                else { 
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter '[clientId]/clientIn' or '[clientId]/clientOut'");
                    return context;
                }

                //correct 
                client.SendMessage(msg1.ToMqttApplicationMessage()).Wait();
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.Ok);
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter valid clientId.");            
                
            return context;
        }


    }
}

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
    [RestResource(BasePath = "/manager")]
    class Manager
    {
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/all")]
        public IHttpContext GetAllClientManager(IHttpContext context)
        {
            context.Response.SendJSON(Broker.clientManagers);
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[managerId]")]
        public IHttpContext GetClientManagerInfo(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["managerId"]);
            string managerId = context.Request.PathParameters["managerId"];
            if (managerId == "" || Broker.clientManagers.Select(i => i.Key.Equals(managerId)).First() == false)
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid managerId");
            else
                context.Response.SendJSON(Broker.clientManagers[managerId]);

            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.PUT, PathInfo = "/[managerId]/intercept/[value]")]
        public IHttpContext UpdateClientManager(IHttpContext context)
        {
            var valid = bool.TryParse(context.Request.PathParameters["value"], out bool intercept);
            if (!valid)
            {
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid value for 'intercept'");
            }
            else
            {
                string managerId = context.Request.PathParameters["managerId"];
                if (managerId == "" || Broker.clientManagers.Select(i => i.Key.Equals(managerId)).First() == false) 
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid managerId");
                else {
                    Broker.clientManagers[managerId].intercept = intercept;
                    context.Response.SendJSON(Broker.clientManagers[managerId]);
                }
            }
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.PUT, PathInfo = "/[managerId]/messages")]
        public IHttpContext GetManagerMessages(IHttpContext context)
        {
            var valid = bool.TryParse(context.Request.PathParameters["value"], out bool intercept);
            if (!valid)
            {
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid value for 'intercept'");
            }
            else
            {
                string managerId = context.Request.PathParameters["managerId"];
                if (Broker.clientManagers.FirstOrDefault(i => i.Key == managerId).Value != null)
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid managerId");
                else
                {
                    Broker.clientManagers[managerId].intercept = intercept;
                    context.Response.SendJSON(new ClientManagerWithMessages(Broker.clientManagers[managerId]).messages);
                }
            }
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[managerId]/[whichWay]")]
        public IHttpContext GetClientInfo(IHttpContext context)
        {
            string managerId = context.Request.PathParameters["managerId"];
            if (managerId != "" && Broker.clientManagers.Select(i => i.Key.Equals(managerId)).First() == true) { 
                if (context.Request.PathParameters["whichWay"] == "clientIn")
                    context.Response.SendJSON(Broker.clientManagers[managerId].clientIn);
                else if (context.Request.PathParameters["whichWay"] == "clientOut")
                    context.Response.SendJSON(Broker.clientManagers[managerId].clientOut);
                else
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter '[managerId]/in' or '[managerId]/out'");
            }else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid managerId");
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.PUT, PathInfo = "/[managerId]/[whichWay]")]
        public IHttpContext UpdateClient(IHttpContext context)
        {
            if (bool.TryParse(context.Request.Payload, out bool run)) { 
                string managerId = context.Request.PathParameters["managerId"];
                if (managerId != "" && Broker.clientManagers.Select(i => i.Key.Equals(managerId)).First() == true)
                {
                    if (context.Request.PathParameters["whichWay"] == "clientIn") {
                        Broker.clientManagers[managerId].clientIn.run = run;
                        context.Response.SendJSON(Broker.clientManagers[managerId].clientIn);
                    }
                    else if (context.Request.PathParameters["whichWay"] == "clientOut") {
                        Broker.clientManagers[managerId].clientOut.run = run;
                        context.Response.SendJSON(Broker.clientManagers[managerId].clientOut);
                    }
                    else
                        context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter '[managerId]/in' or '[managerId]/out'");
                }
                else
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid managerId");
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter true/false for 'run'");
            return context;
        }

        /*
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[managerId]/[whichWay]/replay/[msgId]")]
        public IHttpContext GetMessagesOfClientManagers(IHttpContext context)
        {
            Console.WriteLine(context.Request.PathParameters["msgId"]);
            var isNumber = int.TryParse(context.Request.PathParameters["msgId"], out int msgId);
            if (isNumber) { 
                Console.WriteLine(context.Request.PathParameters["managerId"]);
                string managerId = context.Request.PathParameters["managerId"];
                if (managerId == "" || Broker.clientManagers.Select(i => i.Key.Equals(managerId)).First() == false)
                    context.Response.SendJSON(Broker.db.messageList.Where(i => i.MsgId == msgId && i.managerId == managerId).FirstOrDefault());
                else
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid managerId");  
            }else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter a valid msgId");

            return context;
        }*/

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/[managerId]/[whichWay]/send")]
        public IHttpContext SendMessage(IHttpContext context)
        {
            //validation of sent parameters
            MQTTProxyMessage msg1;
            try { 
                msg1 = JsonConvert.DeserializeObject<MQTTProxyMessage>(context.Request.Payload);
            }catch(Exception e)
            {
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, e);
                return context;
            }

            var managerId = context.Request.QueryString["managerId"];
            var manager = Broker.clientManagers.FirstOrDefault(i => i.Key.Equals(managerId)).Value;

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
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter '[managerId]/in' or '[managerId]/out'");
                    return context;
                }

                //correct 
                client.SendMessage(msg1.ToMqttApplicationMessage()).Wait();
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.Ok);
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "Please enter valid managerId.");            
                
            return context;
        }


    }
}

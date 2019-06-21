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
    [RestResource(BasePath = "/api/policy")]
    class Rule
    {
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/all")]
        public IHttpContext GetAllMessages(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
#endif
            context.Response.SendJSON(Broker.policyManager.rulebook);
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
            Policy rule1;
            try
            {
                rule1 = JsonConvert.DeserializeObject<Policy>(context.Request.Payload);
                //Just throw an exception to get into catch
                if (rule1 == null) throw new Exception();
            }
            catch (Exception e)
            {
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, e);
                return context;
            }
            Broker.policyManager.rulebook.Add(rule1);
            context.Response.SendJSON(rule1);
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/[policyName]")]
        public IHttpContext GetMessage(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
#endif
            string policyName = context.Request.PathParameters["policyName"];
            context.Response.SendJSON(Broker.policyManager.rulebook.Where(elem => elem.name == policyName).FirstOrDefault());
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/[policyName]")]
        public IHttpContext UpdateMessage(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
#endif
            string policyName = context.Request.PathParameters["policyName"];
            Policy policy = Broker.policyManager.rulebook.FirstOrDefault(elem => elem.name == policyName);
            if (policy != null)
            {
                try
                {
                    Policy newPolicy = JsonConvert.DeserializeObject<Policy>(context.Request.Payload);
                    policy.name = newPolicy.name;
                    policy.ifCompareToEqual = newPolicy.ifCompareToEqual;
                    policy.ifCompareToGreater = newPolicy.ifCompareToGreater;
                    policy.ifCompareToSmaller = newPolicy.ifCompareToSmaller;
                    policy.compareTo = newPolicy.compareTo;
                    context.Response.SendJSON(policy);
                }
                catch (Exception e)
                {
                    context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, e);
                }
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "There is no message associated with this msgId");
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.DELETE, PathInfo = "/[policyName]")]
        public IHttpContext DeleteMessage(IHttpContext context)
        {
#if DEBUG
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
#endif
            string policyName = context.Request.PathParameters["policyName"];
            Policy policy = Broker.policyManager.rulebook.FirstOrDefault(elem => elem.name == policyName);
            if (policy != null)
            {
                Broker.policyManager.rulebook.Remove(policy);
                context.Response.SendJSON(policy);
            }
            else
                context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.BadRequest, "There is no message associated with this msgId");
            return context;
        }

#if DEBUG
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.OPTIONS, PathInfo = "/[policyName]")]
        public IHttpContext AddCORSHeader(IHttpContext context)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "OPTIONS, HEAD, GET, DELETE, POST, PUT";
            context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.Ok);
            return context;
        }
#endif
    }
}

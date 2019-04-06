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
    [RestResource(BasePath = "/api/auth")]
    class Authentication
    {
        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.GET, PathInfo = "/status")]
        public IHttpContext Status(IHttpContext context)
        {
            context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.ServiceUnavailable, new NotImplementedException());
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/login")]
        public IHttpContext Login(IHttpContext context)
        {
            context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.ServiceUnavailable, new NotImplementedException());
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/logout")]
        public IHttpContext Logout(IHttpContext context)
        {
            context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.ServiceUnavailable, new NotImplementedException());
            return context;
        }

        [RestRoute(HttpMethod = Grapevine.Shared.HttpMethod.POST, PathInfo = "/pwd")]
        public IHttpContext ChangePassword(IHttpContext context)
        {
            //check if old password same then replace with new one
            context.Response.SendResponse(Grapevine.Shared.HttpStatusCode.ServiceUnavailable, new NotImplementedException());
            return context;
        }
    }
}

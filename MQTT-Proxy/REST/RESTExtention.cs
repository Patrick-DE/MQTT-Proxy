using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy.REST
{
    public static class RESTExtention
    {
        public static void SendJSON<T>(this IHttpResponse resp, T value)
        {
            resp.ContentType = Grapevine.Shared.ContentType.JSON;
            resp.SendResponse(JsonConvert.SerializeObject(value));
        }
    }
}

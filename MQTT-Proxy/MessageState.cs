﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Proxy
{
    public enum MessageState
    {
        New,
        Sent,
        Intercepted,
        Modified,
        Dropped
    }
}

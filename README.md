# MQTT-Proxy
This software enables your to capture mqtt traffic and manipulate the content of the packets.
A paper to this work is written in German and can be found here: [Bachelor Thesis](https://github.com/Patrick-DE/Konzeption-und-Entwicklung-eines-MQTT-Proxies-f-r-Security-Assesments)

## Run
```
Usage:
Minimal form (TargetPort: 1883, Proxy: 127.0.0.1:1883, Web|REST: 80)
./MQTT-Proxy.exe <targetIP>
Short form (Proxy will be: 127.0.0.1:1883, Web|REST: 80)
./MQTT-Proxy.exe <targetIP> <targetPort>
Full form
./MQTT-Proxy.exe <targetIP> <targetPort> <ownIP> <ownPort> <Web|RESTPort>

```
  
## Configuration
- Default Broker: 127.0.0.1:1883
- Default REST/WebUI: 127.0.0.1:80

If you want to change the IP's and Ports
- REST+Websocket+Broker: MQTT-Proxy\Program.cs
- Websocket: \frontend\src\main.js
- WebUI: \frontend\config\prod.env.js or dev.env.js depending on your deployment


## Screenshots
### Clients
![picture of clients](https://i.imgur.com/WT3nDkK.png)

### Client details
![detailed client information](https://i.imgur.com/YFOpJlI.png)

### Messages
![captured messages](https://i.imgur.com/HjjteBA.png)

### Message editor
![edit message payload](https://i.imgur.com/hyjp1oO.png)

### Craft Messages
![create messages](https://i.imgur.com/ha2EvO1.png)

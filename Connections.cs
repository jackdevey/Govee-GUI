using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace GoveeAPI
{
    class Connections
    {
        List<Device> devicesList = new List<Device>();

        // Work out if the govee api is active through the /ping request.
        // if online return 'Online' else return 'Offline'

        public string apiKey = Environment.GetEnvironmentVariable("GOVEE_KEY", EnvironmentVariableTarget.User);
        public string status()
        {
            string data = string.Empty;
            string url = @"https://developer-api.govee.com/ping";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }

            if (data == "Pong") return "Online";
            else return "Offline";
            
        }

        public List<Device> devices()
        {
            int iid = 0;
            string data = string.Empty;
            string url = @"https://developer-api.govee.com/v1/devices";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers["Govee-API-Key"] = apiKey;
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }

            dynamic jsonData = JsonConvert.DeserializeObject(data);
      
            foreach (var obj in jsonData.data.devices)
            {
                var device = new Device();
                device.iid = iid;
                device.device = obj.device;
                device.model = obj.model;
                device.deviceName = obj.deviceName;
                device.controllable = obj.controllable;
                device.retrievable = obj.retrievable;
                device.supportCmds = obj.supportCmds;

                iid = iid + 1;
                devicesList.Add(device);
            }

            return devicesList;
        }

        public bool turn(int iid, string option)
        {
            Device device = devicesList[iid];

            string url = @"https://developer-api.govee.com/v1/devices/control";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers["Govee-API-Key"] = apiKey;
            request.Method = "PUT";
            request.ContentType = "application/json";

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string sendingData = @"
                {
                    'device': '" + device.device + @"',
                    'model': '" + device.model + @"',
                    'cmd': {
                        'name': 'turn',
                        'value': 'off'
                    }
                }
            ";
            streamWriter.Write(sendingData);
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();
            return true;
        }

        public string sendReq(string url, string data, string method, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers["Govee-API-Key"] = apiKey;
            request.Method = method;
            request.ContentType = contentType;

            if(data != "")
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }
            }

            var httpResponse = (HttpWebResponse)request.GetResponse();

            using (Stream stream = httpResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }


            return data;
        }

        public void queryDevice(Device device)
        {
            string url = "https://developer-api.govee.com/v1/devices/state?device=" + HttpUtility.UrlEncode(device.device) + "&model=" + device.model;
            string state = sendReq(url, "", "GET", "");
            dynamic jsonData = JsonConvert.DeserializeObject(state);

            device.on = Convert.ToBoolean(jsonData.data.properties[0].online);
            
            Console.WriteLine("helli");
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Net;
using System.Web;
using System.Windows.Forms;

namespace GoveeAPI
{

    // The class for a single govee device. Public variable
    // names are the same as in the govee api endpoint.

    // Jack Devey 24/01/2021

    class Device
    {
        // Internal id assigned by the program
        public int iid { get; set; }

        // The device MAC address
        public string device { get; set; }

        // Device model number
        public string model { get; set; }

        // Device name, assigned by the user
        public string deviceName { get; set; }

        // Can be controlled by the program
        public bool controllable { get; set; }

        // Can be retrieved by the program
        public bool retrievable { get; set; }

        // Array of commands supported by the device
        public JArray supportCmds { get; set; }

        // Is the device turned on?
        public bool on
        {
            get {
                string url = "https://developer-api.govee.com/v1/devices/state?device=" + HttpUtility.UrlEncode(device) + "&model=" + model;
                Connections conn = new Connections();
                string state = conn.sendReq(url, "", "GET", "");
                dynamic jsonData = JsonConvert.DeserializeObject(state);
                if (jsonData.data.properties[1].powerState == "on") return true;
                else return false;
            }
            set {
                string valStr;
                string data;

                // Convert the value to what govee wants
                if (value) valStr = "on";
                else valStr = "off";

                // Make json data
                data = "{ \"device\": \"" + device + "\", \"model\": \"" + model + "\", \"cmd\": { \"name\": \"turn\", \"value\": \""+ valStr +"\" } }";

                // Send a request
                Connections connections = new Connections();
                connections.sendReq("https://developer-api.govee.com/v1/devices/control", data, "PUT", "application/json");
            }
        }

        // The colour value of the device
        public System.Drawing.Color color
        {
            get
            {
                string url = "https://developer-api.govee.com/v1/devices/state?device=" + HttpUtility.UrlEncode(device) + "&model=" + model;
                Connections conn = new Connections();
                string state = conn.sendReq(url, "", "GET", "");
                dynamic jsonData = JsonConvert.DeserializeObject(state);

                int r = Convert.ToInt32(jsonData.data.properties[3].color.r);
                int g = Convert.ToInt32(jsonData.data.properties[3].color.g);
                int b = Convert.ToInt32(jsonData.data.properties[3].color.b);

                string colorhex = "#" + Convert.ToString(r, 16) + Convert.ToString(g, 16) + Convert.ToString(b, 16);

                return ColorTranslator.FromHtml(colorhex);
            }
            set
            {
                int r, g, b;
                string data;

                // Convert the value to what govee wants
                r = value.R;
                g = value.G;
                b = value.B;

                // Make json data
                data = "{ \"device\": \"" + device + "\", \"model\": \"" + model + "\", \"cmd\": { \"name\": \"color\", \"value\": {\"r\": "+r+ ", \"g\": " + g + ", \"b\": " + b + "} } }";

                // Send a request
                Connections connections = new Connections();
                string back = connections.sendReq("https://developer-api.govee.com/v1/devices/control", data, "PUT", "application/json");

                dynamic json = JObject.Parse(back);

                if (json.code == "200")
                {
                    string message = "Successfully changed the colour of  " + deviceName + " to " + value;
                    string title = "Success";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string message = "Error while changing the colour of " + deviceName + " to " + value;
                    string title = "Error";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public int brightness
        {
            get
            {
                string url = "https://developer-api.govee.com/v1/devices/state?device=" + HttpUtility.UrlEncode(device) + "&model=" + model;
                Connections conn = new Connections();
                string state = conn.sendReq(url, "", "GET", "");
                dynamic jsonData = JsonConvert.DeserializeObject(state);
                return jsonData.data.properties[2].brightness;
            }
            set
            {
                string data;

                // Make json data
                data = "{ \"device\": \"" + device + "\", \"model\": \"" + model + "\", \"cmd\": { \"name\": \"brightness\", \"value\": " + value + " } }";

                // Send a request
                Connections connections = new Connections();
                string back = connections.sendReq("https://developer-api.govee.com/v1/devices/control", data, "PUT", "application/json");

                dynamic json = JObject.Parse(back);

                if(json.code == "200")
                {
                    string message = "Successfully changed the brightness of  " + deviceName + " to " + value + "%";
                    string title = "Success";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string message = "Error while changing the brightness of " + deviceName + " to " + value + "%";
                    string title = "Error";
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace GoveeAPI
{
    public partial class Form1 : Form
    {
        int currentDeviceIID = 0;
        List<Device> devices = new List<Device>();
        Connections connections = new Connections();

        public Form1()
        {
            InitializeComponent();

            string api = Environment.GetEnvironmentVariable("GOVEE_KEY", EnvironmentVariableTarget.User);

            if (api == null)
            {
                firstTimeSetup();
            }else
            {
                connections.apiKey = api;
            } 

            serverStatus.Text = "Status: " + connections.status();
            devices = connections.devices();

            foreach (Device device in devices)
            {
                listBox1.Items.Add(device.deviceName);
            }

            updateUI();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentDeviceIID = listBox1.SelectedIndex;
            updateUI();
        }

        public void updateUI()
        {
            groupBox1.Text = "Controlling " + devices[currentDeviceIID].deviceName;
            if (devices[currentDeviceIID].on)
            {
                button1.Text = "Turn off";
            }
            else
            {
                button1.Text = "Turn on";
            }

            System.Drawing.Color color = devices[currentDeviceIID].color;

            textBox1.Text = HexConverter(color);

            int brightness = devices[currentDeviceIID].brightness;
            trackBar1.Value = brightness;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // When the state of the light is toggled
        private void button1_Click(object sender, EventArgs e)
        {
            if (devices[currentDeviceIID].on)
            {
                devices[currentDeviceIID].on = false;
                button1.Text = "Turn on";
            }else
            {
                devices[currentDeviceIID].on = true;
                button1.Text = "Turn off";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = HexConverter(colorDialog1.Color);
            }
        }

        private void firstTimeSetup()
        {
            Form2 testDialog = new Form2();
            string apikey;

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (testDialog.ShowDialog(this) == DialogResult.OK)
            {
                apikey = testDialog.textBox1.Text;
                Environment.SetEnvironmentVariable("GOVEE_KEY", apikey, EnvironmentVariableTarget.User);
                connections.apiKey = apikey;
            }
            else
            {
                //this.txtResult.Text = "Cancelled";
            }
            testDialog.Dispose();
        }

        private static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private void colour_save_Click(object sender, EventArgs e)
        {
            string hexString = textBox1.Text;
            //replace # occurences
            if (hexString.IndexOf('#') != -1)
                hexString = hexString.Replace("#", "");

            int r, g, b = 0;

            r = int.Parse(hexString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
            g = int.Parse(hexString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
            b = int.Parse(hexString.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            devices[currentDeviceIID].color = System.Drawing.Color.FromArgb(r, g, b);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int brightness = trackBar1.Value;
            devices[currentDeviceIID].brightness = brightness;
        }
    }
}

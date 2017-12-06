/*
 * Created by SharpDevelop.
 * User: uau556
 * Date: 4-12-2017
 * Time: 9:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace MQTT_Client
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private delegate void myUICallBack(string myStr, TextBox ctl);
        static MqttClient client;
        private void myUI(string myStr, TextBox ctl)
        {
            if (this.InvokeRequired)
            {
                myUICallBack myUpdate = new myUICallBack(myUI);
                this.Invoke(myUpdate, myStr, ctl);
            }
            else
            {
                ctl.AppendText(myStr + Environment.NewLine);
            }
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            myUI(System.Text.Encoding.UTF8.GetString(e.Message), MessageTextBox);
        }
		
		public MainForm()
		{
			InitializeComponent();
		}
		
		private void MainForm_Load(object sender, EventArgs e)
        {
            this.AcceptButton = this.ConnectButton;
            QosComboBox.SelectedIndex = 0;
            UserComboBox.SelectedIndex = 0;
            HostComboBox.SelectedIndex = 0;
            SubScrComboBox.SelectedIndex = 0;
        }

        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            if (client != null && client.IsConnected) client.Disconnect();
        }

		
		void PublishButtonClick(object sender, EventArgs e)
		{
	        if (PubMessageTextBox.Text.Length == 0)
            {
                label4.Text = "No message to send";
            }
            else if (PubTopicTextBox.Text.Length == 0)
            {
                label4.Text = "Publish topic can't be empty";
            }
            else if (PubTopicTextBox.Text.IndexOf('#') != -1 || PubTopicTextBox.Text.IndexOf('+') != -1)
            {
                label4.Text = "Publish topic can't include wildcard(# , +)";
            }
            else
            {
                label4.Text = "";
                client.Publish(PubTopicTextBox.Text, Encoding.UTF8.GetBytes(PubMessageTextBox.Text), (byte)QosComboBox.SelectedIndex, RetainCheckBox.Checked);
            }

		}
		void ConnectButtonClick(object sender, EventArgs e)
		{
	        int port;
            //if(HostTextBoxold.Text.Length == 0)
            if(HostComboBox.Text.Length  == 0)
            {
                label4.Text = "Invalid host";
            }
            else if (!Int32.TryParse(PortTextBox.Text, out port))
            {
                label4.Text = "Invalid port";
            }
            else
            {                
                try
                {
                	client = new MqttClient(HostComboBox.Text);
                	//client = new MqttClient(HostTextBoxold.Text);
                    client.ProtocolVersion = MqttProtocolVersion.Version_3_1;
                    
                    //client.Connect(Guid.NewGuid().ToString());
                    //client.Connect(Guid.NewGuid().ToString().Substring(0, 22),textBox1.Text,textBox2.Text,true,120);
                    client.Connect(Guid.NewGuid().ToString().Substring(0, 22),UserComboBox.Text,textBox2.Text,true,120);
                    
                    client.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(client_MqttMsgPublishReceived);
                }
                catch
                {
                    label4.Text = "Can't connect to server";
                }
                if (client != null && client.IsConnected)
                {
                    this.AcceptButton = this.PublishButton;
                    label4.Text = "";
                    MessageTextBox.Clear();
                    SubscribeButton.Enabled = true;
                    PublishButton.Enabled = true;
                    UnsubscribeButton.Enabled = true;
                    DisconnectButton.Enabled = true;
                    ConnectButton.Enabled = false;
                    //HostTextBoxold.Enabled = false;
                    UserComboBox.Enabled = false;
                    PortTextBox.Enabled = false;
                    
                }else{
                	label4.Text = "Connected to server is false";
                }
            }

		}
		void ClearButtonClick(object sender, EventArgs e)
		{
	        MessageTextBox.Clear();
		}
		void DisconnectButtonClick(object sender, EventArgs e)
		{
	        if (client != null && client.IsConnected) client.Disconnect();
            SubscribeButton.Enabled = false;
            PublishButton.Enabled = false;
            UnsubscribeButton.Enabled = false;
            DisconnectButton.Enabled = false;
            ConnectButton.Enabled = true;            
            //HostTextBoxold.Enabled = true;
            UserComboBox.Enabled = true;
            PortTextBox.Enabled = true;
            SubListBox.Items.Clear();

		}
		void SubscribeButtonClick(object sender, EventArgs e)
		{
	        if (SubTopicTextBox.Text.Length == 0)
            {
                label4.Text = "Subscribe topic can't be empty";
            }
            else
            {
                label4.Text = "";
                client.Subscribe(new string[] { SubTopicTextBox.Text }, new byte[] { (byte)QosComboBox.SelectedIndex });
                SubListBox.Items.Add(SubTopicTextBox.Text);
            }

		}
		void UnsubscribeButtonClick(object sender, EventArgs e)
		{
	        if (SubListBox.SelectedItem == null)
            {
                label4.Text = "Select topic to unscribe";
            }
            else
            {
                label4.Text = "";
                client.Unsubscribe(new string[] { SubListBox.SelectedItem.ToString() });
                SubListBox.Items.Remove(SubListBox.SelectedItem);
            }

		}
		void TextBox1TextChanged(object sender, EventArgs e)
		{
	
		}
		void Label11Click(object sender, EventArgs e)
		{
	
		}
		void HostComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
	
		}
		void UserComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
	
		}
		void SubScrComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			SubTopicTextBox.Text = SubScrComboBox.SelectedItem.ToString() ;
		}
		void QosComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
	
		}
		void Label12Click(object sender, EventArgs e)
		{
	
		}
	}
}

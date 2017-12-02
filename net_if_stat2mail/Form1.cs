using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace net_if_stat2mail
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static string GetLocalIpAddress()
        {
            foreach (var netI in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netI.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                    (netI.NetworkInterfaceType != NetworkInterfaceType.Ethernet ||
                     netI.OperationalStatus != OperationalStatus.Up)) continue;
                foreach (var uniIpAddrInfo in netI.GetIPProperties().UnicastAddresses.Where(x => netI.GetIPProperties().GatewayAddresses.Count > 0))
                {

                    if (uniIpAddrInfo.Address.AddressFamily == AddressFamily.InterNetwork &&
                        uniIpAddrInfo.AddressPreferredLifetime != uint.MaxValue)
                        

               return uniIpAddrInfo.Address.ToString(); }
            }
           // Logger.Log("You local IPv4 address couldn't be found...");
            return null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string p2="R", name="", rx_byte, tx_byte, rx_packet, tx_packet;
            int p1=0;
          //  label1.Text = GetLocalIpAddress();
            foreach (var netI in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netI.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                    (netI.NetworkInterfaceType != NetworkInterfaceType.Ethernet ||
                     netI.OperationalStatus != OperationalStatus.Up)) continue;
                foreach (var uniIpAddrInfo in netI.GetIPProperties().UnicastAddresses.Where(x => netI.GetIPProperties().GatewayAddresses.Count > 0))
                {
                 //   if (uniIpAddrInfo.Address.AddressFamily == AddressFamily.InterNetwork &&
                 //       uniIpAddrInfo.AddressPreferredLifetime != uint.MaxValue)
                                  
                }
            name = netI.Name;
                    rx_byte = netI.GetIPv4Statistics().BytesSent.ToString();
                    tx_byte = netI.GetIPv4Statistics().BytesReceived.ToString();
                    rx_packet = netI.GetIPv4Statistics().UnicastPacketsSent.ToString();
                    tx_packet = netI.GetIPv4Statistics().UnicastPacketsReceived.ToString();
                    label2.Text += p1++.ToString()+"_"+name +"_"+ rx_byte + "_"+tx_byte+Environment.NewLine;
            }
        }
    }
}

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace net_if_stat2mail
{
    public partial class Form1 : Form
    {public string path=@"e:\!\";//+net_if_stat2mail.cfg
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
            string data = "", fn = "";
            //формировать имя файла по типу
            // имякомп_день-мес-год_час-мин-сек_время аптайма.txt
            string p2 ="R", name="", rx_byte, tx_byte, rx_packet, tx_packet;
            int p1=0;
          //  label1.Text = GetLocalIpAddress();
       foreach (var netI in NetworkInterface.GetAllNetworkInterfaces())
         {if (netI.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
           (//netI.NetworkInterfaceType != NetworkInterfaceType.Ethernet ||
            netI.OperationalStatus != OperationalStatus.Up)) continue;
            name = (netI.Name+"        ").Substring(0,8);
            rx_byte = "                  "+netI.GetIPv4Statistics().BytesSent.ToString();
            rx_byte = rx_byte.Substring(rx_byte.Length - 18, 18);
            tx_byte = "                  " + netI.GetIPv4Statistics().BytesReceived.ToString();//18 symb
            tx_byte = tx_byte.Substring(tx_byte.Length - 18, 18);
            rx_packet = "                  " + netI.GetIPv4Statistics().UnicastPacketsSent.ToString();
            rx_packet = rx_packet.Substring(rx_packet.Length - 18, 18);
            tx_packet = "                  " + netI.GetIPv4Statistics().UnicastPacketsReceived.ToString();
            tx_packet = tx_packet.Substring(tx_packet.Length - 18, 18);
                //
                string ip_addr="";
                foreach (var uniIpAddrInfo in netI.GetIPProperties().UnicastAddresses.Where(x => netI.GetIPProperties().GatewayAddresses.Count > 0))
                {ip_addr+=uniIpAddrInfo.Address.ToString()+"_";}
            
            //
            
            label2.Text += 
                    p1++.ToString()+"_"+name +"_"+ rx_byte + "_"+tx_byte+"_"+ip_addr+Environment.NewLine;
            data+=
                    p1++.ToString() + "_" + name + "_" + rx_byte + "_" + tx_byte + "_" + ip_addr + Environment.NewLine;
            }
            //скидываем в файл
            string s = DateTime.Now.ToString("dd-MMMM-yyyy_HH-mm");
            string[] m_ru = {"января", "февраля","марта", "апреля","мая", "июня", "июля", "августа", "сентября", "октября", "ноября", "декабря" };
            string[] m_en = {"jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };
            for (int i = 0; i < 12; i++) s=s.Replace(m_ru[i], m_en[i]);
            //формируем имя файла
            string mname = Environment.MachineName, upti = UpTime.ToString().Remove(UpTime.ToString().LastIndexOf("."));
            label1.Text = mname + "_" + s+ "_" + upti;
            fn = mname + "_" + s + "_" + upti;
            fn = fn.Replace(":", "_");
            fn += ".txt";
            File.WriteAllText(@path+@fn,data);
            //отправляем файл на почту...
            send_mail(@path + @fn);
            button1.Text += "ok";}
        public TimeSpan UpTime
        {get
            {using (var uptime = new PerformanceCounter("System", "System Up Time"))
                {uptime.NextValue();       //Call this an extra time before reading its value
                 return TimeSpan.FromSeconds(uptime.NextValue());}}}
        public void send_mail(string attach)
        {
            if (File.Exists(@path + "net_if_stat2mail.cfg")){            
        //Format net_if_stat2mail.cfg: servsmtp/n port/n login/n pass/n 2mail
                string[] cfg = File.ReadAllLines(@path + "net_if_stat2mail.cfg");
                if (cfg.Length == 5) {
                    //smtp сервер
                    string smtpHost = cfg[0];//"smtp.gmail.com";                         
                    int smtpPort = Convert.ToInt16(cfg[1]);//587;//smtp порт                                        
                    string login = cfg[2];//логин
                    string pass = cfg[3];//пароль 
                    SmtpClient client = new SmtpClient(smtpHost, smtpPort);//создаем подключение
                    client.Credentials = new NetworkCredential(login, pass);
                    client.EnableSsl = true;
                    string from = cfg[2]; ;//От кого письмо
                    string to = cfg[4]; ;//Кому письмо
                    string subject = "Письмо от local"+ DateTime.Now.ToString("dd-MMMM-yyyy_HH-mm"); ;  
                    string body = "";//Текст письма                   
//Вложение для письма Если нужно не одно вложение, для каждого создаем отдельный Attachment
                    Attachment attData = new Attachment(@attach);
                    MailMessage mess = new MailMessage(from, to, subject, body);//Создаем сообщение  
                    mess.Attachments.Add(attData);//прикрепляем вложение
                    mess.SubjectEncoding = Encoding.UTF8;//прописываем заголовок 
                    mess.BodyEncoding = Encoding.UTF8;
                    //   mess.Headers["Content-type"] = "text/plain; charset=windows-1251";
                    try { client.Send(mess); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                } else MessageBox.Show("Read stricture config file_"+ cfg.Length.ToString());
            } else MessageBox.Show("Error read config file");
        }
        private void button2_Click(object sender, EventArgs e)
        {
          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
    }
 
}

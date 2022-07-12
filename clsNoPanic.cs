using System;
using System.Configuration;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace NoPanic
{
    class clsNoPanic
    {
        public bool Etat = true;
        
        private static int Port_Alerte = Properties.Settings.Default.Port_Alerte;
        private static int Port_Presence = Properties.Settings.Default.Port_Presence;
        private int nbrTest = 1;
        private string mIP;
        private UdpClient udp_alerte;
        private UdpClient udp_presence;
        private IAsyncResult ar_alerte = null;
        private IAsyncResult ar_presence = null;
        private System.Timers.Timer tTestPresence = new System.Timers.Timer();

        public clsNoPanic() {
            tTestPresence.Elapsed += new ElapsedEventHandler(Envoyer_Presence);

            try {
                udp_alerte = new UdpClient(Port_Alerte);
                udp_alerte.Client.ReceiveBufferSize = 4096;
                udp_alerte.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udp_presence = new UdpClient(Port_Presence);
                udp_presence.Client.ReceiveBufferSize = 4096;
                udp_presence.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList) { if (ip.AddressFamily == AddressFamily.InterNetwork) { mIP = ip.ToString(); break; } }
            } catch { Etat = false; }

            Ecouter_Alerte();
            Ecouter_Presence();

            if (Properties.Settings.Default.TestPresence_Frequence != 0) {
                tTestPresence.Interval = Properties.Settings.Default.TestPresence_Frequence * 60000;
                Envoyer_Presence(null, null);
                tTestPresence.Enabled = true;
            }
        }

        private void Ecouter_Alerte() {
            try { ar_alerte = udp_alerte.BeginReceive(Recevoir_Alerte, new object()); } catch { Etat = false; }
        }
        private void Ecouter_Presence() {
            try { ar_presence = udp_presence.BeginReceive(Recevoir_Presence, new object()); } catch { Etat = false; }
        }
        private void Recevoir_Alerte(IAsyncResult ar_alerte) {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port_Alerte);
            byte[] bytes = udp_alerte.EndReceive(ar_alerte, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            if (message.StartsWith("NoPanic|Alerte|") && (mIP != ip.Address.ToString())) { message = message.Split('|')[2]; } else { Ecouter_Alerte(); return; }
            switch (message) {
                case "CONFIRMATION":
                    if (Properties.Settings.Default.Alerte_Confirmation != "") {
                        bool frmExist = false;
                        FormCollection fc = Application.OpenForms;
                        foreach (Form frm in fc) { if (frm.Text == "Alerte") { frmExist = true; break; } }
                        if (frmExist == false) {
                            frmAlerte fAlert1 = new frmAlerte(Properties.Settings.Default.Alerte_Confirmation);
                            Ecouter_Alerte();
                            fAlert1.ShowDialog();
                        } else { Ecouter_Alerte(); }
                    }
                    break;
                default:
                    Envoyer(ip.Address.ToString(), "Alerte|CONFIRMATION", Port_Alerte);
                    try {
                        if (Properties.Settings.Default.Alerte_Son) {
                            SoundPlayer player = new SoundPlayer(Properties.Resources.Alerte);
                            player.Load();
                            player.Play();
                        }
                    } catch { }
                    frmAlerte fAlert2 = new frmAlerte(message);
                    Ecouter_Alerte();
                    fAlert2.ShowDialog();
                    break;
            }
        }
        private void Recevoir_Presence(IAsyncResult ar_presence) {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port_Presence);
            byte[] bytes = udp_presence.EndReceive(ar_presence, ref ip);
            string message = Encoding.ASCII.GetString(bytes);
            if (message.StartsWith("NoPanic|Presence|") && (mIP != ip.Address.ToString())) { message = message.Split('|')[2]; } else { Ecouter_Presence(); return; }
            switch (message) {
                case "PRESENCE":
                    Envoyer(ip.Address.ToString(), "Presence|PRESENT", Port_Presence);
                    break;
                case "PRESENT":
                    tTestPresence.Interval = Properties.Settings.Default.TestPresence_Frequence * 60000;
                    nbrTest = 0;
                    Etat = true;
                    break;
                case "CONFIGURER":
                    Envoyer(ip.Address.ToString(), "Presence|CONFIGURATION|" + Environment.MachineName + "|" + System.Reflection.Assembly.GetEntryAssembly().Location + "|" + ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath + "|" + Environment.UserName, Port_Presence);
                    break;
            }
            Ecouter_Presence();
        }
        private void Envoyer_Presence(object source, ElapsedEventArgs e) {
            nbrTest = nbrTest + 1;
            if (nbrTest >= 2) { 
                Etat = false;
                tTestPresence.Interval = 30000;
            }
            foreach (string IP in Properties.Settings.Default.TestPresence_IP.Split(',')) {
                Envoyer(IP, "Presence|PRESENCE", Port_Presence);
            }
        }

        public void Envoyer(string sIP, string sMessage, int Port) {
            try {
                UdpClient client = new UdpClient(Port);
                IPEndPoint ip = null;
                if (sIP.Contains(".")) {
                    ip = new IPEndPoint(IPAddress.Parse(sIP), Port);
                } else {
                    ip = new IPEndPoint(Dns.GetHostAddresses(sIP)[0], Port);
                }
                byte[] bytes = Encoding.ASCII.GetBytes("NoPanic|" + sMessage);
                client.Send(bytes, bytes.Length, ip);
                client.Close();
            } catch { Etat = false; }
        }

        ~clsNoPanic() {
            try { udp_alerte.Close(); udp_presence.Close(); } catch { }
        }
    }
}
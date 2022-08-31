using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Media;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace NoPanic
{
    class ClsNoPanic
    {
        private long Present;
        private string IP;
        private UdpClient u;
        private static readonly int Port_Ecoute = Properties.Settings.Default.Port_Ecoute;
        private static readonly int Port_Envoi = Properties.Settings.Default.Port_Envoi;
        private readonly System.Timers.Timer tTestPresence = new System.Timers.Timer();

        public delegate void Etat_Change_Event();
        public event Etat_Change_Event Etat_Change;
        private int _Etat;
        public int Etat
        {
            get { return _Etat; }
            set {
                if (_Etat == value) { return; }
                _Etat = value;
                Etat_Change?.Invoke();
            }
        }

        public ClsNoPanic()
        {
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(Change_IP);
            tTestPresence.Elapsed += new ElapsedEventHandler(Envoyer_Presence);
            Change_IP(null, null);
        }
        private void Change_IP(object sender, EventArgs e) {
            tTestPresence.Enabled = false;
            if (u != null) { u.Close(); }
            IP = "";
            Present = 1;

            try {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList) {
                    if ((ip.AddressFamily == AddressFamily.InterNetwork) && (ip.ToString().StartsWith(Properties.Settings.Default.InterfaceFilter_IP))) {
                        IP = ip.ToString();
                        break;
                    }
                }
                if (IP == "") {
                    Etat = 1;
                } else {
                    Etat = 2;
                    u = new UdpClient(new IPEndPoint(IPAddress.Parse(IP), Port_Ecoute));
                    u.Client.ReceiveBufferSize = 4096;
                    u.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    Ecouter();

                    if (Properties.Settings.Default.TestPresence_Frequence != 0) {
                        tTestPresence.Interval = Properties.Settings.Default.TestPresence_Frequence * 1000 / 2;
                        Envoyer_Presence(null, null);
                        tTestPresence.Enabled = true;
                    }
                }
            } catch { Etat = 0; }
        }
        
        private void Ecouter() {
            try { IAsyncResult u_ar = u.BeginReceive(Recevoir, new object()); } catch { }
        }
        private void Recevoir(IAsyncResult u_ar) {
            try {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port_Ecoute);
                byte[] bytes = u.EndReceive(u_ar, ref ip);
                string message = Encoding.ASCII.GetString(bytes);

                if (message.StartsWith("NoPanic|") && (IP != ip.Address.ToString())) { message = message.Split('|')[1]; } else { Ecouter(); return; }
                switch (message) {
                    case "ALERTE":
                        if (Properties.Settings.Default.Alerte_Confirmation != "") {
                            bool frmExist = false;
                            FormCollection fc = Application.OpenForms;
                            foreach (Form frm in fc) { if (frm.Text == "Alerte") { frmExist = true; break; } }
                            if (frmExist == false) {
                                frmAlerte fAlert1 = new frmAlerte(Properties.Settings.Default.Alerte_Confirmation);
                                fAlert1.ShowDialog();
                            }
                        }
                        break;
                    case "PRESENT":
                        Etat = 2;
                        Present = 0;
                        break;
                    case "PRESENCE":
                        Envoyer(ip.Address.ToString(), "PRESENT");
                        Etat = 2;
                        Present = 0;
                        break;
                    case "CONFIGURER":
                        Envoyer(ip.Address.ToString(), "CONFIGURATION|" + Environment.MachineName + "|" + System.Reflection.Assembly.GetEntryAssembly().Location + "|" + ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath + "|" + Environment.UserName);
                        break;
                    default:
                        Envoyer(ip.Address.ToString(), "ALERTE");
                        try {
                            if (Properties.Settings.Default.Alerte_Son) {
                                SoundPlayer player = new SoundPlayer(Properties.Resources.Alerte);
                                player.Load();
                                player.Play();
                            }
                        } catch { }
                        frmAlerte fAlert2 = new frmAlerte(message);
                        fAlert2.ShowDialog();
                        break;
                }
            } catch { }
            Ecouter();
        }
        private void Envoyer(string sIP, string sMessage) {
            try {
                UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Parse(IP), Port_Envoi));
                IPEndPoint mip = null;
                if (sIP.Contains(".")) {
                    mip = new IPEndPoint(IPAddress.Parse(sIP), Port_Ecoute); 
                } else {
                    mip = new IPEndPoint(Dns.GetHostAddresses(sIP)[0], Port_Ecoute);
                }
                byte[] bytes = Encoding.ASCII.GetBytes("NoPanic|" + sMessage);
                client.Send(bytes, bytes.Length, mip);
                client.Close();
            } catch { }
        }
        
        private void Envoyer_Presence(object source, ElapsedEventArgs e) {
            Present += 1;
            if (Present >= 2) {
                foreach (string IP in Properties.Settings.Default.TestPresence_IP.Split(',')) {
                    Envoyer(IP, "PRESENCE");
                }
            }
            if (Present >= 3) { Etat = 0; }
        }
        public void Envoyer_Alerte() {
            string Alerte_Message = Properties.Settings.Default.Alerte_Message;
            
            try {
                UserPrincipal userPrincipal = UserPrincipal.Current;
                if (userPrincipal.DisplayName == null) {
                    Alerte_Message = Alerte_Message.Replace("%NOM%", Environment.UserName);
                } else {
                    Alerte_Message = Alerte_Message.Replace("%NOM%", userPrincipal.DisplayName);
                }
            }
            catch { Alerte_Message = Alerte_Message.Replace("%NOM%", Environment.UserName); }

            Alerte_Message = Alerte_Message.Replace("%LOGIN%", Environment.UserName);
            
            foreach (string IP in Properties.Settings.Default.Alerte_IP.Split(',')) {
                Envoyer(IP, Alerte_Message);
            }
        }

        ~ClsNoPanic() { try { u.Close(); } catch { } }
    }
}
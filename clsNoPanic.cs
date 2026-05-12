using System;
using System.DirectoryServices.AccountManagement;
using System.Media;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.Mail;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.IO;

namespace NoPanic
{
    internal class ClsNoPanic : IDisposable
    {
        private int Present = 1;
        private string IP = "";
        private UdpClient u = null;
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
        private void Change_IP(object sender, EventArgs e)
        {
            tTestPresence.Enabled = false;
            u?.Close();
            IP = "";
            Present = 1;

            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if ((ip.AddressFamily == AddressFamily.InterNetwork) && ip.ToString().StartsWith(Properties.Settings.Default.InterfaceFilter_IP))
                    {
                        IP = ip.ToString();
                        break;
                    }
                }
                if (IP == "")
                {
                    Etat = 1;
                }
                else
                {
                    Etat = 2;
                    u = new UdpClient(new IPEndPoint(IPAddress.Parse(IP), Port_Ecoute));
                    u.Client.ReceiveBufferSize = 4096;
                    u.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    Ecouter();

                    if (Properties.Settings.Default.TestPresence_Frequence != 0)
                    {
                        tTestPresence.Interval = Properties.Settings.Default.TestPresence_Frequence * 1000 / 2;
                        Envoyer_Presence(null, null);
                        tTestPresence.Start();
                    }
                }
            }
            catch { Etat = 0; }
        }

        private void Ecouter()
        {
            try { IAsyncResult u_ar = u.BeginReceive(Recevoir, new object()); } catch { Change_IP(null, null); }
        }
        private void Recevoir(IAsyncResult u_ar)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, Port_Ecoute);
                byte[] bytes = u.EndReceive(u_ar, ref ip);
                string message = Encoding.UTF8.GetString(bytes);
                
                var parts = message.Split('|');
                if (parts.Length < 3) return;

                if (message.StartsWith("NOPANIC|") && message.EndsWith("|END")  && (IP != ip.Address.ToString()))
                {
                    message = message.Split('|')[1];
                    switch (message)
                    {
                        case "CONFIRM":
                            if (Properties.Settings.Default.Alerte_Confirmation != "")
                            {
                                bool frmExist = false;
                                FormCollection fc = Application.OpenForms;
                                foreach (Form frm in fc) { if (frm.Text == "Alerte") { frmExist = true; break; } }
                                if (frmExist == false)
                                {
                                    LogAlerte("RECU", ip.Address.ToString(), "ALERTE - CONFIRMATION");
                                    frmAlerte fAlert1 = new frmAlerte(Properties.Settings.Default.Alerte_Confirmation);
                                    _ = fAlert1.ShowDialog();
                                }
                            }
                            break;
                        case "PRESENT":
                            Etat = 2;
                            Present = 0;
                            break;
                        case "PRESENCE":
                            Envoyer(ip.Address.ToString(), "PRESENT");
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(message)) { return; }
                            LogAlerte("RECU", ip.Address.ToString(), "ALERTE - " + message);

                            Envoyer(ip.Address.ToString(), "CONFIRM");
                            if (Properties.Settings.Default.Alerte_Son)
                            {
                                try {
                                    using (SoundPlayer player = new SoundPlayer(Properties.Resources.Alerte)) {
                                        player.Load();
                                        player.Play();
                                    }
                                } catch { }
                            }

                            frmAlerte fAlert2 = new frmAlerte(message);
                            _ = fAlert2.ShowDialog();
                            break;
                    }
                }
            }
            catch { Etat = 0; }
            finally { if (u != null) { Ecouter(); } }
        }
        private void Envoyer(string sIP, string sMessage)
        {
            try
            {
                using (var client = new UdpClient())
                {
                    IPEndPoint mip = sIP.Contains(".")
                        ? new IPEndPoint(IPAddress.Parse(sIP), Port_Ecoute)
                        : new IPEndPoint(Dns.GetHostAddresses(sIP)[0], Port_Ecoute);
                    byte[] bytes = Encoding.UTF8.GetBytes("NOPANIC|" + sMessage + "|END");
                    client.Send(bytes, bytes.Length, mip);
                }
            } 
            catch { Etat = 0; } 
        }

        private void Envoyer_Presence(object source, ElapsedEventArgs e)
        {
            if (Present < 3) { Present++; }
            if (Present >= 2)
            {
                foreach (string mIP in Properties.Settings.Default.TestPresence_IP.Split(','))
                {
                    Envoyer(mIP.Trim(), "PRESENCE");
                }
            }
            if (Present == 3) { Etat = 0; }
        }
        public void Envoyer_Alerte()
        {
            bool frmExist = false;
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc) { if (frm.Text == "Alerte") { frmExist = true; break; } }
            if (frmExist == false)
            {
                string Alerte_Message = Properties.Settings.Default.Alerte_Message ?? string.Empty;
                string login = !string.IsNullOrWhiteSpace(Environment.UserName)
                    ? Environment.UserName
                    : Environment.MachineName;

                string nomUtilisateur = login;
                try {
                    var displayName = UserPrincipal.Current?.DisplayName;
                    if (!string.IsNullOrWhiteSpace(displayName)) { nomUtilisateur = displayName; }
                } catch { }

                Alerte_Message = Alerte_Message.Replace("%NOM%", nomUtilisateur).Replace("%LOGIN%", login);

                foreach (string mIP in Properties.Settings.Default.Alerte_IP.Split(','))
                {
                    Envoyer(mIP.Trim(), Alerte_Message);
                }
                LogAlerte("ENVOYE", Environment.MachineName, Alerte_Message);

                if ((Properties.Settings.Default.Alerte_Mail_From != "") && (Properties.Settings.Default.Alerte_Mail_To != "") && (Properties.Settings.Default.Alerte_SMTP != ""))
                {
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(Properties.Settings.Default.Alerte_Mail_From)
                    };
                    mailMessage.To.Add(Properties.Settings.Default.Alerte_Mail_To);
                    mailMessage.Subject = Properties.Settings.Default.Alerte_Titre;
                    mailMessage.Body = Properties.Settings.Default.Alerte_Titre + " " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " : " + Alerte_Message;
                    SmtpClient smtpClient = new SmtpClient
                    {
                        Host = Properties.Settings.Default.Alerte_SMTP,
                        Port = Properties.Settings.Default.Alerte_SMTP_Port
                    };
                    try { smtpClient.Send(mailMessage); } catch { }
                }
            }
        }

        private void LogAlerte(string type, string ip, string message)
        {
            try
            {
                string dossier = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"NoPanic");
                
                Directory.CreateDirectory(dossier);
                string fichier = Path.Combine(dossier, "alertes.log");

                if (File.Exists(fichier))
                {
                    FileInfo fi = new FileInfo(fichier);
                    if (fi.Length > 1_000_000)
                    {
                        string backup = Path.Combine(dossier, "alertes.bak");
                        if (File.Exists(backup)) { File.Delete(backup); }
                        File.Move(fichier, backup);
                    }
                }

                string ligne = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {type,-7} | {ip} | {message}";
                File.AppendAllText(fichier, ligne + Environment.NewLine, Encoding.UTF8);
            } catch { }
        }

        public void Dispose()
        {
            tTestPresence?.Stop();
            tTestPresence?.Dispose();
            u?.Close();
        }
    }
}
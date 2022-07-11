using System;
using System.Runtime.InteropServices;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;

namespace NoPanic
{
    public partial class frmPrincipale : Form
    {
        // Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static int Alerte_Touche = Properties.Settings.Default.Alerte_Touche;
        private bool nbrEtat = false;
        private clsNoPanic udp_alerte = new clsNoPanic();

        public frmPrincipale() {
            InitializeComponent();
            lblDescription.Text = "Programme d'alerte en cas d'agresssion";
            lblVersion.Text = "NoPanic - Version 1.1 - SIDSIC 54";
            lblEtat.Text = "Etat de fonctionnement : OK";
            RegisterHotKey(Handle, 42512, Alerte_Touche, (int)Keys.F12);
            switch (Alerte_Touche) {
                case 1:
                    tiNoPanic.Text = "NoPanic - ALT+F12";
                    break;
                case 2:
                    tiNoPanic.Text = "NoPanic - CTRL+F12";
                    break;
                case 3:
                    tiNoPanic.Text = "NoPanic - CTRL+ALT+F12";
                    break;
                case 4:
                    tiNoPanic.Text = "NoPanic - SHIFT+F12";
                    break;
                case 5:
                    tiNoPanic.Text = "NoPanic - SHIFT+ALT+F12";
                    break;
                case 6:
                    tiNoPanic.Text = "NoPanic - CTRL+SHIFT+F12";
                    break;
                case 7:
                    tiNoPanic.Text = "NoPanic - CTRL+ALT+SHIFT+F12";
                    break;
                default:
                    tiNoPanic.Text = "NoPanic";
                    break;
            }
            if (Properties.Settings.Default.TestPresence_Frequence != 0) {
                tmrEtat.Interval = Properties.Settings.Default.TestPresence_Frequence * 30000;
                tmrEtat_Tick(null, null);
                tmrEtat.Enabled = true;
            }
        }
        private void tiNoPanic_Click(object sender, MouseEventArgs e) {
            btnFermer.Focus();
            Show();
        }
        private void btnFermer_Click(object sender, EventArgs e) {
            Hide();
        }
        private void btnQuitter_Click(object sender, EventArgs e) {
            if (Properties.Settings.Default.Quitter_Message == "") {
                UnregisterHotKey(Handle, 42512);
                Application.Exit();
            }
            DialogResult result = MessageBox.Show(Properties.Settings.Default.Quitter_Message, Properties.Settings.Default.Quitter_Titre, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes) {
                UnregisterHotKey(Handle, 42512);
                Application.Exit();
            }
        }
        private void frmPrincipale_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Hide();
            }
        }
        protected override void WndProc(ref Message m) {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 42512) {
                string Alerte_Message = Properties.Settings.Default.Alerte_Message;

                try {
                    UserPrincipal userPrincipal = UserPrincipal.Current;
                    Alerte_Message = Alerte_Message.Replace("%NOM%", userPrincipal.DisplayName);
                } catch {
                    Alerte_Message = Alerte_Message.Replace("%NOM%", Environment.UserName);
                }
                Alerte_Message = Alerte_Message.Replace("%LOGIN%", Environment.UserName);
                foreach (string IP in Properties.Settings.Default.Alerte_IP.Split(',')) {
                    udp_alerte.Envoyer(IP, Alerte_Message, Properties.Settings.Default.Port_Alerte);
                }
            }
            base.WndProc(ref m);
        }
        private void tmrEtat_Tick(object sender, EventArgs e) {
            if (udp_alerte.Etat == true) {
                nbrEtat = false;
                tiNoPanic.Icon = Properties.Resources.Etat_OK;
                lblEtat.Text = "Etat de fonctionnement : OK";
            } else {
                tiNoPanic.Icon = Properties.Resources.Etat_NOK;
                lblEtat.Text = "Etat de fonctionnement : ERREUR";
                if ((nbrEtat == false) && (Properties.Settings.Default.Erreur_Message != "")) { 
                    tiNoPanic.ShowBalloonTip(700, "Avertissement", Properties.Settings.Default.Erreur_Message, ToolTipIcon.Warning);
                    nbrEtat = true;
                }
            }
        }
    }
}
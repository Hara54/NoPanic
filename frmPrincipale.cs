using System;
using System.Reflection;
using System.Runtime.InteropServices;
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

        private static readonly int Alerte_Touche = Properties.Settings.Default.Alerte_Touche;
        private readonly ClsNoPanic udp_alerte = new ClsNoPanic();

        public frmPrincipale() {
            InitializeComponent();
            lblDescription.Text = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Name +  " - Version " + Assembly.GetExecutingAssembly().GetName().Version;
            udp_alerte.Etat_Change += Etat_Update;
            udp_alerte.Etat = true;

            string Key = "";
            RegisterHotKey(Handle, 42512, Alerte_Touche, (int)Keys.F12);
            switch (Alerte_Touche) {
                case 1:
                    Key = "ALT+F12";
                    break;
                case 2:
                    Key = "CTRL+F12";
                    break;
                case 3:
                    Key = "CTRL+ALT+F12";
                    break;
                case 4:
                    Key = "SHIFT+F12";
                    break;
                case 5:
                    Key = "SHIFT+ALT+F12";
                    break;
                case 6:
                    Key = "CTRL+SHIFT+F12";
                    break;
                case 7:
                    Key = "CTRL+ALT+SHIFT+F12";
                    break;
                default:
                    Key = "INCONNUE";
                    break;
            }
            lblKey.Text = Key;
            tiNoPanic.Text = Assembly.GetExecutingAssembly().GetName().Name + " - " + Key;
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
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 42512) { udp_alerte.Envoyer_Alerte(); }
            base.WndProc(ref m);
        }
        private void Etat_Update() {
            if (udp_alerte.Etat == true) {
                tiNoPanic.Icon = Properties.Resources.Etat_OK;
                lblEtat.Text = "OK";
                lblEtat.ForeColor = System.Drawing.Color.DarkGreen;
            } else {
                tiNoPanic.Icon = Properties.Resources.Etat_NOK;
                lblEtat.Text = "ERREUR";
                lblEtat.ForeColor = System.Drawing.Color.DarkRed;
                if (Properties.Settings.Default.Erreur_Message != "") { 
                    tiNoPanic.ShowBalloonTip(700, "Avertissement", Properties.Settings.Default.Erreur_Message, ToolTipIcon.Warning);
                }
            }
        }
    }
}
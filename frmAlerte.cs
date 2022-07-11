using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NoPanic
{
    public partial class frmAlerte : Form
    {
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public frmAlerte(string message)
        {
            InitializeComponent();
            lblAlerte.Text = Properties.Settings.Default.Alerte_Titre;
            lblUtilisateur.Text = message;
        }
        private void frmAlerte_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        private void tmrAlerte_Tick(object sender, EventArgs e) {
            if (lblAlerte.ForeColor == System.Drawing.Color.Red) {
                lblAlerte.ForeColor = System.Drawing.Color.White;
            } else {
                lblAlerte.ForeColor = System.Drawing.Color.Red;
            }
        }
        private void btnAlerte_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
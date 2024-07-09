using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NoPanic
{
    public partial class frmAlerte : Form
    {
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public frmAlerte(string message)
        {
            InitializeComponent();
            lblAlerte.Text = Properties.Settings.Default.Alerte_Titre;
            lblUtilisateur.Text = message;
        }
        private void FrmAlerte_Load(object sender, EventArgs e)
        {
            _ = SetWindowPos(Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        private void TmrAlerte_Tick(object sender, EventArgs e)
        {
            lblAlerte.ForeColor = lblAlerte.ForeColor == System.Drawing.Color.Red ? System.Drawing.Color.White : System.Drawing.Color.Red;
        }
        private void BtnAlerte_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
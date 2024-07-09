namespace NoPanic
{
    partial class frmAlerte
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblUtilisateur = new System.Windows.Forms.Label();
            this.tmrAlerte = new System.Windows.Forms.Timer(this.components);
            this.lblAlerte = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAlerte = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblUtilisateur
            // 
            this.lblUtilisateur.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUtilisateur.Location = new System.Drawing.Point(11, 68);
            this.lblUtilisateur.Name = "lblUtilisateur";
            this.lblUtilisateur.Size = new System.Drawing.Size(628, 47);
            this.lblUtilisateur.TabIndex = 1;
            this.lblUtilisateur.Text = "lblUtilisateur";
            this.lblUtilisateur.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmrAlerte
            // 
            this.tmrAlerte.Enabled = true;
            this.tmrAlerte.Interval = 600;
            this.tmrAlerte.Tick += new System.EventHandler(this.TmrAlerte_Tick);
            // 
            // lblAlerte
            // 
            this.lblAlerte.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlerte.ForeColor = System.Drawing.Color.Red;
            this.lblAlerte.Location = new System.Drawing.Point(11, 23);
            this.lblAlerte.Name = "lblAlerte";
            this.lblAlerte.Size = new System.Drawing.Size(628, 45);
            this.lblAlerte.TabIndex = 2;
            this.lblAlerte.Text = "ALERTE AGRESSION";
            this.lblAlerte.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnAlerte);
            this.panel1.Controls.Add(this.lblUtilisateur);
            this.panel1.Controls.Add(this.lblAlerte);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(659, 141);
            this.panel1.TabIndex = 3;
            // 
            // btnAlerte
            // 
            this.btnAlerte.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAlerte.Location = new System.Drawing.Point(634, 3);
            this.btnAlerte.Name = "btnAlerte";
            this.btnAlerte.Size = new System.Drawing.Size(19, 20);
            this.btnAlerte.TabIndex = 3;
            this.btnAlerte.Text = "X";
            this.btnAlerte.UseVisualStyleBackColor = true;
            this.btnAlerte.Click += new System.EventHandler(this.BtnAlerte_Click);
            // 
            // frmAlerte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(659, 141);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAlerte";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alerte";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmAlerte_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblUtilisateur;
        private System.Windows.Forms.Timer tmrAlerte;
        private System.Windows.Forms.Label lblAlerte;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAlerte;
    }
}
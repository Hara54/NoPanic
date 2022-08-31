namespace NoPanic
{
    partial class frmPrincipale
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrincipale));
            this.tiNoPanic = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnFermer = new System.Windows.Forms.Button();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblEtat = new System.Windows.Forms.Label();
            this.btnQuitter = new System.Windows.Forms.Button();
            this.lblEtatText = new System.Windows.Forms.Label();
            this.lblKeyText = new System.Windows.Forms.Label();
            this.lblKey = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tiNoPanic
            // 
            this.tiNoPanic.Icon = ((System.Drawing.Icon)(resources.GetObject("tiNoPanic.Icon")));
            this.tiNoPanic.Text = "NoPanic";
            this.tiNoPanic.Visible = true;
            this.tiNoPanic.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TiNoPanic_Click);
            // 
            // btnFermer
            // 
            this.btnFermer.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnFermer.Location = new System.Drawing.Point(162, 115);
            this.btnFermer.Name = "btnFermer";
            this.btnFermer.Size = new System.Drawing.Size(99, 23);
            this.btnFermer.TabIndex = 0;
            this.btnFermer.Text = "Fermer";
            this.btnFermer.UseVisualStyleBackColor = true;
            this.btnFermer.Click += new System.EventHandler(this.BtnFermer_Click);
            // 
            // lblDescription
            // 
            this.lblDescription.Font = new System.Drawing.Font("Comic Sans MS", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(12, 9);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(249, 23);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Description";
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEtat
            // 
            this.lblEtat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEtat.Location = new System.Drawing.Point(137, 71);
            this.lblEtat.Name = "lblEtat";
            this.lblEtat.Size = new System.Drawing.Size(124, 23);
            this.lblEtat.TabIndex = 3;
            this.lblEtat.Text = "Etat";
            this.lblEtat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnQuitter
            // 
            this.btnQuitter.Location = new System.Drawing.Point(12, 115);
            this.btnQuitter.Name = "btnQuitter";
            this.btnQuitter.Size = new System.Drawing.Size(99, 23);
            this.btnQuitter.TabIndex = 4;
            this.btnQuitter.Text = "Quitter";
            this.btnQuitter.UseVisualStyleBackColor = true;
            this.btnQuitter.Click += new System.EventHandler(this.BtnQuitter_Click);
            // 
            // lblEtatText
            // 
            this.lblEtatText.Location = new System.Drawing.Point(12, 71);
            this.lblEtatText.Name = "lblEtatText";
            this.lblEtatText.Size = new System.Drawing.Size(126, 23);
            this.lblEtatText.TabIndex = 5;
            this.lblEtatText.Text = "Etat de fonctionnement :";
            this.lblEtatText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblKeyText
            // 
            this.lblKeyText.Location = new System.Drawing.Point(12, 48);
            this.lblKeyText.Name = "lblKeyText";
            this.lblKeyText.Size = new System.Drawing.Size(126, 23);
            this.lblKeyText.TabIndex = 6;
            this.lblKeyText.Text = "Touches d\'activation :";
            this.lblKeyText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblKey
            // 
            this.lblKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKey.Location = new System.Drawing.Point(137, 48);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(124, 23);
            this.lblKey.TabIndex = 7;
            this.lblKey.Text = "Key";
            this.lblKey.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmPrincipale
            // 
            this.AcceptButton = this.btnFermer;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnFermer;
            this.ClientSize = new System.Drawing.Size(273, 145);
            this.Controls.Add(this.lblKey);
            this.Controls.Add(this.lblKeyText);
            this.Controls.Add(this.lblEtatText);
            this.Controls.Add(this.btnQuitter);
            this.Controls.Add(this.lblEtat);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.btnFermer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPrincipale";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NoPanic";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPrincipale_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon tiNoPanic;
        private System.Windows.Forms.Button btnFermer;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblEtat;
        private System.Windows.Forms.Button btnQuitter;
        private System.Windows.Forms.Label lblEtatText;
        private System.Windows.Forms.Label lblKeyText;
        private System.Windows.Forms.Label lblKey;
    }
}


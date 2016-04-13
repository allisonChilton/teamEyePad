namespace ALSProject
{
    partial class ALSMessageBox
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
            this.btnDismiss = new ALSProject.ALSButton();
            this.SuspendLayout();
            // 
            // btnDismiss
            // 
            this.btnDismiss.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDismiss.btnType = ALSProject.ALSButton.ButtonType.normal;
            this.btnDismiss.dwellTimeInterval = 100;
            this.btnDismiss.FlatAppearance.BorderSize = 0;
            this.btnDismiss.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDismiss.Font = new System.Drawing.Font("Microsoft Sans Serif", 47.73894F);
            this.btnDismiss.Location = new System.Drawing.Point(12, 152);
            this.btnDismiss.Name = "btnDismiss";
            this.btnDismiss.Size = new System.Drawing.Size(509, 98);
            this.btnDismiss.TabIndex = 0;
            this.btnDismiss.Text = "Ok";
            this.btnDismiss.UseVisualStyleBackColor = false;
            this.btnDismiss.Click += new System.EventHandler(this.btnDismiss_Click);
            // 
            // ALSMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(533, 262);
            this.Controls.Add(this.btnDismiss);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ALSMessageBox";
            this.Text = "ALSMessageBox";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private ALSButton btnDismiss;
    }
}
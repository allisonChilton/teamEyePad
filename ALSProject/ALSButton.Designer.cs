﻿namespace ALSProject
{
    partial class ALSButton
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ALSButton
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FlatAppearance.BorderSize = 0;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Size = new System.Drawing.Size(126, 94);
            this.UseVisualStyleBackColor = false;
            this.Click += new System.EventHandler(this.ALSButton_Click);
            this.MouseEnter += new System.EventHandler(this.ALSButton_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ALSButton_MouseLeave);
            this.Resize += new System.EventHandler(this.ALSButton_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}

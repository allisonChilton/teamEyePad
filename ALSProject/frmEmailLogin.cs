﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALSProject
{
    public partial class frmEmailLogin : Form
    {
        protected ALSAlarm btnAlarm;
        protected ALSButton btnLogout;
        protected ALSButton btnCancel;
        protected ALSButton btnLogin;
        protected ALSTextbox txtButton;
        protected ALSTextbox txtPassword;
        protected Keyboard keyboard;

        public delegate void Back(object sender, EventArgs args);
        public event Back Cancel_Click;
        public event Back Login_Click;

        #region Constructor
        public frmEmailLogin(bool isQwerty)
        {
            InitializeComponent();
            InitializeControls(isQwerty);
        }

        private void InitializeControls(bool isQwerty)
        {
            btnAlarm = new ALSAlarm();
            btnLogout = new ALSButton();
            btnCancel = new ALSButton();
            btnLogin = new ALSButton();
            txtButton = new ALSTextbox();
            txtPassword = new ALSTextbox();
            if (isQwerty)
                keyboard = new KeyboardControl3();
            else
                keyboard = new KeyboardControl2();

            btnLogout.Text = "Log\nOut";
            btnCancel.Text = "Cancel";
            btnLogin.Text = "Log\nIn";

            Controls.Add(btnAlarm);
            Controls.Add(btnLogout);
            Controls.Add(btnCancel);
            Controls.Add(btnLogin);
            Controls.Add(txtButton);
            Controls.Add(txtPassword);
            Controls.Add(keyboard);

            btnLogout.Click += BtnLogout_Click;
            btnCancel.Click += BtnCancel_Click;
            btnLogin.Click += BtnLogin_Click;

            txtButton.Font = new Font(txtButton.Font.FontFamily, 20);
            txtPassword.Font = new Font(txtPassword.Font.FontFamily, 20);

            keyboard.HideTextBox();
            txtButton.Focus();
        }

        #endregion

        #region Public Methods
        public void SetKeyboard(Keyboard k)
        {
            Controls.Remove(keyboard);
            keyboard = k;
            keyboard.HideTextBox();
            Controls.Add(keyboard);
            frmEmailLogin_Resize(this, EventArgs.Empty);
        }
        #endregion

        #region Events
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            EmailClient client = EmailFactory.GetEmailClient();
            client.setLogin("imap.gmail.com", "smtp.gmail.com", txtButton.Text, txtPassword.Text);
            Hide();
            if (Cancel_Click != null)
                Cancel_Click(this, e);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Hide();
            if (Cancel_Click != null)
                Cancel_Click(this, e);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            //Do stuff
        }

        private void frmEmailLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void frmEmailLogin_Resize(object sender, EventArgs e)
        {
            int buttonWidth = (Width - MainMenu.GAP * 7) / 6;
            btnAlarm.Size = new Size(buttonWidth, buttonWidth);
            btnLogout.Size = btnAlarm.Size;
            btnCancel.Size = btnAlarm.Size;
            btnLogin.Size = btnAlarm.Size;

            txtButton.Size = new Size(buttonWidth * 2 + MainMenu.GAP, (buttonWidth - MainMenu.GAP) / 2);
            txtPassword.Size = txtButton.Size;

            keyboard.Size = new Size(Width - MainMenu.GAP * 2, Height - buttonWidth - MainMenu.GAP * 3);

            btnAlarm.Location = new Point(MainMenu.GAP, MainMenu.GAP);
            btnLogout.Location = new Point(btnAlarm.Right + MainMenu.GAP, btnAlarm.Top);
            txtButton.Location = new Point(btnLogout.Right + MainMenu.GAP, btnAlarm.Top);
            txtPassword.Location = new Point(txtButton.Left, txtButton.Bottom + MainMenu.GAP);
            btnCancel.Location = new Point(txtButton.Right + MainMenu.GAP, btnAlarm.Top);
            btnLogin.Location = new Point(btnCancel.Right + MainMenu.GAP, btnAlarm.Top);
            keyboard.Location = new Point(MainMenu.GAP, btnAlarm.Bottom + MainMenu.GAP);
        }
        #endregion
    }
}

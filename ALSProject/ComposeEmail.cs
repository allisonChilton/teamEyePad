using System;
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
    public partial class ComposeEmail : Form
    {
        public enum EmailType { Compose, Reply, ReplyAll, Forward };
        private EmailType type;

        public delegate void Event(object sender, EventArgs args);
        public event Event Cancel_Click;
        public event Event Send_Click;

        protected ALSAlarm btnAlarm;
        protected ALSTextbox txtTo, txtSubject, txtBody;
        protected ALSButton btnCancel;
        protected ALSButton btnSend;
        protected Keyboard keyboard;
        protected EmailMessage previousMessage;

        #region Constructors
        public ComposeEmail(bool isQwerty)
        {
            InitializeComponent();
            InitializeControls(isQwerty);
        }
        #endregion

        #region Public Methods
        public void SetKeyboard(Keyboard k)
        {
            Controls.Remove(keyboard);
            keyboard = k;
            keyboard.HideTextBox();
            Controls.Add(keyboard);
            ComposeEmail_Resize(this, EventArgs.Empty);

            keyboard.OnPressed += Press_Key;
        }

        public void SetEmailType(EmailType type)
        {
            this.type = type;
        }

        public void SetEmailType(EmailType type, EmailMessage previousMessage)
        {
            this.previousMessage = previousMessage;

            string addr = previousMessage.sourceAddress;
            string subj = previousMessage.subject;
            switch (type)
            {
                case EmailType.Reply:
                case EmailType.ReplyAll:
                    if (addr != null && addr != "")
                    {
                        txtTo.Text = addr;
                    }
                    if (subj != null && subj != "")
                    {
                        txtSubject.Text = subj;
                    }
                    goto case EmailType.Forward;
                case EmailType.Forward:
                    if (subj != null && subj != "")
                    {
                        txtSubject.Text = subj;
                    }
                    break;
            }


        }
        #endregion

        #region Events
        private void TxtBody_Click(object sender, EventArgs e)
        {
            if (txtBody.Text.Equals("Body:"))
            {
                txtBody.Text = "";
            }

        }

        private void TxtSubject_Click(object sender, EventArgs e)
        {
            if (txtSubject.Text.Equals("Subject:"))
            {
                txtSubject.Text = "";
            }
        }

        private void TxtTo_Click(object sender, EventArgs e)
        {
            if (txtTo.Text.Equals("To:"))
            {
                txtTo.Text = "";
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            EmailMessage message = new EmailMessage(txtSubject.Text, txtBody.Text, txtTo.Text, txtSubject.Text, new DateTime());
            EmailClient Client = EmailFactory.GetEmailClient();

            switch (type)
            {
                case EmailType.Compose:
                    Client.sendMessage(message);
                    break;
                case EmailType.Forward:
                    Client.sendForward(previousMessage, txtSubject.Text, txtBody.Text);
                    break;
                case EmailType.Reply:
                case EmailType.ReplyAll:
                    Client.sendReply(previousMessage, txtBody.Text);
                    break;
            }

            Hide();
            if (Send_Click != null)
                Send_Click(this, e);
        }

        private void Press_Key(object sender, EventArgs e)
        {
            SendKeys.Send(keyboard.GetMostRecentEntry());
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Hide();
            if (Cancel_Click != null)
                Cancel_Click(this, e);
        }

        private void ComposeEmail_Resize(object sender, EventArgs e)
        {
            int buttonWidth = (Width - MainMenu.GAP * 7) / 6;
            btnAlarm.Size = new Size(buttonWidth, buttonWidth);
            btnCancel.Size = btnAlarm.Size;
            btnSend.Size = btnAlarm.Size;

            btnAlarm.Location = new Point(MainMenu.GAP, MainMenu.GAP);
            int labelHeight = (btnAlarm.Bottom - MainMenu.GAP * 2) / 2;
            btnSend.Location = new Point(Width - btnSend.Width - MainMenu.GAP, btnAlarm.Top);
            btnCancel.Location = new Point(btnSend.Left - btnCancel.Width - MainMenu.GAP, btnAlarm.Top);
            txtTo.Location = new Point(btnAlarm.Right + MainMenu.GAP, btnAlarm.Top);
            txtSubject.Location = new Point(btnAlarm.Right + MainMenu.GAP, txtTo.Bottom + MainMenu.GAP);

            txtTo.Size = new Size(btnCancel.Left - txtTo.Left - MainMenu.GAP, 3 * buttonWidth/8);
            txtSubject.Size = txtTo.Size;

            txtBody.Location = new Point(MainMenu.GAP, btnAlarm.Bottom + MainMenu.GAP);
            keyboard.Location = new Point(MainMenu.GAP, txtBody.Bottom + MainMenu.GAP);
            int bodyHeight = (Height - btnAlarm.Bottom - MainMenu.GAP * 3) / 3;

            txtBody.Size = new Size(Width - MainMenu.GAP * 2, bodyHeight);
            keyboard.Size = new Size(Width - MainMenu.GAP * 2, bodyHeight * 2);
        }

        private void ComposeEmail_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Private Methods
        private void InitializeControls(bool isQwerty)
        {
            btnAlarm = new ALSAlarm();
            txtTo = new ALSTextbox();
            txtSubject = new ALSTextbox();
            txtBody = new ALSTextbox();
            btnCancel = new ALSButton();
            btnSend = new ALSButton();

            if (isQwerty)
                keyboard = new LargeButtonKeyboard();
            else
                keyboard = new QwertyKeyboard();

            txtTo.Multiline = true;
            txtSubject.Multiline = true;

            btnCancel.Text = "Cancel";
            btnSend.Text = "Send";
            txtTo.Text = "To:";
            txtSubject.Text = "Subject:";
            txtBody.Text = "Body:";

            Controls.Add(btnAlarm);
            Controls.Add(txtTo);
            Controls.Add(txtSubject);
            Controls.Add(txtBody);
            Controls.Add(btnCancel);
            Controls.Add(btnSend);
            Controls.Add(keyboard);

            btnCancel.Click += BtnCancel_Click;
            btnSend.Click += BtnSend_Click;
            txtTo.Click += TxtTo_Click;
            txtSubject.Click += TxtSubject_Click;
            txtBody.Click += TxtBody_Click;

            txtTo.Font = new Font(txtTo.Font.FontFamily, 20);
            txtSubject.Font = new Font(txtSubject.Font.FontFamily, 20);
            txtBody.Font = new Font(txtBody.Font.FontFamily, 20);

            keyboard.HideTextBox();
            txtBody.Multiline = true;

        }
        #endregion


        
    }
}

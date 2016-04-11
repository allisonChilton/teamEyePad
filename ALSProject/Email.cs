using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALSProject
{
    public partial class Email : Form
    {
        public object Zoom { get; private set; }

        public event MainMenuClick MainMenu_Click;
        public delegate void MainMenuClick(object sender, EventArgs args);

        protected ALSAlarm btnAlarm;
        protected ALSButton btnSwitchAccount;
        protected ALSButton btnCompose;
        protected ALSButton btnRefresh;
        protected ALSButton btnDelete;
        protected ALSButton btnMainMenu;
        protected ALSButton btnMoveUp;
        protected ALSButton btnMoveDown;
        protected ALSButton btnSelect;
        protected ListBox lbEmails;
        protected DeleteEmailConfirmation frmDeleteEmail;
        protected frmEmailLogin frmEmailLogin;
        protected ComposeEmail frmComposeEmail;
        protected ViewEmail frmViewEmail;
        protected bool refreshing;
        protected int pageNum = 0;
        protected List<EmailMessage> Messages;

        private bool folderMode = false;

        #region Constructors
        public Email(bool isQwerty)
        {
            InitializeComponent();
            InitializeControls();
            InitializeForms(isQwerty);

            Resize += Email2_Resize;

            BtnRefresh_Click(this, EventArgs.Empty);
        }

        private void InitializeControls()
        {
            btnAlarm = new ALSAlarm();
            btnSwitchAccount = new ALSButton();
            btnCompose = new ALSButton();
            btnRefresh = new ALSButton();
            btnDelete = new ALSButton();
            btnMainMenu = new ALSButton();

            btnMoveUp = new ALSButton();
            btnMoveDown = new ALSButton();
            btnSelect = new ALSButton();
            lbEmails = new ListBox();

            btnSwitchAccount.Text = "Switch\nAccount";
            btnCompose.Text = "Compose";
            btnRefresh.Text = "Refresh";
            btnDelete.Text = "Delete";
            btnMainMenu.Text = "Main\nMenu";

            btnMoveUp.BackgroundImage = Properties.Resources.UpArrow;
            btnMoveUp.BackgroundImageLayout = ImageLayout.Zoom;
            btnMoveDown.BackgroundImage = Properties.Resources.DownArrow;
            btnMoveDown.BackgroundImageLayout = ImageLayout.Zoom;
            btnSelect.Text = "Select";

            lbEmails.Font = new Font(lbEmails.Font.FontFamily, 20);

            btnSwitchAccount.Click += BtnSwitchAccount_Click;
            btnCompose.Click += BtnCompose_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnDelete.Click += BtnDelete_Click;
            btnMainMenu.Click += BtnMainMenu_Click;
            btnMoveUp.Click += BtnMoveUp_Click;
            btnMoveDown.Click += BtnMoveDown_Click;
            btnSelect.Click += BtnSelect_Click;

            Controls.Add(btnAlarm);
            Controls.Add(btnSwitchAccount);
            Controls.Add(btnCompose);
            Controls.Add(btnRefresh);
            Controls.Add(btnDelete);
            Controls.Add(btnMainMenu);

            Controls.Add(btnMoveUp);
            Controls.Add(btnMoveDown);
            Controls.Add(btnSelect);
            Controls.Add(lbEmails);


            lbEmails.Focus();
            lbEmails.SelectedIndexChanged += LbEmails_SelectedIndexChanged;
        }

        private void setToRightOf(ALSButton btnRight, ALSButton btnLeft)
        {
            btnRight.Location = new Point(btnLeft.Right + MainMenu.GAP, btnLeft.Top);
        }
        #endregion

        #region Public Methods
        public void SetKeyboard(Keyboard k)
        {
            frmEmailLogin.SetKeyboard(k);
            try
            {
                k = (Keyboard)k.Clone();
                frmComposeEmail.SetKeyboard(k);
            }
            catch
            {
                MessageBox.Show("There was an error loading the settings");
            }
        }
        #endregion

        #region Events
        private void BtnSwitchAccount_Click(object sender, EventArgs e)
        {
            frmEmailLogin.Show();
            //throw new Exception("You so exception");
        }

        private void BtnCompose_Click(object sender, EventArgs e)
        {
            frmComposeEmail.Show();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            if (!refreshing)
            {
                //refreshing = true;

                EmailClient Client = EmailFactory.GetEmailClient();
                Client.retrieveMail();
                Messages = Client.getMailHistory();
                refreshMessages(lbEmails);
                //refreshing = false;

                //Thread thread = new Thread(refresh);
                //thread.IsBackground = true;
                //thread.Start(this);
            }

        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (lbEmails.Items.Count == 0)
                return;
            if (lbEmails.SelectedIndex < 0)
                lbEmails.SelectedIndex = 0;
            //Hide();
            //frmDeleteEmail.Show();
            frmDeleteEmail.Visible = true;
        }

        private void BtnMainMenu_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (MainMenu_Click != null)
                MainMenu_Click(this, e);
        }

        private void BtnMoveUp_Click(object sender, EventArgs e)
        {
            if (lbEmails.SelectedIndex < 0 && lbEmails.Items.Count > 0)
                lbEmails.SelectedIndex = 0;
            else if (lbEmails.SelectedIndex > 0)
                lbEmails.SelectedIndex--;
        }

        private void BtnMoveDown_Click(object sender, EventArgs e)
        {
            if (lbEmails.SelectedIndex < lbEmails.Items.Count - 1)
                lbEmails.SelectedIndex++;
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (lbEmails.SelectedIndex == -1)
                lbEmails.SelectedIndex = 0;

            if (!folderMode)
            {
                if (lbEmails.Items.Count > 0)
                {

                    if (lbEmails.SelectedIndex == 0)
                    {
                        //switch Folder
                        this.lbListFolders();

                    }
                    else if (lbEmails.SelectedIndex > 0)
                    {
                        frmViewEmail.SetMailMessage(Messages[lbEmails.SelectedIndex - 1]);
                        frmViewEmail.Show();
                    }
                }
            }
            else
            {
                EmailClient Client = EmailFactory.GetEmailClient();
                Client.retrieveMail(lbEmails.SelectedItem.ToString());
                Messages = Client.getMailHistory();
                refreshMessages(lbEmails);
            }
        }

        private void Email2_Resize(object sender, EventArgs e)
        {
            //Top buttons
            int btnWidth = (Width - 7 * MainMenu.GAP) / 6;
            btnAlarm.Size = new Size(btnWidth, btnWidth);
            btnSwitchAccount.Size = btnAlarm.Size;
            btnCompose.Size = btnAlarm.Size;
            btnRefresh.Size = btnAlarm.Size;
            btnDelete.Size = btnAlarm.Size;
            btnMainMenu.Size = btnAlarm.Size;

            btnAlarm.Location = new Point(MainMenu.GAP, MainMenu.GAP);
            setToRightOf(btnSwitchAccount, btnAlarm);
            setToRightOf(btnCompose, btnSwitchAccount);
            setToRightOf(btnRefresh, btnCompose);
            setToRightOf(btnDelete, btnRefresh);
            setToRightOf(btnMainMenu, btnDelete);

            //Right side buttons
            int sideButtonHeight = (Height - btnMainMenu.Bottom - MainMenu.GAP * 4) / 3;
            sideButtonHeight = Math.Min(btnWidth, sideButtonHeight);

            btnMoveUp.Size = new Size(sideButtonHeight, sideButtonHeight);
            btnMoveDown.Size = btnMoveUp.Size;
            btnSelect.Size = btnMoveUp.Size;

            btnMoveUp.Location = new Point(btnMainMenu.Right - btnMoveUp.Width, btnMainMenu.Bottom + MainMenu.GAP);
            btnMoveDown.Location = new Point(btnMainMenu.Right - btnMoveUp.Width, btnMoveUp.Bottom + MainMenu.GAP);
            btnSelect.Location = new Point(btnMainMenu.Right - btnMoveUp.Width, btnMoveDown.Bottom + MainMenu.GAP);

            //List box
            lbEmails.Location = new Point(MainMenu.GAP, btnAlarm.Bottom + MainMenu.GAP);
            lbEmails.Size = new Size(btnMoveUp.Left - MainMenu.GAP * 2, Height - btnAlarm.Bottom - MainMenu.GAP * 2);
        }

        private void LbEmails_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnMoveUp.Enabled = lbEmails.SelectedIndex > 0;
            btnMoveDown.Enabled = lbEmails.SelectedIndex < lbEmails.Items.Count - 1;
        }

        private void Show(object sender, EventArgs e)
        {
            Show();
        }
        #endregion

        #region Private Methods
        private void InitializeForms(bool isQwerty)
        {
            frmDeleteEmail = new DeleteEmailConfirmation();
            frmDeleteEmail.Visible = false;
            frmDeleteEmail.DeleteEmail_Click += FrmDeleteEmail_DeleteEmail_Click;

            frmEmailLogin = new frmEmailLogin(isQwerty);
            frmEmailLogin.Visible = false;
            frmEmailLogin.Cancel_Click += Show;

            frmComposeEmail = EmailFactory.GetComposeEmail();
            frmComposeEmail.Cancel_Click += Show;
            frmComposeEmail.Send_Click += Show;

            frmViewEmail = new ViewEmail();
            frmViewEmail.Back_Click += Show;
        }

        private void lbListFolders()
        {
            EmailClient Client = EmailFactory.GetEmailClient();
            String[] folders = Client.ListFolders();
            lbEmails.Items.Clear();
            foreach (string s in folders)
            {
                lbEmails.Items.Add(s);
            }
            folderMode = true;
        }

        private void FrmDeleteEmail_DeleteEmail_Click(bool isConfirmation)
        {
            //Show();
            frmDeleteEmail.Visible = false;



            int temp = lbEmails.SelectedIndex;
            if (isConfirmation)
            {
                EmailClient Client = EmailFactory.GetEmailClient();
                Client.DeleteMessage(Messages[lbEmails.SelectedIndex]);
                lbEmails.Items.RemoveAt(lbEmails.SelectedIndex);
            }
            if (lbEmails.Items.Count == 0)
                return;
            if (temp < lbEmails.Items.Count)
                lbEmails.SelectedIndex = temp;
            else
                lbEmails.SelectedIndex = temp - 1;
        }

        private void refreshMessages(ListBox lbEmails)
        {
            EmailClient Client = EmailFactory.GetEmailClient();
            folderMode = false;
            lbEmails.Items.Clear();
            string folderName = Client.getCurrentFolder();
            lbEmails.Items.Add("Folder: " + folderName + " | Select to change folder");

            foreach (EmailMessage message in Messages)
            {
                /*String line = "Subject: " + message.subject + " | From: " + message.sourceAddress +
                    " | Date:" + message.date.ToShortDateString() + //" " + message.date.ToShortTimeString() +
                    " | Body: " + message.body;*/

                string address;
                if (folderName == "[Gmail]/Sent Mail")
                    address = message.destinationAddress;
                else
                    address = message.sourceAddress;

                String line = message.subject + " | " + address +
                    " | " + message.date.ToShortDateString() + //" " + message.date.ToShortTimeString() +
                    " | " + message.body;


                lbEmails.Items.Add(line);
            }
        }
        #endregion
    }
}

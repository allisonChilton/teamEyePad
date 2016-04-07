using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using EyeXFramework;
using Tobii.EyeX.Framework;
using System.Speech.Synthesis;
using Timer = System.Windows.Forms.Timer;

namespace ALSProject
{
    public partial class MainMenu : Form
    {
        public static long startTime;
        /*
            main UI for our application
            maximize and quit buttons are temporary

            I had to look up the difference between the constructor and Form_Load. Superficially they seem to do the same thing. The constructor
            is immediately called on creation regardless of whether the form is displayed or not. The _Load method is called when the form becomes
            visible to the user. I am moving the code to the _Load section because that allows all forms within our program to be created before
            any of the elements are loaded. This isn't necessarily helpful now but may be useful in the future.
        */

        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int WM_APPCOMMAND = 0x319;

        public Form self { get; set; }
        public const int GAP = 10;

        protected TextToSpeech texttospeech;
        protected Callout callout;
        protected SettingsPage1 settingsScreen;
        protected QuitForm quitScreen;
        protected Browser browser;
        protected Email email;
        protected Timer closeTimer;
        protected Notebook notebook;
        protected Factory factory;
        private CVInterface tobiiInt;
        private Thread eyeTrackingThread;
        private static LockScreen lockScreen;
        private static SpeechSynthesizer voice;

        #region Constructors
        public MainMenu()
        {
            InitializeComponent();
            this.self = this;
            btnAlarm.Click += new System.EventHandler(alarmBut_Click);
            lockScreen = new LockScreen();

            //Temp code
            tobiiInt = new CVInterface();
            eyeTrackingThread = new Thread(tobiiInt.StartEyeTracking);
            eyeTrackingThread.Name = "Eye Tracking Thread";
            eyeTrackingThread.Start();

            voice = new SpeechSynthesizer();

            voice.SetOutputToDefaultAudioDevice();
            voice.Volume = 100;
            voice.SelectVoiceByHints(VoiceGender.Male);

            factory = new Factory();

            texttospeech = factory.Texttospeech;
            notebook = factory.Notebook;
            callout = factory.Callout;
            quitScreen = factory.QuitScreen;
            browser = factory.Browser;
            email = factory.Email;
            //Settings must be created last to update all of the other application's properties
            settingsScreen = factory.SettingsScreen;
            




            texttospeech.Visible = false;
            notebook.Visible = false;
            callout.Visible = false;
            settingsScreen.Visible = false;
            browser.Visible = false;
            email.Visible = false;

            texttospeech.MainMenu_Click += MainMenu_Show;
            notebook.MainMenu_Click += MainMenu_Show;
            callout.MainMenu_Click += MainMenu_Show;
            settingsScreen.MainMenu_Click += MainMenu_Show;
            quitScreen.MainMenu_Click += MainMenu_Show;
            browser.MainMenu_Click += MainMenu_Show;
            email.MainMenu_Click += MainMenu_Show;
            texttospeech.GetBtnCallout().Click += new System.EventHandler(this.openCallouts);

            texttospeech.Callouts_Click += Callouts_Show;
            callout.TextToSpeech_Click += TextToSpeech_Show;

            settingsScreen.SetKeyboard += SettingsScreen_SetKeyboard;
            
            this.VisibleChanged += UI_VisibleChanged;
            
            settingsScreen.btnResetCallouts.Click += new System.EventHandler(this.resetCallouts);

            foreach (ALSButton btn in callout.getMenuBtns())
            {
                if (btn.Text == "Main Menu" || btn.Text == "Text to Speech")
                {
                    btn.Click += new System.EventHandler(this.closeCallouts);
                }
            }

            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            if (resolution.Width < 840 || resolution.Height < 580)
            {
                MessageBox.Show("You are using a computer with a screen resolution less than recommended. Undesired results may incur.");
            }

            settingsScreen.ApplySettings();

            //listen for close
            closeTimer = new Timer();
            closeTimer.Enabled = true;
            closeTimer.Interval = 1000;
            closeTimer.Tick += new EventHandler(closeTimeEvent);
        }
        #endregion

        #region Public Methods
        public void OpenTTS()
        {

            texttospeech.Show();
        }
        
        public static void showLockScreen()
        {
            lockScreen.Show();
            lockScreen.Focus();
        }

        public static void SetVoiceSpeed(int speed)
        {
            if (speed >= -10 && speed <= 10)
            {
                voice.Rate = speed;
            }
        }

        public static void Speak(string text)
        {
            voice.SpeakAsyncCancelAll();
            voice.SpeakAsync(text);
        }
        #endregion

        #region Events
        private void SettingsScreen_SetKeyboard(Keyboard k)
        {
            try
            {
                Keyboard tempKeyboard = (Keyboard)k.Clone();
                texttospeech.SetKeyboard(tempKeyboard);
                tempKeyboard = (Keyboard)k.Clone();
                callout.GetAddCallout().SetKeyboard(tempKeyboard);
                tempKeyboard = (Keyboard)k.Clone();
                notebook.GetNotepage().SetKeyboard(tempKeyboard);
                tempKeyboard = (Keyboard)k.Clone();
                email.SetKeyboard(tempKeyboard);
                tempKeyboard = (Keyboard)k.Clone();
                tempKeyboard.SetBrowserMode(true);
                browser.SetKeyboard(tempKeyboard);


            }
            catch (Exception)
            {
                MessageBox.Show("There was an error loading the settings.");
            }
        }

        private void User_Interface_Load(object sender, EventArgs e)
        {
            resizeScreen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void quit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void alsButton4_Click(object sender, EventArgs e)
        {
            callout.Show();
            this.Hide();
        }

        private void quitBut_Click(object sender, EventArgs e)
        {

            if (!btnAlarm.isAlarmOn())
            {
                quitScreen.Show();
                this.Hide();
            }
        }

        private void setBut_Click(object sender, EventArgs e)
        {
            settingsScreen.Show();
            this.Hide();
        }

        private void UI_VisibleChanged(object sender, EventArgs e)
        {
            if (btnAlarm.isAlarmOn())
            {
                btnQuit.BackColor = Color.Red;
                btnQuit.Enabled = false;
            }
            else
            {
                btnQuit.BackColor = ALSButton.baseColor;
                btnQuit.Enabled = true;
            }
        }

        private void alarmBut_Click(object sender, EventArgs e)
        {
            if (!btnAlarm.isAlarmOn())
            {
                btnQuit.BackColor = ALSButton.baseColor;
                btnQuit.Enabled = true;
            }
            else
            {
                btnQuit.BackColor = Color.Red;
                btnQuit.Enabled = false;
            }
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            notebook.Show();
            texttospeech.Hide();
            this.Hide();
        }

        private void ttsBut_Click(object sender, EventArgs e)
        {
            texttospeech.Show();
            this.Hide();
        }

        private void Callouts_Show(object sender, EventArgs args)
        {
            callout.Show();
        }

        private void TextToSpeech_Show(object sender, EventArgs args)
        {
            texttospeech.Show();
        }

        private void MainMenu_Show(object sender, EventArgs e)
        {
            this.Show();
        }

        private void closeTimeEvent(object sender, EventArgs e)
        {

            if (callout != null && settingsScreen != null && texttospeech != null)
            {
                //this.Close();
            }
        }

        private void resetCallouts(object sender, EventArgs e)
        {
            callout.resetList();
        }

        private void closeCallouts(object sender, EventArgs e)
        {
            if (((ALSButton)sender).Text == "Main Menu")
            {
                this.Show();
                callout.Hide();
            }
            else if (((ALSButton)sender).Text == "Text to Speech")
            {
                texttospeech.Show();
                callout.Hide();
            }
        }

        public void openCallouts(object sender, EventArgs e)
        {
            callout.Show();
            texttospeech.Hide();
        }

        private void UI_FormClosed(object sender, FormClosedEventArgs e)
        {
            CVInterface.PleaseStop();

            Application.Exit();

        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            browser.Show();
            this.Hide();
        }

        private void btnNotebook_Click(object sender, EventArgs e)
        {
            showLockScreen();
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            email.Show();
            this.Hide();
        }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Private Methods
        private void resizeScreen()
        {
            int height = (this.Height - 4 * MainMenu.GAP) / 3;
            int width = (this.Width);


            btnAlarm.Location = new Point(GAP, GAP);
            btnAlarm.Size = new Size(height, height);

            btnMin.Location = new Point(GAP, 2 * GAP + height);
            btnMin.Size = new Size(height, height);

            btnBrowser.Location = new Point(GAP, 3 * GAP + 2 * height);
            btnBrowser.Size = new Size(height, height);

            btnTTS.Location = new Point(width / 2 - height / 2, GAP);
            btnTTS.Size = new Size(height, height);

            btnEmail.Location = new Point(width / 2 - height / 2, 2 * GAP + height);
            btnEmail.Size = new Size(height, height);

            setBut.Location = new Point(width / 2 - height / 2, 3 * GAP + 2 * height);
            setBut.Size = new Size(height, height);

            btnNotebook.Location = new Point(width - GAP - height, GAP);
            btnNotebook.Size = new Size(height, height);

            alsButton4.Location = new Point(width - GAP - height, 2 * GAP + height);
            alsButton4.Size = new Size(height, height);

            btnQuit.Location = new Point(width - GAP - height, 3 * GAP + 2 * height);
            btnQuit.Size = new Size(height, height);


        }

        private Keyboard GetNewKeyboard(bool isQwerrty)
        {
            if (isQwerrty)
                return new LargeButtonKeyboard();
            return new QwertyKeyboard();
        }
        #endregion
    }
}

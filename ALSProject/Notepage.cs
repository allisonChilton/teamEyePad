﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALSProject
{
    public partial class Notepage : Form
    {

        protected const int MENU_BUTTON_SIZE = 140;
        protected const int ARROW_KEY_SIZE = 80;
        protected Keyboard keyboard;
        protected ALSButton alarm, speak, back;
        protected ALSButton up, left, right, down, backWord, forwardWord;
        protected ALSButton _lock;

        public delegate void BackClick(object sender, EventArgs args);
        public event BackClick Back_Click;

        #region Constructors
        public Notepage(bool isQwerty)
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            if (isQwerty)
                keyboard = new LargeButtonKeyboard();
            else
                keyboard = new QwertyKeyboard();

            alarm = new ALSAlarm();
            speak = new ALSButton();
            back = new ALSButton();
            up = new ALSButton();
            left = new ALSButton();
            right = new ALSButton();
            down = new ALSButton();
            backWord = new ALSButton();
            forwardWord = new ALSButton();
            _lock = new ALSButton();

            Controls.Add(alarm);
            Controls.Add(speak);
            Controls.Add(back);
            Controls.Add(up);
            Controls.Add(left);
            Controls.Add(right);
            Controls.Add(down);
            Controls.Add(backWord);
            Controls.Add(forwardWord);
            Controls.Add(keyboard);
            Controls.Add(_lock);

            speak.Click += new EventHandler(Speak_Click);
            back.Click += new EventHandler(btnBack_Click);
            up.Click += new EventHandler(Up_Click);
            left.Click += new EventHandler(Left_Click);
            right.Click += new EventHandler(Right_Click);
            down.Click += new EventHandler(Down_Click);
            backWord.Click += new EventHandler(BackWord_Click);
            forwardWord.Click += new EventHandler(ForwardWord_Click);

            speak.Text = "Speak";
            back.Text = "Notebook";
            up.Text = "Up";
            left.Text = "Left";
            right.Text = "Right";
            down.Text = "Down";
            backWord.Text = "Left one word";
            forwardWord.Text = "right one word";
            _lock.BackgroundImage = Properties.Resources.Lock;

            _lock.BackgroundImageLayout = ImageLayout.Zoom;
            _lock.Click += _lock_Click;

            initControlsRecursive(this.Controls);
            keyboard.setClearConfirmation(true);
        }
        #endregion

        #region Public Methods
        public string getText()
        {
            return keyboard.GetText();
        }

        public ALSButton getBackBtn()
        {
            return back;
        }

        public void setText(string text)
        {
            if (text != null)
                keyboard.SetText(text);
        }

        public void ClearText() //erases the textbox for when a new note is to be added
        {
            keyboard.SetText("");
        }

        public void setCursorAtEnd()
        {
            keyboard.SetSelection(keyboard.GetText().Length, 0);
        }

        public ALSButton getSaveButton()
        {
            return new ALSButton();
        }

        //public void makeKeyboard(bool isQwerty)
        //{
        //    Controls.Remove(keyboard);
        //    if (isQwerty)
        //        keyboard = new KeyboardControl3();
        //    else
        //        keyboard = new KeyboardControl2();
        //    Notepage_Resize(this, null);
        //    Invalidate();
        //}

        public void SetKeyboard(Keyboard k)
        {
            Controls.Remove(keyboard);
            keyboard = k;
            Controls.Add(keyboard);
            keyboard.setClearConfirmation(true);
            keyboard.Location = new Point(MainMenu.GAP, MainMenu.GAP);
            Notepage_Resize(this, EventArgs.Empty);
        }
        #endregion

        #region Events
        private void _lock_Click(object sender, EventArgs e)
        {
            MainMenu.showLockScreen();
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            ClearTextConfirmation confirm = new ClearTextConfirmation();
            confirm.Visible = true;
        }

        private void ForwardWord_Click(object sender, EventArgs e)
        {
            keyboard.SetTextBoxFocus();
            string content = keyboard.GetText().Substring(keyboard.GetSelectionStart());

            //Find the first whitespace. Move the cursor at the end of the whitespace
            var match = Regex.Match(content, @"\s*\S+\s+");
            if (match.Success)
            {
                keyboard.SetSelection(keyboard.GetSelectionStart() + match.Index + match.Length, 0);
            }
            else
            {
                keyboard.SetSelection(keyboard.GetText().Length, 0);
            }
        }

        private void BackWord_Click(object sender, EventArgs e)
        {
            keyboard.SetTextBoxFocus();

            string content = keyboard.GetText().Substring(0, keyboard.GetSelectionStart());

            var match = Regex.Match(content, @"\s+\S+\s*$");
            if (match.Success)
            {
                keyboard.SetSelection(match.Index, 0);
            }
            else
            {
                keyboard.SetSelection(0, 0);
            }
        }

        private void Down_Click(object sender, EventArgs e)
        {
            keyboard.SetTextBoxFocus();
            SendKeys.Send("{DOWN}");
        }

        private void Right_Click(object sender, EventArgs e)
        {
            keyboard.SetTextBoxFocus();

            int kStart = keyboard.GetSelectionStart();
            if (kStart != keyboard.GetText().Length)
                keyboard.SetSelection(kStart + 1, 0);
        }

        private void Left_Click(object sender, EventArgs e)
        {
            keyboard.SetTextBoxFocus();

            int kStart = keyboard.GetSelectionStart();
            if (kStart != 0)
                keyboard.SetSelection(kStart - 1, 0);
        }

        private void Up_Click(object sender, EventArgs e)
        {
            keyboard.SetTextBoxFocus();
            SendKeys.Send("{UP}");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (Back_Click != null)
                Back_Click(this, e);
            this.Hide();
        }

        private void Speak_Click(object sender, EventArgs e)
        {
            int kStart = keyboard.GetSelectionStart();
            if (kStart == keyboard.GetText().Length)
                MainMenu.Speak(keyboard.GetText());
            else
                MainMenu.Speak(keyboard.GetText().Substring(keyboard.GetSelectionStart()));
        }

        private void Notepage_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Type printable characters in text box
        }

        private void Notepage_Load(object sender, EventArgs e)
        {
            alarm.Size = new Size(MENU_BUTTON_SIZE, MENU_BUTTON_SIZE);
            speak.Size = new Size(MENU_BUTTON_SIZE, MENU_BUTTON_SIZE);
            back.Size = new Size(MENU_BUTTON_SIZE, MENU_BUTTON_SIZE);
            _lock.Size = new Size(MENU_BUTTON_SIZE, MENU_BUTTON_SIZE);
            up.Size = new Size(ARROW_KEY_SIZE, ARROW_KEY_SIZE);
            left.Size = new Size(ARROW_KEY_SIZE, ARROW_KEY_SIZE);
            right.Size = new Size(ARROW_KEY_SIZE, ARROW_KEY_SIZE);
            down.Size = new Size(ARROW_KEY_SIZE, ARROW_KEY_SIZE);
            backWord.Size = new Size((int)(ARROW_KEY_SIZE * 1.5), ARROW_KEY_SIZE);
            forwardWord.Size = new Size((int)(ARROW_KEY_SIZE * 1.5), ARROW_KEY_SIZE);

            alarm.Location = new Point(MainMenu.GAP, MainMenu.GAP);
            speak.Location = new Point(MainMenu.GAP + alarm.Right, MainMenu.GAP);
            keyboard.Location = new Point(MainMenu.GAP, MainMenu.GAP);
        }

        private void Notepage_Resize(object sender, EventArgs e)
        {
            back.Location = new Point(this.Right - MENU_BUTTON_SIZE - MainMenu.GAP, MainMenu.GAP);
            forwardWord.Location = new Point(Width - MainMenu.GAP - forwardWord.Width, Height - MainMenu.GAP - ARROW_KEY_SIZE);
            backWord.Location = new Point(forwardWord.Left - MainMenu.GAP - backWord.Width, forwardWord.Top);
            right.Location = new Point(Width - MainMenu.GAP - ARROW_KEY_SIZE, forwardWord.Top - MainMenu.GAP - ARROW_KEY_SIZE);
            down.Location = new Point(right.Left - MainMenu.GAP - ARROW_KEY_SIZE, right.Top);
            left.Location = new Point(down.Left - MainMenu.GAP - ARROW_KEY_SIZE, right.Top);
            up.Location = new Point(down.Left, down.Top - MainMenu.GAP - ARROW_KEY_SIZE);
            keyboard.SetTextBoxLocation(new Point(2 * MENU_BUTTON_SIZE + MainMenu.GAP * 2, speak.Top));
            _lock.Location = new Point(back.Left, MENU_BUTTON_SIZE + 2 * MainMenu.GAP);

            keyboard.Size = new Size(left.Location.X - MainMenu.GAP * 2, this.Height - 2 * MainMenu.GAP);
            keyboard.SetTextBoxSize(new Size(keyboard.Right - 2 * MENU_BUTTON_SIZE - 3 * MainMenu.GAP, MENU_BUTTON_SIZE));
            keyboard.SetTextBoxFocus();
        }

        private void Notepage_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region Private Events
        void initControlsRecursive(Control.ControlCollection coll)
        {
            foreach (Control c in coll)
            {
                c.MouseClick += (sender, e) =>
                {
                    updateCursor();
                };
                initControlsRecursive(c.Controls);
            }
        }

        private void updateCursor()
        {
            keyboard.SetTextBoxFocus();
        }

        #endregion
    }
}

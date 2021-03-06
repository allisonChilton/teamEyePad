﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace ALSProject
{
    public partial class TextToSpeech : Form
    {
        protected ClearTextConfirmation clearTextConfirmation;
        protected Keyboard alsKeyboard;

        public delegate void MainMenuClick(object sender, EventArgs args);
        public delegate void CalloutsClick(object sender, EventArgs args);
        public event MainMenuClick MainMenu_Click;
        public event CalloutsClick Callouts_Click;


        #region Constructors
        public TextToSpeech(bool isQwerty)
        {
            InitializeComponent();

            clearTextConfirmation = new ClearTextConfirmation();

            btnCallouts.setFontSize();
            btnMenu.setFontSize();
            btnSpeak.setFontSize();

            initControlsRecursive(this.Controls);
            this.MouseClick += (sender, e) =>
            {
                updateCursor();
                //*TODO Delete temporary code
                getCurrentSentence();
            };

            if (isQwerty)
                alsKeyboard = new QwertyKeyboard();
            else
                alsKeyboard = new LargeButtonKeyboard();

            Controls.Add(alsKeyboard);
            alsKeyboard.Location = new Point(MainMenu.GAP, MainMenu.GAP);
            alsKeyboard.SendToBack();

            alsKeyboard.setClearConfirmation(true);
        }
        #endregion

        #region Public Methods
        public void ClearText()
        {
            alsKeyboard.SetText("");
            predictReset();
            this.Enabled = true;
        }

        public string GetText()
        {
            return alsKeyboard.GetText();
        }

        public void setText(string text)
        {
            if (text != null)
                alsKeyboard.SetText(text);
        }

        public void SetKeyboard(Keyboard k)
        {
            Controls.Remove(alsKeyboard);
            alsKeyboard = k;
            Controls.Add(alsKeyboard);
            alsKeyboard.setClearConfirmation(true);
            alsKeyboard.Location = new Point(MainMenu.GAP, MainMenu.GAP);
            TextToSpeech_Resize(this, EventArgs.Empty);
        }

        public ALSButton GetBtnCallout()
        {
            return btnCallouts;
        }
        #endregion

        #region Events
        protected virtual void btnMenu_Click(object sender, EventArgs e)
        {
            Hide();
            if (MainMenu_Click != null)
                MainMenu_Click(this, e);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            clearTextConfirmation.Visible = true;
        }

        protected void btnSpeak_Click(object sender, EventArgs e)
        {
            MainMenu.Speak(alsKeyboard.GetText());
        }

        protected void textBox1_TextChanged(object sender, EventArgs e)
        {
            updateCursor();
        }

        private void TextToSpeech_Resize(object sender, EventArgs e)
        {
            alsKeyboard.SetTextBoxLocation(new Point(btnSpeak.Right, 0));
            alsKeyboard.SetTextBoxSize(new Size(btnCallouts.Left - btnSpeak.Right - MainMenu.GAP * 2, btnCallouts.Height));
            alsKeyboard.Size = new Size(Width - MainMenu.GAP * 2, Height - MainMenu.GAP * 2);
        }

        protected void btnCallouts_Click(object sender, EventArgs e)
        {
            Hide();
            if (Callouts_Click != null)
                Callouts_Click(this, e);
        }
        #endregion

        #region Private Events
        protected void predictReset()
        {
            this.alsKeyboard.ResetPrediction();
        }

        protected string getCurrentSentence()
        {
            //Assuming that we don't want the cursor to move around, we want this.
            string text = alsKeyboard.GetText();

            var finalSentence = Regex.Match(text, "[.!?][^.!?]*$");

            //If you do want to move the caret around use this, then concatenate them
            var firstHalf = Regex.Match(text.Substring(0, alsKeyboard.GetSelectionStart()), "[.!?][^.!?]*$");
            var secondHalf = Regex.Match(text.Substring(alsKeyboard.GetSelectionStart()), "[^.!?]*[.!?]");

            string sentence = "";

            if (firstHalf.Success)
            {
                MessageBox.Show(text.Substring(firstHalf.Index, firstHalf.Length));
            }

            if (secondHalf.Success)
            {
                MessageBox.Show(text.Substring(secondHalf.Index, secondHalf.Length));
                System.Diagnostics.Debug.WriteLine(Text.Substring(secondHalf.Index, secondHalf.Length));
            }

            return sentence;
        }

        private void initControlsRecursive(System.Windows.Forms.Control.ControlCollection coll)
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

        protected void updateCursor()
        {
            alsKeyboard.SetTextBoxFocus();
            alsKeyboard.SetSelection(alsKeyboard.GetText().Length, 0);
        }
        #endregion
    }
}
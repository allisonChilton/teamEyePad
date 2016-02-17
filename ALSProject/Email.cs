﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALSProject
{
    public partial class Email : Form //this is the GUI part of e-mail
    {

        protected ALSButton[,] notes;
        protected const int EDIT_BUTTON_WIDTH = 100;
        protected const int NUM_NOTES = 6;
        protected bool isEditMode;
        protected int pageNum = 0;

        EmailClient client;

        public delegate void MainMenuClick(object sender, EventArgs args);
        public event MainMenuClick MainMenu_Click;


        // why did you do this this way
        private void btnMenu_Click(object sender, EventArgs e)
        {
            Hide();
            if (MainMenu_Click != null)
                MainMenu_Click(this, e);
        }

        public Email()
        {
            InitializeComponent();

            client = new EmailClient();
            this.CreateLayout(new System.ComponentModel.ComponentResourceManager(typeof(Email)));
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //Sending messages works just fine.
            //client.sendMessage(new MailMessage("teameyepad@gmail.com", "allison.chilton@outlook.com", "Test Email", "This is the body of the test."));

            //this interacts with the server and gets the first e-mail it finds. This is hard-coded for testing purposes 
            //but can obviously be dynamic in the future

            client.retrieveMail();

            //this code block adds the messages returned by the server to the text box. This is just for debugging purposes.

            foreach (String s in client.getMailHistory())
            {
                txtBody.Text += s + "\n";
            }

            //this whole section is looking for the encoded part of the body and the tries to decode it. 
            int index = txtBody.Text.IndexOf("base64") + 10;
            String message = txtBody.Text.Substring(index, txtBody.TextLength - index);


            //this is how I got rid of all the escaped characters. I just split the string on the invalid characters and then rejoined the remaining strings
            String[] splitMessage = message.Split(new char[] { '\r', '\n' });

            StringBuilder sb = new StringBuilder();

            foreach (String s in splitMessage)
            {
                sb.Append(s);
            }


            //this is the composite string after the split, which is just the block of base64 text without the escaped characters
            String test = sb.ToString();

            //converts the base64 text. This doesn't work yet
            message = Convert.FromBase64String(test).ToString();

            //adds the content of the message to the end of the textbox.
            txtBody.Text += "\n" + message;


        }

        private void Email_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        /* public partial class Email : Form
        {
            EmailClient client;

            */


    }
}

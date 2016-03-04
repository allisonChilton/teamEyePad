﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALSProject
{
    public partial class ALSTextbox : TextBox
    {

        Timer dwellTimer = new Timer(), decayTimer = new Timer();
        private int heightDivider = 30;
        double heightCounter;
        Graphics gr;
        bool clicked;
        Color baseColor = Color.White;
        bool firstTime;
        static bool isDecay;

        public ALSTextbox()
        {

            InitializeComponent();
            dwellTimer = new Timer();
            dwellTimer.Enabled = false;
            dwellTimer.Tick += new EventHandler(dwellTimeEvent);

            isDecay = false;
            decayTimer = new Timer();
            decayTimer.Tick += decayTimer_Tick;
            decayTimer.Interval = 150;
        }

        protected void dwellTimeEvent(object sender, EventArgs e)
        {
            double tempHeightCounter = heightCounter + (1.0 * Height) / heightDivider;

            CreateGraphics().FillRectangle(new SolidBrush(Color.FromArgb(127, 128, 128, 128)), new Rectangle(0, this.Height - (int)tempHeightCounter, this.Width, (int)tempHeightCounter - (int)heightCounter));


            if (heightCounter > this.Height)
            {
                this.OnClick(e);

                //Restart button
                heightCounter = 0;
                dwellTimer.Start();
            }
            else
            {
                heightCounter = tempHeightCounter;
            }
        }

        private void decayTimer_Tick(object sender, EventArgs e)
        {
            double addHeight = Height / heightDivider;
            Invalidate(new Rectangle(0, this.Height - (int)heightCounter, Width, (int)addHeight));

            if (heightCounter <= 0)
            {
                heightCounter = 0;
                decayTimer.Stop();
            }
            else
            {
                heightCounter -= addHeight;
            }
        }

        private void ALSTextbox_MouseEnter(object sender, EventArgs e)
        {
            dwellTimer.Enabled = true;
            firstTime = true;
            decayTimer.Stop();
        }

        private void ALSTextBox_MouseLeave(object sender, EventArgs e)
        {
            dwellTimer.Enabled = false;
            clicked = false;
            firstTime = true;
            if (isDecay)
                decayTimer.Start();
            else
                heightCounter = 0;


        }

        //deletes rectangle, restores image if any
        protected void ClearRect()
        {
            String text = Text;
            try
            {
                Image temp = (Image)this.BackgroundImage.Clone(); //deep copy)
                gr.Clear(baseColor);
                this.BackgroundImage = temp;
            }
            catch (Exception)
            {         //this just prevents the program from crashing if there is no Background Image set
                this.CreateGraphics().Clear(baseColor);    //clears rectangle if there is no image
            }
            Text = text;

            heightCounter = 0;
            clicked = false;
        }


        private void ALSTextbox_Resize(object sender, EventArgs e)
        {
            gr = this.CreateGraphics();
        }

        public static void setDecay(bool isDecay)
        {
            ALSTextbox.isDecay = isDecay;
        }

        private void ALSTextbox_Click(object sender, EventArgs e)
        {

            if (!clicked)
            { //prevents rapid clicks
                clicked = true;
                ClearRect();
                dwellTimer.Enabled = true;

                this.Focus();

                //reset
                dwellTimer.Stop();
                dwellTimer.Start();
                this.Refresh();
            }

        }
    }
}

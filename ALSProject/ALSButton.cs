using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.Timers;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;

namespace ALSProject
{
    public partial class ALSButton : Button
    {
        public enum ButtonType { key, normal, immutable };

        public delegate void Clear_Click(object sender, EventArgs args);
        public event Clear_Click ClearClick;

        public static long startTime;
        public static Color baseColor = Color.FromArgb(224, 224, 224);
        public ButtonType btnType
        {
            get; set;
        }
        public int dwellTimeInterval
        {
            get
            {
                return dwellTimer.Interval;
            }
            set
            {
                dwellTimer.Interval = value;
            }
        }

        protected double heightCounter;
        protected static bool isDecay;
        protected Timer dwellTimer, decayTimer;
        protected bool clicked = false; //prevents rapid clicks
        protected int heightDivider = 30;
        protected static List<ALSButton> alsButtons = new List<ALSButton>();
        protected bool firstTime = true;

        #region Constructors
        public ALSButton()
        {
            InitializeComponent();
            isDecay = false;

            heightCounter = 0;
            dwellTimer = new Timer();
            dwellTimer.Tick += new EventHandler(dwellTimeEvent);

            decayTimer = new Timer();
            decayTimer.Tick += decayTimer_Tick;
            decayTimer.Interval = 150;

            alsButtons.Add(this);
            btnType = ButtonType.normal;

            TextChanged += ALSButton_TextChanged;
            Disposed += ALSButton_Disposed;
        }
        #endregion

        #region Public Methods

        public static void setTimerSpeed(double speed, ButtonType buttonType)
        {
            if (speed < 0)
                return;
            foreach (ALSButton btn in alsButtons)
            {
                if (btn.btnType.Equals(buttonType))
                {
                    btn.dwellTimer.Interval = Math.Max((int)(100 - speed * 10), 1);
                    btn.decayTimer.Interval = btn.dwellTimer.Interval * 3;
                }
            }
        }

        // This function checks the room size and your text and appropriate font for your text to fit in room
        // Text is the string which it's bounds is more than room bounds.
        public void setFontSize()
        {
            Graphics g = CreateGraphics();
            SizeF RealSize = g.MeasureString(Text, Font);
            float HeightScaleRatio = (Height - 18) / RealSize.Height;
            float WidthScaleRatio = (Width - 18) / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = Font.Size * ScaleRatio;

            Font = new Font(Font.FontFamily, Math.Min(ScaleFontSize < 8 ? 5 : ScaleFontSize, 50));
        }

        public static void setDecay(bool isDecay)
        {
            ALSButton.isDecay = isDecay;
            ALSTextbox.setDecay(isDecay);
        }

        public static void toggleDecay()
        {
            isDecay = !isDecay;
        }

        public static bool getDecay()
        {
            return isDecay;
        }
        #endregion

        #region Events

        private void ALSButton_Disposed(object sender, EventArgs e)
        {
            if (dwellTimer != null)
                dwellTimer.Stop();
            if (decayTimer != null)
                decayTimer.Stop();
        }

        private void ALSButton_TextChanged(object sender, EventArgs e)
        {
            setFontSize();
        }

        private void decayTimer_Tick(object sender, EventArgs e)
        {
            double addHeight = Height / heightDivider;
            if (firstTime)
            {
                CreateGraphics().FillRectangle(new SolidBrush(Color.FromArgb(127, 128, 128, 128)), new Rectangle(0, this.Height - (int)heightCounter + (int)addHeight, this.Width, Height));
                firstTime = false;
            }
            else
            {
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
            
        }

        protected void dwellTimeEvent(object sender, EventArgs e)
        {
            double tempHeightCounter = heightCounter + (1.0 * Height) / heightDivider;

            if (firstTime)
            {
                CreateGraphics().FillRectangle(new SolidBrush(Color.FromArgb(127, 128, 128, 128)), new Rectangle(0, this.Height - (int)tempHeightCounter, this.Width, (int)tempHeightCounter));
                firstTime = false;
            }
            else
            {
                CreateGraphics().FillRectangle(new SolidBrush(Color.FromArgb(127, 128, 128, 128)), new Rectangle(0, this.Height - (int)tempHeightCounter, this.Width, (int)tempHeightCounter - (int)heightCounter));
            }

            if (heightCounter > this.Height)
            {
                this.PerformClick();

                //Restart button
                heightCounter = 0;
                //dwellTimer.Start();
            }
            else
            {
                heightCounter = tempHeightCounter;
            }
        }

        private void ALSButton_MouseEnter(object sender, EventArgs e)
        {
            dwellTimer.Enabled = true;
            firstTime = true;
            decayTimer.Stop();
        }

        private void ALSButton_MouseLeave(object sender, EventArgs e)
        {
            dwellTimer.Enabled = false;
            clicked = false;
            firstTime = true;
            if (isDecay)
                decayTimer.Start();
            else
                heightCounter = 0;
        }

        private void ALSButton_Resize(object sender, EventArgs e)
        {
            setFontSize();
        }

        private void ALSButton_Click(object sender, EventArgs e)
        {
            startTime = DateTime.Now.ToFileTime();
            //prevents rapid clicks
            if (!clicked)
            {
                clicked = true;
                heightCounter = 0;

                if (sender != null && sender is ALSButton && ClearClick != null)
                {
                    ALSButton button = (ALSButton)sender;
                    if (button.Text.Equals("Clear"))
                    {
                        ClearClick(this, e);
                    }
                }

                //reset
                dwellTimer.Stop();
                decayTimer.Stop();
            }
        }

        #endregion
    }
}
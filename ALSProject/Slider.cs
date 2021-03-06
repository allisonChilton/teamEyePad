using System;
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
    public partial class Slider : UserControl
    {
        public enum direction { LEFT, RIGHT, NEUTRAL };
        private double minAmount, maxAmount, increment;
        public double value;

        protected Rectangle slideBox;
        protected Rectangle bounds;
        protected ALSButton btnLeft, btnRight;
        protected Label title;
        protected static Color sliderColor = Color.FromArgb(220, 220, 220);

        public delegate void btnRight_Click(object sender, EventArgs e);
        public delegate void btnLeft_Click(object sender, EventArgs e);
        public event btnRight_Click BtnRight_Click;
        public event btnLeft_Click BtnLeft_Click;
        
        #region Constructors
        public Slider(string title) : this(title, 5) { }

        public Slider(string title, double value)
        {
            InitializeComponent();
            initSlider(1, 0, 10);
            if (value >= minAmount && value <= maxAmount)
                this.value = value;
            this.title = new Label();
            this.title.Text = title;
            this.title.Font = this.Font;
            this.title.ForeColor = Color.AntiqueWhite;
            Controls.Add(this.title);

            btnLeft = new ALSButton();
            btnLeft.Text = "Slower";
            btnLeft.Click += BtnLeft_Click1;
            Controls.Add(btnLeft);

            btnRight = new ALSButton();
            btnRight.Text = "Faster";
            btnRight.Click += BtnRight_Click1;
            Controls.Add(btnRight);

        }
        #endregion

        #region Public Methods
        public void initSlider(double inc, double min, double max)
        {
            increment = inc;
            minAmount = min;
            maxAmount = max;

            int numIncrements = (int)((max - min) / inc);

            value = numIncrements / 2 * inc + min;
            int leftBounds = (int)((value - minAmount) / (maxAmount - minAmount) * this.Width);
            slideBox = new Rectangle(leftBounds, 0, this.Width / 100, this.Height);
            bounds = new Rectangle(Location.X, Location.Y, Size.Width, Size.Height);
        }
        #endregion

        #region Events
        public void BtnLeft_Click1(object sender, EventArgs e)
        {
            UpdatePos(direction.LEFT);
            if (BtnLeft_Click != null)
                BtnLeft_Click(this, e);
        }

        private void BtnRight_Click1(object sender, EventArgs e)
        {
            UpdatePos(direction.RIGHT);
            if (BtnRight_Click != null)
                BtnRight_Click(this, e);
        }

        private void Slider_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = this.CreateGraphics();
            gr.Clear(sliderColor);
            gr.FillRectangle(new SolidBrush(Color.FromArgb(255, 32, 32, 32)), new Rectangle(0, 0, Width, Height));
            gr.FillRectangle(Brushes.SlateGray, bounds);
            gr.FillRectangle(Brushes.AntiqueWhite, slideBox);
        }

        private void Slider_Resize(object sender, EventArgs e)
        {
            btnLeft.Size = new Size(Width / 6, Height);
            btnRight.Size = new Size(Width / 6, Height);
            btnRight.Location = new Point((5 * Width) / 6, 0);

            bounds = new Rectangle(btnLeft.Right, (3 * Height) / 4, (4 * Size.Width) / 6, Height / 4);
            slideBox = new Rectangle(slideBox.X, bounds.Y, slideBox.Width, bounds.Height);

            //Update text size and location
            title.Size = new Size(Width / 2, Height / 2);
            resizeText();
            Graphics g = CreateGraphics();
            SizeF textSize = g.MeasureString(title.Text, title.Font);
            title.Size = new Size((int)textSize.Width + 10, (int)textSize.Height + 10);
            title.Location = new Point((Width - title.Width) / 2, 0);

            //Update slideBox
            UpdatePos(direction.NEUTRAL);
        }
        #endregion

        #region Private Events
        private void UpdatePos(Slider.direction dir)
        {
            switch (dir)
            {
                case direction.LEFT:
                    if (value > minAmount)
                        value -= increment;
                    break;
                case direction.RIGHT:
                    if (value < maxAmount)
                        value += increment;
                    break;
            }
            int leftBounds;
            if (value >= maxAmount)
                leftBounds = bounds.Width - slideBox.Width;
            else
                leftBounds = (int)((value - minAmount) / (maxAmount - minAmount) * bounds.Width);

            slideBox.Location = new Point(bounds.X + leftBounds, slideBox.Location.Y);

            Invalidate();
        }

        private void resizeText()
        {
            Graphics g = CreateGraphics();
            SizeF RealSize = g.MeasureString(title.Text, title.Font);
            float HeightScaleRatio = ((4 * Height) / 6) / RealSize.Height;
            float WidthScaleRatio = ((4 * Width) / 6) / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = Font.Size * ScaleRatio;

            title.Font = new Font(Font.FontFamily, Math.Min(ScaleFontSize < 8 ? 5 : ScaleFontSize, 50));
        }
        #endregion
    }
}
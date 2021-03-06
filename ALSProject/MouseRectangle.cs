using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ALSProject
{
    class MouseRectangle
    {
        protected const int MOUSEWAIT = 15;
        protected const int RECT_SIZE = 100;

        protected Rectangle bounds;
        protected Timer timer;
        protected Form parentForm;
        protected Control parentControl;
        protected int mouseCount = -1;
        protected bool clickmode = false;

        #region Constructors
        public MouseRectangle(Form parent)
        {
            timer = new Timer();
            timer.Tick += new System.EventHandler(timerEvent);
            timer.Interval = 100;
            timer.Enabled = true;
            bounds.Size = new Size(RECT_SIZE, RECT_SIZE);
            parentForm = parent;

        }

        public MouseRectangle(Control parent)
        {
            timer = new Timer();
            timer.Tick += new System.EventHandler(timerEvent);
            timer.Interval = 100;
            timer.Enabled = false;
            bounds.Size = new Size(RECT_SIZE, RECT_SIZE);
            parentControl = parent;
            bounds.Location = new Point(Cursor.Position.X - bounds.Size.Width / 2, Cursor.Position.Y - bounds.Size.Height / 2);
        }
        #endregion

        #region Public Methods
        public void setClickmode(bool mode)
        {
            clickmode = mode;
        }

        public void UpdateRect()
        {
            /*
            bool doEvent = false;
            if (parentForm != null && parentForm.Visible)
            {
                gr = parentForm.CreateGraphics();
                doEvent = true;
            }
            else if (parentControl != null && parentControl.Visible)
            {
                gr = parentControl.CreateGraphics();
                doEvent = true;
            }


            if (doEvent)
            {
                gr.Clear(Color.FromArgb(32, 32, 32));
                bounds.Location = new Point(Cursor.Position.X - bounds.Size.Width / 2, Cursor.Position.Y - bounds.Size.Height / 2);
                gr.FillRectangle(Brushes.SkyBlue, bounds);
            }
            */
        }
        #endregion

        #region Events
        private void timerEvent(object sender, EventArgs e)
        {
            mouseCount++;
            if ((parentForm != null && parentForm.Visible) || (parentControl != null && parentControl.Visible))
            {

                if (!bounds.Contains(Cursor.Position))
                {
                    bounds.Location = new Point(Cursor.Position.X - bounds.Size.Width / 2, Cursor.Position.Y - bounds.Size.Height / 2);
                    parentForm.CreateGraphics().Clear(Color.FromArgb(32, 32, 32));
                    parentForm.CreateGraphics().FillRectangle(Brushes.SkyBlue, bounds);
                    mouseCount = -1;
                    

                }

                if (mouseCount == MOUSEWAIT && clickmode)
                {
                    MouseSimulator.ClickLeftMouseButton();
                    System.Media.SystemSounds.Asterisk.Play();
                }
                else if (mouseCount >= MOUSEWAIT)
                    mouseCount = -1;
            }
        }
        #endregion
    }
}
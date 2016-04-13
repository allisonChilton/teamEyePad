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
    public partial class ALSMessageBox : Form
    {
        Label lblMessage;
        public ALSMessageBox(String message)
        {
            InitializeComponent();
            lblMessage = new Label();
            lblMessage.AutoSize = true;
            lblMessage.Text = message;
            lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblMessage.Location = new System.Drawing.Point(20, 12);
            lblMessage.ForeColor = System.Drawing.Color.White;
            
            this.Controls.Add(lblMessage);

        }

        private void btnDismiss_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

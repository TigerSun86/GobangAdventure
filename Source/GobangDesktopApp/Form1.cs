using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GobangDesktopApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            int h = Screen.PrimaryScreen.WorkingArea.Height;
            int w = Screen.PrimaryScreen.WorkingArea.Width;
            int boardSize = (int)(Math.Min(w, h) * 0.7);
            this.ClientSize = new Size(boardSize, boardSize);
        }


        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            this.panel2.GameDisplayPanel_Paint(sender, e);
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            panel2.GameDisplayPanel_MouseUp(sender, e);
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            panel2.GameDisplayPanel_Resize(sender, e);
        }
    }
}

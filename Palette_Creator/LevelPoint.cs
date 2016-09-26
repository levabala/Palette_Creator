using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Palette_Creator
{
    class LevelPoint : Control
    {
        private Point MouseDownLocation;

        public LevelPoint(Point pos)
        {
            Width = 30;
            Height = 30;            
            BackColor = Color.Aqua;

            Left = pos.X - 30;
            Top = pos.Y;

            MouseDown += LP_MouseDown;
            MouseMove += LP_MouseMove;
        }

        private void LP_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
            }
        }

        private void LP_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Left = e.X + Left - MouseDownLocation.X;
                Top = e.Y + Top - MouseDownLocation.Y;
            }
        }
    }
}

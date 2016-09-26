using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Palette_Creator
{
    public partial class Form1 : Form
    {
        Form form2;
        List<Point> points = new List<Point>();
        List<Color> colors = new List<Color>();
        Point startPoint = new Point(0,0);
        Point endPoint = new Point(0, 0);
        string imagePath = @"D:\work\images\1.jpg";

        Palette palette = new Palette();

        public Form1()
        {
            form2 = new Form();
            form2.TopMost = true;            
            form2.Load += Form2_Load;            

            form2.Paint += Form2_Paint;
            form2.KeyPress += Form2_KeyPress;         

            InitializeComponent();
            Paint += Form1_Paint;
            MouseDown += Form1_MouseDown;
            MouseUp += Form1_MouseUp;
            MouseMove += Form1_MouseMove;
        }

        private void Form2_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 's':
                    palette.Smooth();
                    form2.Invalidate();
                    break;
                case 'm':                    
                    palette.Multiply();
                    form2.Invalidate();
                    break;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            form2.Left = Left + Width;
            form2.Top = Top;
            form2.Height = Height;  
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Text = e.Location.ToString() + " Color: ";
            string c = "";
            if (e.X >= bm.Width || e.Y >= bm.Height) c = "None";
            else
            {
                Color cc = bm.GetPixel(e.X, e.Y);
                c = cc.ToString();
            }
            Text += c;
            endPoint = e.Location;
            if (e.Button == MouseButtons.Left) Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            endPoint = e.Location;
            float dx = endPoint.X - startPoint.X;
            float dy = endPoint.Y - startPoint.Y;

            float pointsCount = (dx > dy) ? dx : dy;

            float stepx = dx / pointsCount;
            float stepy = dy / pointsCount;

            points = new List<Point>();
            
            for (int i = 0; i < pointsCount; i++)
            {
                points.Add(new Point(startPoint.X + (int)(stepx * i), startPoint.Y + (int)(stepy * i)));
            }
            getPalette();

            Invalidate();
        }

        private void getPalette()
        {
            colors = new List<Color>();
            foreach (Point p in points)
            {
                if (p.X >= bm.Width || p.Y >= bm.Height) continue;
                colors.Add(bm.GetPixel(p.X, p.Y));
            }
            palette.SetColors(colors);
            form2.Invalidate();
        }

        Pen myPen = new Pen(Color.Blue, 3);
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(bm, 0, 0, bm.Width, bm.Height);

            g.DrawLine(myPen, startPoint, endPoint);
        }

        Pen palettePen = new Pen(Color.Black, 1);        
        private void Form2_Paint(object sender, PaintEventArgs e)
        {            
            Graphics g = e.Graphics;
            for (int i = 0; i < palette.colors.Count; i++)
            {
                palettePen.Color = palette.colors[i];
                g.DrawLine(palettePen, new Point(5, i), new Point(15, i));
            }
        }
        
        Bitmap bm;
        private void Form1_Load(object sender, EventArgs e)
        {
            bm = new Bitmap(imagePath);

            form2.Show();
        }
    }
}

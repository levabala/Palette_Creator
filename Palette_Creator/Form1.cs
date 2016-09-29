using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Palette_Creator
{
    public partial class Form1 : Form
    {
        AdditionalForm form2;
        List<Point> points = new List<Point>();
        List<Color> colors = new List<Color>();
        List<LevelPoint> levelPoints = new List<LevelPoint>();
        List<int[]> lines = new List<int[]>();        
        Point startPoint = new Point(0,0);
        Point endPoint = new Point(0, 0);
        string imagePath = @"D:\work\images\1.jpg";
        Matrix m = new Matrix();
        Matrix paletteMatrix = new Matrix();
        Matrix btmMatrix = new Matrix();

        bool drawSrc = true;        
        List<Palette> palettes = new List<Palette>();
        Pen palettePen = new Pen(Color.Black, 1);
        Pen levelPen = new Pen(Color.Black, 1);

        public Form1()
        {
            ReadFiles();

            levelPen.DashStyle = DashStyle.Dash;

            
            form2 = new AdditionalForm();
            
            form2.TopMost = true;                      
            form2.Load += Form2_Load;            

            form2.Paint += Form2_Paint;
            form2.KeyPress += Form2_KeyPress;         

            InitializeComponent();
            Paint += Form1_Paint;
            MouseDown += Form1_MouseDown;
            MouseUp += Form1_MouseUp;
            MouseMove += Form1_MouseMove;
            MouseWheel += Form1_MouseWheel;
            MouseClick += Form1_MouseClick;
            KeyPress += Form1_KeyPress;
            FormClosed += Form1_FormClosed;            
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 'К': //beautiful code >_<
                case 'к':
                case 'R':
                case 'r':
                    m = new Matrix();
                    Invalidate();
                    break;
                case 's':
                    string date = DateTime.Now.ToString();
                    date = Regex.Replace(date, @"\s+", "");
                    date = Regex.Replace(date, @":", "-");
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Plot" + date + ".bmp";
                    btm.Save(path);
                    Text = "Saved";
                    break;
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                drawSrc = true;
                btmMatrix = m;
                m = paletteMatrix;
            }
            Invalidate();
        }

        private void CreateLevelPoint(Palette pal)
        {            
            int y = palettes[0].colors.Count;
            int palIndex = palettes.IndexOf(pal);
            for (int i = 0; i < palIndex; i++)
                y += palettes[i].colors.Count;
            LevelPoint lp = new LevelPoint(new Point(form2.ClientRectangle.Width-10, y), pal);
            lp.MouseClick += Lp_MouseClick;
            lp.KeyPress += Form2_KeyPress;
            lp.MouseMove += Lp_MouseMove;
            lp.MouseUp += Lp_MouseUp;
            form2.Controls.Add(lp);
            levelPoints.Add(lp);

            Button addBut = new Button();
            addBut.Text = "Add";
            addBut.Click += AddBut_Click;
            addBut.Height = 30;
            addBut.Top = form2.ClientRectangle.Height - 30;
            addBut.Left = form2.ClientRectangle.Width - addBut.Width;
            addBut.KeyPress += Form2_KeyPress;
            form2.Controls.Add(addBut);
        }

        private void Lp_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            LevelPoint lp = (LevelPoint)sender;

            int y = palettes[0].colors.Count;
            int palIndex = palettes.IndexOf(lp.dependentPal);
            for (int i = 0; i < palIndex; i++)
                y += palettes[i].colors.Count;
            if (y < lp.Top)
            {

            }
        }

        private void Lp_MouseMove(object sender, MouseEventArgs e)
        {
            form2.Invalidate();
        }

        string data_folder = @"C:\Users\levabala\Downloads\a";
        List<Point> testPoints = new List<Point>();
        Bitmap btm = new Bitmap(1,1);
        double paletteCoef = 0;
        int max, min;        
        private void ReadFiles()
        {
            string[] files = Directory.GetFiles(data_folder, "*_a.*");
            List<Tuple<int, string>> ff = new List<Tuple<int, string>>();
            foreach (string f in files)
            {
                ff.Add(new Tuple<int, string>(int.Parse(Path.GetExtension(f).Replace(".","")), f));
            }
            var ffS = ff.OrderBy(t => t.Item1);

            //int number;            
            foreach (var t in ffS)
            {
                string p = t.Item2;
                lines.Add(new int[1024]);
                byte[] buff = File.ReadAllBytes(p);
                min = max = BitConverter.ToInt32(buff, 0);
                for (int i = 0; i < buff.Length - 4; i += 4)
                {
                    int intens = BitConverter.ToInt32(buff, i);
                    lines[lines.Count - 1][i / 4] = intens;
                    if (intens > max) max = intens;
                    if (intens < min) min = intens;
                }
            }           
        }

        private void refreshBitmap()
        {
            Palette palette = new Palette();
            List<Color> colors = new List<Color>();
            foreach (Palette pal in palettes)
            {
                colors.AddRange(pal.colors);
            }
            palette.SetColors(colors);
            if (palette.colors.Count == 0) return;

            btm = new Bitmap(lines[0].Length, lines.Count);
            paletteCoef = (double)palette.colors.Count / (max - min);
            for (int a = 0; a < lines.Count; a++)
            {
                int[] arr = lines[a];
                for (int i = 0; i < arr.Length; i++)
                {
                    int intens = arr[i];
                    int colorI = (int)((double)(intens - min) * paletteCoef);
                    if (colorI > palette.colors.Count - 1) colorI = palette.colors.Count - 1;
                    btm.SetPixel(i, a, palette.colors[colorI]);
                }
            }
        }

        private void Lp_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                form2.Controls.Remove((LevelPoint)sender);
        }

        private void AddBut_Click(object sender, EventArgs e)
        {
            //CreateLevelPoint();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //palette.WriteXML();
        }

        private PointF DataPoint(PointF scr)
        {
            Matrix mr = m.Clone();
            mr.Invert();
            PointF[] po = new PointF[] { new PointF(scr.X, scr.Y) };
            mr.TransformPoints(po);
            return po[0];
        }
        private Point DataPoint(Point scr)
        {
            Matrix mr = m.Clone();
            mr.Invert();
            Point[] po = new Point[] { new Point(scr.X, scr.Y) };
            mr.TransformPoints(po);
            return po[0];
        }
        private void Form2_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case 's':
                    foreach (Palette pal in palettes)
                        pal.Smooth();
                    form2.Invalidate();
                    break;
                case 'm':
                    foreach (Palette pal in palettes)
                        pal.Multiply();
                    form2.Invalidate();
                    break;
                case 't':
                    foreach (Palette pal in palettes)
                        pal.Sort();
                    form2.Invalidate();
                    break;
                case 'r':
                    refreshBitmap();
                    Invalidate();
                    break;
                case 'c':
                    palettes = new List<Palette>();
                    foreach (LevelPoint lp in levelPoints)
                        form2.Controls.Remove(lp);
                    levelPoints = new List<LevelPoint>();
                    form2.Invalidate();
                    refreshBitmap();
                    Invalidate();
                    break;
                case 'f':
                    foreach (Palette pal in palettes)
                        pal.Compress();
                    form2.Invalidate();
                    break;
                case 'q':
                    palettes.Remove(palettes[palettes.Count - 1]);
                    levelPoints.Remove(levelPoints[levelPoints.Count - 1]);
                    form2.Invalidate();
                    refreshBitmap();
                    Invalidate();
                    break;
            }            
            Invalidate();
        }
            
        private void Form2_Load(object sender, EventArgs e)
        {            
            form2.Left = Left + Width;
            form2.Top = Top;
            form2.Height = Height;                        
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {            
            string c = "";
            var pos = DataPoint(e.Location);
            Text = pos.ToString() + " Color: ";
            if (e.X >= bm.Width || e.Y >= bm.Height || (pos.X >= bm.Width || pos.Y >= bm.Height || pos.X < 0 || pos.Y < 0)) c = "None";            
            else
            {                
                Color cc = bm.GetPixel(pos.X, pos.Y);
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
            if (e.Button == MouseButtons.Right) return;
            endPoint = e.Location;            

            float dx = endPoint.X - startPoint.X;
            float dy = endPoint.Y - startPoint.Y;

            float pointsCount = Math.Abs((dx > dy) ? dx : dy);

            float stepx = dx / pointsCount;
            float stepy = dy / pointsCount;

            points = new List<Point>();            

            for (int i = 0; i < pointsCount; i++)
            {
                Point p = new Point(startPoint.X + (int)(stepx * i), startPoint.Y + (int)(stepy * i));
                p = DataPoint(p);
                points.Add(p);
            }
            getPalette();

            Invalidate();
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            PointF pos = DataPoint(e.Location);
            bool inXscale = e.Location.Y < 50;
            bool inYscale = e.Location.X < 50;
            float z = e.Delta > 0 ? 1.1f : 1.0f / 1.1f;
            float kx = z;
            float ky = z;
            if (ModifierKeys.HasFlag(Keys.Control) || inXscale) ky = 1;
            if (ModifierKeys.HasFlag(Keys.Shift) || inYscale) kx = 1;
            PointF po = DataPoint(e.Location);
            m.Translate(po.X, po.Y);
            m.Scale(kx, ky);
            m.Translate(-po.X, -po.Y);

            startPoint = MousePosition;
            endPoint = MousePosition;

            Invalidate();            
        }

        private void getPalette()
        {
            drawSrc = false;
            paletteMatrix = m;
            m = btmMatrix;
            colors = new List<Color>();
            foreach (Point p in points)
            {
                if (p.X >= bm.Width || p.Y >= bm.Height || p.X < 0 || p.Y < 0) continue;
                colors.Add(bm.GetPixel(p.X, p.Y));
            }
            Palette palette = new Palette();
            palette.SetColors(colors);
            palettes.Add(palette);
            refreshBitmap();
            CreateLevelPoint(palette);
            form2.Invalidate();
        }

        Pen myPen = new Pen(Color.Blue, 3);
        private void Form1_Paint(object sender, PaintEventArgs e)
        {            
            Graphics g = e.Graphics;            
            g.Transform = m;
            if (!drawSrc) g.DrawImage(btm, 0, 0);
            else
            {                                
                g.DrawImage(bm, 0, 0, bm.Width, bm.Height);
                g.Transform = new Matrix();
                g.DrawLine(myPen, startPoint, endPoint);
            }
            //g.DrawLines(Pens.Green, testPointsArr);
            /*g.DrawImage(bm, 0, 0, bm.Width, bm.Height);
            g.Transform = new Matrix();
            */
        }
        
        private void Form2_Paint(object sender, PaintEventArgs e)
        {            
            Graphics g = e.Graphics;            
            int y = 0;
            foreach (Palette pal in palettes)
            {
                foreach (Color c in pal.colors)
                {
                    palettePen.Color = c;
                    g.DrawLine(palettePen, new Point(5, y), new Point(15, y));
                    y++;
                }
            }            
            foreach (LevelPoint lp in levelPoints)
            {
                //g.DrawLine(Pens.Black, 0, lp.Top+lp.Height/2, form2.ClientRectangle.Width, lp.Top + lp.Height / 2);
                g.DrawLine(Pens.Black, 0, lp.Top, form2.ClientRectangle.Width, lp.Top);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Palette_Creator
{
    class Palette
    {
        public List<Color> colors = new List<Color>();
        public Palette()
        {

        }
        public Palette(List<Color> cs)
        {
            colors = cs;
        }

        public void SetColors(List<Color> cs)
        {
            colors = cs;
        }

        public void Smooth()
        {
            if (colors.Count < 3) return;

            List<Color> newColors = new List<Color>();
            newColors.Add(SumColors(colors[0], colors[1]));
            for (int i = 0; i < colors.Count-1; i++)
            {
                newColors.Add(SumColors(colors[i], colors[i + 1]));
            }
            newColors.Add(SumColors(colors[colors.Count - 2], colors[colors.Count - 1]));
            colors = newColors;
        }

        public Color SumColors(Color c1, Color c2)
        {
            Color color = Color.FromArgb((c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2);
            return color;
        }

        public void Multiply()
        {
            List<Color> newColors = new List<Color>();            
            for (int i = 1; i < colors.Count - 1; i++)
            {
                newColors.Add(SumColors(colors[i - 1], colors[i]));
                newColors.Add(SumColors(colors[i], colors[i + 1]));
            }            
            colors = newColors;
        }
    }
}

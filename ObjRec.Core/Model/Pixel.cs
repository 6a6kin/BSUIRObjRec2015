using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjRec.Core.Model
{
    public class Pixel
    {
        public double Red { get; set; }
        public double Green { get; set; }
        public double Blue { get; set; }

        public int Intensity
        {
            get { return (int) (0.2126*Red + 0.7152*Green + 0.0722*Blue); }
            set { Red = Green = Blue = value; }
        }

        public Pixel()
        {
        }

        public Pixel(Color color)
        {
            Red = color.R;
            Green = color.G;
            Blue = color.B;
        }

        public void Clear()
        {
            Red = Green = Blue = 0.0;
        }

        public void CorrectColor()
        {
            if (Blue > 255)
            { Blue = 255; }
            else if (Blue < 0)
            { Blue = 0; }

            if (Green > 255)
            { Green = 255; }
            else if (Green < 0)
            { Green = 0; }

            if (Red > 255)
            { Red = 255; }
            else if (Red < 0)
            { Red = 0; }
        }
    }
}

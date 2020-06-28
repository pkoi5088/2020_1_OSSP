using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GIFMaker.Core
{
    public class BitmapFrame
    {
        public Bitmap bitmap { get; }
        public double pts { get; }

        public BitmapFrame(Bitmap _bitmap, double _pts)
        {
            bitmap = _bitmap;
            pts = _pts;
        }
    }
}

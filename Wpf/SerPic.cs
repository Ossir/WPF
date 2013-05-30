using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wpf
{
    [Serializable]
    class SerPic
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public string PicArray;
        int Count;

        public SerPic()
        {
        }

        public SerPic(double x, double y, double width, double height, string pic)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.PicArray = pic;
        }

        public void SetCount(int c)
        {
            this.Count = c;
        }

        public int GetCount()
        {
            return this.Count;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Wpf
{
    [Serializable]
    class SerText
    {
        public double X;
        public double Y;
        public string FontName;
        public double FontSize;
        public string FontColor;
        //public int PicNumber;
        public string Text;
        //public FontStyle FontStyle;
        //public Font FontInfo;

        public SerText()
        {
        }

        public SerText(double x, double y, string fn, double fs, string fc, /*int pn,*/ string t /*FontStyle st*//*, Font fi*/)
        {
            this.X = x;
            this.Y = y;
            this.FontName = fn;
            this.FontSize = fs;
            this.FontColor = fc;
            //this.PicNumber = pn;
            this.Text = t;
            //this.FontStyle = st;
            //this.FontInfo = fi;
        }
    }
}

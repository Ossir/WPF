using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wpf
{
    [Serializable]
    class SerGrid
    {
        public double X;
        public double Y;
        public int columns;
        public int rows;
        public List<string> text;

        public SerGrid()
        {
        }

        public SerGrid(double x, double y, int rows, int columns)
        {
            this.X = x;
            this.Y = y;
            this.rows = rows;
            this.columns = columns;
        }
    }
}

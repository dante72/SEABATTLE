using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSeaBattle
{
    public class Point
    {
        public int X { set; get; }
        public int Y { set; get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static bool operator ==(Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

    }
}

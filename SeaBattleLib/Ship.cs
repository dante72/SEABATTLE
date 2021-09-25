using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaBattleLib;

namespace SeaBattleLib
{
    enum Orientation
    {
        Horizontal,
        Vertical
    }
    class Ship
    {
        readonly public List<Point> Location = new List<Point>();
        readonly public List<Point> Area = new List<Point>();

        public Ship(Point head, int partCount = 1, Orientation orientation = Orientation.Horizontal)
        {
            for (int i = 0; i < partCount; i++)
                Location.Add(new Point(
                                        head.X + (orientation == Orientation.Vertical ? i : 0),
                                        head.Y + (orientation == Orientation.Horizontal ? i : 0),
                                        (int)Textures.Ship
                                       )
                            );
            for (int i = head.X - 1; i < head.X + (orientation == Orientation.Vertical ? partCount : 1) + 1; i++)
                for (int j = head.Y - 1; j < head.Y + (orientation == Orientation.Horizontal ? partCount : 1) + 1; j++)
                    {
                        var point = new Point(i, j, (int)Textures.Miss);
                        if (!Location.Any(x => x == point))
                            Area.Add(point);
                    }
        }
    }
}

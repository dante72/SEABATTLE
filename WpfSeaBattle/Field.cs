using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSeaBattle
{
    public class Field
    {
        int[,] mass;

        List<Ship> ships = new List<Ship>();
        public int VerticalItemsCount { private set; get; }
        public int HorizontalItemsCount { private set; get; }
        public Field(int verticalItemsCount, int horizontalItemsCount)
        {
            VerticalItemsCount = verticalItemsCount;
            HorizontalItemsCount = horizontalItemsCount;

            mass = new int[verticalItemsCount, horizontalItemsCount];
            for (int i = 0; i < verticalItemsCount; i++)
                for (int j = 0; j < horizontalItemsCount; j++)
                    mass[i, j] = 0;

            ships.Add(new Ship(new Point(1, 1), 4, Orientation.Vertical));
            ships.ForEach(ship => ship.Area.ForEach(p => mass[p.X, p.Y] = 2));
            //ships.ForEach(ship => ship.Location.ForEach(p => mass[p.X, p.Y] = 1));

        }

        public int this[int x, int y]
        {
            set { mass[x, y] = value; }
            get { return mass[x, y]; }
        }
    }
}


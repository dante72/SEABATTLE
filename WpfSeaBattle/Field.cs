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

            mass[0, 0] = 1; mass[0, 1] = 1; mass[0, 2] = 1; mass[0, 3] = 1; // delete
        }

        public int this[int x, int y]
        {
            set { mass[x, y] = value; }
            get { return mass[x, y]; }
        }
    }
}

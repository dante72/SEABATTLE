using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleLib {
    public class Field {
        private Cell[,] _points;

        public List<Ship> Ships { get; } = new List<Ship>();
        public int VerticalItemsCount { get; }
        public int HorizontalItemsCount { get; }
        public Field(int verticalItemsCount, int horizontalItemsCount) {
            VerticalItemsCount = verticalItemsCount;
            HorizontalItemsCount = horizontalItemsCount;

            _points = new Cell[verticalItemsCount, horizontalItemsCount];
            for (int i = 0; i < verticalItemsCount; i++)
                for (int j = 0; j < horizontalItemsCount; j++)
                    _points[i, j] = new Cell(i, j, Textures.Water);

            Ships.Add(new Ship(new Cell(1, 1, Textures.Ship), 4, Orientation.Vertical));


            //ships.ForEach(ship => ship.Area.ForEach(p => points[p.X, p.Y].Texture = p.Texture));
            Ships.ForEach(ship => ship.Location.ForEach(p => _points[p.X, p.Y].Texture = p.Texture));

        }

        public Cell this[int x, int y] {
            get => _points[x, y];
            set => _points[x, y] = value;

        }
    }
}


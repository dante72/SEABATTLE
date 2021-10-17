using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaBattleLib;

namespace SeaBattleLib {
    public enum Orientation { Horizontal, Vertical }

    [Serializable]
    public class Ship {
        public List<Cell> Location { get; } = new List<Cell>();
        public List<Cell> Area { get; } = new List<Cell>();

        public Ship(Cell head, int decksCount, Orientation orientation, int rowsCount, int columnsCount) {
            for (int i = 0; i < decksCount; i++)
                Location.Add(new Cell(
                                        head.X + (orientation == Orientation.Vertical ? i : 0),
                                        head.Y + (orientation == Orientation.Horizontal ? i : 0),
                                        Textures.Ship
                                       )
                            );
            for (int i = head.X - 1; i < head.X + (orientation == Orientation.Vertical ? decksCount : 1) + 1; i++)
                for (int j = head.Y - 1; j < head.Y + (orientation == Orientation.Horizontal ? decksCount : 1) + 1; j++) {
                    var point = new Cell(i, j, Textures.Miss);
                    if (!Location.Any(x => x == point))
                        if (point.X >= 0 && point.X < columnsCount && point.Y >= 0 && point.Y < rowsCount)
                            Area.Add(point);
                }
        }

        private static Random Random { get; } = new Random();
        public static Ship GenerateRandomShip(int rowsCount, int columnsCount, int decksCount = 1) =>
            new Ship(new Cell(Random.Next(columnsCount), Random.Next(rowsCount)),
                decksCount, (Orientation)Random.Next(Enum.GetValues<Orientation>().Length), rowsCount, columnsCount);
    }
}

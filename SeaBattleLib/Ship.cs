using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeaBattleLib;

namespace SeaBattleLib {
    [Serializable]
    public enum Orientation { Horizontal, Vertical }

    public class Ship {
        public List<Cell> Location { get; } = new List<Cell>();
        public List<Cell> Area { get; } = new List<Cell>();
        public bool IsDestroyed { get; set; } = false;


        public Ship(Cell head, int partCount = 1, Orientation orientation = Orientation.Horizontal) {
            for (int i = 0; i < partCount; i++)
                Location.Add(new Cell(
                                        head.X + (orientation == Orientation.Vertical ? i : 0),
                                        head.Y + (orientation == Orientation.Horizontal ? i : 0),
                                        Textures.Ship
                                       )
                            );
            for (int i = head.X - 1; i < head.X + (orientation == Orientation.Vertical ? partCount : 1) + 1; i++)
                for (int j = head.Y - 1; j < head.Y + (orientation == Orientation.Horizontal ? partCount : 1) + 1; j++) {
                    var point = new Cell(i, j, Textures.Miss);
                    if (!Location.Any(x => x == point))
                        Area.Add(point);
                }
        }
    }
}

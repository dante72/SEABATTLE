using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleLib {
    public class Field {
        private readonly Cell[,] _points;

        public List<Ship> Ships { get; } = new List<Ship>();
        public int RowsCount { get; }
        public int ColumnsCount { get; }
        public Field(int rowsCount, int columnsCount, List<Ship> ships = null) {
            RowsCount = rowsCount;
            ColumnsCount  = columnsCount;

            _points = new Cell[rowsCount, columnsCount];
            for (int i = 0; i < rowsCount; i++)
                for (int j = 0; j < columnsCount; j++)
                    _points[i, j] = new Cell(i, j, Textures.Water);

            if (ships is not null)
                Ships = ships;


            Ships.ForEach(ship => ship.Location.ForEach(p => _points[p.X, p.Y].Texture = p.Texture));

        }

        public Cell this[int x, int y] {
            get => _points[x, y];
            set => _points[x, y] = value;
        }

        private void AddShip(Ship ship) {
            Ships.Add(ship);
            Ships.ForEach(ship => ship.Location.ForEach(p => _points[p.X, p.Y].Texture = p.Texture));
        }

        /*1 корабль — ряд из 4 клеток(«четырёхпалубный»; линкор)
        2 корабля — ряд из 3 клеток(«трёхпалубные»; крейсера)
        3 корабля — ряд из 2 клеток(«двухпалубные»; эсминцы)
        4 корабля — 1 клетка(«однопалубные»; торпедные катера)*/

        //При размещении корабли не могут касаться друг друга сторонами и углами - Area
        public static Field GenerateRandomField(int rowsCount, int columnsCount) {
            Field field = new Field(rowsCount, columnsCount);
            ////Создаем 1 линкор
            //field.GenerateRandomShips(1, 4);
            ////Создаем 2 крейсера
            //field.GenerateRandomShips(2, 3);
            ////Создаем 3 эсминцы
            //field.GenerateRandomShips(3, 2);
            ////Создаем 4 катера
            //field.GenerateRandomShips(4, 1);
            for (int i = 1, j = 4; i <= 4 && j >= 1; i++, j--)
                field.GenerateRandomShips(i, j);
            return field;
        }

        private void GenerateRandomShips(int shipsCount, int decksCount) {
            Enumerable.Range(0, shipsCount).ToList().ForEach(_ => {
                while (true) {
                    Ship battleShip = Ship.GenerateRandomShip(RowsCount, ColumnsCount , decksCount);
                    if (IsLocationIsNotBusy(battleShip)) {
                        AddShip(battleShip);
                        break;
                    }
                }
            });
        }

        private bool IsLocationIsNotBusy(Ship ship) {
            for (int i = 0; i < ship.Location.Count; i++)
                if (!(ship.Location[i].X >= 0 && ship.Location[i].X < ColumnsCount )
                    || !(ship.Location[i].Y >= 0 && ship.Location[i].Y < RowsCount))
                    return false;

            foreach (var point in ship.Location)
                foreach (var ship_ in Ships)
                    if (ship_.Area.Where(p => p == point).Any() || ship_.Location.Any(p => p == point))
                        return false;
            return true;
        }
    }
}


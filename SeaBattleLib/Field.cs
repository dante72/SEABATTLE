using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleLib {
    public class Field {
        private Cell[,] _points;
        private List<Ship> _ships = new List<Ship>();
        public List<Ship> Ships 
        { 
            get { return _ships; }
            set 
            {
                if (value != null)
                    _ships = value;
                ToDrawShips();
            }
        }
        public int VerticalItemsCount { get; }
        public int HorizontalItemsCount { get; }
        public Field(int verticalItemsCount, int horizontalItemsCount, List<Ship> ships = null) {
            VerticalItemsCount = verticalItemsCount;
            HorizontalItemsCount = horizontalItemsCount;

            _points = new Cell[verticalItemsCount, horizontalItemsCount];
            for (int i = 0; i < verticalItemsCount; i++)
                for (int j = 0; j < horizontalItemsCount; j++)
                    _points[i, j] = new Cell(i, j, Textures.Water);

            if (ships != null)
                Ships = ships;

            //Ships.Add(new Ship(new Cell(1, 1, Textures.Ship), 4, Orientation.Vertical));


            //ships.ForEach(ship => ship.Area.ForEach(p => points[p.X, p.Y].Texture = p.Texture));
            //Ships.ForEach(ship => ship.Location.ForEach(p => _points[p.X, p.Y].Texture = p.Texture));

        }

        private void ToDrawShips()
        {
            Ships.ForEach(ship => ship.Location.ForEach(p => _points[p.X, p.Y].Texture = p.Texture));
        }

        private void ToDrawAreas()
        {
            Ships.ForEach(ship => ship.Area.ForEach(p => _points[p.X, p.Y].Texture = p.Texture));
        }

        public Cell this[int x, int y] {
            get => _points[x, y];
            set => _points[x, y] = value;

        }

        public void AddShip(Ship ship)
        {
            Ships.Add(ship);
            //ships.ForEach(ship => ship.Area.ForEach(p => mass[p.X, p.Y].Value = p.Value));
            Ships.ForEach(ship => ship.Location.ForEach(p => _points[p.X, p.Y].Texture = p.Texture));
        }

        /*1 корабль — ряд из 4 клеток(«четырёхпалубный»; линкор)
        2 корабля — ряд из 3 клеток(«трёхпалубные»; крейсера)
        3 корабля — ряд из 2 клеток(«двухпалубные»; эсминцы)
        4 корабля — 1 клетка(«однопалубные»; торпедные катера)*/

        //При размещении корабли не могут касаться друг друга сторонами и углами - Area
        public static Field GenerateRandomField(int verticalItemsCount, int horizontalItemsCount)
        {
            Field field = new Field(verticalItemsCount, horizontalItemsCount);
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

        private void GenerateRandomShips(int shipsCount, int partCount)
        {
            Enumerable.Range(0, shipsCount).ToList().ForEach(_ =>
            {
                while (true)
                {
                    Ship battleShip = Ship.GenerateRandomShip(VerticalItemsCount, HorizontalItemsCount, partCount);
                    if (IsLocationIsNotBusy(battleShip))
                    {
                        AddShip(battleShip);
                        break;
                    }
                }
            });
        }

        private bool IsLocationIsNotBusy(Ship ship)
        {
            for (int i = 0; i < ship.Location.Count; i++)
                if (!(ship.Location[i].X >= 0 && ship.Location[i].X < HorizontalItemsCount)
                    || !(ship.Location[i].Y >= 0 && ship.Location[i].Y < VerticalItemsCount))
                    return false;

            foreach (var point in ship.Location)
                foreach (var ship_ in Ships)
                    if (ship_.Area.Where(p => p == point).Any() || ship_.Location.Any(p => p == point))
                        return false;
            return true;
        }
    }
}


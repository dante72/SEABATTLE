using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleLib {
    public static class CellExtension {
        public static void Shoot(this Cell cell) {
            if (cell.Texture == Textures.Ship)
                cell.Texture = Textures.Destroyed;
            else
                cell.Texture = Textures.Miss;
        }
    }
}

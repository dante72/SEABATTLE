using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleLib
{
    public static class PointExtension
    {
        public static void Shoot(this Point point)
        {
            if (point.Value == (int)Textures.Ship)
                point.Value = (int)Textures.Destroyed;
            else
                point.Value = (int)Textures.Miss;
        }
    }
}

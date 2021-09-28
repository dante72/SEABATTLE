using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleLib {

    public enum GameStatus { DidNotStart, GameIsOn, GameOver, PlayerHasLeftGame }
    public static class Message {
        public const byte Сonnection = 1;
        public const byte GameStatus = 2;
        public const byte WhoseShot = 3;
        public const byte Shot = 4;
        public const byte ChatNotice = 5;
    }
}

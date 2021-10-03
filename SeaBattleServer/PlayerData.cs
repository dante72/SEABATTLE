using SeaBattleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleServer {
    class PlayerData {
        public PlayerData(TcpClient client, Field field, PlayerWalks playerWalks) {
            Client = client;
            Field = field;
            PlayerWalks = playerWalks;
        }

        public TcpClient Client { get; }
        public Field Field { get; }
        public PlayerWalks PlayerWalks { get; }


    }
}

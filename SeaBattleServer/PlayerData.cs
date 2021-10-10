using SeaBattleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleServer {
    class PlayerData {
        public PlayerData(TcpClient client, Field field, CurrentPlayer currentPlayer) {
            Client = client;
            Field = field;
            CurrentPlayer = currentPlayer;
        }

        public TcpClient Client { get; }
        public Field Field { get; }
        public CurrentPlayer CurrentPlayer { get; }


    }
}

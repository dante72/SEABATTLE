using SeaBattleLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleServer {
    class SendMessageClient {
        public static async Task SendСonnectionMessage(PlayerData player) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.Сonnection);
            writer.Write((byte)player.CurrentPlayer);

            byte[] buffer = stream.ToArray();

            await player.Client.GetStream().WriteAsync(buffer, 0, buffer.Length);

            //MemoryStream streamToSend = new MemoryStream();
            //MemoryStream streamToSerialize = new MemoryStream();
            //BinaryWriter writer = new BinaryWriter(streamToSend);
            //BinaryFormatter formatterIn = new BinaryFormatter();
            //byte[] buffer;

            //writer.Write(Message.Сonnection);;

            //formatterIn.Serialize(streamToSerialize, player.Field.Ships.ToArray());
            //buffer = streamToSerialize.ToArray();
            //writer.Write(buffer.Length);
            //writer.Write(buffer);

            //buffer = streamToSend.ToArray();

            //await player.Client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendGameStatusMessage(PlayerData player, GameStatus gameStatus) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.GameStatus);
            writer.Write((byte)gameStatus);
            byte[] buffer = stream.ToArray();

            await player.Client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendWhoseShotMessage(PlayerData player, CurrentPlayer currentPlayer) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.WhoseShot);
            writer.Write((byte)currentPlayer);
            byte[] buffer = stream.ToArray();

            await player.Client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendShotMessage(PlayerData player, Cell cell) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Message.Shot);

            writer.Write(cell.CellToByteArray());
            byte[] buffer = stream.ToArray();

            await player.Client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendChatNoticeMessage(PlayerData player, byte[] buffer) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.ChatNotice);
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await player.Client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendGameOverMessage(PlayerData player, string textMessage) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.GameOver);
            byte[] buffer = Encoding.UTF8.GetBytes(textMessage);
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await player.Client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendPlayerHasLeftGameMessage(PlayerData player) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.PlayerHasLeftGame);
            byte[] buffer = Encoding.UTF8.GetBytes("Потеряна связь с другим игроком!\n\rИгровая сессия будет прервана.");
            writer.Write(buffer.Length);
            writer.Write(buffer);
            buffer = stream.ToArray();

            await player.Client.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }
    }
}

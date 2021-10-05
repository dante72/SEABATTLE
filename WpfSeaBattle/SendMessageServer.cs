using SeaBattleLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WpfSeaBattle {
    class SendMessageServer {

        public static async Task SendСonnectionMessage(TcpClient server, Field field) {
            MemoryStream streamToSend = new MemoryStream();
            MemoryStream streamToSerialize = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(streamToSend);
            BinaryFormatter formatterIn = new BinaryFormatter();

            writer.Write(Message.Сonnection);

            formatterIn.Serialize(streamToSerialize, field.Ships.ToArray());
            byte[] buffer = streamToSerialize.ToArray();
            writer.Write(buffer.Length);
            writer.Write(buffer);

            buffer = streamToSend.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendShotMessage(TcpClient server, Cell cell) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Message.Shot);

            writer.Write(cell.CellToByteArray());
            byte[] buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendShotMessage(TcpClient server, Cell point) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.Shot);
            writer.Write(point.CellToByteArray());
            byte[] buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }
    }
}

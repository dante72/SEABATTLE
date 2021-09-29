using SeaBattleLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfSeaBattle {
    class SendMessageServer {
        public static async Task SendСonnectionMessage(TcpClient server) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.Сonnection);
            byte[] buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }

        public static async Task SendShotMessage(TcpClient server, Cell point) {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Message.Shot);
            writer.Write(point.PointToByteArray());
            byte[] buffer = stream.ToArray();

            await server.GetStream().WriteAsync(buffer, 0, buffer.Length);
        }
    }
}

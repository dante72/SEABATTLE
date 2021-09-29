using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SeaBattleLib;

namespace Server
{
    class Program
    {
        static void Main(string[] args) => new Program().Run();

        private TcpListener listener;
        private List<TcpClient> players = new List<TcpClient>();
        private const int fieldX = 10;

        const int port = 7543;
        public void Run()
        {
            string str_ip;
            try
            {
                using (StreamReader sr = new StreamReader("ip.txt"))
                {
                    str_ip = sr.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("Введите ваш IP:");
                str_ip = Console.ReadLine();
                using (StreamWriter sw = new StreamWriter("ip.txt"))
                {
                    sw.WriteLine(str_ip);
                }
            }

            IPAddress ip = IPAddress.Parse(str_ip);
            listener = new TcpListener(ip, port);
            Console.WriteLine($"{ip}: {port}");
            listener.Start();

            while (true)
            {
                TcpClient player = listener.AcceptTcpClient();
                players.Add(player);

                if (players.Count == 2)
                {
                    Field field1 = new Field(fieldX, fieldX);
                    Field field2 = new Field(fieldX, fieldX);

                    List<Task> tasks = new List<Task>();

                    tasks.Add(new Task(() => Listen(players[0], players[1], field1, field2)));
                    tasks.Add(new Task(() => Listen(players[1], players[0], field2, field1)));
                    tasks.ForEach(task => task.Start());
                    Task.WaitAll(tasks.ToArray());

                    break;
                }
            }
        }

        void Listen(TcpClient player, TcpClient player2, Field field, Field field2)
        {
            byte[] buff = new byte[1024];

            while (true)
            {
                try
                {
                    int messageCode = -1;
                    player.GetStream().Read(buff, 0, buff.Length);

                }
                catch { }
            }
        }
    }
}

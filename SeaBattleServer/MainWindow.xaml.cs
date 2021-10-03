using SeaBattleLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeaBattleServer {

    public partial class MainWindow : Window {
        private string _ipAddress;
        private int _port;
        private IList<PlayerData> _players;
        public ObservableCollection<string> Logs { get; }
        private PlayerWalks _playerWalks;
        private GameStatus _gameStatus;

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            _playerWalks = PlayerWalks.PlayerOne;
            _gameStatus = GameStatus.DidNotStart;
            _players = new List<PlayerData>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            //ConfigWindow dialog = new ConfigWindow();
            //if (dialog.ShowDialog() == true) {
            //    _ipAddress = dialog.IpAddress;
            //    _port = dialog.Port;
            //}
            //else {
            //    Close();
            //    return;
            //}

            TcpListener listener = new TcpListener(IPAddress.Parse(_ipAddress), _port);
            Title = $"IP = {_ipAddress} Port = {_port}";
            listener.Start();
            ServeClients(listener);
        }

        private async void ServeClients(TcpListener listener) {
            while (true) {
                if (_players.Count == 2)
                    return;
                TcpClient client = await listener.AcceptTcpClientAsync();
                PlayerData player = NewPlayer(client);
                lock (_players) {
                    _players.Add(player);
                }
                await SendMessageClient.SendСonnectionMessage(player);

                ServeClient(player);
            }
        }

        private async void ServeClient(PlayerData player) {
            try {
                while (true) {
                    byte[] buffer = await player.Client.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.Shot) {
                        buffer = await player.Client.ReadFromStream(1);
                        Textures texture = (Textures)buffer[0];

                        buffer = await player.Client.ReadFromStream(4);
                        int x = BitConverter.ToInt32(buffer, 0);

                        buffer = await player.Client.ReadFromStream(4);
                        int y = BitConverter.ToInt32(buffer, 0);

                        Cell cell;

                        foreach (PlayerData other in _players)
                            if (other.PlayerWalks != player.PlayerWalks) {
                                other.Field[y, x].Shoot();
                                cell = other.Field[y, x];

                            }

                        //foreach (PlayerData other in _players)
                        //    await SendMessageClient.SendMoveMessage(other, cell);

                        _playerWalks = _playerWalks == PlayerWalks.PlayerOne ? PlayerWalks.PlayerTwo : PlayerWalks.PlayerOne;

                        Dispatcher.Invoke(() => Logs.Add($"Сделал ход {player.Client.Client.RemoteEndPoint} {DateTime.Now}"));

                    }

                    else if (message == Message.ChatNotice) {
                        buffer = await player.Client.ReadFromStream(4);
                        buffer = await player.Client.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        foreach (PlayerData other in _players)
                            if (other.Client.Client.RemoteEndPoint != player.Client.Client.RemoteEndPoint)
                                await SendMessageClient.SendChatNoticeMessage(other, buffer);
                        Dispatcher.Invoke(() => Logs.Add($"Отправил сообщение в чат {player.Client.Client.RemoteEndPoint} {DateTime.Now}"));
                    }

                    //Рассылка статуса игры
                    foreach (PlayerData other in _players)
                        await SendMessageClient.SendGameStatusMessage(other, _gameStatus);

                    if (_gameStatus != GameStatus.DidNotStart && message != Message.ChatNotice) {
                        //Проверка закончилась ли игра
                        if (_gameStatus == GameStatus.GameOver) {
                            await ReportGameOver();
                            BreakConnection();
                            Close();
                            return;
                        }
                        //Отправка кто теперь ходит
                        foreach (PlayerData other in _players)
                            await SendMessageClient.SendWhoseShotMessage(other, _playerWalks);
                    }
                }
            }
            catch (Exception) {
                Dispatcher.Invoke(() => Logs.Add($"Покинул игру {player.Client.Client.RemoteEndPoint} {DateTime.Now}"));
                foreach (PlayerData other in _players)
                    if (other.Client.Client.RemoteEndPoint != player.Client.Client.RemoteEndPoint)
                        await SendMessageClient.SendPlayerHasLeftGameMessage(other);
                Close();
                return;
            }
        }

        private PlayerData NewPlayer(TcpClient client) {

            PlayerData playerData;

            if (_players.Count == 0) {
                playerData = new PlayerData(client, Field.GenerateRandomField(10, 10), PlayerWalks.PlayerOne);
                Dispatcher.Invoke(() => Logs.Add($"Первый игрок {client.Client.RemoteEndPoint} {DateTime.Now}"));
            }
            else {
                playerData = new PlayerData(client, Field.GenerateRandomField(10, 10), PlayerWalks.PlayerTwo);
                Dispatcher.Invoke(() => Logs.Add($"Второй игрок {client.Client.RemoteEndPoint} {DateTime.Now}"));
                _gameStatus = GameStatus.GameIsOn;
            }

            return playerData;
        }

        private async Task ReportGameOver() {
            foreach (PlayerData other in _players) {
                if (other.PlayerWalks == _playerWalks)
                    await SendMessageClient.SendGameOverMessage(player, "Вы победили!");
                else
                    await SendMessageClient.SendGameOverMessage(other, "Вы проиграли!");
            }

        }

        private void BreakConnection() {
            foreach (PlayerData player in _players) {
                if (player.Client.Client.Connected)
                    player.Client.Client.Shutdown(SocketShutdown.Both);
                player.Client.Client.Close();
            }
        }
    }
}

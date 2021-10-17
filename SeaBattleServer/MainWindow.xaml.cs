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
        private List<PlayerData> _players;
        public ObservableCollection<string> Logs { get; }
        private CurrentPlayer _currentPlayer;
        private GameStatus _gameStatus;

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            Logs = new ObservableCollection<string>();
            _currentPlayer = CurrentPlayer.PlayerOne;
            _gameStatus = GameStatus.DidNotStart;
            _players = new List<PlayerData>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ConfigWindow dialog = new ConfigWindow();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true) {
                _ipAddress = dialog.IpAddress;
                _port = dialog.Port;
            }
            else {
                Close();
                return;
            }

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
               
                ServeClient(client);
            }
        }

        private async void ServeClient(TcpClient client) {

            
            try {
                while (true) {
                    byte[] buffer = await client.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.Сonnection) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        BinaryFormatter formatterOut = new BinaryFormatter();
                        MemoryStream stream = new MemoryStream(buffer);
                        Ship[] ships = (Ship[])formatterOut.Deserialize(stream);
                        await AddNewClient(client, new Field(10, 10, ships.ToList()));
                    }

                    else if (message == Message.Shot) {
                        buffer = await client.ReadFromStream(1);
                        Textures texture = (Textures)buffer[0];

                        buffer = await client.ReadFromStream(4);
                        int x = BitConverter.ToInt32(buffer, 0);

                        buffer = await client.ReadFromStream(4);
                        int y = BitConverter.ToInt32(buffer, 0);
                        PlayerData opponent = _players.Where(other => other.Client.Client.RemoteEndPoint != client.Client.RemoteEndPoint).First();

                        opponent.Field[x, y].Shoot();
                        Cell cell = opponent.Field[x, y];
                        if (opponent.Field[x, y].Texture != Textures.Destroyed)
                            _currentPlayer = _currentPlayer == CurrentPlayer.PlayerOne ? CurrentPlayer.PlayerTwo : CurrentPlayer.PlayerOne;
                        

                        foreach (PlayerData other in _players)
                            await SendMessageClient.SendShotMessage(other, cell);

                        if(IsAllShipsDestroyed(opponent.Field))
                            _gameStatus = GameStatus.GameOver;

                        Dispatcher.Invoke(() => Logs.Add($"Сделал ход {client.Client.RemoteEndPoint} {DateTime.Now}"));

                    }

                    else if (message == Message.ChatNotice) {
                        buffer = await client.ReadFromStream(4);
                        buffer = await client.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        Logs.Add(Encoding.UTF8.GetString(buffer));
                        foreach (PlayerData other in _players)
                            if (other.Client.Client.RemoteEndPoint != client.Client.RemoteEndPoint)
                                await SendMessageClient.SendChatNoticeMessage(other, buffer);

                        Dispatcher.Invoke(() => Logs.Add($"Отправил сообщение в чат {client.Client.RemoteEndPoint} {DateTime.Now}"));
                    }

                    //Рассылка статуса игры
                    foreach (PlayerData other in _players)
                        await SendMessageClient.SendGameStatusMessage(other, _gameStatus);

                    if (_gameStatus != GameStatus.DidNotStart && message != Message.ChatNotice) {
                        //Проверка закончилась ли игра
                        if (_gameStatus == GameStatus.GameOver) {
                            //Рассылка сообщения об окончании игры и отключение всех клиентов
                            await ReportGameOver();
                            BreakConnection();
                            Close();
                            return;
                        }
                        //Отправка кто теперь ходит
                        foreach (PlayerData other in _players)
                            await SendMessageClient.SendWhoseShotMessage(other, _currentPlayer);
                    }
                }
            }
            catch (Exception ex) {
                Dispatcher.Invoke(() => Logs.Add($"{ex.Message} {DateTime.Now}"));
                Dispatcher.Invoke(() => Logs.Add($"Покинул игру {client.Client.RemoteEndPoint} {DateTime.Now}"));
                //Отправка сообщения о потери связи с други игроком
                foreach (PlayerData other in _players)
                    if (other.Client.Client.RemoteEndPoint != client.Client.RemoteEndPoint)
                        await SendMessageClient.SendPlayerHasLeftGameMessage(other);
                Close();
                return;
            }
        }

        //Добавление нового игрока
        private async Task AddNewClient(TcpClient client, Field field) {
            PlayerData playerData;
            lock (_players) {
                if (_players.Count == 0) {
                    playerData = new PlayerData(client, field, CurrentPlayer.PlayerOne);
                    Dispatcher.Invoke(() => Logs.Add($"Первый игрок {client.Client.RemoteEndPoint} {DateTime.Now}"));
                }
                else {
                    playerData = new PlayerData(client, field, CurrentPlayer.PlayerTwo);
                    Dispatcher.Invoke(() => Logs.Add($"Второй игрок {client.Client.RemoteEndPoint} {DateTime.Now}"));
                    _gameStatus = GameStatus.GameIsOn;
                }
                _players.Add(playerData);
            }
            await SendMessageClient.SendСonnectionMessage(playerData);
        }


        private async Task ReportGameOver() {
            foreach (PlayerData other in _players) {
                if (other.CurrentPlayer == _currentPlayer)
                    await SendMessageClient.SendGameOverMessage(other, "Вы победили!");
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

        private bool IsAllShipsDestroyed(Field field) {
            for (int i = 0; i < field.RowsCount; i++)
                for (int j = 0; j < field.ColumnsCount ; j++)
                    if (field[i, j].Texture == Textures.Ship)
                        return false;
            return true;    
            
        }
    }
}

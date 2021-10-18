using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SeaBattleLib;

namespace WpfSeaBattle {

    public partial class MainWindow : Window {
        private TcpClient _server;
        private string _ipAddress;
        private int _port;
        private CurrentPlayer _currentPlayer;
        private CurrentPlayer _player;
        private string _name;
        private GameStatus _gameStatus;

        public ObservableCollection<string> Chat { get; }
        public Field FieldWithShips { get; }
        public Field FieldWithShots { get; }

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            Chat = new ObservableCollection<string>();

            FieldWithShips = Field.GenerateRandomField(10, 10);
            FieldWithShots = new Field(10, 10);
            _gameStatus = GameStatus.DidNotStart;
            CreateFieldView(battleField1, FieldWithShots);
            CreateFieldView(battleField2, FieldWithShips, true);
            battleField1.IsEnabled = false;

        }

        private void CreateFieldView(WrapPanel wp, Field field, bool fogOfWar = false) {

            for (int i = 0; i < field.RowsCount; i++)
                for (int j = 0; j < field.ColumnsCount ; j++) {
                    var button = new ToggleButton();

                    button.Width = wp.Height / field.RowsCount;
                    button.Height = wp.Width / field.ColumnsCount ;
                    button.IsChecked = fogOfWar;
                    button.DataContext = field[i, j];
                    button.Click += Button_Click;

                    wp.Children.Add(button);
                }
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {
            ToggleButton button = e.Source as ToggleButton;
            Cell cell = button.DataContext as Cell;
            await SendMessageServer.SendShotMessage(_server, cell);

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            while (true) {
                ConnectionWindow dialog = new ConnectionWindow();
                dialog.Owner = this;
                if (dialog.ShowDialog() == true) {
                    _name = dialog.PlayerName;
                    _ipAddress = dialog.IpAddress;
                    _port = dialog.Port;
                }
                else {
                    Close();
                    return;
                }

                try {
                    _server = new TcpClient(_ipAddress, _port);
                    break;
                }
                catch (SocketException ex) {
                    MessageBox.Show(ex.Message);
                }
            }

            await SendMessageServer.SendСonnectionMessage(_server, FieldWithShips);

            ListenToServer();
        }


        private async void ListenToServer() {
            try {
                while (true) {
                    byte[] buffer = await _server.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.Сonnection) {
                        buffer = await _server.ReadFromStream(1);
                        _player = (CurrentPlayer)buffer[0];
                    }
                    else if (message == Message.GameStatus) {
                        buffer = await _server.ReadFromStream(1);
                        _gameStatus = (GameStatus)buffer[0];
                        if (_gameStatus == GameStatus.DidNotStart)
                            gameStatusTextBlock.Text = "Статус игры: Игра не началась";
                        else if(_gameStatus == GameStatus.GameIsOn)
                            gameStatusTextBlock.Text = "Статус игры: Идет игра";
                        else
                            gameStatusTextBlock.Text = "Статус игры: Игра оконченна";
                    }
                    else if (message == Message.WhoseShot) {
                        buffer = await _server.ReadFromStream(1);
                        _currentPlayer = (CurrentPlayer)buffer[0];
                        if (_currentPlayer == _player)
                            whoseMoveTextBlock.Text = "Ход: Ваш";
                        else
                            whoseMoveTextBlock.Text = "Ход: Противника";
                    }
                    else if (message == Message.Shot) {
                        buffer = await _server.ReadFromStream(1);
                        Textures texture = (Textures)buffer[0];

                        buffer = await _server.ReadFromStream(4);
                        int x = BitConverter.ToInt32(buffer, 0);

                        buffer = await _server.ReadFromStream(4);
                        int y = BitConverter.ToInt32(buffer, 0);

                        Cell cell = new Cell(x, y, texture);

                        if (_currentPlayer == _player)
                            FieldWithShots[x, y].Texture = cell.Texture;
                        else
                            FieldWithShips[x, y].Texture = cell.Texture;
                    }
                    else if (message == Message.ChatNotice) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        Chat.Add(Encoding.UTF8.GetString(buffer));
                        Border border = (Border)VisualTreeHelper.GetChild(listBox, 0);
                        ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                        scrollViewer.ScrollToBottom();
                    }
                    else if (message == Message.GameOver || message == Message.PlayerHasLeftGame) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));
                        
                        MessageBox.Show($"{Encoding.UTF8.GetString(buffer)}");
                        BreakConnection();
                        battleField1.IsEnabled = false;
                        return;
                    }
                    LockUnlockBattleField();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                BreakConnection();
                Application.Current.MainWindow.Close();
                return;
            }
        }

        private void BreakConnection() {
            if (_server.Client.Connected)
                _server.Client.Shutdown(SocketShutdown.Both);
            _server.Client.Close();
        }

        private void LockUnlockBattleField() {
            if (_gameStatus == GameStatus.GameIsOn && _currentPlayer == _player)
                battleField1.IsEnabled = true;
            else
                battleField1.IsEnabled = false;
        }

        private async void SendToChat_Click(object sender, RoutedEventArgs e) {
            if (!_server.Connected) {
                chatTextBox.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(chatTextBox.Text))
                return;
            string textMessage = $"{_name}: {chatTextBox.Text}";
            Chat.Add(textMessage);
            Border border = (Border)VisualTreeHelper.GetChild(listBox, 0);
            ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
            scrollViewer.ScrollToBottom();
            chatTextBox.Text = "";
            await SendMessageServer.SendChatNoticeMessage(_server, textMessage);
        }
    }
}

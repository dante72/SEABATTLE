using System;
using System.Collections.Generic;
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
        //private string _name;
        private GameStatus _gameStatus;
        //private string _chatMessage;
        

        public Field FieldWithShips { get; }
        public Field FieldWithShots { get; }

        private Field field;
        private Field enemyField;

        public MainWindow() {
            InitializeComponent();

            //field = new Field(10, 10);
            field = Field.GenerateRandomField(10, 10);
            enemyField = field;
            CreateFieldView(battleField1, field);
            CreateFieldView(battleField2, enemyField, true);
        }

        static private void CreateFieldView(WrapPanel wp, Field field, bool fogOfWar = false) {
            for (int i = 0; i < field.VerticalItemsCount; i++)
                for (int j = 0; j < field.HorizontalItemsCount; j++) {
                    var button = new ToggleButton();

                    button.Width = wp.Height / field.VerticalItemsCount;
                    button.Height = wp.Width / field.HorizontalItemsCount;
                    button.IsChecked = fogOfWar;
                    button.DataContext = field[i, j];
                    button.Click += Button_Click;

                    wp.Children.Add(button);
                }
        }

        private static void Button_Click(object sender, RoutedEventArgs e) {
            var button = e.Source as ToggleButton;
            var point = button.DataContext as SeaBattleLib.Cell;
            point.Shoot();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            while (true) {
                ConnectionWindow dialog = new ConnectionWindow();
                if (dialog.ShowDialog() == true) {
                    //_name = dialog.PlayerName;
                    _ipAddress = dialog.IpAddress;
                    _port = dialog.Port;
                }
                else {
                    //Close();
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
            await SendMessageServer.SendСonnectionMessage(_server);
            ListenToServer();
        }


        private async void ListenToServer() {
            try {
                while (true) {
                    byte[] buffer = await _server.ReadFromStream(1);
                    byte message = buffer[0];

                    if (message == Message.Сonnection) {
                        buffer = await _server.ReadFromStream(4);
                        buffer = await _server.ReadFromStream(BitConverter.ToInt32(buffer, 0));

                        BinaryFormatter formatterOut = new BinaryFormatter();
                        MemoryStream stream = new MemoryStream(buffer);
                        foreach (var ship in (List<Ship>)formatterOut.Deserialize(stream)) {
                            FieldWithShips.Ships.Add(ship);
                        }

                    }
                    else if (message == Message.GameStatus) {
                        buffer = await _server.ReadFromStream(1);
                        _gameStatus = (GameStatus)buffer[0];
                    }
                    else if (message == Message.WhoseShot) {

                    }
                    else if (message == Message.Shot) {
                        buffer = await _server.ReadFromStream(1);
                        Textures texture = (Textures)buffer[0];

                        buffer = await _server.ReadFromStream(4);
                        int x = BitConverter.ToInt32(buffer, 0);

                        buffer = await _server.ReadFromStream(4);
                        int y = BitConverter.ToInt32(buffer, 0);

                        Cell cell = new Cell(x, y, texture);
                    }
                    else if (message == Message.ChatNotice) {
                        
                    }

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
    }
}

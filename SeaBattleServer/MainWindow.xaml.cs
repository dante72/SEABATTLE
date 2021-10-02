using SeaBattleLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        public ObservableCollection<string> Logs { get; }
        private PlayerWalks _playerWalks;
        private GameStatus _gameStatus;

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            _playerWalks = PlayerWalks.PlayerOne;
            _gameStatus = GameStatus.DidNotStart;
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

            //TcpListener listener = new TcpListener(IPAddress.Parse(_ipAddress), _port);
            //Title = $"IP = {_ipAddress} Port = {_port}";
            //listener.Start();
            //ServeClients(listener);
        }

        private async void ServeClients(TcpListener listener) {
            //while (true) {
            //    if (_clients.Count == 2)
            //        return;
            //    TcpClient client = await listener.AcceptTcpClientAsync();
            //    ServeClient(client);
            //}
        }
    }
}

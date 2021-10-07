using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfSeaBattle {
    /// <summary>
    /// Логика взаимодействия для ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window {
        public string PlayerName { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        private bool isServer { get; set; }
        public ConnectionWindow() {
            InitializeComponent();
        }
        public ConnectionWindow(bool isServer)
        {
            InitializeComponent();

            this.isServer = isServer;
            if (isServer)
            {
                grid.Children[0].Visibility = Visibility.Collapsed;
                grid.Children[1].Visibility = Visibility.Collapsed;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void okButton_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(playerNameTextBox.Text) && !isServer) {
                MessageBox.Show(
                    "Введите имя!",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                return;
            }
            if (string.IsNullOrEmpty(ipAddressTextBox.Text)) {
                MessageBox.Show(
                    "Введите IP адресс сервера!",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                return;
            }
            if (string.IsNullOrEmpty(ipAddressTextBox.Text)) {
                MessageBox.Show(
                    "Введите порт сервера!",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                return;
            }
            PlayerName = playerNameTextBox.Text;
            IpAddress = ipAddressTextBox.Text;
            try {
                Port = int.Parse(portTextBox.Text);
            }
            catch (FormatException) {

                MessageBox.Show(
                    "Номер порта может состоять только из чисел!",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
                return;
            }
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (StreamReader sr = new StreamReader("Info.txt"))
                {
                    ipAddressTextBox.Text = sr.ReadLine();
                    portTextBox.Text = sr.ReadLine();
                    if (!isServer)
                        playerNameTextBox.Text = sr.ReadLine();
                }
            }
            catch { }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("Info.txt"))
            {
                sw.WriteLine(ipAddressTextBox.Text);
                sw.WriteLine(portTextBox.Text);
                if (!isServer)
                    sw.WriteLine(playerNameTextBox.Text);
            }
        }
    }
}

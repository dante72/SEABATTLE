using System;
using System.Windows;

namespace SeaBattleServer {

    public partial class ConfigWindow : Window {
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public ConfigWindow() {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void okButton_Click(object sender, RoutedEventArgs e) {
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
    }
}

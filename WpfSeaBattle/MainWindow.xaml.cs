using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WpfSeaBattle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var field1 = new Field(10, 10);
            CreateFieldView(battleField1, field1);
            CreateFieldView(battleField2, field1, true);
        }

        static private void CreateFieldView(WrapPanel wp, Field field, bool fogOfWar = false)
        {
            for (int i = 0; i < field.VerticalItemsCount; i++)
                for (int j = 0; j < field.HorizontalItemsCount; j++)
                {
                    var button = new ToggleButton();

                    button.Width = wp.Height / field.VerticalItemsCount;
                    button.Height = wp.Width / field.HorizontalItemsCount;
                    button.IsChecked = fogOfWar;
                    button.DataContext = field[i, j];
                    button.Click += Button_Click;

                    wp.Children.Add(button);
                }
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = e.Source as ToggleButton;
            var point = button.DataContext as Point;
            point.Shoot();
        }
    }
}

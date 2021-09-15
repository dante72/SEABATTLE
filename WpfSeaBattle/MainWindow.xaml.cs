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
                    wp.Children.Add(new ToggleButton()
                    {
                        Width = wp.Height / field.VerticalItemsCount,
                        Height = wp.Width / field.HorizontalItemsCount,
                        IsChecked = fogOfWar
                    });
                }

            for (int i = 0; i < field.VerticalItemsCount; i++)
                for (int j = 0; j < field.HorizontalItemsCount; j++)
                {
                    (wp.Children[i * field.VerticalItemsCount + j] as ToggleButton)
                        .DataContext = field[i, j];
                }
        }
    }
}

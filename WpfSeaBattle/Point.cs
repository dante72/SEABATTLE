using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSeaBattle
{
    public class Point : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string status)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(status));
            }
        }

        public int X { set; get; }
        public int Y { set; get; }

        int _value;
        public int Value {
            set {
                _value = value;
                OnPropertyChanged("Value");
            }
            get {
                return _value;
            }
        }

        public Point() { Value = -1; } 
        public Point(int x, int y, int value = -1)
        {
            X = x;
            Y = y;
            Value = value;
        }
        public static bool operator ==(Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

    }
}

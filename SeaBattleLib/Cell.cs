using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattleLib {
    [Serializable]
    public class Cell : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string status) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(status));

        public int X { get; }
        public int Y { get; }

        private Textures _texture;
        public Textures Texture {
            get => _texture;
            set {
                _texture = value;
                OnPropertyChanged(nameof(Texture));
            }
        }

        public Cell(int x, int y, Textures texture = Textures.Water) {
            X = x;
            Y = y;
            Texture = texture;
        }

        public static bool operator ==(Cell left, Cell right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Cell left, Cell right) => !(left == right);


        public byte[] PointToByteArray() {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write((byte)Texture);
            writer.Write(X);
            writer.Write(Y);

            return stream.ToArray();
        }
    }
}

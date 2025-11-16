using System.Windows;
using System.Windows.Media.Imaging;

namespace MindWeaveClient.ViewModel
{
    public class PuzzlePieceViewModel : BaseViewModel
    {
        public CroppedBitmap PieceImage { get; set; }
        public int PieceId { get; }
        public Point CorrectPosition { get; }
        public bool IsCorrectlyPlaced { get; set; } // Deberías implementar OnPropertyChanged si esto cambia la UI

        private double _x;
        public double X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(); }
        }

        private double _y;
        public double Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(); }
        }

        private int _zIndex;
        public int ZIndex
        {
            get => _zIndex;
            set { _zIndex = value; OnPropertyChanged(); }
        }

        // === INICIO MODIFICACIÓN ===
        // 1. Añade las propiedades para almacenar el tamaño
        public int Width { get; private set; }
        public int Height { get; private set; }
        // === FIN MODIFICACIÓN ===

        public PuzzlePieceViewModel(
            BitmapSource fullImage,
            int pieceId,
            int sourceX, int sourceY,
            int width, int height, // <-- Estos valores ya los estás recibiendo
            double correctX, double correctY,
            double startX, double startY)
        {
            PieceId = pieceId;
            CorrectPosition = new Point(correctX, correctY);
            IsCorrectlyPlaced = false;

            // === INICIO MODIFICACIÓN ===
            // 2. Asigna los valores a las nuevas propiedades
            this.Width = width;
            this.Height = height;
            // === FIN MODIFICACIÓN ===

            // Esta parte (la creación de la imagen) está perfecta
            Int32Rect cropRect = new Int32Rect(sourceX, sourceY, width, height);
            PieceImage = new CroppedBitmap(fullImage, cropRect);
            PieceImage.Freeze(); // Buena práctica para rendimiento

            // Esta parte también está perfecta
            X = startX;
            Y = startY;
            ZIndex = 0;
        }
    }
}
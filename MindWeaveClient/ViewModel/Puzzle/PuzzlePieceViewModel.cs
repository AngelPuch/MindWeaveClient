// --- Archivo COMPLETO Y ACTUALIZADO ---

using System.Windows;
using System.Windows.Media.Imaging;

// El namespace debe ser este, según tu estructura de archivos
namespace MindWeaveClient.ViewModel.Puzzle
{
    public class PuzzlePieceViewModel : BaseViewModel
    {
        public CroppedBitmap PieceImage { get; set; }
        public int PieceId { get; }
        public Point CorrectPosition { get; }

        // --- PROPIEDADES NUEVAS Y MODIFICADAS ---

        // Guarda la posición inicial ("bandeja") para poder regresar la pieza
        public double OriginalX { get; }
        public double OriginalY { get; }

        // Reemplaza 'IsCorrectlyPlaced'. Se activa por el servidor.
        private bool _isPlaced;
        public bool IsPlaced
        {
            get => _isPlaced;
            set
            {
                _isPlaced = value;
                OnPropertyChanged();
                // Si está colocada, no puede estar "sostenida"
                if (value) IsHeldByOther = false;
            }
        }

        // Se activa cuando OTRO jugador está arrastrando esta pieza
        private bool _isHeldByOther;
        public bool IsHeldByOther
        {
            get => _isHeldByOther;
            set { _isHeldByOther = value; OnPropertyChanged(); }
        }

        // --- FIN DE PROPIEDADES NUEVAS ---

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

        public int Width { get; private set; }
        public int Height { get; private set; }


        // --- CONSTRUCTOR MODIFICADO ---
        // Se añaden 'originalX' y 'originalY' al final
        public PuzzlePieceViewModel(
            BitmapSource fullImage,
            int pieceId,
            int sourceX, int sourceY,
            int width, int height,
            double correctX, double correctY,
            double startX, double startY,
            double originalX, double originalY) // <-- Añadidos
        {
            PieceId = pieceId;
            CorrectPosition = new Point(correctX, correctY);

            // Inicializar los campos
            _isPlaced = false;
            _isHeldByOther = false;

            this.Width = width;
            this.Height = height;

            // Guardar la posición de la "bandeja"
            this.OriginalX = originalX;
            this.OriginalY = originalY;

            Int32Rect cropRect = new Int32Rect(sourceX, sourceY, width, height);
            PieceImage = new CroppedBitmap(fullImage, cropRect);
            PieceImage.Freeze();

            // La posición actual (X, Y) empieza donde dice 'startX' y 'startY'
            X = startX;
            Y = startY;
            ZIndex = 0;
        }
    }
}
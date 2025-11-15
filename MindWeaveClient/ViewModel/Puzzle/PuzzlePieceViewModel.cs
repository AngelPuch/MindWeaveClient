using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MindWeaveClient.ViewModel
{
    public class PuzzlePieceViewModel : BaseViewModel
    {
        public int PieceId { get; }
        public Point CorrectPosition { get; } 
        public bool IsCorrectlyPlaced { get; set; } = false;

        private ImageSource pieceImage;
        public ImageSource PieceImage
        {
            get => pieceImage;
            set { pieceImage = value; OnPropertyChanged(); }
        }

        private double x;
        public double X 
        {
            get => x;
            set { x = value; OnPropertyChanged(); }
        }

        private double y;
        public double Y 
        {
            get => y;
            set { y = value; OnPropertyChanged(); }
        }

        private int _zIndex;
        public int ZIndex 
        {
            get => _zIndex;
            set { _zIndex = value; OnPropertyChanged(); }
        }

        public PuzzlePieceViewModel(
            BitmapSource fullImage, 
            int id,
            int sourceX, int sourceY, int width, int height, 
            double correctX, double correctY)
        {
            this.PieceId = id;
            this.CorrectPosition = new Point(correctX, correctY);

           
            this.PieceImage = new CroppedBitmap(
                fullImage,
                new Int32Rect(sourceX, sourceY, width, height)
            );
            this.PieceImage.Freeze(); 

            // TODO: Ponerle una posición inicial aleatoria
            this.X = new Random().Next(0, 300);
            this.Y = new Random().Next(0, 300);
            this.ZIndex = 0;
        }
    }
}
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MindWeaveClient.ViewModel.Puzzle
{
    public class PuzzlePieceViewModel : BaseViewModel
    {
        public CroppedBitmap PieceImage { get; set; }
        public int PieceId { get; }

        public double OriginalX { get; }
        public double OriginalY { get; }

        public double CorrectX { get; set; }
        public double CorrectY { get; set; }

        private bool isPlaced;
        public bool IsPlaced
        {
            get => isPlaced;
            set
            {
                isPlaced = value;
                OnPropertyChanged();
                if (value)
                {
                    IsHeldByOther = false;
                }
            }
        }

        private bool isHeldByOther;
        public bool IsHeldByOther
        {
            get => isHeldByOther;
            set
            {
                isHeldByOther = value;
                OnPropertyChanged();
            }
        }

        private double x;
        public double X
        {
            get => x;
            set
            {
                x = value;
                OnPropertyChanged();
            }
        }

        private double y;
        public double Y
        {
            get => y;
            set
            {
                y = value;
                OnPropertyChanged();
            }
        }

        private int zIndex;
        public int ZIndex
        {
            get => zIndex;
            set
            {
                zIndex = value;
                OnPropertyChanged();
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int? TopNeighborId { get; }

        public int? BottomNeighborId { get; }

        public int? LeftNeighborId { get; }

        public int? RightNeighborId { get; }

        public List<PuzzlePieceViewModel> PieceGroup { get; set; }

        public double DragOffsetX { get; set; }

        public double DragOffsetY { get; set; }


        public PuzzlePieceViewModel(
            BitmapSource fullImage,
            int pieceId,
            int sourceX, int sourceY,
            int width, int height,
            double correctX, double correctY,
            double startX, double startY,
            double originalX, double originalY,
            int? topNeighborId,
            int? bottomNeighborId,
            int? leftNeighborId,
            int? rightNeighborId)
        {
            PieceId = pieceId;

            this.CorrectX = correctX;
            this.CorrectY = correctY;

            isPlaced = false;
            isHeldByOther = false;

            this.Width = width;
            this.Height = height;

            this.OriginalX = originalX;
            this.OriginalY = originalY;

            Int32Rect cropRect = new Int32Rect(sourceX, sourceY, width, height);
            PieceImage = new CroppedBitmap(fullImage, cropRect);
            PieceImage.Freeze();

            X = startX;
            Y = startY;
            ZIndex = 0;

            this.TopNeighborId = topNeighborId;
            this.BottomNeighborId = bottomNeighborId;
            this.LeftNeighborId = leftNeighborId;
            this.RightNeighborId = rightNeighborId;

        }
    }
}
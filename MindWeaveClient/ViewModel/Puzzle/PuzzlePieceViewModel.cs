using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using MindWeaveClient.PuzzleManagerService;

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


        public PuzzlePieceViewModel(BitmapSource fullImage, PuzzlePieceDefinitionDto data)
        {
            this.PieceId = data.PieceId;
            this.Width = data.Width;
            this.Height = data.Height;

            this.CorrectX = data.CorrectX;
            this.CorrectY = data.CorrectY;

            this.OriginalX = data.InitialX;
            this.OriginalY = data.InitialY;
            this.X = data.InitialX;
            this.Y = data.InitialY;

            this.ZIndex = 0;
            this.isPlaced = false;
            this.isHeldByOther = false;

            this.TopNeighborId = data.TopNeighborId;
            this.BottomNeighborId = data.BottomNeighborId;
            this.LeftNeighborId = data.LeftNeighborId;
            this.RightNeighborId = data.RightNeighborId;

            Int32Rect cropRect = new Int32Rect(data.SourceX, data.SourceY, data.Width, data.Height);
            this.PieceImage = new CroppedBitmap(fullImage, cropRect);
            this.PieceImage.Freeze();
        }
    }
}
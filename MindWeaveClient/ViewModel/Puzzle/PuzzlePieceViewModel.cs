using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MindWeaveClient.PuzzleManagerService;

namespace MindWeaveClient.ViewModel.Puzzle
{
    public class PuzzlePieceViewModel : BaseViewModel
    {
        public BitmapSource PieceImage { get; set; }
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

        private Brush borderColor;
        public Brush BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BorderThickness));
            }
        }

        public double BorderThickness => borderColor != null && borderColor != Brushes.Transparent ? 4 : 0;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int RenderWidth { get; private set; }
        public int RenderHeight { get; private set; }

        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }

        public int? TopNeighborId { get; }
        public int? BottomNeighborId { get; }
        public int? LeftNeighborId { get; }
        public int? RightNeighborId { get; }

        public List<PuzzlePieceViewModel> PieceGroup { get; set; }

        public double DragOffsetX { get; set; }
        public double DragOffsetY { get; set; }

        public PuzzlePieceViewModel(PuzzlePieceDefinitionDto data)
        {
            PieceId = data.PieceId;
            Width = data.Width;
            Height = data.Height;

            RenderWidth = data.RenderWidth > 0 ? data.RenderWidth : data.Width;
            RenderHeight = data.RenderHeight > 0 ? data.RenderHeight : data.Height;
            OffsetX = data.OffsetX;
            OffsetY = data.OffsetY;

            CorrectX = data.CorrectX;
            CorrectY = data.CorrectY;

            OriginalX = data.InitialX;
            OriginalY = data.InitialY;
            X = data.InitialX;
            Y = data.InitialY;

            ZIndex = 0;
            isPlaced = false;
            isHeldByOther = false;

            TopNeighborId = data.TopNeighborId;
            BottomNeighborId = data.BottomNeighborId;
            LeftNeighborId = data.LeftNeighborId;
            RightNeighborId = data.RightNeighborId;

            BorderColor = Brushes.Transparent;

            if (data.PieceImageBytes != null && data.PieceImageBytes.Length > 0)
            {
                PieceImage = ConvertBytesToBitmapSource(data.PieceImageBytes);
            }
        }

        private static BitmapSource ConvertBytesToBitmapSource(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) return null;

            try
            {
                var bitmapImage = new BitmapImage();
                using (var memStream = new MemoryStream(imageBytes))
                {
                    memStream.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memStream;
                    bitmapImage.EndInit();
                }
                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch
            {
                return null;
            }
        }
    }
}
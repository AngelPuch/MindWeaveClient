using System;
using System.Windows.Media; // <-- ¡ASEGÚRATE DE QUE ESTE USING ESTÉ PRESENTE!

namespace MindWeaveClient.ViewModel
{
    public class PuzzleDisplayInfo : BaseViewModel
    {
        private bool isSelected;

        public int PuzzleId { get; }
        public string Name { get; }


        public ImageSource PuzzleImage { get; }

        public bool IsUploaded { get; }
        public string LocalFilePath { get; }

        public byte[] PuzzleBytes { get; }

        public bool IsSelected
        {
            get => isSelected;
            set { isSelected = value; OnPropertyChanged(); }
        }


        public PuzzleDisplayInfo(int puzzleId, string name, ImageSource puzzleImage, bool isUploaded, byte[] puzzleBytes = null, string localFilePath = null)
        {
            this.PuzzleId = puzzleId;
            this.Name = name;
            this.PuzzleImage = puzzleImage;
            this.IsUploaded = isUploaded;
            this.PuzzleBytes = puzzleBytes;
            this.LocalFilePath = localFilePath; 
            this.IsSelected = false;
        }
    }
}
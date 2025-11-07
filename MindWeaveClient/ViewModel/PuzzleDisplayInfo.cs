namespace MindWeaveClient.ViewModel
{
    public class PuzzleDisplayInfo : BaseViewModel
    {
        private bool isSelected;

        public int PuzzleId { get; }
        public string Name { get; }
        public string ImagePath { get; }

        public bool IsSelected
        {
            get => isSelected;
            set { isSelected = value; OnPropertyChanged(); }
        }

        public PuzzleDisplayInfo(int puzzleId, string name, string imagePath)
        {
            this.PuzzleId = puzzleId;
            this.Name = name;
            this.ImagePath = imagePath;
            this.IsSelected = false;
        }
    }
}

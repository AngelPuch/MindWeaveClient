namespace MindWeaveClient.ViewModel.Puzzle
{
    public class PuzzleSlotViewModel : BaseViewModel
    {
        private double x;
        private double y;
        private int width;
        private int height;

        public double X
        {
            get => x;
            set { x = value; OnPropertyChanged(); }
        }

        public double Y
        {
            get => y;
            set { y = value; OnPropertyChanged(); }
        }

        public int Width
        {
            get => width;
            set { width = value; OnPropertyChanged(); }
        }

        public int Height
        {
            get => height;
            set { height = value; OnPropertyChanged(); }
        }

        public PuzzleSlotViewModel(double x, double y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
    }
}
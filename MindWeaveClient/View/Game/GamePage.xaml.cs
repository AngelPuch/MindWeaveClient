using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MindWeaveClient.ViewModel.Game;
using MindWeaveClient.ViewModel.Puzzle;

namespace MindWeaveClient.View.Game
{
    public partial class GamePage : Page
    {
        private PuzzlePieceViewModel _draggedPiece;
        private Point _mouseOffset;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.Unloaded += GamePage_Unloaded;
        }

        private void Piece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pieceView = sender as PuzzlePieceView;
            if (pieceView == null) return;

            var pieceViewModel = pieceView.DataContext as PuzzlePieceViewModel;

            if (pieceViewModel == null || pieceViewModel.IsPlaced || pieceViewModel.IsHeldByOther)
            {
                return;
            }

            _draggedPiece = pieceViewModel;
            _mouseOffset = e.GetPosition(pieceView);
            pieceView.CaptureMouse();
            _draggedPiece.ZIndex = 100;

            var viewModel = this.DataContext as GameViewModel;
            viewModel?.startDraggingPiece(_draggedPiece);

            e.Handled = true;
        }

        private void Piece_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedPiece == null)
            {
                return;
            }

            var canvas = FindVisualParent<Canvas>(sender as DependencyObject);
            if (canvas == null)
            {
                return;
            }

            Point mousePos = e.GetPosition(canvas);

            double newX = mousePos.X - _mouseOffset.X;
            double newY = mousePos.Y - _mouseOffset.Y;

            _draggedPiece.X = newX;
            _draggedPiece.Y = newY;
        }

        private void Piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedPiece == null)
            {
                return;
            }

            var pieceView = sender as PuzzlePieceView;
            if (pieceView == null) return;

            pieceView.ReleaseMouseCapture();
            _draggedPiece.ZIndex = 0;
            var viewModel = this.DataContext as GameViewModel;

            // --- INICIO DE LA LÓGICA DEL "IMÁN" (PASO 3) ---

            const double SNAP_TOLERANCE = 10.0;

            double currentX = _draggedPiece.X;
            double currentY = _draggedPiece.Y;

            double correctX = _draggedPiece.CorrectX;
            double correctY = _draggedPiece.CorrectY;

            bool isSnap = Math.Abs(correctX - currentX) < SNAP_TOLERANCE &&
                          Math.Abs(correctY - currentY) < SNAP_TOLERANCE;

            if (isSnap)
            {
                // 1. EFECTO IMÁN: Mueve la pieza a su lugar final exacto
                _draggedPiece.X = correctX;
                _draggedPiece.Y = correctY;

                // 2. Notifica al servidor la posición CORRECTA
                viewModel?.DropPiece(_draggedPiece, correctX, correctY);
            }
            else
            {
                // 1. NO HAY SNAP: La pieza se queda donde está
                // 2. Notifica al servidor la posición "flotante"
                viewModel?.DropPiece(_draggedPiece, currentX, currentY);
            }

            // --- FIN DE LA LÓGICA DEL "IMÁN" ---

            _draggedPiece = null;
            e.Handled = true;
        }


        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }

        private void GamePage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is GameViewModel vm)
            {
                vm.cleanup();
            }
            this.Unloaded -= GamePage_Unloaded;
        }
    }
}
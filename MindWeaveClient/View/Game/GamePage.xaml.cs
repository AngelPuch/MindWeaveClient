using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MindWeaveClient.ViewModel; 

namespace MindWeaveClient.View.Game 
{

    public partial class GamePage : Page
    {
        private PuzzlePieceViewModel _draggedPiece;

        private Point _mouseOffset;

        private const int SNAP_THRESHOLD = 20;

      
        public GamePage()
        {
            InitializeComponent();
        }


        private void Piece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pieceImage = sender as Image;
            var pieceViewModel = pieceImage?.DataContext as PuzzlePieceViewModel;

            if (pieceViewModel == null || pieceViewModel.IsCorrectlyPlaced)
            {
                return;
            }

            _draggedPiece = pieceViewModel;

            _mouseOffset = e.GetPosition(pieceImage);

            pieceImage.CaptureMouse();

            _draggedPiece.ZIndex = 100;
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

            var pieceImage = sender as Image;

            pieceImage.ReleaseMouseCapture();

            _draggedPiece.ZIndex = 0;

       
            double deltaX = Math.Abs(_draggedPiece.X - _draggedPiece.CorrectPosition.X);
            double deltaY = Math.Abs(_draggedPiece.Y - _draggedPiece.CorrectPosition.Y);

            if (deltaX < SNAP_THRESHOLD && deltaY < SNAP_THRESHOLD)
            {
                _draggedPiece.X = _draggedPiece.CorrectPosition.X;
                _draggedPiece.Y = _draggedPiece.CorrectPosition.Y;
                _draggedPiece.IsCorrectlyPlaced = true;
                _draggedPiece.ZIndex = -1; // Ponerla detrás de las demás

                // --- ¡¡¡IMPORTANTE!!! ---
                // ¡Aquí es donde notificas al servidor que colocaste una pieza!
                // var viewModel = this.DataContext as GameViewModel;
                // viewModel.SendPiecePlacedCommand.Execute(_draggedPiece.PieceId);
            }

            _draggedPiece = null;
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
    }
}
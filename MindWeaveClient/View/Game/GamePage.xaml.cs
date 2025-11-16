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

        // Constructor corregido (¡este ya lo tienes bien!)
        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.Unloaded += GamePage_Unloaded;
        }

        private void Piece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // --- INICIO CORRECCIÓN 1 ---
            // El 'sender' es el UserControl (PuzzlePieceView), no la Imagen.
            var pieceView = sender as PuzzlePieceView;
            if (pieceView == null) return;

            var pieceViewModel = pieceView.DataContext as PuzzlePieceViewModel;
            // --- FIN CORRECCIÓN 1 ---

            if (pieceViewModel == null || pieceViewModel.IsPlaced || pieceViewModel.IsHeldByOther)
            {
                return;
            }

            _draggedPiece = pieceViewModel;

            // --- INICIO CORRECCIÓN 2 ---
            // Obtenemos el offset relativo al UserControl (la pieza)
            _mouseOffset = e.GetPosition(pieceView);
            // Capturamos el mouse en el UserControl
            pieceView.CaptureMouse();
            // --- FIN CORRECCIÓN 2 ---

            _draggedPiece.ZIndex = 100;

            var viewModel = this.DataContext as GameViewModel;
            viewModel?.startDraggingPiece(_draggedPiece);

            e.Handled = true; // Prevenimos que el evento se propague
        }

        private void Piece_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedPiece == null)
            {
                return;
            }

            // El 'sender' es el PuzzlePieceView que capturó el mouse.
            // Necesitamos encontrar el Canvas padre para obtener la posición absoluta.
            var canvas = FindVisualParent<Canvas>(sender as DependencyObject);
            if (canvas == null)
            {
                return;
            }

            Point mousePos = e.GetPosition(canvas);

            // Esta lógica es correcta: Posición del mouse en el canvas MENOS el offset
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

            // --- INICIO CORRECCIÓN 3 ---
            // El 'sender' es el UserControl (PuzzlePieceView)
            var pieceView = sender as PuzzlePieceView;
            if (pieceView == null) return;

            // Soltamos la captura del mouse
            pieceView.ReleaseMouseCapture();
            // --- FIN CORRECCIÓN 3 ---

            _draggedPiece.ZIndex = 0;

            // Esta lógica es perfecta.
            // Notificamos al ViewModel, que notificará al servidor.
            var viewModel = this.DataContext as GameViewModel;
            viewModel?.DropPiece(_draggedPiece, _draggedPiece.X, _draggedPiece.Y);

            _draggedPiece = null;
            e.Handled = true; // Prevenimos que el evento se propague
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
            // Esta lógica de limpieza es perfecta.
            if (this.DataContext is GameViewModel vm)
            {
                vm.Cleanup();
            }
            this.Unloaded -= GamePage_Unloaded;
        }
    }
}
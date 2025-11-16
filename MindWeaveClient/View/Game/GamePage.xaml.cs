// --- Archivo COMPLETO Y ACTUALIZADO ---

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

        // El umbral de snap YA NO SE USA AQUÍ. El servidor decide.
        // private const int SNAP_THRESHOLD = 20;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.Unloaded += GamePage_Unloaded;
        }

        private void Piece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pieceImage = sender as Image;
            var pieceViewModel = pieceImage?.DataContext as PuzzlePieceViewModel;

            // --- LÓGICA MODIFICADA ---
            // Usamos las nuevas propiedades para la validación
            if (pieceViewModel == null || pieceViewModel.IsPlaced || pieceViewModel.IsHeldByOther)
            {
                return;
            }

            _draggedPiece = pieceViewModel;
            _mouseOffset = e.GetPosition(pieceImage);
            pieceImage.CaptureMouse();
            _draggedPiece.ZIndex = 100;

            // --- NUEVO ---
            // Notificamos al ViewModel que hemos empezado a arrastrar
            var viewModel = this.DataContext as GameViewModel;
            viewModel?.startDraggingPiece(_draggedPiece);
        }

        private void Piece_MouseMove(object sender, MouseEventArgs e)
        {
            // Esta lógica está perfecta. Sigue siendo local para
            // que el movimiento se sienta fluido.
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

            // --- LÓGICA ANTIGUA ELIMINADA ---
            // Ya no comprobamos el snap aquí.
            // if (deltaX < SNAP_THRESHOLD && deltaY < SNAP_THRESHOLD)
            // { ... }

            // --- NUEVO ---
            // Notificamos al ViewModel que hemos soltado la pieza.
            // El ViewModel se lo dirá al servidor.
            // El servidor responderá con un callback (OnPiecePlaced o OnPieceMoved)
            // que el GameViewModel recibirá, y la pieza se moverá
            // automáticamente a su posición final gracias al binding.
            var viewModel = this.DataContext as GameViewModel;
            viewModel?.DropPiece(_draggedPiece, _draggedPiece.X, _draggedPiece.Y);

            // TODO: Si sueltas el mouse FUERA del canvas, deberías
            // llamar a viewModel?.ReleasePiece(_draggedPiece);
            // Esto requiere un evento MouseUp en el Canvas o la Ventana.

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

        private void GamePage_Unloaded(object sender, RoutedEventArgs e)
        {
            // Cuando la página se descarga (ej. el usuario vuelve al menú),
            // llamamos a Cleanup() en el ViewModel.
            if (this.DataContext is GameViewModel vm)
            {
                vm.Cleanup();
            }

            // Quitar el manejador para evitar múltiples llamadas
            this.Unloaded -= GamePage_Unloaded;
        }
    }
}
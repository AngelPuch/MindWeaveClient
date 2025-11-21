using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MindWeaveClient.ViewModel.Game;
using MindWeaveClient.ViewModel.Puzzle;

namespace MindWeaveClient.View.Game
{
    public partial class GamePage : Page
    {
        private List<PuzzlePieceViewModel> draggedGroup;
        private readonly GameViewModel gameViewModel;
        private bool isDragging;

        private const double SNAP_THRESHOLD = 20.0;
        private const double BOARD_SNAP_TOLERANCE = 10.0;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.gameViewModel = viewModel;
            this.Unloaded += gamePageUnloaded;
        }

        private async void Piece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pieceView = sender as PuzzlePieceView;
            if (pieceView == null)
            {
                return;
            }

            var clickedPiece = pieceView.DataContext as PuzzlePieceViewModel;

            // VALIDACIÓN CRÍTICA: No permitir arrastrar si ya está colocada o siendo sostenida por otro
            if (clickedPiece == null || clickedPiece.IsPlaced || clickedPiece.IsHeldByOther)
            {
                Debug.WriteLine($"[DRAG BLOCKED] Piece {clickedPiece?.PieceId}: IsPlaced={clickedPiece?.IsPlaced}, IsHeldByOther={clickedPiece?.IsHeldByOther}");
                return;
            }

            // Solicitar al servidor el lock de esta pieza
            await gameViewModel?.startDraggingPiece(clickedPiece);

            // Preparar el grupo para arrastre (esperamos confirmación del servidor)
            this.draggedGroup = clickedPiece.PieceGroup;
            this.isDragging = false;

            Point dragStartPoint = e.GetPosition(this.PuzzleItemsControl);

            // Calcular ZIndex para el grupo arrastrado
            var views = this.draggedGroup
                .Select(getViewForPiece)
                .Where(v => v != null)
                .ToList();

            int maxZ = 0;
            if (views.Any())
            {
                maxZ = views.Max(v => Panel.GetZIndex(v));
            }

            foreach (var view in views)
            {
                Panel.SetZIndex(view, maxZ + 1);
            }

            // Calcular offsets para todo el grupo
            foreach (var piece in this.draggedGroup)
            {
                piece.DragOffsetX = piece.X - dragStartPoint.X;
                piece.DragOffsetY = piece.Y - dragStartPoint.Y;
            }

            pieceView.CaptureMouse();
            Debug.WriteLine($"[DRAG INIT] Piece {clickedPiece.PieceId}, Group size: {draggedGroup.Count}");

            e.Handled = true;
        }

        private void Piece_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.draggedGroup == null || e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            // VALIDACIÓN: Solo mover si NO está siendo sostenida por otro jugador
            var firstPiece = draggedGroup.FirstOrDefault();
            if (firstPiece != null && firstPiece.IsHeldByOther)
            {
                Debug.WriteLine($"[DRAG BLOCKED IN MOVE] Piece held by other player");
                return;
            }

            this.isDragging = true;
            Point currentPoint = e.GetPosition(this.PuzzleItemsControl);

            // Mover todo el grupo
            foreach (var piece in this.draggedGroup)
            {
                piece.X = currentPoint.X + piece.DragOffsetX;
                piece.Y = currentPoint.Y + piece.DragOffsetY;
            }

            e.Handled = true;
        }

        private async void Piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.draggedGroup == null)
            {
                return;
            }

            var pieceView = sender as FrameworkElement;
            if (pieceView == null)
            {
                return;
            }

            pieceView.ReleaseMouseCapture();

            // Verificar si alguna pieza del grupo está siendo sostenida por otro
            var firstPiece = draggedGroup.FirstOrDefault();
            if (firstPiece != null && firstPiece.IsHeldByOther)
            {
                Debug.WriteLine($"[DROP CANCELLED] Piece held by other player");
                this.draggedGroup = null;
                this.isDragging = false;
                return;
            }

            // Si realmente se arrastró la pieza
            if (isDragging)
            {
                // Primero intentar snap piece-to-piece
                bool pieceSnapOccurred = checkForPieceToPieceSnap();

                if (pieceSnapOccurred)
                {
                    Debug.WriteLine($"[SNAP P2P] Piece-to-piece snap occurred");
                    // Enviar las nuevas posiciones al servidor después del snap
                    foreach (var piece in draggedGroup)
                    {
                        await gameViewModel?.dropPiece(piece, piece.X, piece.Y);
                    }
                }
                else
                {
                    // Si no hubo snap P2P, intentar snap al tablero
                    await handleBoardSnapAndDrop();
                }
            }
            else
            {
                // Si no se movió (solo click), liberar la pieza
                Debug.WriteLine($"[DRAG CANCELLED] No movement detected, releasing");
                foreach (var piece in draggedGroup)
                {
                    await gameViewModel?.releasePiece(piece);
                }
            }

            this.draggedGroup = null;
            this.isDragging = false;
            e.Handled = true;
        }

        private async System.Threading.Tasks.Task handleBoardSnapAndDrop()
        {
            if (draggedGroup == null || !draggedGroup.Any())
            {
                return;
            }

            var firstPiece = draggedGroup[0];
            double currentX = firstPiece.X;
            double currentY = firstPiece.Y;
            double correctX = firstPiece.CorrectX;
            double correctY = firstPiece.CorrectY;

            // Verificar si está cerca de la posición correcta en el tablero
            bool isBoardSnap = Math.Abs(correctX - currentX) < BOARD_SNAP_TOLERANCE &&
                               Math.Abs(correctY - currentY) < BOARD_SNAP_TOLERANCE;

            if (isBoardSnap)
            {
                Debug.WriteLine($"[SNAP BOARD] Piece {firstPiece.PieceId} snapping to board");

                // Calcular el offset para alinear todo el grupo
                double snapOffsetX = correctX - currentX;
                double snapOffsetY = correctY - currentY;

                // Aplicar snap visual a todo el grupo
                foreach (var piece in draggedGroup)
                {
                    piece.X += snapOffsetX;
                    piece.Y += snapOffsetY;
                }

                // Enviar todas las piezas al servidor con sus nuevas posiciones
                foreach (var piece in draggedGroup)
                {
                    await gameViewModel?.dropPiece(piece, piece.X, piece.Y);
                }
            }
            else
            {
                Debug.WriteLine($"[DROP] Normal drop without snap");
                // Drop normal sin snap
                foreach (var piece in draggedGroup)
                {
                    await gameViewModel?.dropPiece(piece, piece.X, piece.Y);
                }
            }
        }

        private bool checkForPieceToPieceSnap()
        {
            var allPieces = gameViewModel.PiecesCollection;
            var otherPieces = allPieces.Where(p => p.PieceGroup != this.draggedGroup).ToList();

            foreach (var pieceFromDragGroup in this.draggedGroup)
            {
                if (pieceFromDragGroup.IsPlaced)
                {
                    continue;
                }

                if (tryFindSnapWithStationaryPieces(pieceFromDragGroup, otherPieces))
                {
                    return true;
                }
            }
            return false;
        }

        private bool tryFindSnapWithStationaryPieces(PuzzlePieceViewModel draggedPiece, List<PuzzlePieceViewModel> otherPieces)
        {
            foreach (var stationaryPiece in otherPieces)
            {
                // Saltar piezas ya colocadas en el tablero
                if (stationaryPiece.IsPlaced)
                {
                    continue;
                }

                Point? targetPos = getTargetSnapPosition(draggedPiece, stationaryPiece);

                if (targetPos.HasValue)
                {
                    if (attemptSnapToTarget(draggedPiece, stationaryPiece, targetPos.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Point? getTargetSnapPosition(PuzzlePieceViewModel dragged, PuzzlePieceViewModel stationary)
        {
            // Derecha: dragged va a la izquierda de stationary
            if (dragged.RightNeighborId == stationary.PieceId)
            {
                return new Point(stationary.X - dragged.Width, stationary.Y);
            }
            // Izquierda: dragged va a la derecha de stationary
            if (dragged.LeftNeighborId == stationary.PieceId)
            {
                return new Point(stationary.X + stationary.Width, stationary.Y);
            }
            // Abajo: dragged va arriba de stationary
            if (dragged.BottomNeighborId == stationary.PieceId)
            {
                return new Point(stationary.X, stationary.Y - dragged.Height);
            }
            // Arriba: dragged va debajo de stationary
            if (dragged.TopNeighborId == stationary.PieceId)
            {
                return new Point(stationary.X, stationary.Y + stationary.Height);
            }

            return null;
        }

        private bool attemptSnapToTarget(PuzzlePieceViewModel dragged, PuzzlePieceViewModel stationary, Point target)
        {
            if (isClose(dragged, target.X, target.Y))
            {
                double snapOffsetX = target.X - dragged.X;
                double snapOffsetY = target.Y - dragged.Y;

                Debug.WriteLine($"[SNAP P2P] Merging piece {dragged.PieceId} with {stationary.PieceId}");

                alignAndMerge(stationary.PieceGroup, snapOffsetX, snapOffsetY, dragged, stationary);
                return true;
            }
            return false;
        }

        private static bool isClose(PuzzlePieceViewModel piece, double targetX, double targetY)
        {
            double deltaX = piece.X - targetX;
            double deltaY = piece.Y - targetY;
            return (deltaX * deltaX + deltaY * deltaY) < (SNAP_THRESHOLD * SNAP_THRESHOLD);
        }

        private void alignAndMerge(
            List<PuzzlePieceViewModel> stationaryGroup,
            double offsetX,
            double offsetY,
            PuzzlePieceViewModel draggedPiece,
            PuzzlePieceViewModel stationaryPiece)
        {
            // Alinear todas las piezas del grupo arrastrado
            foreach (var piece in this.draggedGroup)
            {
                piece.X += offsetX;
                piece.Y += offsetY;
            }

            // Fusionar los grupos
            stationaryGroup.AddRange(this.draggedGroup);

            // Actualizar la referencia PieceGroup en todas las piezas arrastradas
            var piecesToReassign = new List<PuzzlePieceViewModel>(this.draggedGroup);
            foreach (var piece in piecesToReassign)
            {
                piece.PieceGroup = stationaryGroup;
            }

            Debug.WriteLine($"[MERGE] New group size: {stationaryGroup.Count}");
        }

        private FrameworkElement getViewForPiece(PuzzlePieceViewModel pieceVm)
        {
            return this.PuzzleItemsControl.ItemContainerGenerator.ContainerFromItem(pieceVm) as FrameworkElement;
        }

        private void gamePageUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
            this.Unloaded -= gamePageUnloaded;
        }
    }
}
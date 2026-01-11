using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private const double BOARD_SNAP_TOLERANCE = 15.0;
        private const int MOVE_UPDATE_INTERVAL_MS = 50;
        private const int Z_INDEX_DRAGGING = 1000;
        private const int Z_INDEX_PLACED = 1;

        private List<PuzzlePieceViewModel> draggedGroup;
        private DateTime lastUiUpdate = DateTime.MinValue;
        private DateTime lastMoveUpdateTime = DateTime.MinValue;
        private readonly GameViewModel gameViewModel;
        private bool isLocalDragging;

        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.gameViewModel = viewModel;

            if (this.gameViewModel != null)
            {
                this.gameViewModel.ForceReleaseLocalDrag += onForceReleaseLocalDrag;
            }

            this.Unloaded += gamePageUnloaded;
        }

        private void onForceReleaseLocalDrag()
        {
            if (isLocalDragging)
            {
                isLocalDragging = false;
                draggedGroup = null;
                Mouse.Capture(null);
            }
        }

        private void pieceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pieceView = sender as PuzzlePieceView;
            if (pieceView == null) return;

            var clickedPiece = pieceView.DataContext as PuzzlePieceViewModel;
            if (clickedPiece == null || clickedPiece.IsPlaced || clickedPiece.IsHeldByOther) return;

            isLocalDragging = true;
            this.draggedGroup = clickedPiece.PieceGroup;
            Point dragStartPoint = e.GetPosition(this.PuzzleItemsControl);

            updateGroupZIndex(this.draggedGroup, Z_INDEX_DRAGGING);

            foreach (var piece in this.draggedGroup)
            {
                piece.DragOffsetX = piece.X - dragStartPoint.X;
                piece.DragOffsetY = piece.Y - dragStartPoint.Y;

                if (gameViewModel != null)
                {
                    _ = gameViewModel.startDraggingPiece(piece);
                }
            }

            pieceView.CaptureMouse();
            e.Handled = true;
        }

        private void pieceMouseMove(object sender, MouseEventArgs e)
        {
            var now = DateTime.UtcNow;

            if (!shouldProcessMouseMove(now, e)) return;

            lastUiUpdate = now;

            Point currentPoint = e.GetPosition(this.PuzzleItemsControl);
            Rect validBounds = getValidBounds();

            var proposedPositions = calculateProposedPositions(currentPoint);
            var boundaryShifts = calculateBoundaryShifts(proposedPositions, validBounds);

            applyPositionsWithOffset(proposedPositions, boundaryShifts);
            sendMoveUpdatesIfNeeded(now);

            e.Handled = true;
        }

        private bool shouldProcessMouseMove(DateTime now, MouseEventArgs e)
        {
            if ((now - lastUiUpdate).TotalMilliseconds < 16) return false;
            if (this.draggedGroup == null) return false;
            if (e.LeftButton != MouseButtonState.Pressed) return false;
            if (!isLocalDragging) return false;

            return true;
        }

        private Rect getValidBounds()
        {
            GeneralTransform transform = this.TransformToDescendant(this.PuzzleItemsControl);

            if (transform != null)
            {
                return transform.TransformBounds(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            }

            return new Rect(0, 0, this.PuzzleItemsControl.ActualWidth, this.PuzzleItemsControl.ActualHeight);
        }

        private List<(PuzzlePieceViewModel piece, double x, double y)> calculateProposedPositions(Point currentPoint)
        {
            var proposedPositions = new List<(PuzzlePieceViewModel piece, double x, double y)>();

            foreach (var piece in this.draggedGroup)
            {
                double px = currentPoint.X + piece.DragOffsetX;
                double py = currentPoint.Y + piece.DragOffsetY;
                proposedPositions.Add((piece, px, py));
            }

            return proposedPositions;
        }

        private static (double offsetX, double offsetY) calculateBoundaryShifts(
            List<(PuzzlePieceViewModel piece, double x, double y)> proposedPositions,
            Rect validBounds)
        {
            double requiredShiftRight = 0, requiredShiftLeft = 0;
            double requiredShiftDown = 0, requiredShiftUp = 0;

            foreach (var item in proposedPositions)
            {
                calculateHorizontalShifts(item, validBounds, ref requiredShiftRight, ref requiredShiftLeft);
                calculateVerticalShifts(item, validBounds, ref requiredShiftDown, ref requiredShiftUp);
            }

            double finalOffsetX = calculateFinalOffset(requiredShiftRight, requiredShiftLeft);
            double finalOffsetY = calculateFinalOffset(requiredShiftDown, requiredShiftUp);

            return (finalOffsetX, finalOffsetY);
        }

        private static void calculateHorizontalShifts(
            (PuzzlePieceViewModel piece, double x, double y) item,
            Rect validBounds,
            ref double requiredShiftRight,
            ref double requiredShiftLeft)
        {
            if (item.x < validBounds.Left)
            {
                double violation = validBounds.Left - item.x;
                if (violation > requiredShiftRight) requiredShiftRight = violation;
            }

            double pieceRight = item.x + item.piece.RenderWidth;
            if (pieceRight > validBounds.Right)
            {
                double violation = pieceRight - validBounds.Right;
                if (violation > requiredShiftLeft) requiredShiftLeft = violation;
            }
        }

        private static void calculateVerticalShifts(
            (PuzzlePieceViewModel piece, double x, double y) item,
            Rect validBounds,
            ref double requiredShiftDown,
            ref double requiredShiftUp)
        {
            if (item.y < validBounds.Top)
            {
                double violation = validBounds.Top - item.y;
                if (violation > requiredShiftDown) requiredShiftDown = violation;
            }

            double pieceBottom = item.y + item.piece.RenderHeight;
            if (pieceBottom > validBounds.Bottom)
            {
                double violation = pieceBottom - validBounds.Bottom;
                if (violation > requiredShiftUp) requiredShiftUp = violation;
            }
        }

        private static double calculateFinalOffset(double positiveShift, double negativeShift)
        {
            if (positiveShift > 0)
            {
                return positiveShift;
            }

            if (negativeShift > 0)
            {
                return -negativeShift;
            }

            return 0;
        }

        private static void applyPositionsWithOffset(
            List<(PuzzlePieceViewModel piece, double x, double y)> proposedPositions,
            (double offsetX, double offsetY) shifts)
        {
            foreach (var item in proposedPositions)
            {
                item.piece.X = item.x + shifts.offsetX;
                item.piece.Y = item.y + shifts.offsetY;
            }
        }

        private void sendMoveUpdatesIfNeeded(DateTime now)
        {
            if ((now - lastMoveUpdateTime).TotalMilliseconds <= MOVE_UPDATE_INTERVAL_MS) return;

            lastMoveUpdateTime = now;

            foreach (var piece in this.draggedGroup)
            {
                _ = gameViewModel?.movePiece(piece, piece.X, piece.Y);
            }
        }

        private async void pieceMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.draggedGroup == null || !isLocalDragging) return;

            var pieceView = sender as FrameworkElement;
            if (pieceView == null) return;

            pieceView.ReleaseMouseCapture();

            // SNAP TO PIECE logic removed completely.
            // Proceed directly to Board Snap check.
            var piecesToDrop = new List<PuzzlePieceViewModel>(this.draggedGroup);

            await handleBoardSnapAndDrop(piecesToDrop);

            this.draggedGroup = null;
            isLocalDragging = false;
            e.Handled = true;
        }

        private void gamePageUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.gameViewModel != null)
            {
                this.gameViewModel.ForceReleaseLocalDrag -= onForceReleaseLocalDrag;
            }

            if (this.DataContext is IDisposable disposable)
            {
                disposable.Dispose();
            }
            this.Unloaded -= gamePageUnloaded;
        }

        private Task handleBoardSnapAndDrop(List<PuzzlePieceViewModel> groupToDrop)
        {
            if (groupToDrop == null || !groupToDrop.Any()) return Task.CompletedTask;

            var firstPiece = groupToDrop[0];
            double currentX = firstPiece.X;
            double currentY = firstPiece.Y;
            double correctX = firstPiece.CorrectX;
            double correctY = firstPiece.CorrectY;

            bool isBoardSnap = Math.Abs(correctX - currentX) < BOARD_SNAP_TOLERANCE &&
                               Math.Abs(correctY - currentY) < BOARD_SNAP_TOLERANCE;

            if (isBoardSnap)
            {
                double snapOffsetX = correctX - currentX;
                double snapOffsetY = correctY - currentY;

                foreach (var piece in groupToDrop)
                {
                    piece.X += snapOffsetX;
                    piece.Y += snapOffsetY;
                }
            }

            foreach (var piece in groupToDrop)
            {
                _ = gameViewModel?.dropPiece(piece, piece.X, piece.Y);
            }

            return Task.CompletedTask;
        }

        private static void updateGroupZIndex(List<PuzzlePieceViewModel> group, int zIndex)
        {
            if (group == null) return;
            foreach (var piece in group)
            {
                piece.ZIndex = zIndex;
            }
        }
    }
}
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

        private const double SNAP_THRESHOLD = 20.0;
        private const double BOARD_SNAP_TOLERANCE = 10.0;


        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.gameViewModel = viewModel;
            this.Unloaded += GamePage_Unloaded;
        }

        private void Piece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pieceView = sender as PuzzlePieceView;
            if (pieceView == null)
            {
                return;
            }

            var clickedPiece = pieceView.DataContext as PuzzlePieceViewModel;

            if (clickedPiece == null || clickedPiece.IsPlaced || clickedPiece.IsHeldByOther)
            {
                return;
            }

            this.draggedGroup = clickedPiece.PieceGroup;

            Point dragStartPoint = e.GetPosition(this.PuzzleItemsControl);

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

            foreach (var piece in this.draggedGroup)
            {
                piece.DragOffsetX = piece.X - dragStartPoint.X;
                piece.DragOffsetY = piece.Y - dragStartPoint.Y;
            }

            pieceView.CaptureMouse();
            gameViewModel?.startDraggingPiece(clickedPiece); 

            e.Handled = true;
        }

        private void Piece_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.draggedGroup != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(this.PuzzleItemsControl);

                foreach (var piece in this.draggedGroup)
                {
                    piece.X = currentPoint.X + piece.DragOffsetX;
                    piece.Y = currentPoint.Y + piece.DragOffsetY;
                }
                e.Handled = true;
            }
        }

        private void Piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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

            bool pieceSnapOccurred = checkForPieceToPieceSnap();

            if (pieceSnapOccurred)
            {
                foreach (var piece in draggedGroup)
                {
                    gameViewModel?.dropPiece(piece, piece.X, piece.Y);
                }
            }
            else
            {
                handleBoardSnapAndDrop();
            }

            this.draggedGroup = null;
            e.Handled = true;
        }

        private void handleBoardSnapAndDrop()
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

            bool isBoardSnap = Math.Abs(correctX - currentX) < BOARD_SNAP_TOLERANCE &&
                               Math.Abs(correctY - currentY) < BOARD_SNAP_TOLERANCE;

            if (isBoardSnap)
            {
                double snapOffsetX = correctX - currentX;
                double snapOffsetY = correctY - currentY;

                foreach (var piece in draggedGroup)
                {
                    piece.X += snapOffsetX;
                    piece.Y += snapOffsetY;
                    gameViewModel?.dropPiece(piece, piece.X, piece.Y);
                }
            }
            else
            {
                foreach (var piece in draggedGroup)
                {
                    gameViewModel?.dropPiece(piece, piece.X, piece.Y);
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
            if (dragged.RightNeighborId == stationary.PieceId)
            {
                return new Point(stationary.X - dragged.Width, stationary.Y);
            }
            if (dragged.LeftNeighborId == stationary.PieceId)
            {
                return new Point(stationary.X + stationary.Width, stationary.Y);
            }
            if (dragged.BottomNeighborId == stationary.PieceId)
            {
                return new Point(stationary.X, stationary.Y - dragged.Height);
            }
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

            foreach (var piece in this.draggedGroup)
            {
                piece.X += offsetX;
                piece.Y += offsetY;
            }

            stationaryGroup.AddRange(this.draggedGroup);
            var piecesToReassign = new List<PuzzlePieceViewModel>(this.draggedGroup);
            foreach (var piece in piecesToReassign)
            {
                piece.PieceGroup = stationaryGroup;
            }
        }

        private FrameworkElement getViewForPiece(PuzzlePieceViewModel pieceVm)
        {
            return this.PuzzleItemsControl.ItemContainerGenerator.ContainerFromItem(pieceVm) as FrameworkElement;
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
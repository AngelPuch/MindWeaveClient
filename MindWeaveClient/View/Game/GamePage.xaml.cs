using System;
using System.Collections.Generic;
using System.Diagnostics; // Added for Trace
using System.Linq;
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
        // --- Fields for Group Dragging ---
        private List<PuzzlePieceViewModel> _draggedGroup;
        private readonly GameViewModel _gameViewModel;

        // --- Snap Thresholds (Step 6) ---

        /// <summary>
        /// The pixel distance to trigger a piece-to-piece snap.
        /// </summary>
        private const double SNAP_THRESHOLD = 20.0;

        /// <summary>
        /// The pixel distance to trigger a snap-to-board (correct final position) snap.
        /// </summary>
        private const double BOARD_SNAP_TOLERANCE = 10.0;


        public GamePage(GameViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this._gameViewModel = viewModel; // Store viewmodel reference
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

            // --- Group Drag Logic (Step 5) ---

            // 1. Get the entire group this piece belongs to
            this._draggedGroup = clickedPiece.PieceGroup;

            // **** CORRECCIÓN 1 ****
            // Get position relative to the ItemsControl, not the Canvas
            Point dragStartPoint = e.GetPosition(this.PuzzleItemsControl);

            // 2. Bring the entire group to the front (Z-Index)
            var views = this._draggedGroup
                .Select(GetViewForPiece)
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

            // 3. Calculate drag offset for EACH piece in the group
            foreach (var piece in this._draggedGroup)
            {
                piece.DragOffsetX = piece.X - dragStartPoint.X;
                piece.DragOffsetY = piece.Y - dragStartPoint.Y;
            }

            pieceView.CaptureMouse();
            _gameViewModel?.startDraggingPiece(clickedPiece); // Notify server about the clicked piece

            e.Handled = true;
        }

        private void Piece_MouseMove(object sender, MouseEventArgs e)
        {
            // Check if we are dragging a group and the mouse button is still pressed
            if (this._draggedGroup != null && e.LeftButton == MouseButtonState.Pressed)
            {
                // **** CORRECCIÓN 2 ****
                // Get position relative to the ItemsControl, not the Canvas
                Point currentPoint = e.GetPosition(this.PuzzleItemsControl);

                // --- Group Move Logic (Step 5) ---
                // Move EACH piece in the group based on its individual offset
                foreach (var piece in this._draggedGroup)
                {
                    piece.X = currentPoint.X + piece.DragOffsetX;
                    piece.Y = currentPoint.Y + piece.DragOffsetY;
                }
                e.Handled = true;
            }
        }

        // --- El resto del archivo (MouseLeftButtonUp y los helpers) 
        // --- permanece exactamente igual que en el Paso 6.

        private void Piece_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this._draggedGroup == null)
            {
                return;
            }

            var pieceView = sender as FrameworkElement;
            if (pieceView == null)
            {
                return;
            }

            pieceView.ReleaseMouseCapture();

            // --- New Snap and Merge Logic (Step 6) ---

            // 1. Check for piece-to-piece snap. This will auto-align and merge if true.
            bool pieceSnapOccurred = CheckForPieceToPieceSnap();

            if (pieceSnapOccurred)
            {
                // A merge happened. The _draggedGroup pieces are now part of a new group
                // and their X/Y properties have been updated by AlignAndMerge().
                // We must notify the server of the new positions.
                foreach (var piece in _draggedGroup)
                {
                    _gameViewModel?.DropPiece(piece, piece.X, piece.Y);
                }
            }
            else
            {
                // 2. No piece-to-piece snap. Check for snap-to-board.
                // This method will handle both snapping the group to the board
                // OR notifying the server of the final floating position.
                HandleBoardSnapAndDrop();
            }

            this._draggedGroup = null; // Clear the dragged group
            e.Handled = true;
        }

        /// <summary>
        /// Handles the fallback logic for snapping the entire group to the board
        /// OR notifying the server of the final floating position if no snap occurs.
        /// </summary>
        private void HandleBoardSnapAndDrop()
        {
            if (_draggedGroup == null || !_draggedGroup.Any())
            {
                return;
            }

            // Check the first piece of the group for a board snap
            var firstPiece = _draggedGroup[0];
            double currentX = firstPiece.X;
            double currentY = firstPiece.Y;
            double correctX = firstPiece.CorrectX;
            double correctY = firstPiece.CorrectY;

            bool isBoardSnap = Math.Abs(correctX - currentX) < BOARD_SNAP_TOLERANCE &&
                               Math.Abs(correctY - currentY) < BOARD_SNAP_TOLERANCE;

            if (isBoardSnap)
            {
                // --- SNAP TO BOARD ---
                // Calculate the offset needed to move the first piece to its correct spot
                double snapOffsetX = correctX - currentX;
                double snapOffsetY = correctY - currentY;

                // Apply this offset to ALL pieces in the group
                foreach (var piece in _draggedGroup)
                {
                    piece.X += snapOffsetX;
                    piece.Y += snapOffsetY;
                    // Notify server of the *correct* final position
                    _gameViewModel?.DropPiece(piece, piece.X, piece.Y);
                }
            }
            else
            {
                // --- NO SNAP (FLOATING) ---
                // No snap occurred. Just notify the server of the
                // final floating position for all pieces in the group.
                foreach (var piece in _draggedGroup)
                {
                    _gameViewModel?.DropPiece(piece, piece.X, piece.Y);
                }
            }
        }

        /// <summary>
        /// Checks if any piece in the _draggedGroup is close enough to a
        /// valid neighbor in another group to snap and merge.
        /// </summary>
        /// <returns>True if a snap and merge occurred, otherwise false.</returns>
        private bool CheckForPieceToPieceSnap()
        {
            var allPieces = _gameViewModel.PiecesCollection;

            // Optimization: only check against pieces that are NOT in our dragged group
            var otherPieces = allPieces.Where(p => p.PieceGroup != this._draggedGroup).ToList();

            foreach (var pieceFromDragGroup in this._draggedGroup)
            {
                Trace.WriteLine($"Checking Piece {pieceFromDragGroup.PieceId}. " +
                                $"Neighbor IDs: R={pieceFromDragGroup.RightNeighborId}, " +
                                $"L={pieceFromDragGroup.LeftNeighborId}, " +
                                $"T={pieceFromDragGroup.TopNeighborId}, " +
                                $"B={pieceFromDragGroup.BottomNeighborId}");
                // No need to check pieces that are already placed
                if (pieceFromDragGroup.IsPlaced)
                {
                    continue;
                }

                foreach (var stationaryPiece in otherPieces)
                {
                    double targetX, targetY;
                    double snapOffsetX, snapOffsetY;

                    // Check 1: Is 'stationaryPiece' the neighbor to the RIGHT?
                    if (pieceFromDragGroup.RightNeighborId == stationaryPiece.PieceId)
                    {
                        targetX = stationaryPiece.X - pieceFromDragGroup.Width;
                        targetY = stationaryPiece.Y;
                        if (IsClose(pieceFromDragGroup, targetX, targetY))
                        {
                            snapOffsetX = targetX - pieceFromDragGroup.X;
                            snapOffsetY = targetY - pieceFromDragGroup.Y;
                            AlignAndMerge(stationaryPiece.PieceGroup, snapOffsetX, snapOffsetY, pieceFromDragGroup, stationaryPiece);
                            return true; // Snap!
                        }
                    }
                    // Check 2: Is 'stationaryPiece' the neighbor to the LEFT?
                    else if (pieceFromDragGroup.LeftNeighborId == stationaryPiece.PieceId)
                    {
                        targetX = stationaryPiece.X + stationaryPiece.Width;
                        targetY = stationaryPiece.Y;
                        if (IsClose(pieceFromDragGroup, targetX, targetY))
                        {
                            snapOffsetX = targetX - pieceFromDragGroup.X;
                            snapOffsetY = targetY - pieceFromDragGroup.Y;
                            AlignAndMerge(stationaryPiece.PieceGroup, snapOffsetX, snapOffsetY, pieceFromDragGroup, stationaryPiece);
                            return true; // Snap!
                        }
                    }
                    // Check 3: Is 'stationaryPiece' the neighbor BELOW?
                    else if (pieceFromDragGroup.BottomNeighborId == stationaryPiece.PieceId)
                    {
                        targetX = stationaryPiece.X;
                        targetY = stationaryPiece.Y - pieceFromDragGroup.Height;
                        if (IsClose(pieceFromDragGroup, targetX, targetY))
                        {
                            snapOffsetX = targetX - pieceFromDragGroup.X;
                            snapOffsetY = targetY - pieceFromDragGroup.Y;
                            AlignAndMerge(stationaryPiece.PieceGroup, snapOffsetX, snapOffsetY, pieceFromDragGroup, stationaryPiece);
                            return true; // Snap!
                        }
                    }
                    // Check 4: Is 'stationaryPiece' the neighbor ABOVE?
                    else if (pieceFromDragGroup.TopNeighborId == stationaryPiece.PieceId)
                    {
                        targetX = stationaryPiece.X;
                        targetY = stationaryPiece.Y + stationaryPiece.Height;
                        if (IsClose(pieceFromDragGroup, targetX, targetY))
                        {
                            snapOffsetX = targetX - pieceFromDragGroup.X;
                            snapOffsetY = targetY - pieceFromDragGroup.Y;
                            AlignAndMerge(stationaryPiece.PieceGroup, snapOffsetX, snapOffsetY, pieceFromDragGroup, stationaryPiece);
                            return true; // Snap!
                        }
                    }
                }
            }
            return false; // No snap found
        }

        /// <summary>
        /// Checks if a piece is within the SNAP_THRESHOLD of a target coordinate.
        /// </summary>
        private bool IsClose(PuzzlePieceViewModel piece, double targetX, double targetY)
        {
            double deltaX = piece.X - targetX;
            double deltaY = piece.Y - targetY;
            // Use squared distance to avoid expensive Sqrt()
            return (deltaX * deltaX + deltaY * deltaY) < (SNAP_THRESHOLD * SNAP_THRESHOLD);
        }

        /// <summary>
        /// Aligns the dragged group to the stationary group and merges them.
        /// </summary>
        /// <param name="stationaryGroup">The group we are snapping TO.</param>
        /// <param name="offsetX">The X alignment offset.</param>
        /// <param name="offsetY">The Y alignment offset.</param>
        /// <param name="draggedPiece">The specific piece from the dragged group that triggered the snap.</param>
        /// <param name="stationaryPiece">The specific piece from the stationary group that was snapped to.</param>
        private void AlignAndMerge(
            List<PuzzlePieceViewModel> stationaryGroup,
            double offsetX,
            double offsetY,
            PuzzlePieceViewModel draggedPiece,
            PuzzlePieceViewModel stationaryPiece)
        {
            Trace.WriteLine($"Snap! Merging group of {draggedPiece.PieceId} ({_draggedGroup.Count} pieces) into group of {stationaryPiece.PieceId} ({stationaryGroup.Count} pieces).");

            // 1. Move the entire 'draggedGroup' to align perfectly
            foreach (var piece in this._draggedGroup)
            {
                piece.X += offsetX;
                piece.Y += offsetY;
            }

            // 2. Merge the groups.
            // Add all pieces from the dragged group to the stationary group.
            stationaryGroup.AddRange(this._draggedGroup);

            // 3. Re-assign the group reference.
            // All pieces from the dragged group must now point to the 'stationaryGroup'.
            // We create a new list to iterate over, as we are changing the 
            // 'PieceGroup' property which could be used in the 'otherPieces' LINQ query.
            var piecesToReassign = new List<PuzzlePieceViewModel>(this._draggedGroup);
            foreach (var piece in piecesToReassign)
            {
                piece.PieceGroup = stationaryGroup;
            }
        }

        /// <summary>
        /// Gets the UI container (ContentPresenter) for a given piece ViewModel.
        /// This is the robust way to get the element for Z-Index manipulation.
        /// </summary>
        private FrameworkElement GetViewForPiece(PuzzlePieceViewModel pieceVm)
        {
            // PuzzleItemsControl is the x:Name of the ItemsControl in GamePage.xaml
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
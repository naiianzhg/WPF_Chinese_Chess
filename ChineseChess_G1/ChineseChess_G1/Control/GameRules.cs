using System;
using System.Collections.Generic;
using System.Text;
using ChineseChess.Model;
using System.Linq; // List.Last()
using System.Threading;

namespace ChineseChess.Control
{
    class GameRules
    {
        // Initialize the Xiangqi game system
        public static void iniGame()
        {
            Board.iniChessBoard();
        }

        // Return the check situation of current team
        public static bool isChecked()
        {
            // Get the position of the general, and find out if there is a checked on the board
            int[] redGeneralLocation = Board.redGeneralPosition, blkGeneralLocation = Board.blkGeneralPosition;

            // Calculate the valid moves of all the pieces
            for (int row = 0; row < Board.pieces.GetLength(0); row++)
            {
                for (int col = 0; col < Board.pieces.GetLength(1); col++)
                {
                    if (Board.pieces[row, col] != null && Board.pieces[row, col].colour != Board.currentColour % 2)
                    {
                        if (Board.currentColour % 2 == 0 && Board.pieces[row, col].calculateValidMoveList(new int[] { row, col }).Contains(blkGeneralLocation[0] * 10 + blkGeneralLocation[1]) ||
                            Board.currentColour % 2 == 1 && Board.pieces[row, col].calculateValidMoveList(new int[] { row, col }).Contains(redGeneralLocation[0] * 10 + redGeneralLocation[1]))
                            return true;
                    }
                }
            }
            return false;
        }

        // when one team is checked, if all valid moves of all the pieces from this team are not able to avoid cheked, it is a checkmate
        public static bool isCheckmate()
        {
            bool isChkmt = true;
            List<int> validMoveList;
            int[] validMove;

            // If there is a check situation

            // Traversal of the chess board
            for (int row = 0; row < Board.pieces.GetLength(0); row++)
            {
                for (int col = 0; col < Board.pieces.GetLength(1); col++)
                {
                    // among all the pieces, check if they move to any of their validmoves that allows to avoid the check, if not it is checkmate
                    if (Board.pieces[row, col] != null && Board.pieces[row, col].colour == Board.currentColour % 2)
                    {
                        validMoveList = Board.pieces[row, col].calculateValidMoveList(new int[] { row, col });
                        // try every validmove in the validmove list
                        for (int i = 0; i < validMoveList.Count; i++)
                        {
                            validMove = new int[] { validMoveList[i] / 10, validMoveList[i] % 10 };
                            // Assume the piece move to one of the validmove position, if any piece is eaten, store the piece so that it can be put back later
                            Pieces virtualEatenPiece = Board.pieces[validMove[0], validMove[1]];
                            PiecesHandler.moveTo(new int[] { row, col }, validMove, 1);
                            isChkmt = isChecked();
                            PiecesHandler.moveTo(validMove, new int[] { row, col }, 1);
                            Board.pieces[validMove[0], validMove[1]] = virtualEatenPiece;
                            // if it is confirmed that there is no checkmate(able to avoid check) yet, no need for other traversal
                            if (!isChkmt) return false;
                        }
                    }
                }
            }
            return true;

        }
    }
}

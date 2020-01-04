using System;
using System.Collections.Generic;
using System.Text;
using ChineseChess.Model;
using ChineseChess.View;
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

        // return a bool[] where bool[0] is the checked situation for black, whereas bool[1] for red
        public static bool[] isChecked()
        {
            // Get the position of the general, and find out if there is a checked on the board
            int[] redGeneralLocation = Board.getRedGeneralPosition(), blkGeneralLocation = Board.getBlkGeneralPosition();
            
            // chked[0] = black is checked or not, chked[1] = red is checked or not
            bool[] chked = new bool[2];
            
            // Calculate the valid moves of all the pieces
            for (int row = 0; row < Board.pieces.GetLength(0); row++)
            {
                for (int col = 0; col < Board.pieces.GetLength(1); col++)
                {
                    // once the valid moves of one red pieces contains the black general, the black is checked
                    // if the black is already confirmed as in check, no need for the other traversal
                    if (!chked[0] && Board.pieces[row, col] != null && Board.pieces[row, col].colour == 1)
                    {
                        if (Board.pieces[row, col].calculateValidMoveList(new int[] { row, col }).Contains(blkGeneralLocation[0] * 10 + blkGeneralLocation[1]))
                        {
                            chked[0] = true;
                            break;
                        }
                    }
                    // or once the valid moves of one black pieces contains the red general, the red is checked
                    // if the red is already confirmed as in check, no need for the other traversal
                    else if (!chked[1] && Board.pieces[row, col] != null && Board.pieces[row, col].colour == 0)
                    {
                        if (Board.pieces[row, col].calculateValidMoveList(new int[] { row, col }).Contains(redGeneralLocation[0] * 10 + redGeneralLocation[1]))
                        {
                            chked[1] = true;
                            break;
                        }
                    }
                }
            }
            return chked;
        }

        // when one team is checked, if all valid moves of all the pieces from this team are not able to avoid cheked, it is a checkmate
        public static bool isCheckmate()
        {
            bool isChkmt = true;
            List<int> validMoveList;
            int[] validMove;
            
            // If there is a check situation
            if (isChecked()[Board.currentColour % 2])
            {
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
                                PiecesHandler.tracelessMoveTo(new int[] { row, col }, validMove);
                                if (!isChecked()[Board.currentColour % 2]) isChkmt = false;
                                PiecesHandler.tracelessMoveTo(validMove, new int[] { row, col });
                                Board.pieces[validMove[0], validMove[1]] = virtualEatenPiece;
                                // if it is confirmed that there is no checkmate(able to avoid check) yet, no need for other traversal
                                if (!isChkmt) break;
                            }
                        }
                        if (!isChkmt) break;
                    }
                    if (!isChkmt) break;
                }
            }
            // If there is no check situation, isChkmt will always be false/impossible
            else isChkmt = false;

            return isChkmt;
        }

        // Withdraw move
        public static void withdraw()
        {
            // If there is no pieces available for withdraw
            if (Board.lastDestLocationList.Count == 0)
                throw new Exception("You have no move to withdraw");
            
            // Move back the piece
            PiecesHandler.tracelessMoveTo(Board.getLastDestLocation(), Board.getLastOriLocation());
            // If there is an eaten piece, put it back to the board, else put null
            Board.pieces[Board.getLastDestLocation()[0], Board.getLastDestLocation()[1]] = Board.getLastEatenPiece();
            // Remove the last element of lastOriLocationList, lastDestLocationList and lastEatenPieceList after regret
            Board.removeLastOriLocation();
            Board.removeLastDestLocation();
            Board.removeLastEatenPiece();

            // Change back the turn
            Board.currentColour--;
        }

        // Regret
        public static void regret()
        {
            // If there is no pieces available for withdraw
            if (Board.lastDestLocationList.Count < 2)
                throw new Exception("You have no move to regret");
            // If there is no more chances for the player to regret
            if (Board.regretAmount[Board.currentColour % 2] == 0)
                throw new Exception("You have no chance to regret");

            withdraw();
            withdraw();

            // Reduce of regret chance by 1
            Board.regretAmount[Board.currentColour % 2]--;
        }
    }
}

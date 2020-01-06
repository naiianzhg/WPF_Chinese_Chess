using System;
using System.Collections.Generic;
using System.Text;
using System.Linq; // List.Last()
using ChineseChess.Control;

namespace ChineseChess.Model
{
    class Board
    {
        public static Pieces[,] pieces { get; set; }
        // The current colour of the game, odd for red, even for black
        public static int currentColour { get; set; }
        // The amount for regret where regretAmount[0] = regretAmount for black, where as regretAmount[1] for red
        public static int[] regretAmount { get; set; }

        // Store all the moving operation from the beginning to the end
        // Last original position list
        public static List<int> lastOriLocationList { get; set; }
        // Last destination position list
        public static List<int> lastDestLocationList { get; set; }
        // Last be-eaten piece
        public static List<Pieces> lastEatenPieceList { get; set; }

        // Store the moves of the manual
        public static List<int> manualOriLocationList { get; set; }
        public static List<int> manualDestLocationList { get; set; }

        // ---
        public static List<int> redPieces { get; set; }
        public static List<int> blkPieces { get; set; }
        public static int[] redGeneralPosition { get; set; }
        public static int[] blkGeneralPosition { get; set; }

        // In the constructor, we initialize all the pieces and put them in the board which is an array of pieces
        public Board()
        {
            // Data initialization
            pieces = new Pieces[10, 9];
            currentColour = 1;
            lastOriLocationList = new List<int>();
            lastDestLocationList = new List<int>();
            lastEatenPieceList = new List<Pieces>();
            // each team has 3 chances to regret
            regretAmount = new int[] { 3, 3 };
            manualOriLocationList = new List<int>();
            manualDestLocationList = new List<int>();
            redGeneralPosition = new int[2];
            blkGeneralPosition = new int[2];

            // Initialize the pieces and store them in chess board
            int color = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    pieces[9 * i, 8 * j] = new Rook(color);
                    pieces[9 * i, 6 * j + 1] = new Horse(color);
                    pieces[9 * i, 4 * j + 2] = new Elephant(color);
                    pieces[9 * i, 2 * j + 3] = new Advisor(color);
                    pieces[5 * i + 2, 6 * j + 1] = new Cannon(color);
                }
                for (int j = 0; j < 5; j++)
                {
                    pieces[3 * i + 3, 2 * j] = new Soldier(color);
                }
                pieces[9 * i, 4] = new General(color);
                color = 1;
            }

            // Collect all pieces locations
            Board.piecesCollection();
        }

        public static void iniChessBoard()
        {
            new Board();
        }

        public static void changeTurn()
        {
            currentColour++;
        }

        // After enter the original location, save it
        public static void addLastOriLocation(int[] chosenLocation)
        {
            lastOriLocationList.Add(chosenLocation[0] * 10 + chosenLocation[1]);
        }

        // Get last original location
        public static int[] getLastOriLocation()
        {
            int[] lastOriLocation = new int[2];
            lastOriLocation[0] = lastOriLocationList.Last() / 10;
            lastOriLocation[1] = lastOriLocationList.Last() % 10;
            return lastOriLocation;
        }

        // Remove last original location
        public static void removeLastOriLocation()
        {
            if (lastOriLocationList.Count > 0) lastOriLocationList.RemoveAt(lastOriLocationList.Count - 1);
        }

        // After enter the destination location, save it
        public static void addLastDestLocation(int[] chosenLocation)
        {
            lastDestLocationList.Add(chosenLocation[0] * 10 + chosenLocation[1]);
        }

        // Get last destination location
        public static int[] getLastDestLocation()
        {
            int[] lastDestLocation = new int[2];
            lastDestLocation[0] = lastDestLocationList.Last() / 10;
            lastDestLocation[1] = lastDestLocationList.Last() % 10;
            return lastDestLocation;
        }

        // Remove last destination location
        public static void removeLastDestLocation()
        {
            if (lastDestLocationList.Count > 0) lastDestLocationList.RemoveAt(lastDestLocationList.Count - 1);
        }

        // Before eating any piece, store the pice
        public static void addLastEatenPiece(Pieces eatenPiece)
        {
            lastEatenPieceList.Add(eatenPiece);
        }

        // Get last eaten piece
        public static Pieces getLastEatenPiece()
        {
            return lastEatenPieceList.Last();
        }

        // Remove last original location
        public static void removeLastEatenPiece()
        {
            if (lastEatenPieceList.Count > 0) lastEatenPieceList.RemoveAt(lastEatenPieceList.Count - 1);
        }

        public static void piecesCollection()
        {
            redPieces = new List<int>();
            blkPieces = new List<int>();
            for (int row = 0; row < pieces.GetLength(0); row++)
            {
                for (int col = 0; col < pieces.GetLength(1); col++)
                {
                    if (pieces[row, col] != null && pieces[row, col].calculateValidMoveList(new int[] { row, col }).Count > 0)
                    {
                        if (pieces[row, col].colour == 0)
                        {
                            blkPieces.Add(row * 10 + col);
                            if (pieces[row, col].GetType() == typeof(General))
                            {
                                blkGeneralPosition[0] = row;
                                blkGeneralPosition[1] = col;
                            }
                        }
                        else
                        {
                            redPieces.Add(row * 10 + col);
                            if (pieces[row, col].GetType() == typeof(General))
                            {
                                redGeneralPosition[0] = row;
                                redGeneralPosition[1] = col;
                            }
                        }
                    }
                }
            }
        }
    }
}

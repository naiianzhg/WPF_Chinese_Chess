using System;
using System.Collections.Generic;
using System.Text;
using System.Linq; // List.Last()

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

            // Initialize the pieces and store them in chess board
            // Store Soldiers
            for (int i = 0; i < 5; i++)
            {
                pieces[3, i * 2] = new Soldier(0);
                pieces[6, i * 2] = new Soldier(1);
            }

            // Store black Rooks
            pieces[0, 0] = new Rook(0);
            pieces[0, 8] = new Rook(0);
            // Store red Rooks
            pieces[9, 0] = new Rook(1);
            pieces[9, 8] = new Rook(1);

            // Store black Horses
            pieces[0, 1] = new Horse(0);
            pieces[0, 7] = new Horse(0);
            // Store red Horses
            pieces[9, 1] = new Horse(1);
            pieces[9, 7] = new Horse(1);

            // Store black Elephants
            pieces[0, 2] = new Elephant(0);
            pieces[0, 6] = new Elephant(0);
            // Store red Elephants
            pieces[9, 2] = new Elephant(1);
            pieces[9, 6] = new Elephant(1);

            // Store black Advisors
            pieces[0, 3] = new Advisor(0);
            pieces[0, 5] = new Advisor(0);
            // Store red Advisors
            pieces[9, 3] = new Advisor(1);
            pieces[9, 5] = new Advisor(1);

            // Store black Cannons
            pieces[2, 1] = new Cannon(0);
            pieces[2, 7] = new Cannon(0);
            // Store red Cannons
            pieces[7, 1] = new Cannon(1);
            pieces[7, 7] = new Cannon(1);

            // Store black General
            pieces[0, 4] = new General(0);
            // Store red General
            pieces[9, 4] = new General(1);
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
            lastOriLocation[0] = lastOriLocationList.Last()/ 10;
            lastOriLocation[1] = lastOriLocationList.Last() % 10;
            return lastOriLocation;
        }

        // Remove last original location
        public static void removeLastOriLocation()
        {
            lastOriLocationList.Remove(lastOriLocationList.Last());
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
            lastDestLocationList.Remove(lastDestLocationList.Last());
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
            lastEatenPieceList.Remove(lastEatenPieceList.Last());
        }

        // Return the position of the Red General
        public static int[] getRedGeneralPosition()
        {
            int[] redGeneralPosition = new int[2];
            for (int row = 0; row < pieces.GetLength(0); row++)
            {
                for (int col = 0; col < pieces.GetLength(1); col++)
                {
                    // Not null piece + General piece + Red piece
                    if (pieces[row, col] != null &&
                        pieces[row, col].GetType() == typeof(General)
                        && pieces[row, col].colour % 2 == 1)
                    {
                        redGeneralPosition[0] = row;
                        redGeneralPosition[1] = col;
                    }
                }
            }
            return redGeneralPosition;
        }

        // Return the position of the Black General
        public static int[] getBlkGeneralPosition()
        {
            int[] blkGeneralPosition = new int[2];
            for (int row = 0; row < pieces.GetLength(0); row++)
            {
                for (int col = 0; col < pieces.GetLength(1); col++)
                {
                    // Not null piece + General piece + Red piece
                    if (pieces[row, col] != null &&
                        pieces[row, col].GetType() == typeof(General)
                        && pieces[row, col].colour % 2 == 0)
                    {
                        blkGeneralPosition[0] = row;
                        blkGeneralPosition[1] = col;
                    }
                }
            }
            return blkGeneralPosition;
        }

    }
}

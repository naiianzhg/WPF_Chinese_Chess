using System;
using System.Collections.Generic;
using System.Text;
using ChineseChess.Model;
using System.Linq; // List.Last()

namespace ChineseChess.Control
{
    class PiecesHandler
    {
        // Receive the input original location (piece location) and if the location is valid calculate its valid moves
        public static void chooseOri(int row, int col)
        {
            int[] chosenOriLocation = new int[] { row, col };

            // Check if there is no piece
            if (Board.pieces[row, col] == null) throw new Exception("There is no piece here");

            // Check if the chosen piece belongs to the other team
            if (Board.pieces[row, col].colour != Board.currentColour % 2) throw new Exception("This piece is not belong to you");

            // Check if the chosen piece has any possible move
            if (Board.pieces[row, col].calculateValidMoveList(chosenOriLocation).Count == 0) throw new Exception("This piece cannot move anywhere");

            // Save this chosen original location as last original location
            Board.addLastOriLocation(chosenOriLocation);
        }

        // Receive the input destination location (move location) from the user
        public static void chooseDest(int row, int col)
        {
            List<int> validMove = Board.pieces[Board.getLastOriLocation()[0], Board.getLastOriLocation()[1]].
                calculateValidMoveList(Board.getLastOriLocation());
            int[] chosenDestLocation = new int[] { row, col };

            // Check if the chosenLocation is in the valid move list of the chosen piece
            if (!validMove.Contains(chosenDestLocation[0] * 10 + chosenDestLocation[1]))
            {
                // If the player did not choose the correct position to move, the last saved ori location will be remove
                Board.removeLastOriLocation();
                throw new Exception("This move doesn't comply with the rule");
            }

            // If this move is dangerous, warn the player
            if (isDangerousMove(row, col))
            {
                // If the player did not choose the correct position to move, the last saved ori location will be remove
                Board.removeLastOriLocation();
                throw new Exception("Suicide move is forbidden in this game");
            }

            // save this chosen destination location as last destination location
            Board.addLastDestLocation(chosenDestLocation);

            // Update the pieces positions and current colour in Board
            moveTo(Board.getLastOriLocation(), chosenDestLocation);

            // Change the current colour to another
            Board.changeTurn();
        }

        // If the player enter a dangerous move (cause a checked to himself), ask for confirmation
        public static bool isDangerousMove(int row, int col)
        {
            bool isDangerous = false;
            // Assume the piece move to the chosen destination, if any piece is eaten, store the piece so that it can be put back later
            Pieces virtualEatenPiece = Board.pieces[row, col];
            moveTo(Board.getLastOriLocation(), new int[] { row, col }, 1);
            // If a team will be checked after moving and it is its turn at the same time, the player need to confirme the dangerous move
            if (GameRules.isChecked()) isDangerous = true;
            // Move the piece back to original position to continue
            moveTo(new int[] { row, col }, Board.getLastOriLocation(), 1);
            Board.pieces[row, col] = virtualEatenPiece;
            return isDangerous;
        }

        // The real moving operation, the eaten piece will be stored, * parametre i is marked for move with storing eaten piece location caused by a move, and i is set to 0 by defaut, if i == 1 the eaten piece will not be stored *
        public static void moveTo(int[] oriLocation, int[] destLocation, int i = 0)
        {
            // If there is a piece in destLocation which will be eaten, store its position else store null
            if (i == 0) Board.addLastEatenPiece(Board.pieces[destLocation[0], destLocation[1]]);
            // Move the chosen piece to the destination and reset the original position to null
            Board.pieces[destLocation[0], destLocation[1]] = Board.pieces[oriLocation[0], oriLocation[1]];
            Board.pieces[oriLocation[0], oriLocation[1]] = null;
            Board.piecesCollection();
        }
        
        // Withdraw move
        public static void withdraw()
        {
            // If there is no pieces available for withdraw
            if (Board.lastDestLocationList.Count == 0)
                throw new Exception("You have no move to withdraw");

            // Move back the piece
            PiecesHandler.moveTo(Board.getLastDestLocation(), Board.getLastOriLocation(), 1);
            // If there is an eaten piece, put it back to the board, else put null
            Board.pieces[Board.getLastDestLocation()[0], Board.getLastDestLocation()[1]] = Board.getLastEatenPiece();
            // Remove the last element of lastOriLocationList, lastDestLocationList and lastEatenPieceList after regret
            Board.removeLastOriLocation();
            Board.removeLastDestLocation();
            Board.removeLastEatenPiece();

            // Change back the turn
            Board.currentColour--;
        }

        // Regret move
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

        // When time runs out, automatically move for the player
        public static void randomMove()
        {
            List<int> canChoose, canMove;
            if (Board.currentColour % 2 == 0) { canChoose = Board.blkPieces; }
            else { canChoose = Board.redPieces; }
            Random rd = new Random();
            int i = rd.Next(canChoose.Count - 1);
            int x = canChoose[i] / 10;
            int y = canChoose[i] % 10;
            if (Board.lastOriLocationList.Count != Board.lastDestLocationList.Count) Board.removeLastOriLocation();
            // Save this chosen original location as last original location
            chooseOri(x, y);
            canMove = Board.pieces[x, y].calculateValidMoveList(new int[] { x, y });
            int j = rd.Next(canMove.Count - 1);
            chooseDest(canMove[j] / 10, canMove[j] % 10);
        }

    }
}
    
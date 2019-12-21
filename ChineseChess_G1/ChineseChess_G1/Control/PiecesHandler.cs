using System;
using System.Collections.Generic;
using System.Text;
using ChineseChess.Model;
using ChineseChess.View;
using System.Linq; // List.Last()

namespace ChineseChess.Control
{
    class PiecesHandler
    {
        // Parse the input string location in int location
        public static int[] parseLocation(string s, out bool isValid)
        {
            string[] strLocation;
            int[] location = new int[2];
            int splitAmount = 0, splitIndex = 0, numAmount = 0;
            isValid = false;

            // Split the input string 'x,y' into two parts x and y and storing into int[] (int[0] = x, int[1] = y)
            // if there is only one none-digit character between two numbers as spliting character, it is valid for format
            // Obtain the (1.)amount of none-digit character, (2.)the index of the last none-digit character and (3.)the amount of numbers
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsDigit(s[i]))
                {
                    splitAmount++;
                    splitIndex = i;
                }
                else numAmount++;
            }

            // The input must has (1.)only one non-digit character, (2.)the character must be surrounded by two numbers
            // if not, it is an informat input
            if (numAmount > 1 && splitAmount == 1 && splitIndex != s.Length - 1 && splitIndex != 0)
            {
                // The player should have entered comma as the spliting character
                // In case of not miss-enter too much times, no matter what spliting char the player use, the program can process
                // Obtain the string array location
                strLocation = s.Split(s[splitIndex]);
                // Convert the string location to int location
                for (int i = 0; i < strLocation.Length; i++)
                {
                    location[i] = Convert.ToInt32(strLocation[i]);
                }

                // Check the validity of the input position, if it is not out of range the board
                // Then the choose input is valid
                if (location[0] < 0 || location[0] > 9) throw new Exception("Row index out of range");
                else if (location[1] < 0 || location[1] > 8) throw new Exception("Column index out of range");
                else isValid = true;
            }
            else throw new Exception("Informat input");

            return location;
        }

        // Receive the input original location (piece location) and if the location is valid calculate its valid moves
        public static void chooseOri(int row, int col)
        {
            int[] chosenOriLocation = new int[] { row, col };

            // Check if the chosen piece belongs to the other team
            if (Board.pieces[row, col].colour != Board.currentColour % 2)
            {
                throw new Exception("This piece is not belong to you");
            }

            // Check if the chosen piece has any possible move
            if (Board.pieces[row, col].calculateValidMoveList(chosenOriLocation).Count == 0)
            {
                throw new Exception("This piece cannot move anywhere");
            }

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
                throw new Exception("This move doesn't comply with the rule");
            }

            //// If the player enter a location that is not able to avoid being checked or that will cause a check, warn the player and ask for confirmation
            //isValid = confirmDangerousMove(chosenDestLocation, isValid);
               
            // save this chosen destination location as last destination location
            Board.addLastDestLocation(chosenDestLocation);

            // Update the pieces positions and current colour in Board
            moveTo(Board.getLastOriLocation(), chosenDestLocation);

            //// After moving display the regret message except for the 1 round
            //DisplayMessage.displayRegretMessage();

            // Change the current colour to another
            Board.changeTurn();
        }

        // If the player enter a dangerous move (cause a checked to himself), ask for confirmation
        public static bool confirmDangerousMove(int[] chosenDestLocation, bool isValid)
        {

            // Assume the piece move to the chosen destination, if any piece is eaten, store the piece so that it can be put back later
            Pieces virtualEatenPiece = Board.pieces[chosenDestLocation[0], chosenDestLocation[1]];
            tracelessMoveTo(Board.getLastOriLocation(), chosenDestLocation);
            // If a team will be checked after moving and it is its turn at the same time, the player need to confirme the dangerous move
            if (GameRules.isChecked()[Board.currentColour % 2])
            {
                DisplayMessage.displayMoveConfirmation();
                // If the player does not confirm, then the input is invalid
                if (Console.ReadLine() == "n")
                {
                    isValid = false;
                    // Clear the exception message line when the user enter a right input
                    Console.SetCursorPosition(0, 26);
                    DisplayMessage.clearConsoleLine();
                }
            }
            // Clear the confirmation message
            Console.SetCursorPosition(0, 28);
            DisplayMessage.clearConsoleLine();
            // Anyhow, move the piece back to original position to continue
            tracelessMoveTo(chosenDestLocation, Board.getLastOriLocation());
            Board.pieces[chosenDestLocation[0], chosenDestLocation[1]] = virtualEatenPiece;

            return isValid;
        }

        // The real moving operation, the eaten piece will be stored
        public static void moveTo(int[] oriLocation, int[] destLocation)
        {
            // Move the chosen piece to the destination and reset the original position to null
            Pieces temp = Board.pieces[oriLocation[0], oriLocation[1]];
            Board.pieces[oriLocation[0], oriLocation[1]] = null;
            // If there is a piece in destLocation which will be eaten, store its position else store null
            if (Board.pieces[destLocation[0], destLocation[1]] != null)
                Board.addLastEatenPiece(Board.pieces[destLocation[0], destLocation[1]]);
            else Board.addLastEatenPiece(null);
            Board.pieces[destLocation[0], destLocation[1]] = temp;
        }

        // This virtualMoveTo method is for calculating dangerous move or regret move back, there is no eaten piece storing inside so that it is called traceless
        public static void tracelessMoveTo(int[] oriLocation, int[] destLocation)
        {
            // Move the chosen piece to the destination and reset the original position to null
            Pieces temp = Board.pieces[oriLocation[0], oriLocation[1]];
            Board.pieces[oriLocation[0], oriLocation[1]] = null;
            Board.pieces[destLocation[0], destLocation[1]] = temp;
        }
    }
}

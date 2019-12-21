using System;
using System.Collections.Generic;
using System.Text;
using ChineseChess.Model;
using ChineseChess.Control;

namespace ChineseChess.View
{
    class DisplayMessage
    {
        // Ask if the player want to regret last move
        public static void displayRegretMessage()
        {
            // Display the chess board in console
            DisplayBoard.displayChessPanel();

            // Clear the input line
            Console.SetCursorPosition(0, 25);
            clearConsoleLine();
            // Clear the input message line
            Console.SetCursorPosition(0, 24);
            clearConsoleLine();
            // Display the regret confirmation message
            // If the player has no chance for regret anymore, no need to show this message
            if (Board.regretAmount[Board.currentColour % 2] > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (Board.regretAmount[Board.currentColour % 2] == 1)
                    Console.Write("This is your last chance to regret, sure? (y/n) ");
                else if (Board.regretAmount[Board.currentColour % 2] > 1)
                    Console.Write(Board.regretAmount[Board.currentColour % 2].ToString() + " chances left - Do you need to regret? (y/n) ");
                Console.ResetColor();
                if (Console.ReadLine() == "y") GameRules.regret();
            }
        }

        // Ask the player to choose a piece to move
        public static void displayAskChooseMessage()
        {
            // Display the chess board in console
            DisplayBoard.displayChessPanel();
            // Display the current colour
            displayCurrentColour();

            // Clear the input line
            Console.SetCursorPosition(0, 25);
            clearConsoleLine();
            // Clear the input message line
            Console.SetCursorPosition(0, 24);
            clearConsoleLine();
            // Display the input message
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Please choose a piece to move (in form \'[row][notDigit][column]\' e.g. \'2,1\' \'7;7\'): ");
            Console.ResetColor();
        }

        // Ask the player to choose a position to move to
        public static void displayAskMoveMessage()
        {
            // Clear the input line
            Console.SetCursorPosition(0, 25);
            clearConsoleLine();
            // Clear the input message line
            Console.SetCursorPosition(0, 24);
            clearConsoleLine();
            // Display the input message
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Please choose a position to move (in form \'[row][notDigit][column]\' e.g. \'2,1\' \'7;7\'): ");
            Console.ResetColor();
        }

        // Display the wrong input message
        public static void displayException(Exception e)
        {
            Console.SetCursorPosition(0, 26);
            // Clear the exception line
            clearConsoleLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(e.Message);
            Console.ResetColor();
        }

        // Display the check message
        public static void displayChecked()
        {
            Console.SetCursorPosition(0, 27);
            // display the check message
            if (GameRules.isChecked()[0])
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" Black is checked! ");
                Console.ResetColor();
            }
            else if (GameRules.isChecked()[1])
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" Red is checked! ");
                Console.ResetColor();
            }
        }

        // Display the current camp/colour message
        public static void displayCurrentColour()
        {
            // Clean the original camp colour message
            Console.SetCursorPosition(0, 23);
            clearConsoleLine();

            // Display the current colour message
            if (Board.currentColour % 2 == 1)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" Move of the red ");
                Console.ResetColor();
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(" Move of the black ");
                Console.ResetColor();
            }
        }

        // Display the dangerous move confirmation message
        public static void displayMoveConfirmation()
        {
            Console.SetCursorPosition(0, 28);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("You risk being checked, are you sure to do this move? (y/n) ");
            Console.ResetColor();
        }

        // Clear the former message
        public static void clearConsoleLine()
        {
            // Clear a line by printing blank space in the width of the window
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop-1);
        }

    }
}

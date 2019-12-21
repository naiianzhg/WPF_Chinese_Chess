using System;
using System.Collections.Generic;
using System.Text;
using ChineseChess.Model;
using ChineseChess.Control;

namespace ChineseChess.View
{
    class DisplayBoard
    {
        const string chessPanel =
            "   0   1   2   3   4   5   6   7   8   \n" +
            " ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓  \n" +
            "0┃  --- --- --- --- --- --- --- --- ┃ 0\n" +
            " ┃ |   |   |   | \\ | / |   |   |   |┃  \n" +
            "1┃  --- --- --- --- --- --- --- --- ┃ 1\n" +
            " ┃ |   |   |   | / | \\ |   |   |   |┃  \n" +
            "2┃  --- --- --- --- --- --- --- --- ┃ 2\n" +
            " ┃ |   |   |   |   |   |   |   |   |┃  \n" +
            "3┃  --- --- --- --- --- --- --- --- ┃ 3\n" +
            " ┃ |   |   |   |   |   |   |   |   |┃  \n" +
            "4┃  --- --- --- --- --- --- --- --- ┃ 4\n" +
            " ┃ |          CHUHE HANJIE         |┃  \n" +
            "5┃  --- --- --- --- --- --- --- --- ┃ 5\n" +
            " ┃ |   |   |   |   |   |   |   |   |┃  \n" +
            "6┃  --- --- --- --- --- --- --- --- ┃ 6\n" +
            " ┃ |   |   |   |   |   |   |   |   |┃  \n" +
            "7┃  --- --- --- --- --- --- --- --- ┃ 7\n" +
            " ┃ |   |   |   | \\ | / |   |   |   |┃  \n" +
            "8┃  --- --- --- --- --- --- --- --- ┃ 8\n" +
            " ┃ |   |   |   | / | \\ |   |   |   |┃  \n" +
            "9┃  --- --- --- --- --- --- --- --- ┃ 9\n" +
            " ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛  \n" +
            "   0   1   2   3   4   5   6   7   8   \n";

        // Display the chess panel
        public static void displayChessPanel()
        {
            // Display the chess grid
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(0, 0);
            Console.Write(chessPanel);
            Console.ResetColor();
            // Display the pieces on the board
            displayChessPiece();
        }

        // Display the chess pieces on the panel
        public static void displayChessPiece()
        {
            for (int row = 0; row < Board.pieces.GetLength(0); row++)
            {
                for (int col = 0; col < Board.pieces.GetLength(1); col++)
                {
                    if (Board.pieces[row, col] != null)
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        // Piece colour
                        if (Board.pieces[row, col] != null & Board.pieces[row, col].colour % 2 == 1)
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        else Console.ForegroundColor = ConsoleColor.Black;
                        // Piece location
                        Console.SetCursorPosition(4 * col + 3, 2 * row + 2);
                        Console.Write(Board.pieces[row, col].type);
                        Console.ResetColor();
                    }
                }
            }
        }

        // Display the valid moves for the chosen piece
        public static void displayValidMove(int[] chosenPieceLocation)
        {
            // calculate the valid move of 
            int[] validMoves = Board.pieces[chosenPieceLocation[0], chosenPieceLocation[1]].calculateValidMoveList(chosenPieceLocation).ToArray();
            foreach (int validMoveLocation in validMoves)
            {
                Console.SetCursorPosition(4 * (validMoveLocation % 10) + 3, 2 * (validMoveLocation / 10) + 2);
                Console.BackgroundColor = ConsoleColor.Cyan;
                // If the valid move position has no piece we put a " ", else we put the type of the piece in its colour
                if (Board.pieces[validMoveLocation / 10, validMoveLocation % 10] != null)
                {
                    // Piece colour
                    if (Board.pieces[validMoveLocation / 10, validMoveLocation % 10].colour % 2 == 1)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(Board.pieces[validMoveLocation / 10, validMoveLocation % 10].type);
                } else Console.Write(" ");
                Console.ResetColor();
            }
        }

    }
}

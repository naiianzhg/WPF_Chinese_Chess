//using System;
//using System.Collections.Generic;
//using System.Text;
//using ChineseChess.Model;

//namespace ChineseChess.View
//{
//    class Display
//    {
//        const string iniGrid =
//            "   0   1   2   3   4   5   6   7   8  *" +
//            " ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓ *" +
//            "0┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┃ |   |   |   | \\ | / |   |   |   |┃ *" +
//            "1┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┃ |   |   |   | / | \\ |   |   |   |┃ *" +
//            "2┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┃ |   |   |   |   |   |   |   |   |┃ *" +
//            "3┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┃ |   |   |   |   |   |   |   |   |┃ *" +
//            "4┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┃ |          CHUHE HANJIE         |┃ *" +
//            "5┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┃ |   |   |   |   |   |   |   |   |┃ *" +
//            "6┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┃ |   |   |   |   |   |   |   |   |┃ *" +
//            "7┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┃ |   |   |   | \\ | / |   |   |   |┃ *" +
//            "8┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┃ |   |   |   | / | \\ |   |   |   |┃ *" +
//            "9┃  --- --- --- --- --- --- --- --- ┃ *" +
//            " ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛ *" +
//            "   0   1   2   3   4   5   6   7   8  *";

//        static string chessGrid = iniGrid;

//        // Display the chess board
//        public static void displayBoard(Board board)
//        {
//            List<int> redCharArr = new List<int>(), blueCharArr = new List<int>();

//            Console.OutputEncoding = System.Text.Encoding.UTF8;

//            // Classify the pieces into 2 colours(camps)
//            for (int i = 0; i < 10; i++)
//            {
//                for (int j = 0; j < 9; j++)
//                {
//                    if (board.pieces[i, j] != null && board.pieces[i, j].colour == 1)
//                    {
//                        redCharArr.Add(4 * j + 78 * i + 81);
//                    } else if (board.pieces[i, j] != null && board.pieces[i, j].colour == 0)
//                    {
//                        blueCharArr.Add(4 * j + 78 * i + 81);
//                    }
//                }
//            }
//            // Print out the chess board
//            for (int i = 0; i < chessGrid.Length; i++)
//            {
//                Console.BackgroundColor = ConsoleColor.Yellow;
//                if (redCharArr.Contains(i))
//                {
//                    Console.ForegroundColor = ConsoleColor.DarkRed;
//                    Console.Write(chessGrid[i]);
//                }
//                else if (blueCharArr.Contains(i))
//                {   
//                    Console.ForegroundColor = ConsoleColor.Black;
//                    Console.Write(chessGrid[i]);
//                }
//                else
//                {
//                    Console.ForegroundColor = ConsoleColor.DarkGray;
//                    Console.Write(chessGrid[i]);
//                }
//                Console.ResetColor();
//                // Use the '*' mark to detect the new line
//                if (chessGrid[i] == '*')
//                {
//                    Console.WriteLine();
//                }
//            }
//        }

//        // Display the chess board with valid moves
//        public static void displayValidMoveBoard(Board board, int[] validMove)
//        {
//            List<int> redCharArr = new List<int>(), blueCharArr = new List<int>();

//            Console.OutputEncoding = System.Text.Encoding.UTF8;

//            // Classify the pieces into 2 colours(camps)
//            for (int i = 0; i < 10; i++)
//            {
//                for (int j = 0; j < 9; j++)
//                {
//                    if (board.pieces[i, j] != null && board.pieces[i, j].colour == 1)
//                    {
//                        redCharArr.Add(4 * j + 78 * i + 81);
//                    }
//                    else if (board.pieces[i, j] != null && board.pieces[i, j].colour == 0)
//                    {
//                        blueCharArr.Add(4 * j + 78 * i + 81);
//                    }
//                }
//            }
//            // Print out the chess board
//            for (int i = 0; i < chessGrid.Length; i++)
//            {
//                Console.BackgroundColor = ConsoleColor.Yellow;

//                // Brushing the background colour of possible moving positions
//                for (int j = 0; j < validMove.Length; j++)
//                {
//                    if (i == 4 * (validMove[j] % 10) + 78 * (validMove[j] / 10) + 81)
//                    {
//                        Console.BackgroundColor = ConsoleColor.Gray;
//                        break;
//                    }
//                }
//                // Brushing pieces in corresponding colours
//                if (redCharArr.Contains(i))
//                {
//                    Console.ForegroundColor = ConsoleColor.DarkRed;
//                    Console.Write(chessGrid[i]);
//                }
//                else if (blueCharArr.Contains(i))
//                {
//                    Console.ForegroundColor = ConsoleColor.Black;
//                    Console.Write(chessGrid[i]);
//                }
//                else // Brushing board in darkgrey
//                {
//                    Console.ForegroundColor = ConsoleColor.DarkGray;
//                    Console.Write(chessGrid[i]);
//                }
//                Console.ResetColor();
//                // Use the '*' mark to detect the new line
//                if (chessGrid[i] == '*')
//                {
//                    Console.WriteLine();
//                }
//            }
//        }

//        // Replace the proper position with pieces
//        // modifying the grid string
//        public static string brushPieces(Board board)
//        {
//            chessGrid = iniGrid;
//            for (int i = 0; i < 10; i++)
//            {
//                for (int j = 0; j < 9; j++)
//                {
//                    if (board.pieces[i, j] != null)
//                    {
//                        chessGrid = chessGrid.
//                            Remove(4 * j + 78 * i + 81, 1).
//                            Insert(4 * j + 78 * i + 81, board.pieces[i, j].type);
//                    }
//                }
//            }
//            return chessGrid;
//        }

//        // Display the chess board and then ask the player to choose a piece to move
//        public static void askChoose(Board board, int currentColour)
//        {
//            // Initialize board
//            chessGrid = iniGrid;

//            // and brush the pieces on it
//            chessGrid = brushPieces(board);

//            // and finally displaly the board
//            Console.SetCursorPosition(0, 0);
//            displayBoard(board);

//            // Display the current colour
//            DisplayMessage.displayCurrentColour();

//            DisplayMessage.displayAskChooseMessage();
//        }

//        // Display the possible valide move of the chosen piece
//        public static int[] chosen(Board board, int[] location)
//        {
//            int[] validMove = board.pieces[location[0], location[1]].countValidMove(board, location).ToArray();

//            // brush the possible valide moves on the chess board with pieces on it
//            Console.SetCursorPosition(0, 0);
//            displayValidMoveBoard(board, validMove);

//            // After chosen remove the exception message
//            Console.SetCursorPosition(0, 26);
//            DisplayMessage.clearConsoleLine();
//            // Also remove the checked message
//            Console.SetCursorPosition(0, 27);
//            DisplayMessage.clearConsoleLine();

//            return validMove;
//        }
//    }
//}

//using System;
//using System.Collections.Generic;
//using System.Text;
//using ChineseChess.Model;
//using ChineseChess.View;

//namespace ChineseChess.Control
//{
//    class Control
//    {
//        // Initialize the Xiangqi game system
//        public static void iniGame()
//        {
//            Board.iniChessBoard();
//            // Display the chess board in console
//            DisplayBoard.displayChessPanel();
//        }

//        // Parse the input string location in int location
//        public static int[] parseLocation(string s, out bool isValid)
//        {
//            string[] strLocation = new string[2];
//            int[] location = new int[2];
//            int splitAmount = 0, splitIndex = 0, numAmount = 0;
//            isValid = false;

//            // Split the input string 'x,y' into two parts x and y and storing into int[] (int[0] = x, int[1] = y)
//            // if there is only one none-digit character between two numbers as spliting character, it is valid for format
//            // Obtain the (1.)amount of none-digit character, (2.)the index of the last none-digit character and (3.)the amount of numbers
//            for (int i = 0; i < s.Length; i++)
//            {
//                if (!char.IsDigit(s[i]))
//                {
//                    splitAmount++;
//                    splitIndex = i;
//                }
//                else numAmount++;
//            }

//            // The input must has (1.)only one non-digit character, (2.)the character must be surrounded by two numbers
//            // if not, it is an informat input
//            if (numAmount > 1 && splitAmount == 1 && splitIndex != s.Length - 1)
//            {
//                // The player should have entered comma as the spliting character
//                // In case of not miss-enter too much times, no matter what spliting char the player use, the program can process
//                // Obtain the string array location
//                strLocation = s.Split(s[splitIndex]);
//                // Convert the string location to int location
//                for (int i = 0; i < strLocation.Length; i++)
//                {
//                    location[i] = Convert.ToInt32(strLocation[i]);
//                }

//                // Check the validity of the input position, if it is not out of range the board
//                // Then the choose input is valid
//                if (location[0] < 0 || location[0] > 9) throw new Exception("Row index out of range");
//                else if (location[1] < 0 || location[1] > 8) throw new Exception("Column index out of range");
//                else isValid = true;
//            }
//            else throw new Exception("Informat input");

//            return location;
//        }

//        public static int[] toChooseOri()
//        {
//            int[] chosenLocation = new int[2];
//            bool isValid = false;
            
//            // In this loop the player will need to input a location until the right one is input
//            do
//            {
//                try
//                {
//                    // Convert the string location to integer location
//                    // if the parseLocation did not throw any exception, formatly the input is correct
//                    chosenLocation = parseLocation(Console.ReadLine(), out isValid);

//                    // Check if the chosen Location is null
//                    if (isValid && Board.pieces[chosenLocation[0], chosenLocation[1]] == null)
//                    {
//                        isValid = false;
//                        throw new Exception("No piece can be chosen");
//                    }

//                    // if there exist a piece in the chosen location
//                    if (isValid && Board.pieces[chosenLocation[0], chosenLocation[1]] != null)
//                    {
//                        // check if the chosen piece is in the current colour, current move cannot be the piece of the opposite side
//                        if (Board.pieces[chosenLocation[0], chosenLocation[1]].colour != Board.currentColour % 2)
//                        {
//                            isValid = false;
//                            throw new Exception("This piece is not belong to you");
//                        }

//                        //Check if the chosen piece has any possible move
//                        if (Board.pieces[chosenLocation[0], chosenLocation[1]].calculateValidMove(chosenLocation).Count == 0)
//                        {
//                            isValid = false;
//                            throw new Exception("This piece cannot move anywhere");
//                        }
//                    }
//                }
//                catch (Exception e)
//                {
//                    DisplayMessage.displayException(e);
//                    DisplayMessage.displayAskChooseMessage();
//                }
//            } while (!isValid);
//            return chosenLocation;
//        }

//        public static int[] toChooseDest(int[] validMove)
//        {
//            int[] chosenLocation = new int[2];
//            bool isValid = false;

//            // In this loop the player will need to input a location until the right one is input
//            do
//            {
//                try
//                {
//                    // Convert the string location to integer location
//                    // if the parseLocation did not throw any exception, formatly the input is correct
//                    chosenLocation = parseLocation(Console.ReadLine(), out isValid);

//                    // Check if the chosenLocation is in the valid move list of the chosen piece
//                    if (isValid && !Array.Exists(validMove, element => element == (chosenLocation[0] * 10 + chosenLocation[1])))
//                    {
//                        isValid = false;
//                        throw new Exception("This move doesn't comply with the rule");
//                    }
//                }
//                catch (Exception e)
//                {
//                    DisplayMessage.displayException(e);
//                    DisplayMessage.displayAskMoveMessage();
//                }
//            } while (!isValid);
//            return chosenLocation;
//        }

//        public static void moveTo(int[] iniLocation, int[] destLocation)
//        {
//            // Move the chosen piece to the destination and reset the original position to null
//            Pieces temp = Board.pieces[iniLocation[0], iniLocation[1]];
//            Board.pieces[iniLocation[0], iniLocation[1]] = null;
//            Board.pieces[destLocation[0], destLocation[1]] = temp;
//        }

//        public static bool calculateCheck(int[] generalLocation)
//        {
//            bool isDangerous = false;
//            int[] enermyLocation = new int[2];

//            // Calculate the valid moves of all the red(b) pieces, once the valid moves of one red(b) pieces contains the black(r) general, the black is checked.
//            for (int i = 0; i < 10; i++)
//            {
//                for (int j = 0; j < 9; j++)
//                {
//                    if (Board.pieces[i, j] != null && Board.pieces[i, j].colour != Board.pieces[generalLocation[0], generalLocation[1]].colour)
//                    {
//                        enermyLocation[0] = i;
//                        enermyLocation[1] = j;
//                        if (Board.pieces[i, j].calculateValidMove(enermyLocation).Contains(generalLocation[0] * 10 + generalLocation[1]))
//                        {
//                            isDangerous = true;
//                            break;
//                        }
//                    }
//                }
//            }
//            return isDangerous;

//            // Check if the black general location is in the valid move list of any of the red pieces
//            // 1. Vertical direction: only cannon, rook and soldier will possibly eat the general
//            for (int i = 0; i < 9; i++)
//            {
//                if (board.pieces[i, generalLocation[1]] != null &&
//                    board.pieces[i, generalLocation[1]].colour != board.pieces[generalLocation[0], generalLocation[1]].colour &&
//                    (board.pieces[i, generalLocation[1]].GetType() == typeof(Soldier) ||
//                    board.pieces[i, generalLocation[1]].GetType() == typeof(Cannon) ||
//                    board.pieces[i, generalLocation[1]].GetType() == typeof(Rook)))
//                {
//                    pieceLocation[0] = i;
//                    pieceLocation[1] = generalLocation[1];
//                    if (board.pieces[i, generalLocation[1]].calculateValidMove(board, pieceLocation).Contains(generalLocation[0] * 10 + generalLocation[1]))
//                    {
//                        isDangerous = true;
//                        break;
//                    }
//                }
//            }
//            // 2. Horizontal direction: only cannon, rook and soldier will possibly eat the general
//            for (int j = 0; j < 8; j++)
//            {
//                if (board.pieces[generalLocation[0], j] != null &&
//                    (board.pieces[generalLocation[0], j].GetType() == typeof(Soldier) ||
//                    board.pieces[generalLocation[0], j].GetType() == typeof(Cannon) ||
//                    board.pieces[generalLocation[0], j].GetType() == typeof(Rook)))
//                {
//                    pieceLocation[0] = generalLocation[0];
//                    pieceLocation[1] = j;
//                    if (board.pieces[generalLocation[0], j].calculateValidMove(board, pieceLocation).Contains(generalLocation[0] * 10 + generalLocation[1]))
//                    {
//                        isDangerous = true;
//                        break;
//                    }
//                }
//            }
//            // 3. "Horse direction", in the special 日 movement by rule, only horse can eat the general, the range will be [[x-2->x+2], [y-2->y+2]]
//            // [x-2, y-2]  [x-2, y-1]  [x-2, y]   [x-2, y+1]  [x-2, y+2]
//            // [x-1, y-2]  [x-1, y-1]  [x-1, y]   [x-1, y+1]  [x-1, y+2]
//            // [x, y-2]    [x, y-1]    [general]  [x, y+1]    [x, y+2]
//            // [x+1, y-2]  [x+1, y-1]  [x+1, y]   [x+1, y+1]  [x+1, y+2]
//            // [x+2, y-2]  [x+2, y-1]  [x+2, y]   [x+2, y+1]  [x+2, y+2]
//            // **except for
//            //              1. when the general is on [0,y] or [9,y], in this case the general only need to worry about [x+1,y],[x+2,y] or [x-1,y],[x-2,y]
//            //              2. when the general is on [1,y] or [8,y], in this case the general should worry about [x-1,y],[x+1,y],[x+2,y] or [x+1,y],[x-1,y],[x-2,y]
//            if (generalLocation[0] == 2 || generalLocation[0] == 7) // [2,y] or [7,y]
//            {
//                for (int i = generalLocation[0] - 2; i < generalLocation[0] + 2; i++)
//                {
//                    for (int j = generalLocation[1] - 2; j < generalLocation[1] + 2; j++)
//                    {
//                        if (board.pieces[i, j] != null && board.pieces[i, j].GetType() == typeof(Horse))
//                        {
//                            pieceLocation[0] = i;
//                            pieceLocation[1] = j;
//                            if (board.pieces[generalLocation[0], j].calculateValidMove(board, pieceLocation).Contains(generalLocation[0] * 10 + generalLocation[1]))
//                            {
//                                isDangerous = true;
//                                break;
//                            }
//                        }
//                    }
//                }
//            }
//            else if (generalLocation[0] == 1 || generalLocation[0] == 8) // [1, y] or [8, y]
//            {
//                for (int j = generalLocation[1] - 2; j < generalLocation[1] + 2; j++)
//                {
//                    if (j == generalLocation[1])
//                    {
//                        continue;
//                    }
//                    if (generalLocation[0] == 1) // For the red general
//                    {
//                        for (int i = generalLocation[0] - 1; i < generalLocation[0] + 2; i++)
//                        {
//                            if (i == generalLocation[0])
//                            {
//                                continue;
//                            }
//                            if (board.pieces[i, j] != null && board.pieces[i, j].GetType() == typeof(Horse))
//                            {
//                                pieceLocation[0] = i;
//                                pieceLocation[1] = j;
//                                if (board.pieces[generalLocation[0], j].calculateValidMove(board, pieceLocation).Contains(generalLocation[0] * 10 + generalLocation[1]))
//                                {
//                                    isDangerous = true;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                    else // For the black general
//                    {
//                        for (int i = generalLocation[0] - 2; i < generalLocation[0] + 1; i++)
//                        {
//                            if (i == generalLocation[0])
//                            {
//                                continue;
//                            }
//                            if (board.pieces[i, j] != null && board.pieces[i, j].GetType() == typeof(Horse))
//                            {
//                                pieceLocation[0] = i;
//                                pieceLocation[1] = j;
//                                if (board.pieces[generalLocation[0], j].calculateValidMove(board, pieceLocation).Contains(generalLocation[0] * 10 + generalLocation[1]))
//                                {
//                                    isDangerous = true;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            else if (generalLocation[0] == 0 || generalLocation[0] == 9)
//            {
//                for (int j = generalLocation[1] - 2; j < generalLocation[1] + 2; j++)
//                {
//                    if (generalLocation[0] == 0) // For the red general
//                    {
//                        for (int i = generalLocation[0]; i < generalLocation[0] + 1; i++)
//                        {
//                            if (board.pieces[i, j] != null && board.pieces[i, j].GetType() == typeof(Horse))
//                            {
//                                pieceLocation[0] = i;
//                                pieceLocation[1] = j;
//                                if (board.pieces[generalLocation[0], j].calculateValidMove(board, pieceLocation).Contains(generalLocation[0] * 10 + generalLocation[1]))
//                                {
//                                    isDangerous = true;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                    else // For the black general
//                    {
//                        for (int i = generalLocation[0] - 2; i < generalLocation[0] - 1; i++)
//                        {
//                            if (board.pieces[i, j] != null && board.pieces[i, j].GetType() == typeof(Horse))
//                            {
//                                pieceLocation[0] = i;
//                                pieceLocation[1] = j;
//                                if (board.pieces[generalLocation[0], j].calculateValidMove(board, pieceLocation).Contains(generalLocation[0] * 10 + generalLocation[1]))
//                                {
//                                    isDangerous = true;
//                                    break;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        public static bool isChecked()
//        {
//            int[] generalLocation = new int[2];
//            bool isDangerous = false;

//            if (Board.currentColour % 2 == 1) // when the curren colour is red
//            {
//                // Grab the black general's position
//                for (int i = 0; i <= 2; i++)
//                {
//                    for (int j = 3; j <= 5; j++)
//                    {
//                        if (Board.pieces[i, j] != null && Board.pieces[i, j].GetType() == typeof(General))
//                        {
//                            generalLocation[0] = i;
//                            generalLocation[1] = j;
//                        }
//                    }
//                }
//                isDangerous = calculateCheck(generalLocation);
//            }
//            else // when the current colour is black
//            {
//                // Grab the black general's position
//                for (int i = 7; i <= 9; i++)
//                {
//                    for (int j = 3; j <= 5; j++)
//                    {
//                        if (Board.pieces[i, j] != null && Board.pieces[i, j].GetType() == typeof(General))
//                        {
//                            generalLocation[0] = i;
//                            generalLocation[1] = j;
//                        }
//                    }
//                }
//                isDangerous = calculateCheck(generalLocation);
//            }

//            return isDangerous;
//        }
//    }
//}

using System.Collections.Generic;
using System.Text;
using ChineseChess.Control;

namespace ChineseChess.Model
{

    abstract class Pieces
    {
        // odd->RED, even->BLACK
        public int colour { get; }
        // S,C,R,H,E,A,G
        public string type { get; }
        public List<int> validMoveList { get; set; }
        public string imageUrl { get; set; }
        public string chooseSoundUrl { get; set; }
        public string moveSoundUrl { get; set; }

        public Pieces(int colour, string type)
        {
            this.colour = colour;
            this.type = type;
        }

        public abstract List<int> calculateValidMoveList(int[] location);
        //this is for checking the possible position has no piece or has a not-black piece(eat)
        public List<int> addValidMove(int x, int y, List<int> list)
        {
            if (Board.pieces[x, y] == null || Board.pieces[x, y].colour != colour)
            {
                list.Add(x * 10 + y);
            }
            return list;
        }
    }

    class Soldier : Pieces
    {
        public Soldier(int colour) :
            base(colour, "S")
        {
            chooseSoundUrl = "Resources/chooseSoldier.mp3";
            moveSoundUrl = "Resources/moveSoldier.mp3";
            if (colour == 1) imageUrl = "Resources/RedSoldier.png";
            else imageUrl = "Resources/BlackSoldier.png";
        }

        public override List<int> calculateValidMoveList(int[] location)
        {
            int x = location[0];
            int y = location[1];
            // Initialize the valideMove List
            validMoveList = new List<int>();

            if (colour == 0 & x < 9)
            {
                validMoveList = addValidMove(x + 1, y, validMoveList);
            }
            if (colour == 1 & x > 0)
            {
                validMoveList = addValidMove(x - 1, y, validMoveList);
            }
            if ((colour == 1 & x < 5) | (colour % 2 == 0 & x > 4))
            {
                if (y != 0)
                {
                    validMoveList = addValidMove(x, y - 1, validMoveList);
                }
                if (y != 8)
                {
                    validMoveList = addValidMove(x, y + 1, validMoveList);
                }
            }
            return validMoveList;
        }
    }

    class Cannon : Pieces
    {
        public Cannon(int colour) :
            base(colour, "C")
        {
            chooseSoundUrl = "Resources/chooseCannon.mp3";
            moveSoundUrl = "Resources/moveCannon.mp3";
            if (colour == 1) imageUrl = "Resources/RedCannon.png";
            else imageUrl = "Resources/BlackCannon.png";
        }

        public object[] obj = new object[2];

        public object[] addColumnValidMove(int count, int i, int[] location, List<int> validMoveList)
        {
            if (Board.pieces[i, location[1]] == null && count == 0)
            {
                validMoveList.Add(10 * i + location[1]);
            }
            else if (Board.pieces[i, location[1]] != null)
            {
                ++count;
                if (count == 2 && Board.pieces[i, location[1]].colour != colour)
                {
                    validMoveList.Add(10 * i + location[1]);
                }
            }
            obj[0] = validMoveList;
            obj[1] = count;
            return obj;
        }

        public object[] addRowValidMove(int count, int j, int[] location, List<int> validMoveList)
        {
            if (Board.pieces[location[0], j] == null && count == 0)
            {
                validMoveList.Add(10 * location[0] + j);
            }
            else if (Board.pieces[location[0], j] != null)
            {
                ++count;
                if (count == 2 && Board.pieces[location[0], j].colour != colour)
                {
                    validMoveList.Add(10 * location[0] + j);
                }
            }
            obj[0] = validMoveList;
            obj[1] = count;
            return obj;
        }

        public override List<int> calculateValidMoveList(int[] location)
        {
            // Initialize the valideMove List
            validMoveList = new List<int>();
            // To count the rank of the piece on the route (the first/second/... enermy piece)
            int count = 0;
            // Recieve the return value array of addValidMove functions
            object[] obj = new object[2];

            // UP Column possible moves
            for (int i = location[0] - 1; i >= 0; i--)
            {
                obj = addColumnValidMove(count, i, location, validMoveList);
                validMoveList = (List<int>)obj[0];
                count = (int)obj[1];
            }
            count = 0;

            // DOWN Column possible moves
            for (int i = location[0] + 1; i <= 9; i++)
            {
                obj = addColumnValidMove(count, i, location, validMoveList);
                validMoveList = (List<int>)obj[0];
                count = (int)obj[1];
            }
            count = 0;

            // RIGHT Row possible moves
            for (int j = location[1] + 1; j <= 8; j++)
            {
                obj = addRowValidMove(count, j, location, validMoveList);
                validMoveList = (List<int>)obj[0];
                count = (int)obj[1];
            }
            count = 0;

            // LEFT Row possible moves
            for (int j = location[1] - 1; j >= 0; j--)
            {
                obj = addRowValidMove(count, j, location, validMoveList);
                validMoveList = (List<int>)obj[0];
                count = (int)obj[1];
            }
            count = 0;

            return validMoveList;
        }
    }

    class Rook : Pieces
    {
        public Rook(int colour) :
            base(colour, "R")
        {
            chooseSoundUrl = "Resources/chooseRook.mp3";
            moveSoundUrl = "Resources/moveRook.mp3";
            if (colour == 1) imageUrl = "Resources/RedRook.png";
            else imageUrl = "Resources/BlackRook.png";
        }
        public override List<int> calculateValidMoveList(int[] location)
        {
            int x = location[0];
            int y = location[1];
            // Initialize the valideMove List
            validMoveList = new List<int>();
            for (int i = 1; i <= x; i++)
            {              
                validMoveList = addValidMove(x - i, y, validMoveList);
                if (Board.pieces[x - i, y] != null) break;
            }
            for (int i = 1; i <= 9 - x; i++)
            {
                
                validMoveList = addValidMove(x + i, y, validMoveList);
                if (Board.pieces[x + i, y] != null) break;
            }
            for (int i = 1; i <= y; i++)
            {
                
                validMoveList = addValidMove(x, y - i, validMoveList);
                if (Board.pieces[x, y - i] != null) break;
            }
            for (int i = 1; i <= 8 - y; i++)
            {
                validMoveList = addValidMove(x, y + i, validMoveList);
                if (Board.pieces[x, y + i] != null) break;

            }

            return validMoveList;
        }
    }

    class Horse : Pieces
    {
        public Horse(int colour) :
            base(colour, "H")
        {
            chooseSoundUrl = "Resources/chooseHorse.mp3";
            moveSoundUrl = "Resources/moveHorse.mp3";
            if (colour == 1) imageUrl = "Resources/RedHorse.png";
            else imageUrl = "Resources/BlackHorse.png";
        }

        public override List<int> calculateValidMoveList(int[] location)
        {
            int x = location[0];
            int y = location[1];
            // Initialize the valideMove List
            validMoveList = new List<int>();

            // Cannot be on the buttom boarder or have distance of 1 from the boarder - Upward moving
            // if there is piece blocking the route forward
            if (x > 1 && Board.pieces[x - 1, y] == null)
            {
                // Detecte the position (x-2, y+1)
                if (y < 8)
                {
                    validMoveList = addValidMove(x - 2, y + 1, validMoveList);
                }
                // Also detecte the position (x-2, y-1)
                if (y > 0)
                {
                    validMoveList = addValidMove(x - 2, y - 1, validMoveList);
                }
            }
            // Cannot be on the buttom boarder or have distance of 1 from the boarder - Downward moving
            // if there is piece blocking the route forward
            if (x < 8 && Board.pieces[x + 1, y] == null)
            {
                // Detecte the position (x+2, y+1)
                if (y < 8)
                {
                    validMoveList = addValidMove(x + 2, y + 1, validMoveList);
                }
                // Also detecte the position (x+2, y-1)
                if (y > 0)
                {
                    validMoveList = addValidMove(x + 2, y - 1, validMoveList);
                }
            }
            // Cannot be on the buttom boarder or have distance of 1 from the boarder - Leftward moving
            // if there is piece blocking the route forward
            if (y > 1 && Board.pieces[x, y - 1] == null)
            {
                // Detecte the position (x+1, y-2)
                if (x < 9)
                {
                    validMoveList = addValidMove(x + 1, y - 2, validMoveList);
                }
                // Also detecte the position (x-1, y-2)
                if (x > 0)
                {
                    validMoveList = addValidMove(x - 1, y - 2, validMoveList);
                }
            }
            // Cannot be on the buttom boarder or have distance of 1 from the boarder - Rightward moving
            // if there is piece blocking the route forward
            if (y < 7 && Board.pieces[x, y + 1] == null)
            {
                // Detecte the position (x+1, y+2)
                if (x < 9)
                {
                    validMoveList = addValidMove(x + 1, y + 2, validMoveList);
                }
                // Also detecte the position (x-1, y+2)
                if (x > 0)
                {
                    validMoveList = addValidMove(x - 1, y + 2, validMoveList);
                }
            }

            return validMoveList;
        }
    }

    class Elephant : Pieces
    {
        public Elephant(int colour) :
            base(colour, "E")
        {
            chooseSoundUrl = "Resources/chooseElephant.mp3";
            moveSoundUrl = "Resources/moveElephant.mp3";
            if (colour == 1) imageUrl = "Resources/RedElephant.png";
            else imageUrl = "Resources/BlackElephant.png";
        }

        public override List<int> calculateValidMoveList(int[] location)
        {
            int x = location[0];
            int y = location[1];
            validMoveList = new List<int>();

            //Red or Black elephants has their own moving independent zoo, below is checking if the piece is in the edge of their own zoo to prevent it going outside.

            if (x != 4 & x != 9)
            {
                if (y != 0 && Board.pieces[x + 1, y - 1] == null)
                {
                    validMoveList = addValidMove(x + 2, y - 2, validMoveList);
                }
                if (y != 8 && Board.pieces[x + 1, y + 1] == null)
                {
                    validMoveList = addValidMove(x + 2, y + 2, validMoveList);
                }
            }

            if (x != 0 & x != 5)
            {
                if (y != 0 && Board.pieces[x - 1, y - 1] == null)
                {
                    validMoveList = addValidMove(x - 2, y - 2, validMoveList);
                }
                if (y != 8 && Board.pieces[x - 1, y + 1] == null)
                {
                    validMoveList = addValidMove(x - 2, y + 2, validMoveList);
                }
            }

            return validMoveList;
        }
    }

    class Advisor : Pieces
    {
        public Advisor(int colour) :
            base(colour, "A")
        {
            chooseSoundUrl = "Resources/chooseAdvisor.mp3";
            moveSoundUrl = "Resources/moveAdvisor.mp3";
            if (colour == 1) imageUrl = "Resources/RedAdvisor.png";
            else imageUrl = "Resources/BlackAdvisor.png";
        }

        public override List<int> calculateValidMoveList(int[] location)
        {
            int x = location[0];
            int y = location[1];
            int[] a = { 1, -1 };
            validMoveList = new List<int>();

            // If the piece is on the center of the grid,it can move  
            if (y == 4)
            {
                foreach (int i in a)
                {
                    foreach (int j in a)
                    {
                        validMoveList = addValidMove(x + i, y + j, validMoveList);
                    }
                }
            }
            else // Otherwise, the piece will be on the corner of the grid
            {
                if (colour == 1)
                {
                    validMoveList = addValidMove(8, 4, validMoveList);
                }
                else if (colour == 0)
                {
                    validMoveList = addValidMove(1, 4, validMoveList);
                }
            }

            return validMoveList;
        }
    }

    class General : Pieces
    {
        public General(int colour) :
            base(colour, "G")
        {
            chooseSoundUrl = "Resources/chooseGeneral.mp3";
            moveSoundUrl = "Resources/moveGeneral.mp3";
            if (colour == 1) imageUrl = "Resources/RedGeneral.png";
            else imageUrl = "Resources/BlackGeneral.png";
        }
        public override List<int> calculateValidMoveList(int[] location)
        {
            int x = location[0];
            int y = location[1];
            validMoveList = new List<int>();

            // check if this general can fly to eat the other by traverse the colume of this general

            if (colour == 1)
            {
                for (int i = x - 1; i > 0; i--)
                {
                    if (Board.pieces[i, y] != null)
                    {
                        if (Board.pieces[i, y].GetType() == typeof(General))
                        {
                            validMoveList.Add(i * 10 + y);
                        }
                        else break;
                    }
                }
            }
            else
            {
                for (int i = x + 1; i < 9; i++)
                {
                    if (Board.pieces[i, y] != null)
                    {
                        if (Board.pieces[i, y].GetType() == typeof(General))
                        {
                            validMoveList.Add(i * 10 + y);
                        }
                        else break;
                    }
                }
            }

            // find the normal moving by checking if it is in the edge of general's zoo
            if (y != 3)
            {
                validMoveList = addValidMove(x, y - 1, validMoveList);
            }
            if (y != 5)
            {
                validMoveList = addValidMove(x, y + 1, validMoveList);
            }
            if (x != 2 & x != 9)
            {
                validMoveList = addValidMove(x + 1, y, validMoveList);
            }
            if (x != 0 & x != 7)
            {
                validMoveList = addValidMove(x - 1, y, validMoveList);
            }

            return validMoveList;
        }
    }

}
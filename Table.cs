namespace ConnectFour
{

    public class Table
    {

        public static int Width { get; } = 7;
        public static int Height { get; } = 6;

        public ConsoleColor Color { get; private set; }
        private Disc[,] Discs { get; set; }
        public int DiscCounter { get; private set; }

        public Table(ConsoleColor color)
        {
            Discs = new Disc[Width, Height];
            Color = color;
            DiscCounter = 0;
        }

        public void CopyFrom(Table table) // copy data from another table
        {
            Array.Copy(table.Discs, Discs, Width * Height);
            DiscCounter = table.DiscCounter;
        }

        public bool PlaceDisc(int x, Disc disc) // place a disc
        {
            for (int y = Discs.GetLength(1) - 1; y >= 0; y--) // iterate from y = 5 to y = 0
            {
                if (Discs[x, y] == null) // table has a empty place at (x, y)
                {
                    DiscCounter++; // add DiscCounter
                    Discs[x, y] = new Disc(x, y, disc.Symbol, disc.Color, DiscCounter); // place a new disc
                    return true;
                }
            }
            return false;
        }

        public void RemoveDisc(int x, int y) // remove a disc
        {
            Discs[x, y] = null;
        }

        public int AvailablePlaceY(int x)
        {
            // check available x place and return available y
            for (int y = Discs.GetLength(1) - 1; y >= 0; y--) // iterate from y = 5 to y = 0, bottom to top
            {
                if (Discs[x, y] == null) // table has a empty place at (x, y)
                {
                    return y;
                }
            }
            return -1;
        }

        public int[] CheckOffensiveMove12(ConsoleColor color)  // from 1 to 2 in line
        {
            int[] moves = { 0, 0, 0, 0, 0, 0, 0 };
            return moves;
        }

        public int[] CheckOffensiveMove23(ConsoleColor color) // from 2 to 3 in line
        {
            int[] moves = { 0, 0, 0, 0, 0, 0, 0 };
            return moves;
        }

        public int[] CheckOffensiveMove34(ConsoleColor color) // 3 to 4 in line places
        {
            int[] moves = { 0, 0, 0, 0, 0, 0, 0 };
            return moves;
        }

        public int[] CheckFormMove3(ConsoleColor color) // 2 to connect 3 with both sides opening
        {
            int[] moves = { 0, 0, 0, 0, 0, 0, 0 };
            for (int y = 0; y < 6; y++) // check horizontal
            {
                for (int x = 0; x < 3; x++)
                {
                    if (Discs[x, y] == null && Discs[x + 1, y] == null && Discs[x + 2, y] != null && Discs[x + 3, y] != null && Discs[x + 4, y] == null &&
                        AvailablePlaceY(x) == y && AvailablePlaceY(x + 1) == y && Discs[x + 2, y].Color == color && Discs[x + 3, y].Color == color && AvailablePlaceY(x + 4) == y)
                    {
                        moves[x + 1]++;
                    }
                    if (Discs[x, y] == null && Discs[x + 1, y] != null && Discs[x + 2, y] == null && Discs[x + 3, y] != null && Discs[x + 4, y] == null &&
                        AvailablePlaceY(x) == y && Discs[x + 1, y].Color == color && AvailablePlaceY(x + 2) == y && Discs[x + 3, y].Color == color && AvailablePlaceY(x + 4) == y)
                    {
                        moves[x + 2]++;
                    }
                    if (Discs[x, y] == null && Discs[x + 1, y] != null && Discs[x + 2, y] != null && Discs[x + 3, y] == null && Discs[x + 4, y] == null &&
                        AvailablePlaceY(x) == y && Discs[x + 1, y].Color == color && Discs[x + 2, y].Color == color && AvailablePlaceY(x + 3) == y && AvailablePlaceY(x + 4) == y)
                    {
                        moves[x + 3]++;
                    }
                }
            }

            for (int y = 0; y < 2; y++) // check diagonal
            {
                for (int x = 0; x < 3; x++)
                {
                    if (Discs[x, y] == null && Discs[x + 1, y + 1] == null && Discs[x + 2, y + 2] != null && Discs[x + 3, y + 3] != null && Discs[x + 4, y + 4] == null &&
                        AvailablePlaceY(x) == y && AvailablePlaceY(x + 1) == y + 1 && Discs[x + 2, y + 2].Color == color && Discs[x + 3, y + 3].Color == color && AvailablePlaceY(x + 4) == y + 4)
                    {
                        moves[x + 1]++;
                    }
                    if (Discs[x, y] == null && Discs[x + 1, y + 1] != null && Discs[x + 2, y + 2] == null && Discs[x + 3, y + 3] != null && Discs[x + 4, y + 4] == null &&
                        AvailablePlaceY(x) == y && Discs[x + 1, y + 1].Color == color && AvailablePlaceY(x + 2) == y + 2 && Discs[x + 3, y + 3].Color == color && AvailablePlaceY(x + 4) == y + 4)
                    {
                        moves[x + 2]++;
                    }
                    if (Discs[x, y] == null && Discs[x + 1, y + 1] != null && Discs[x + 2, y + 2] != null && Discs[x + 3, y + 3] == null && Discs[x + 4, y + 4] == null &&
                        AvailablePlaceY(x) == y && Discs[x + 1, y + 1].Color == color && Discs[x + 2, y + 2].Color == color && AvailablePlaceY(x + 3) == y + 3 && AvailablePlaceY(x + 4) == y + 4)
                    {
                        moves[x + 3]++;
                    }
                }

                for (int x = 4; x < 7; x++)
                {
                    if (Discs[x, y] == null && Discs[x - 1, y + 1] == null && Discs[x - 2, y + 2] != null && Discs[x - 3, y + 3] != null && Discs[x - 4, y + 4] == null &&
                        AvailablePlaceY(x) == y && AvailablePlaceY(x - 1) == y + 1 && Discs[x - 2, y + 2].Color == color && Discs[x - 3, y + 3].Color == color && AvailablePlaceY(x - 4) == y + 4)
                    {
                        moves[x - 1]++;
                    }
                    if (Discs[x, y] == null && Discs[x - 1, y + 1] != null && Discs[x - 2, y + 2] == null && Discs[x - 3, y + 3] != null && Discs[x - 4, y + 4] == null &&
                        AvailablePlaceY(x) == y && Discs[x - 1, y + 1].Color == color && AvailablePlaceY(x - 2) == y + 2 && Discs[x - 3, y + 3].Color == color && AvailablePlaceY(x - 4) == y + 4)
                    {
                        moves[x - 2]++;
                    }
                    if (Discs[x, y] == null && Discs[x - 1, y + 1] != null && Discs[x - 2, y + 2] != null && Discs[x - 3, y + 3] == null && Discs[x - 4, y + 4] == null &&
                        AvailablePlaceY(x) == y && Discs[x - 1, y + 1].Color == color && Discs[x - 2, y + 2].Color == color && AvailablePlaceY(x - 3) == y + 3 && AvailablePlaceY(x - 4) == y + 4)
                    {
                        moves[x - 3]++;
                    }
                }
            }
            return moves;
        }

        public int[,] CheckFormLockMove(ConsoleColor color) // potential lock column places
        {
            int[,] moves = new int[2, 7] { { 0, 0, 0, 0, 0, 0, 0 }, { -1, -1, -1, -1, -1, -1, -1 } };
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (Discs[x + 3, y] == null && AvailablePlaceY(x + 3) > y)
                    {
                        if (Discs[x, y] == null && Discs[x + 1, y] != null && Discs[x + 2, y] != null &&
                            AvailablePlaceY(x) == y && Discs[x + 1, y].Color == color && Discs[x + 2, y].Color == color)
                        {
                            moves[0, x]++;
                            if (y > moves[1, x])
                            {
                                moves[1, x] = y;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 1, y] == null && Discs[x + 2, y] != null &&
                            Discs[x, y].Color == color && AvailablePlaceY(x + 1) == y && Discs[x + 2, y].Color == color)
                        {
                            moves[0, x + 1]++;
                            if (y > moves[1, x + 1])
                            {
                                moves[1, x + 1] = y;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 1, y] != null && Discs[x + 2, y] == null &&
                            Discs[x, y].Color == color && Discs[x + 1, y].Color == color && AvailablePlaceY(x + 2) == y)
                        {
                            moves[0, x + 2]++;
                            if (y > moves[1, x + 2])
                            {
                                moves[1, x + 2] = y;
                            }
                        }
                    }
                    else if (Discs[x + 2, y] == null && AvailablePlaceY(x + 2) > y)
                    {
                        if (Discs[x, y] == null && Discs[x + 1, y] != null && Discs[x + 3, y] != null &&
                            AvailablePlaceY(x) == y && Discs[x + 1, y].Color == color && Discs[x + 3, y].Color == color)
                        {
                            moves[0, x]++;
                            if (y > moves[1, x])
                            {
                                moves[1, x] = y;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 1, y] == null && Discs[x + 3, y] != null &&
                            Discs[x, y].Color == color && AvailablePlaceY(x + 1) == y && Discs[x + 3, y].Color == color)
                        {
                            moves[0, x + 1]++;
                            if (y > moves[1, x + 1])
                            {
                                moves[1, x + 1] = y;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 1, y] != null && Discs[x + 3, y] == null &&
                            Discs[x, y].Color == color && Discs[x + 1, y].Color == color && AvailablePlaceY(x + 3) == y)
                        {
                            moves[0, x + 3]++;
                            if (y > moves[1, x + 3])
                            {
                                moves[1, x + 3] = y;
                            }
                        }
                    }
                    else if (Discs[x + 1, y] == null && AvailablePlaceY(x + 1) > y)
                    {
                        if (Discs[x, y] == null && Discs[x + 2, y] != null && Discs[x + 3, y] != null &&
                            AvailablePlaceY(x) == y && Discs[x + 2, y].Color == color && Discs[x + 3, y].Color == color)
                        {
                            moves[0, x]++;
                            if (y > moves[1, x])
                            {
                                moves[1, x] = y;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 2, y] == null && Discs[x + 3, y] != null &&
                            Discs[x, y].Color == color && AvailablePlaceY(x + 2) == y && Discs[x + 3, y].Color == color)
                        {
                            moves[0, x + 2]++;
                            if (y > moves[1, x + 2])
                            {
                                moves[1, x + 2] = y;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 2, y] != null && Discs[x + 3, y] == null &&
                            Discs[x, y].Color == color && Discs[x + 2, y].Color == color && AvailablePlaceY(x + 3) == y)
                        {
                            moves[0, x + 3]++;
                            if (y > moves[1, x + 3])
                            {
                                moves[1, x + 3] = y;
                            }
                        }
                    }
                    else if (Discs[x, y] == null && AvailablePlaceY(x) > y)
                    {
                        if (Discs[x + 3, y] == null && Discs[x + 1, y] != null && Discs[x + 2, y] != null &&
                            AvailablePlaceY(x + 3) == y && Discs[x + 1, y].Color == color && Discs[x + 2, y].Color == color)
                        {
                            moves[0, x + 3]++;
                            if (y > moves[1, x + 3])
                            {
                                moves[1, x + 3] = y;
                            }
                        }
                        if (Discs[x + 3, y] != null && Discs[x + 1, y] == null && Discs[x + 2, y] != null &&
                            Discs[x + 3, y].Color == color && AvailablePlaceY(x + 1) == y && Discs[x + 2, y].Color == color)
                        {
                            moves[0, x + 1]++;
                            if (y > moves[1, x + 1])
                            {
                                moves[1, x + 1] = y;
                            }
                        }
                        if (Discs[x + 3, y] != null && Discs[x + 1, y] != null && Discs[x + 2, y] == null &&
                            Discs[x + 3, y].Color == color && Discs[x + 1, y].Color == color && AvailablePlaceY(x + 2) == y)
                        {
                            moves[0, x + 2]++;
                            if (y > moves[1, x + 2])
                            {
                                moves[1, x + 2] = y;
                            }
                        }
                    }
                }
            }

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (Discs[x + 3, y + 3] == null && AvailablePlaceY(x + 3) > y + 3)
                    {
                        if (Discs[x, y] == null && Discs[x + 1, y + 1] != null && Discs[x + 2, y + 2] != null &&
                            AvailablePlaceY(x) == y && Discs[x + 1, y + 1].Color == color && Discs[x + 2, y + 2].Color == color)
                        {
                            moves[0, x]++;
                            if (y > moves[1, x])
                            {
                                moves[1, x] = y + 3;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 1, y + 1] == null && Discs[x + 2, y + 2] != null &&
                            Discs[x, y].Color == color && AvailablePlaceY(x + 1) == y + 1 && Discs[x + 2, y + 2].Color == color)
                        {
                            moves[0, x + 1]++;
                            if (y > moves[1, x + 1])
                            {
                                moves[1, x + 1] = y + 3;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 1, y + 1] != null && Discs[x + 2, y + 2] == null &&
                            Discs[x, y].Color == color && Discs[x + 1, y + 1].Color == color && AvailablePlaceY(x + 2) == y + 2)
                        {
                            moves[0, x + 2]++;
                            if (y > moves[1, x + 2])
                            {
                                moves[1, x + 2] = y + 3;
                            }
                        }
                    }
                    else if (Discs[x + 2, y + 2] == null && AvailablePlaceY(x + 2) > y + 2)
                    {
                        if (Discs[x, y] == null && Discs[x + 1, y + 1] != null && Discs[x + 3, y + 3] != null &&
                            AvailablePlaceY(x) == y && Discs[x + 1, y + 1].Color == color && Discs[x + 3, y + 3].Color == color)
                        {
                            moves[0, x]++;
                            if (y > moves[1, x])
                            {
                                moves[1, x] = y + 2;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 1, y + 1] == null && Discs[x + 3, y + 3] != null &&
                            Discs[x, y].Color == color && AvailablePlaceY(x + 1) == y + 1 && Discs[x + 3, y + 3].Color == color)
                        {
                            moves[0, x + 1]++;
                            if (y > moves[1, x + 1])
                            {
                                moves[1, x + 1] = y + 2;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 1, y + 1] != null && Discs[x + 3, y + 3] == null &&
                            Discs[x, y].Color == color && Discs[x + 1, y + 1].Color == color && AvailablePlaceY(x + 3) == y + 3)
                        {
                            moves[0, x + 3]++;
                            if (y > moves[1, x + 3])
                            {
                                moves[1, x + 3] = y + 2;
                            }
                        }
                    }
                    else if (Discs[x + 1, y + 1] == null && AvailablePlaceY(x + 1) > y + 1)
                    {
                        if (Discs[x, y] == null && Discs[x + 2, y + 2] != null && Discs[x + 3, y + 3] != null &&
                            AvailablePlaceY(x) == y && Discs[x + 2, y + 2].Color == color && Discs[x + 3, y + 3].Color == color)
                        {
                            moves[0, x]++;
                            if (y > moves[1, x])
                            {
                                moves[1, x] = y + 1;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 2, y + 2] == null && Discs[x + 3, y + 3] != null &&
                            Discs[x, y].Color == color && AvailablePlaceY(x + 2) == y + 2 && Discs[x + 3, y + 3].Color == color)
                        {
                            moves[0, x + 2]++;
                            if (y > moves[1, x + 2])
                            {
                                moves[1, x + 2] = y + 1;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x + 2, y + 2] != null && Discs[x + 3, y + 3] == null &&
                            Discs[x, y].Color == color && Discs[x + 2, y + 2].Color == color && AvailablePlaceY(x + 3) == y + 3)
                        {
                            moves[0, x + 3]++;
                            if (y > moves[1, x + 3])
                            {
                                moves[1, x + 3] = y + 1;
                            }
                        }
                    }
                    else if (Discs[x, y] == null && AvailablePlaceY(x) > y)
                    {
                        if (Discs[x + 3, y + 3] == null && Discs[x + 1, y + 1] != null && Discs[x + 2, y + 2] != null &&
                            AvailablePlaceY(x + 3) == y + 3 && Discs[x + 1, y + 1].Color == color && Discs[x + 2, y + 2].Color == color)
                        {
                            moves[0, x + 3]++;
                            if (y > moves[1, x + 3])
                            {
                                moves[1, x + 3] = y;
                            }
                        }
                        if (Discs[x + 3, y + 3] != null && Discs[x + 1, y + 1] == null && Discs[x + 2, y + 2] != null &&
                            Discs[x + 3, y + 3].Color == color && AvailablePlaceY(x + 1) == y + 1 && Discs[x + 2, y + 2].Color == color)
                        {
                            moves[0, x + 1]++;
                            if (y > moves[1, x + 1])
                            {
                                moves[1, x + 1] = y;
                            }
                        }
                        if (Discs[x + 3, y + 3] != null && Discs[x + 1, y + 1] != null && Discs[x + 2, y + 2] == null &&
                            Discs[x + 3, y + 3].Color == color && Discs[x + 1, y + 1].Color == color && AvailablePlaceY(x + 2) == y + 2)
                        {
                            moves[0, x + 2]++;
                            if (y > moves[1, x + 2])
                            {
                                moves[1, x + 2] = y;
                            }
                        }
                    }
                }
                for (int x = 3; x < 7; x++)
                {
                    if (Discs[x - 3, y + 3] == null && AvailablePlaceY(x - 3) > y + 3)
                    {
                        if (Discs[x, y] == null && Discs[x - 1, y + 1] != null && Discs[x - 2, y + 2] != null &&
                            AvailablePlaceY(x) == y && Discs[x - 1, y + 1].Color == color && Discs[x - 2, y + 2].Color == color)
                        {
                            moves[0, x]++;
                            if (y > moves[1, x])
                            {
                                moves[1, x] = y + 3;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x - 1, y + 1] == null && Discs[x - 2, y + 2] != null &&
                            Discs[x, y].Color == color && AvailablePlaceY(x - 1) == y + 1 && Discs[x - 2, y + 2].Color == color)
                        {
                            moves[0, x - 1]++;
                            if (y > moves[1, x - 1])
                            {
                                moves[1, x - 1] = y + 3;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x - 1, y + 1] != null && Discs[x - 2, y + 2] == null &&
                            Discs[x, y].Color == color && Discs[x - 1, y + 1].Color == color && AvailablePlaceY(x - 2) == y + 2)
                        {
                            moves[0, x - 2]++;
                            if (y > moves[1, x - 2])
                            {
                                moves[1, x - 2] = y + 3;
                            }
                        }
                    }
                    else if (Discs[x - 2, y + 2] == null && AvailablePlaceY(x - 2) > y + 2)
                    {
                        if (Discs[x, y] == null && Discs[x - 1, y + 1] != null && Discs[x - 3, y + 3] != null &&
                            AvailablePlaceY(x) == y && Discs[x - 1, y + 1].Color == color && Discs[x - 3, y + 3].Color == color)
                        {
                            moves[0, x]++;
                            if (y > moves[1, x])
                            {
                                moves[1, x] = y + 2;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x - 1, y + 1] == null && Discs[x - 3, y + 3] != null &&
                            Discs[x, y].Color == color && AvailablePlaceY(x - 1) == y + 1 && Discs[x - 3, y + 3].Color == color)
                        {
                            moves[0, x - 1]++;
                            if (y > moves[1, x - 1])
                            {
                                moves[1, x - 1] = y + 2;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x - 1, y + 1] != null && Discs[x - 3, y + 3] == null &&
                            Discs[x, y].Color == color && Discs[x - 1, y + 1].Color == color && AvailablePlaceY(x - 3) == y + 3)
                        {
                            moves[0, x - 3]++;
                            if (y > moves[1, x - 3])
                            {
                                moves[1, x - 3] = y + 2;
                            }
                        }
                    }
                    else if (Discs[x - 1, y + 1] == null && AvailablePlaceY(x - 1) > y + 1)
                    {
                        if (Discs[x, y] == null && Discs[x - 2, y + 2] != null && Discs[x - 3, y + 3] != null &&
                            AvailablePlaceY(x) == y && Discs[x - 2, y + 2].Color == color && Discs[x - 3, y + 3].Color == color)
                        {
                            moves[0, x]++;
                            if (y > moves[1, x])
                            {
                                moves[1, x] = y + 1;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x - 2, y + 2] == null && Discs[x - 3, y + 3] != null &&
                            Discs[x, y].Color == color && AvailablePlaceY(x - 2) == y + 2 && Discs[x - 3, y + 3].Color == color)
                        {
                            moves[0, x - 2]++;
                            if (y > moves[1, x - 2])
                            {
                                moves[1, x - 2] = y + 1;
                            }
                        }
                        if (Discs[x, y] != null && Discs[x - 2, y + 2] != null && Discs[x - 3, y + 3] == null &&
                            Discs[x, y].Color == color && Discs[x - 2, y + 2].Color == color && AvailablePlaceY(x - 3) == y + 3)
                        {
                            moves[0, x - 3]++;
                            if (y > moves[1, x - 3])
                            {
                                moves[1, x - 3] = y + 1;
                            }
                        }
                    }
                    else if (Discs[x, y] == null && AvailablePlaceY(x) > y)
                    {
                        if (Discs[x - 3, y + 3] == null && Discs[x - 1, y + 1] != null && Discs[x - 2, y + 2] != null &&
                            AvailablePlaceY(x - 3) == y + 3 && Discs[x - 1, y + 1].Color == color && Discs[x - 2, y + 2].Color == color)
                        {
                            moves[0, x - 3]++;
                            if (y > moves[1, x - 3])
                            {
                                moves[1, x - 3] = y;
                            }
                        }
                        if (Discs[x - 3, y + 3] != null && Discs[x - 1, y + 1] == null && Discs[x - 2, y + 2] != null &&
                            Discs[x - 3, y + 3].Color == color && AvailablePlaceY(x - 1) == y + 1 && Discs[x - 2, y + 2].Color == color)
                        {
                            moves[0, x - 1]++;
                            if (y > moves[1, x - 1])
                            {
                                moves[1, x - 1] = y;
                            }
                        }
                        if (Discs[x - 3, y + 3] != null && Discs[x - 1, y + 1] != null && Discs[x - 2, y + 2] == null &&
                            Discs[x - 3, y + 3].Color == color && Discs[x - 1, y + 1].Color == color && AvailablePlaceY(x - 2) == y + 2)
                        {
                            moves[0, x - 2]++;
                            if (y > moves[1, x - 2])
                            {
                                moves[1, x - 2] = y;
                            }
                        }
                    }
                }
            }
            return moves;
        }

        public ConsoleColor CheckStatus() // check horizontal, vertical, and diagonal connections, return winner's disc color
        {
            for (int y = 0; y < 6; y++) // check horizontal
            {
                for (int x = 0; x < 4; x++)
                {
                    if (Discs[x, y] != null && Discs[x + 1, y] != null && Discs[x + 2, y] != null && Discs[x + 3, y] != null &&
                        Discs[x, y].Color == Discs[x + 1, y].Color && Discs[x, y].Color == Discs[x + 2, y].Color && Discs[x, y].Color == Discs[x + 3, y].Color)
                    {
                        return Discs[x, y].Color;
                    }
                }
            }

            for (int y = 3; y < 6; y++) // check vertical
            {
                for (int x = 0; x < 7; x++)
                {
                    if (Discs[x, y] != null && Discs[x, y - 1] != null && Discs[x, y - 2] != null && Discs[x, y - 3] != null &&
                        Discs[x, y].Color == Discs[x, y - 1].Color && Discs[x, y].Color == Discs[x, y - 2].Color && Discs[x, y].Color == Discs[x, y - 3].Color)
                    {
                        return Discs[x, y].Color;
                    }
                }
            }

            for (int y = 3; y < 6; y++) // check diagonal
            {
                for (int x = 0; x < 4; x++)
                {
                    if (Discs[x, y] != null && Discs[x + 1, y - 1] != null && Discs[x + 2, y - 2] != null && Discs[x + 3, y - 3] != null &&
                        Discs[x, y].Color == Discs[x + 1, y - 1].Color && Discs[x, y].Color == Discs[x + 2, y - 2].Color && Discs[x, y].Color == Discs[x + 3, y - 3].Color)
                    {
                        return Discs[x, y].Color;
                    }
                }
                for (int x = 3; x < 7; x++)
                {
                    if (Discs[x, y] != null && Discs[x - 1, y - 1] != null && Discs[x - 2, y - 2] != null && Discs[x - 3, y - 3] != null &&
                        Discs[x, y].Color == Discs[x - 1, y - 1].Color && Discs[x, y].Color == Discs[x - 2, y - 2].Color && Discs[x, y].Color == Discs[x - 3, y - 3].Color)
                    {
                        return Discs[x, y].Color;
                    }
                }
            }
            return ConsoleColor.Black; // No winner
        }

        public bool Full()
        {
            return DiscCounter >= 42;
        }

        public void Render(Char[,] displayCache, int offsetX = 0, int offsetY = 0)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            for (int y = 0; y < 27; y++)
            {
                if (y == 0 || y == 26)
                {
                    for (int x = 0; x < 57; x++)
                    {
                        if (x == 0 || x == 56)
                        {
                            if (y == 0)
                            {
                                displayCache[x + offsetX, y + offsetY] = new Char(x == 0 ? "┏" : "┓", Color);
                            }
                            else
                            {
                                displayCache[x + offsetX, y + offsetY] = new Char(x == 0 ? "┗" : "┛", Color);
                            }
                        }
                        else if (x % 8 == 0)
                        {
                            displayCache[x + offsetX, y + offsetY] = new Char(y == 0 ? "┳" : "┻", Color);
                        }
                        else
                        {
                            displayCache[x + offsetX, y + offsetY] = new Char("━", Color);
                        }
                    }
                }
                else if (y % 4 == 0)
                {
                    for (int x = 0; x < 57; x++)
                    {
                        if (x % 8 == 0)
                        {
                            if (x == 0)
                            {
                                displayCache[x + offsetX, y + offsetY] = new Char("┣", Color);
                            }
                            else if (x == 56)
                            {
                                displayCache[x + offsetX, y + offsetY] = new Char("┫", Color);
                            }
                            else
                            {
                                displayCache[x + offsetX, y + offsetY] = new Char("╋", Color);
                            }
                        }
                        else
                        {
                            displayCache[x + offsetX, y + offsetY] = new Char("━", Color);
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < 57; x++)
                    {
                        if (x % 8 == 0)
                        {
                            displayCache[x + offsetX, y + offsetY] = new Char("┃", Color);
                        }
                        else
                        {
                            displayCache[x + offsetX, y + offsetY] = new Char(" ", Color);
                        }
                    }
                }
            }
            for (int i = 1; i < 8; i++)
            {
                displayCache[i * 8 - 4 + offsetX, 25 + offsetY] = new Char(i.ToString(), ConsoleColor.DarkRed);
            }
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Discs[x, y] != null)
                    {
                        Discs[x, y].Render(displayCache, offsetX, offsetY, true);
                    }
                }
            }
        }
 

        public void Clear()
        {
            // reset Table
            Discs = new Disc[Width, Height];
            DiscCounter = 0;
        }
    }
}
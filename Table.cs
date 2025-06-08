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

        public bool PlaceDisc(int x, Disc disc)
        {
            // place a disc
            return false;
        }

        public void RemoveDisc(int x, int y)
        {
            // remove a disc
            Discs[x, y] = null;
        }

        public int AvailablePlaceY(int x)
        {
            // check available x place and return target y
            for (int y = Discs.GetLength(1) - 1; y >= 0; y--)
            {
                if (Discs[x, y] == null)
                {
                    return y;
                }
            }
            return -1;
        }

        public ConsoleColor CheckStatus()
        {
            // Check horizontal, vertical, and diagonal connections
            for (int y = 0; y < 6; y++)
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

            for (int y = 3; y < 6; y++)
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

            for (int y = 3; y < 6; y++)
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

        public void Render(Char[,] displayCache, int offsetX = 0, int offsetY = 0)
        {
            // render Table's Discs into displayCache
        }

        public void Clear()
        {
            // reset Table
            Discs = new Disc[Width, Height];
            DiscCounter = 0;
        }
    }
}
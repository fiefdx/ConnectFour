namespace ConnectFour
{
    public class Disc
    {
        public string[] Symbol { get; private set; } // symbol for rendering as a disc
        public int Width { get; private set; } // symbol width
        public int Height { get; private set; } // symbol height
        public int X { get; private set; } // colum [0, 6]
        public int Y { get; private set; } // row [0, 5]
        public int N { get; private set; } // step number [0, 42]
        public ConsoleColor Color { get; set; } // disc color

        public Disc(int x, int y, string[] symbol, ConsoleColor color, int n = 0)
        {
            X = x;
            Y = y;
            N = n;
            Symbol = symbol;
            Width = Symbol[0].Length;
            Height = Symbol.Length;
            Color = color;
        }

        public void Render(Char[,] displayCache, int offsetX = 0, int offsetY = 0, bool step = false)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Color == ConsoleColor.Red)
                    {
                        displayCache[X * 8 + 1 + x + offsetX, Y * 4 + 1 + y + offsetY] = new Char(Symbol[y][x].ToString(), Color);
                    }
                    else
                    {
                        displayCache[X * 8 + 1 + x + offsetX, Y * 4 + 1 + y + offsetY] = new Char(Symbol[y][x].ToString(), Color);
                    }
                    if (step && y == 1)
                    {
                        if (x == 2)
                        {
                            displayCache[X * 8 + 1 + x + offsetX, Y * 4 + 1 + y + offsetY] = new Char((N / 10).ToString(), ConsoleColor.DarkGreen);
                        }
                        else if (x == 3)
                        {
                            displayCache[X * 8 + 1 + x + offsetX, Y * 4 + 1 + y + offsetY] = new Char(" ", ConsoleColor.DarkGreen);
                        }
                        else if (x == 4)
                        {
                            displayCache[X * 8 + 1 + x + offsetX, Y * 4 + 1 + y + offsetY] = new Char((N % 10).ToString(), ConsoleColor.DarkGreen);
                        }
                    }
                }
            }
        }


        public override string ToString()
        {
            return $"Disc at ({X}, {Y}) with color {Color}";
        }
    }
}
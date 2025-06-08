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

        public void Render(Char[,] displayCache, int offsetX = 0, int offsetY = 0)
        {
            // render disc's Symbol into displayCache with offsetX and offsetY
            
        }

        public override string ToString()
        {
            return $"Disc at ({X}, {Y}) with color {Color}";
        }
    }
}
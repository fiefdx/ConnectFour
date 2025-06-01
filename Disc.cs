namespace ConnectFour
{
    public class Disc
    {
        public string[] Symbol { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public ConsoleColor Color { get; set; }

        public Disc(int x, int y, string[] symbol, ConsoleColor color)
        {
            X = x;
            Y = y;
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
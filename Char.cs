namespace ConnectFour
{
    public class Char
    // Char class
    // for output colorful character in console
    {
        public string C { get; private set; }
        public ConsoleColor Color { get; private set; }

        public Char(string c, ConsoleColor color)
        {
            C = c;
            Color = color;
        }

        public void Render() // render this char
        {
            Console.ForegroundColor = Color;
            Console.Write(C);
        }
    }
}
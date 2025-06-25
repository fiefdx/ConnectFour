using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace ConnectFour
{
    public class Game
    {

        public Table GameTable { get; private set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public Player TurnPlayer { get; private set; }
        public Player Winner { get; private set; }
        public Char[,] DisplayCache { get; private set; }
        public bool Over { get; private set; }

        public Game(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
            TurnPlayer = Player1;
            Winner = null;
            GameTable = new Table(ConsoleColor.Gray);
            DisplayCache = new Char[76, 27];
            Over = false;
        }

        public void NextTurn()
        {
            if (TurnPlayer == Player1)
            {
                TurnPlayer = Player2;
            }
            else
            {
                TurnPlayer = Player1;
            }
        }

        public bool PlaceDisc(int x, Disc disc)
        {
            return GameTable.PlaceDisc(x, disc);
        }

        public void UpdateWin(ConsoleColor color)
        {
            // update game win status
            if (Player1.Disc.Color == color)
            {
                Over = true;
                Winner = Player1;

            }
            else if (Player2.Disc.Color == color)
            {
                Over = true;
                Winner = Player2;
            }
            else
            {
                if (GameTable.Full())
                {
                    Over = true;
                }
            }
        }

        public void PrintTitle()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            string asciiArt = @"         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
         ┃███████  █████  ██   ██ ██   ██ ██▀▀▀▀▀ ███████ ███████┃ 
         ┃██      ██   ██ ███▄ ██ ███▄ ██ ██▄▄    ██        ███  ┃  
         ┃██      ██   ██ ██▀█▄██ ██▀█▄██ ██▀▀    ██        ███  ┃  
         ┃███████  █████  ██  ███ ██  ███ ██▄▄▄▄▄ ███████   ███  ┃ 
         ┣━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┫
         ┃██░░░██┃░█████░┃██░░░██┃ ██  ██┃██░░░██┃░█████░┃██░░░██┃
         ┃░░███░░┃██░░░██┃░░███░░┃██   ██┃░░███░░┃██░░░██┃░░███░░┃
         ┃██░░░██┃░█████░┃██░░░██┃██▄▄▄██┃██░░░██┃░█████░┃██░░░██┃
         ┣━━━━━━━┻━━━━━━━┻━━━━━━━┃     ██┃━━━━━━━┻━━━━━━━┻━━━━━━━┫
         ┃░░░░░░░░░░░░░░░░░░░░░░░┗━━━━━━━┛░░░░░░░░░░░░░░░░░░░░░░░┃";
            Console.WriteLine(asciiArt);
        }

        public void PrintWinnerP1()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string asciiP1 = @"         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
         ┃     ████▄  ██      ████   ▀██  ██▀ ██████  ████▄      ┃ 
         ┃     ██  ██ ██     ██  ██   ▀█▄▄█▀  ██▄▄    ██  ██     ┃  
         ┃     █████  ██     ██████     ██    ██▀▀    ████▀      ┃  
         ┃     ██     ██████ ██  ██     ██    ██████  ██  ██     ┃ 
         ┣━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┫
         ┃██░░░██┃░█████░┃██░░░██┃  ▄██  ┃██░░░██┃░█████░┃██░░░██┃
         ┃░░███░░┃██░░░██┃░░███░░┃   ██  ┃░░███░░┃██░░░██┃░░███░░┃
         ┃██░░░██┃░█████░┃██░░░██┃   ██  ┃██░░░██┃░█████░┃██░░░██┃
         ┣━━━━━━━┻━━━━━━━┻━━━━━━━┃   ██  ┃━━━━━━━┻━━━━━━━┻━━━━━━━┫
         ┃░░░░░░░░░░░░░░░░░░░░░░░┗━━━━━━━┛░░░░░░░░░░░░░░░░░░░░░░░┃";
            Console.WriteLine(asciiP1);
        }
        public void PrintWinnerP2()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            string asciiP1 = @"         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
         ┃     ████▄  ██      ████   ▀██  ██▀ ██████  ████▄      ┃ 
         ┃     ██  ██ ██     ██  ██   ▀█▄▄█▀  ██▄▄    ██  ██     ┃  
         ┃     █████  ██     ██████     ██    ██▀▀    ████▀      ┃  
         ┃     ██     ██████ ██  ██     ██    ██████  ██  ██     ┃ 
         ┣━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┫
         ┃██░░░██┃░█████░┃██░░░██┃▄█████▄┃██░░░██┃░█████░┃██░░░██┃
         ┃░░███░░┃██░░░██┃░░███░░┃     ██┃░░███░░┃██░░░██┃░░███░░┃
         ┃██░░░██┃░█████░┃██░░░██┃▄█▀▀▀▀ ┃██░░░██┃░█████░┃██░░░██┃
         ┣━━━━━━━┻━━━━━━━┻━━━━━━━┃███████┃━━━━━━━┻━━━━━━━┻━━━━━━━┫
         ┃░░░░░░░░░░░░░░░░░░░░░░░┗━━━━━━━┛░░░░░░░░░░░░░░░░░░░░░░░┃";
            Console.WriteLine(asciiP1);
        }

        public void PrintTie()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string asciiP1 = @"         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
         ┃              ████████  ███████   ██████               ┃ 
         ┃              ▀  ██  ▀    ███     ██▄▄                 ┃  
         ┃                 ██       ███     ██▀▀                 ┃  
         ┃                 ██     ███████   ██████               ┃ 
         ┣━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┫
         ┃██░░░██┃░█████░┃██░░░██┃▄█████▄┃██░░░██┃░█████░┃██░░░██┃
         ┃░░███░░┃██░░░██┃░░███░░┃██   ██┃░░███░░┃██░░░██┃░░███░░┃
         ┃██░░░██┃░█████░┃██░░░██┃██   ██┃██░░░██┃░█████░┃██░░░██┃
         ┣━━━━━━━┻━━━━━━━┻━━━━━━━┃▀█████▀┃━━━━━━━┻━━━━━━━┻━━━━━━━┫
         ┃░░░░░░░░░░░░░░░░░░░░░░░┗━━━━━━━┛░░░░░░░░░░░░░░░░░░░░░░░┃";
            Console.WriteLine(asciiP1);
        }

        public void RenderTable()
        {
            // render table into DisplayCache
            GameTable.Render(DisplayCache, 9, 0);
        }

        public void RenderPlayerInfo()
        {
            // render player info into DisplayCache
            for (int i = 0; i < Player1.Type.Length; i++)
            {
                DisplayCache[i + 2 + (5 - Player1.Type.Length) / 2, 9] = new Char(Player1.Type[i].ToString(), Player1.Disc.Color);
            }
            for (int i = 0; i < Player2.Type.Length; i++)
            {
                DisplayCache[i + 69 + (5 - Player2.Type.Length) / 2, 9] = new Char(Player2.Type[i].ToString(), Player2.Disc.Color);
            }
            Player1.Disc.Render(DisplayCache, 0, 10);
            Player2.Disc.Render(DisplayCache, 67, 10);
            int offsetX = 0;
            if (TurnPlayer.Id == 2)
            {
                offsetX = 67;
            }
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (y == 0 || y == 4)
                    {
                        if (x > 0 && x < 8)
                        {
                            DisplayCache[x + offsetX, y + 10] = new Char("━", ConsoleColor.Green);
                        }
                    }
                    else if (x == 0 || x == 8)
                    {
                        DisplayCache[x + offsetX, y + 10] = new Char("┃", ConsoleColor.Green);
                    }
                }
            }
        }

        public void RenderCursor(int cX, int cY, ConsoleColor color, int offsetX = 9, int offsetY = 0)
        {

            // render a cursor into DisplayCache
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (y == 0 || y == 4)
                    {
                        if (x > 0 && x < 8)
                        {
                            DisplayCache[cX * 8 + x + offsetX, cY * 4 + y + offsetY] = new Char("━", color);
                        }
                    }
                    else if (x == 0 || x == 8)
                    {
                        DisplayCache[cX * 8 + x + offsetX, cY * 4 + y + offsetY] = new Char("┃", color);
                    }
                }
            }
        }

        public void RenderPrompts()
        {
            // render some prompts
            if (Over)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(@"         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓ 
         ┃██      Press 'R' to restart or 'Esc' to exit.       ██┃
         ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine(@"         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
         ┃██   Use Arrow keys to move, Enter to place a disc,  ██┃        
         ┃██    or press Number keys[1-7] to place a disc,     ██┃    
         ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫ 
         ┃██       Press 'R' to restart or 'Esc' to exit.      ██┃
         ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (Player1.Type == "CPU")
            {
                string thought = $"   CPU[{Player1.Thought}]";
                Console.WriteLine("         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
                Console.WriteLine($"         ┃██{thought.PadRight(51)}██┃");
                Console.WriteLine("         ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            }
            else if (Player2.Type == "CPU")
            {
                string thought = $"   CPU[{Player2.Thought}]";
                Console.WriteLine("         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
                Console.WriteLine($"         ┃██{thought.PadRight(51)}██┃");
                Console.WriteLine("         ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            }
        }

        public void Render()
        {
            // render display cache
            for (int y = 0; y < DisplayCache.GetLength(1); y++)
            {
                for (int x = 0; x < DisplayCache.GetLength(0); x++)
                {
                    Char c = DisplayCache[x, y];
                    if (c == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.ForegroundColor = c.Color;
                        Console.Write(c.C);
                    }
                }
                Console.WriteLine();
            }
        }

        public void ClearCache()
        {
            // clear DisplayCache
            for (int y = 0; y < DisplayCache.GetLength(1); y++)
            {
                for (int x = 0; x < DisplayCache.GetLength(0); x++)
                {
                    DisplayCache[x, y] = null;
                }
            }
        }

        public bool IsTie()
        {
            return Over && Winner == null;
        }

        public void RenderFrame(int cX, int cY)
        {
            if (IsTie())
            {
                PrintTie();
            }
            else if (Winner == null)
            {
                PrintTitle();
            }
            else
            {
                if (Winner.Id == 2)
                {
                    PrintWinnerP2();
                }
                else
                {
                    PrintWinnerP1();
                }
            }
            
            RenderTable();
            RenderCursor(cX, cY, TurnPlayer.Disc.Color);
            RenderPlayerInfo();
            Render();
            RenderPrompts();
        }

        public void Clear()
        {
            // reset game
            ClearCache();
            GameTable.Clear();
            Over = false;
            Winner = null;
            TurnPlayer = Player1;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            int x = 0, y = 0; // cursor x, y
            Game game; // game variable
            Console.Clear(); // clear console
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            string Menu = @"         ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓
         ┃███████  █████  ██   ██ ██   ██ ██▀▀▀▀▀ ███████ ███████┃ 
         ┃██      ██   ██ ████ ██ ████ ██ ██▄▄    ██        ███  ┃  
         ┃██      ██   ██ ████ ██ ████ ██ ██▀▀    ██        ███  ┃  
         ┃███████  █████  ██  ███ ██  ███ ██▄▄▄▄▄ ███████   ███  ┃ 
         ┣━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┳━━━━━━━┫
         ┃██░░░██┃░█████░┃██░░░██┃ ██  ██┃██░░░██┃░█████░┃██░░░██┃
         ┃░░███░░┃██░░░██┃░░███░░┃██   ██┃░░███░░┃██░░░██┃░░███░░┃
         ┃██░░░██┃░█████░┃██░░░██┃██▄▄▄██┃██░░░██┃░█████░┃██░░░██┃
         ┣━━━━━━━┻━━━━━━━┻━━━━━━━┃     ██┃━━━━━━━┻━━━━━━━┻━━━━━━━┫
         ┃░░░░░░░░░░░░░░░░░░░░░░░┗━━━━━━━┛░░░░░░░░░░░░░░░░░░░░░░░┃
         ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫
         ┃          Click '1' for Single player mode,            ┃
         ┃          other keys for Two players mode.             ┃
         ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛";
            Console.WriteLine(Menu);

        ConsoleKeyInfo keyInfo = Console.ReadKey(true); // keyboard input
            Disc disc1 = new Disc(0, 0, new string[] 
            {
                "██░░░██",
                "░░███░░",
                "██░░░██" }
            , ConsoleColor.Red); // disc for first player
            Disc disc2 = new Disc(0, 0, new string[] 
            {
                "░█████░",
                "██░░░██",
                "░█████░" }
            , ConsoleColor.Blue); // disc for second player
            

            if (keyInfo.Key == ConsoleKey.D1) // Single player mode
            {
                Random random = new Random();
                if (random.Next(0, 2) == 0) // first player is human, second player is cpu
                {
                    game = new Game(new Player(1, disc1), new CPUPlayer(2, disc2));
                }
                else // first player is cpu, second player is human
                {
                    game = new Game(new CPUPlayer(1, disc1), new Player(2, disc2));
                }
            }
            else // Two players mode
            {
                game = new Game(new Player(1, disc1), new Player(2, disc2));
            }

            if (game.TurnPlayer.Type == "CPU")
            {
                // If it's CPU's turn, let it make a move
                int move = game.TurnPlayer.GetMove(game.GameTable);
                if (move >= 0)
                {
                    game.PlaceDisc(move, game.TurnPlayer.Disc);
                    game.NextTurn();
                }
            }
            Console.Clear(); // clear console for new frame output
            game.RenderFrame(x, y); // output new frame

            bool update = false; // need to update flag
            ConsoleKey key; // keyboard input key
            do // keyboard input event loop
            {
                update = false;
                while (!Console.KeyAvailable)
                {
                    // Do something, but don't read key here
                }

                // Key is available - read it
                key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow) // cursor up
                {
                    if (y > 0)
                    {
                        y--;
                        update = true;
                    }
                }
                else if (key == ConsoleKey.LeftArrow) // cursor left
                {
                    if (x > 0)
                    {
                        x--;
                        update = true;
                    }
                }
                else if (key == ConsoleKey.DownArrow) // cursor down
                {
                    if (y < 5)
                    {
                        y++;
                        update = true;
                    }
                }
                else if (key == ConsoleKey.RightArrow) // cursor right
                {
                    if (x < 6)
                    {
                        x++;
                        update = true;
                    }
                }
                else if (key == ConsoleKey.Enter && !game.Over && game.TurnPlayer.Type == "Human")
                {
                    if (game.PlaceDisc(x, game.TurnPlayer.Disc))
                    {
                        update = true;
                        game.NextTurn();
                    }
                }
                else if ((key == ConsoleKey.D1 || key == ConsoleKey.D2 || key == ConsoleKey.D3 ||
                         key == ConsoleKey.D4 || key == ConsoleKey.D5 || key == ConsoleKey.D6 ||
                         key == ConsoleKey.D7) && !game.Over && game.TurnPlayer.Type == "Human")
                {
                    x = key - ConsoleKey.D1; // Convert key to column index (0-6)
                    if (game.PlaceDisc(x, game.TurnPlayer.Disc))
                    {
                        update = true;
                        game.NextTurn();
                    }
                }
                else if (key == ConsoleKey.Escape) // exit the game
                {
                    break;
                }
                else if (key == ConsoleKey.R) // restart a new game
                {
                    x = 0;
                    y = 0;
                    Console.Clear();
                    game.Clear();
                    game.RenderFrame(x, y);
                    update = true;
                }
                if (update)
                {
                    Console.Clear();
                    game.ClearCache();
                    ConsoleColor win = game.GameTable.CheckStatus();
                    game.UpdateWin(win);
                    if (game.TurnPlayer.Type == "CPU" && !game.Over)
                    {
                        // If it's CPU's turn, let it make a move
                        int move = ((CPUPlayer)game.TurnPlayer).GetMove(game.GameTable);
                        if (move >= 0)
                        {
                            update = true;
                            game.PlaceDisc(move, game.TurnPlayer.Disc);
                            game.NextTurn();
                        }
                        win = game.GameTable.CheckStatus();
                        game.UpdateWin(win);
                    }
                    game.RenderFrame(x, y);
                }
            } while (key != ConsoleKey.Escape);
        }
    }
}
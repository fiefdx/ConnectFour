using System.Drawing;
using System.Security.Cryptography;

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
            DisplayCache = new Char[57, 27];
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

        public void RenderTable()
        {
            // render table into DisplayCache
            GameTable.Render(DisplayCache, 6, 0);
        }

        public void RenderPlayerInfo()
        {
            // render player info into DisplayCache
        }

        public void RenderWinStatus(int playerId)
        {
            // render which player win the game or tie
        }

        public void RenderCursor(int cX, int cY, ConsoleColor color, int offsetX = 0, int offsetY = 0)
        {
            // render a cursor into DisplayCache
        }

        public void RenderPrompts()
        {
            // render some prompts
        }

        public void Render()
        {
            // render DisplayCache into console output
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

        public void RenderFrame(int cX, int cY)
        {
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
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            int x = 0, y = 0; // cursor x, y
            Game game; // game variable
            Console.Clear(); // clear console
            Console.ForegroundColor = ConsoleColor.DarkBlue; // console output color
            Console.WriteLine("Welcome to Connect Four!");
            Console.WriteLine("Click '1' for Single player mode, other keys for Two players mode.");

            ConsoleKeyInfo keyInfo = Console.ReadKey(true); // keyboard input
            Disc disc1 = new Disc(0, 0, new string[] { " XXXXX ", "XXXXXXX", " XXXXX " }, ConsoleColor.Red); // disc for first player
            Disc disc2 = new Disc(0, 0, new string[] { " OOOOO ", "OOOOOOO", " OOOOO " }, ConsoleColor.Blue); // disc for second player
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
                else if (key == ConsoleKey.R && game.Over) // restart a new game
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
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFour
{
    public class Player
    // Human Player class
    {
        public string Type { get; private set; } // "Human" or "CPU"
        public int Id { get; private set; } // 1 or 2
        public Disc Disc { get; private set; } // player's disc
        public string Thought { get; set; } // player's thought

        public Player(int id, Disc disc, string type = "Human")
        {
            Id = id;
            Type = type;
            Disc = disc;
        }

        public virtual int GetMove(Table table)
        {
            return -1;
        }
    }

    public class CPUPlayer : Player
    // CPU Player class
    {
        public CPUPlayer(int id, Disc disc) : base(id, disc, "CPU")
        {

        }

        private int AvailableClosePlace(Table table, int x, int y)
        {
            int[] xs = { -1, -1, -1 }; // close place has only 3 possible ys
            int n = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                int dy = table.AvailablePlaceY(i) - y;
                if (dy <= 1 && dy >= -1)
                {
                    xs[n] = i;
                    n++;
                }
            }
            if (n > 0) // has available ys
            {
                Random random = new Random();
                return xs[random.Next(0, n)]; // random choose 1 y
            }
            return -1; // No available close place found
        }

        private int CheckWinMove(Table table) // check whether has a move for win
        {
            return -1;
        }

        private int CheckDefensiveMove(Table table) // check whether has a move for defensive
        {
            return -1;
        }

        private int CheckOffensiveMove(Table table) // check whether has a move for offensive
        {
            return -1;
        }

        private int CheckRandomMove(Table table) // check whether has a random move
        {
            return -1;
        }

        private int GetAIMove(Table table) // AI move
        {
            int x = -1;
            if (table.Full()) // whether the table is full
            {
                x = CheckWinMove(table);
                Thought = "Win: " + (x + 1);
                if (x == -1) // back to defensive move
                {
                    x = CheckDefensiveMove(table);
                    Thought = "Defensive: " + (x + 1);
                    if (x == -1) // back to offensive move
                    {
                        x = CheckOffensiveMove(table);
                        Thought = "Offensive: " + (x + 1);
                        if (x == -1) // back to random move
                        {
                            x = CheckRandomMove(table);
                            Thought = "Random: " + (x + 1);
                        }
                    }
                }
            }
            return x;
        }

        public override int GetMove(Table table)
        {
            if (Id == 1)
            {
                if (table.DiscCounter == 0)
                {
                    Thought = "Initial: 4";
                    return 3; // Always play in the middle column for Player 1 on the first move
                }
                else if (table.DiscCounter == 2)
                {
                    int x = AvailableClosePlace(table, 3, 5);
                    Thought = "Initial: " + (x + 1);
                    return x; // Always play in the second column for Player 1 on the second move
                }
                return GetAIMove(table); // For Player 1, use AI logic after the first two moves
            }
            else if (Id == 2)
            {
                if (table.DiscCounter == 1 || table.DiscCounter == 0)
                {
                    Thought = "Initial: 4";
                    return 3; // Always play in the middle column for Player 2 on the first move
                }
                return GetAIMove(table); // For Player 2, use AI logic after the first two moves
            }
            else
            {
                return GetAIMove(table); // use AI logic
            }
        }
    }
}
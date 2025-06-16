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

        private int SumSubArray(int[,] a, int d) // sum the values in a subarray
        {
            int sum = 0;
            for (int x = 0; x < Table.Width; x++)
            {
                sum += a[d, x];
            }
            return sum;
        }

        private int MaxSubArray(int[,] a, int d) // get the max value in a subarray
        {
            int max = -1;
            for (int x = 0; x < Table.Width; x++)
            {
                if (a[d, x] > max)
                {
                    max = a[d, x];
                }
            }
            return max;
        }

        private int CheckWinMove(Table table) // check whether has a move for win
        {
            int[] moves2 = table.CheckOffensiveMove23(Disc.Color);
            int[] moves3 = table.CheckOffensiveMove34(Disc.Color);
            for (int x = 0; x < Table.Width; x++)
            {
                if (moves2[x] > 0 && moves3[x] > 0)
                {
                    return x;
                }
            }
            for (int x = 0; x < Table.Width; x++)
            {
                if (moves3[x] > 0)
                {
                    return x;
                }
            }
            return -1; // No win move found
        }

        private int CheckDefensiveMove(Table table) // check whether has a move for defensive
        {
            ConsoleColor opponentColor = Id == 1 ? ConsoleColor.Blue : ConsoleColor.Red; // opponent's color
            int[] opponentMoves2 = table.CheckOffensiveMove23(opponentColor);
            int[] opponentMoves3 = table.CheckOffensiveMove34(opponentColor);
            int max = 0, maxX = -1;
            int[] badMove = { 0, 0, 0, 0, 0, 0, 0 };
            int[] lockMoves = table.CheckLockColumnStatus(Disc.Color, Id);
            int[,] opponentLock = table.CheckFormLockMove(opponentColor);
            for (int x = 0; x < Table.Width; x++)
            {
                if (opponentMoves2[x] > 0 && opponentMoves3[x] > 0)
                {
                    return x;
                }
            }
            for (int x = 0; x < Table.Width; x++)
            {
                if (opponentMoves3[x] > 0)
                {
                    return x;
                }
            }
            int maxY = MaxSubArray(opponentLock, 1);
            for (int x = 0; x < Table.Width; x++)
            {
                if (opponentLock[0, x] > 0 && opponentLock[1, x] == maxY)
                {
                    int y = table.AvailablePlaceY(x);
                    if (y >= 0)
                    {
                        Table t = new Table();
                        t.CopyFrom(table);
                        t.PlaceDisc(x, Disc);
                        int[] afterOpponentMoves3 = t.CheckOffensiveMove34(opponentColor);
                        int[] afterOpponentMoves2 = t.CheckOffensiveMove23(opponentColor);
                        int[] afterLockMoves = t.CheckLockColumnStatus(Disc.Color, Id);
                        if (afterOpponentMoves3.Sum() <= opponentMoves3.Sum() && afterOpponentMoves2.Sum() <= opponentMoves2.Sum() && afterLockMoves.Sum() >= lockMoves.Sum())
                        {
                            return x;
                        }
                    }
                }
            }
            for (int x = 0; x < Table.Width; x++)
            {
                if (opponentMoves2[x] > 1)
                {
                    if (opponentMoves2[x] > max)
                    {
                        int y = table.AvailablePlaceY(x);
                        if (y >= 0)
                        {
                            Table t = new Table();
                            t.CopyFrom(table);
                            t.PlaceDisc(x, Disc);
                            int[] afterOpponentMoves3 = t.CheckOffensiveMove34(opponentColor);
                            int[] afterOpponentMoves2 = t.CheckOffensiveMove23(opponentColor);
                            int[] afterLockMoves = t.CheckLockColumnStatus(Disc.Color, Id);
                            if (afterOpponentMoves3.Sum() > opponentMoves3.Sum() || afterOpponentMoves2.Sum() > opponentMoves2.Sum() || afterLockMoves.Sum() < lockMoves.Sum())
                            {
                                badMove[x]++;
                            }
                            else
                            {
                                max = opponentMoves2[x];
                                maxX = x;
                            }
                        }
                    }
                }
            }
            return maxX;
        }

        private int CheckOffensiveMove(Table table) // check whether has a move for offensive
        {
            return -1;
        }

        private int CheckRandomMove(Table table) // check whether has a random move
        {
            ConsoleColor opponentColor = Id == 1 ? ConsoleColor.Blue : ConsoleColor.Red; // opponent's color
            int x = -1;
            int[] opponentMoves3 = table.CheckOffensiveMove34(opponentColor);
            int[] moves2 = table.CheckOffensiveMove23(Disc.Color);
            int[] moves3 = table.CheckOffensiveMove34(Disc.Color);
            int[] lockMoves = table.CheckLockColumnStatus(Disc.Color, Id);
            int[,] formLock = table.CheckFormLockMove(Disc.Color);
            int[] badMove = { 0, 0, 0, 0, 0, 0, 0 };
            int[] goodMove = { 0, 0, 0, 0, 0, 0, 0 };
            for (int rx = 0; rx < 7; rx++)
            {
                int y = table.AvailablePlaceY(rx);
                if (y >= 0)
                {
                    Table t = new Table();
                    t.CopyFrom(table);
                    t.PlaceDisc(rx, Disc);
                    int[] afterOpponentMoves3 = t.CheckOffensiveMove34(opponentColor);
                    int[] afterMoves2 = t.CheckOffensiveMove23(Disc.Color);
                    int[] afterMoves3 = t.CheckOffensiveMove34(Disc.Color);
                    int[] afterLockMoves = t.CheckLockColumnStatus(Disc.Color, Id);
                    int[,] afterFormLock = t.CheckFormLockMove(Disc.Color);
                    if (afterOpponentMoves3.Sum() > opponentMoves3.Sum())
                    {
                        badMove[rx] += 5;
                    }
                    else if (afterLockMoves.Sum() < lockMoves.Sum())
                    {
                        badMove[rx] += 2;
                    }
                    else if (SumSubArray(afterFormLock, 0) > SumSubArray(formLock, 0))
                    {
                        badMove[rx]++;
                    }
                    else
                    {
                        goodMove[rx] += afterMoves2.Sum() - moves2.Sum() + (afterMoves3.Sum() - moves3.Sum()) * 5;
                    }
                }
                else
                {
                    badMove[rx] += 10;
                }
            }
            int maxV = goodMove.Max();
            if (maxV > 0)
            {
                x = Array.IndexOf(goodMove, maxV);
            }
            else
            {
                int minV = badMove.Min();
                x = Array.IndexOf(badMove, minV);
            }
            return x;
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
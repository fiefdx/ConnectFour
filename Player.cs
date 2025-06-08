using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFour
{
    public class Player
    // Human Player class
    {
        public string Type { get; private set; } // "Human" or "CPU"
        public int Id { get; private set; } // 1 or 2
        public Disc Disc { get; private set; } // player's disc

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

        public override int GetMove(Table table)
        {
            return -1;
        }
    }
}
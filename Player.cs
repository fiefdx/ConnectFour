using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFour
{
    public class Player
    // Human Player class
    {
        public string Type { get; private set; } // "Human" or "CPU"
        public int Id { get; private set; } // 1 or 2
        public ConsoleColor Color { get; set; }

        public Player(int id, ConsoleColor color, string type = "Human")
        {
            Id = id;
            Type = type;
            Color = color;
        }

        public virtual int GetMove(Table table)
        {
            return -1;
        }
    }

    public class CPUPlayer : Player
    // CPU Player class
    {
        public CPUPlayer(int id, ConsoleColor color) : base(id, color, "CPU")
        {

        }

        public override int GetMove(Table table)
        {
            return -1;
        }
    }
}
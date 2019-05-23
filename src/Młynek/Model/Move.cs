using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Młynek.Model
{
    struct Move
    {
        public int From { get; set; }
        public int To { get; set; }
        public long Time { get; set; }
        public long NodesVisited { get; set; }
        public FieldState Player { get; set; }
        public bool CreatesMill { get; set; }
        public int Capture { get; set; }

        public Move(FieldState player, int from = -1, int to = -1, long time = 0, long nodes = 0, int capture = -1, bool createsMill = false)
        {
            Player = player;
            From = from;
            To = to;
            Time = time;
            NodesVisited = nodes;
            Capture = capture;
            CreatesMill = createsMill;
        }

        public bool IsValid()
        {
            return (From != -1 || To != -1) && Player != FieldState.Empty;
        }

        public Move Inverse()
        {
            return new Move(player: Player, to: From, from: To);
        }

        public bool Equals(Move other)
        {
            return To == other.To && From == other.From && Player == other.Player;
        }
    }
}

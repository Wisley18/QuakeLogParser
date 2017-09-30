using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogParser.Models
{
    public class Game
    {
        public string Name { get; set; }
        public int TotalKills { get; set; }
        public int KillsByWorld { get; set; }
        public List<Player> Players { get; set; }
        
    }
}

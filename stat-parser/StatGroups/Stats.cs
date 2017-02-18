using stat_parser.PlayerStatObjects;
using System.Collections.Generic;

namespace stat_parser.StatGroups
{
    public class Stats
    {
        public List<PlayerStats> Players { get; set; }

        public Stats()
        {
            Players = new List<PlayerStats>();
        }
    }
}

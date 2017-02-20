using stat_parser.PlayerStatObjects;
using System.Collections.Generic;

namespace stat_parser.StatGroups
{
    public class QuadStats
    {
        public List<PlayerQuadStats> Players { get; set; }

        public QuadStats()
        {
            Players = new List<PlayerQuadStats>();
        }
    }
}

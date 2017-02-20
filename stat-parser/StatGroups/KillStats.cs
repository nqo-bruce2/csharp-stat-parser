using stat_parser.PlayerStatObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stat_parser.StatGroups
{
    public class KillStats
    {
        public List<PlayerKillStats> Players { get; set; }

        public KillStats()
        {
            Players = new List<PlayerKillStats>();
        }
    }
}

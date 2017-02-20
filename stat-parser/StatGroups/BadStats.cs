using stat_parser.PlayerStatObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stat_parser.StatGroups
{
    public class BadStats
    {
        public List<PlayerBadStats> Players { get; set; }

        public BadStats()
        {
            Players = new List<PlayerBadStats>();
        }
    }
}

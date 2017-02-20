using stat_parser.PlayerStatObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stat_parser.StatGroups
{
    public class EfficiencyStats
    {
        public List<PlayerEfficiencyStats> Players { get; set; }

        public EfficiencyStats()
        {
            Players = new List<PlayerEfficiencyStats>();
        }
    }
}

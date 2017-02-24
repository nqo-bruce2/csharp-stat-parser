using stat_parser.StatGroups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stat_parser
{
    public class Statistics
    {
        public string MatchId { get; set; }
        public string MatchType { get; set; }
        public string MapName { get; set; }
        public int PlayerCount { get; set; }
        public Stats Stats { get; set; }
        public QuadStats QStats { get; set; }
        public BadStats BadStats { get; set; }
        public EfficiencyStats EfficiencyStats { get; set; }
        public KillStats KStats { get; set; }
    }
}

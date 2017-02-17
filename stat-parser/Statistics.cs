using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication3.StatGroups;

namespace ConsoleApplication3
{
    public class Statistics
    {
        public string MatchId { get; set; }
        public string MatchType { get; set; }
        public int PlayerCount { get; set; }
        public Stats Stats { get; set; }
        public QuadStats QStats { get; set; }
        public BadStats BadStats { get; set; }
        public EfficiencyStats EfficiencyStats { get; set; }
        public KillStats KStats { get; set; }
    }
}

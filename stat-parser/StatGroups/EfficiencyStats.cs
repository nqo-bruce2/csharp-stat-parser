using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication3.PlayerStatObjects;

namespace ConsoleApplication3.StatGroups
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication3.PlayerStatObjects;

namespace ConsoleApplication3.StatGroups
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

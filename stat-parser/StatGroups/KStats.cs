using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication3.PlayerStatObjects;

namespace ConsoleApplication3.StatGroups
{
    public class KStats
    {
        public List<PlayerKStats> Players { get; set; }

        public KStats()
        {
            Players = new List<PlayerKStats>();
        }
    }
}

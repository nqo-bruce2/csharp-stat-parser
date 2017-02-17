using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication3.PlayerStatObjects;

namespace ConsoleApplication3.StatGroups
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

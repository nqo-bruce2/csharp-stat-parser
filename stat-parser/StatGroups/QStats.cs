using System.Collections.Generic;
using ConsoleApplication3.PlayerStatObjects;

namespace ConsoleApplication3.StatGroups
{
    public class QStats
    {
        public List<PlayerQuadStats> Players { get; set; }

        public QStats()
        {
            Players = new List<PlayerQuadStats>();
        }
    }
}

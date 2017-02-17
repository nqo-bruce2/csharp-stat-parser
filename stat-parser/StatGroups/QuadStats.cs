using System.Collections.Generic;
using ConsoleApplication3.PlayerStatObjects;

namespace ConsoleApplication3.StatGroups
{
    public class QuadStats
    {
        public List<PlayerQuadStats> Players { get; set; }

        public QuadStats()
        {
            Players = new List<PlayerQuadStats>();
        }
    }
}

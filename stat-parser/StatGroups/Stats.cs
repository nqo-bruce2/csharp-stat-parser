using System.Collections.Generic;

namespace ConsoleApplication3.StatGroups
{
    public class Stats
    {
        public List<PlayerStats> Players { get; set; }

        public Stats()
        {
            Players = new List<PlayerStats>();
        }
    }
}

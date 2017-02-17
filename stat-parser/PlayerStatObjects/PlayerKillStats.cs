using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3.PlayerStatObjects
{
    public class PlayerKillStats
    {
        public string Name { get; set; }
        public int Frag_Count { get; set; }
        public int Enemy_Kill_Count { get; set; }
        public int Self_Kill_Count { get; set; }
        public int Team_Kill_Count { get; set; }
        public int Killed_Count { get; set; }
    }
}

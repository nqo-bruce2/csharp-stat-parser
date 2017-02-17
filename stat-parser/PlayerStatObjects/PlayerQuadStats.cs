using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3.PlayerStatObjects
{
    public class PlayerQuadStats
    {
        public string Name { get; set; }
        public int Quads { get; set; }
        public string Quad_Eff { get; set; }
        public int Quad_Enemy_Kills { get; set; }
        public int Quad_Self_Kills { get; set; }
        public int Quad_Team_Kills { get; set; }
    }
}

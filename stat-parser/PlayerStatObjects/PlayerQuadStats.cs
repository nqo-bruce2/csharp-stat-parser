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
        public string Q_eff { get; set; }
        public int Q_E_K { get; set; }
        public int Q_Self_K { get; set; }
        public int Q_Team_K { get; set; }
    }
}

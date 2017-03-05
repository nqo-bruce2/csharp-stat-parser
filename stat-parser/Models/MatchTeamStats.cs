using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MatchTeamStats
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public string TeamColor { get; set; }
        public string TeamVerdict { get; set; }
        public int TeamTotalFrags { get; set; }
        public int TeamTotalQuads { get; set; }
        public int TeamTotalPents { get; set; }
        public int TeamTotalEyes { get; set; }
        public int TeamTotalRL { get; set; }
        public int TeamTotalLG { get; set; }
        public int TeamTotalGL { get; set; }
        public int TeamTotalSNG { get; set; }
        public int TeamTotalNG { get; set; }
        public int TeamTotalMH { get; set; }
        public int TeamTotalRA { get; set; }
        public int TeamTotalYA { get; set; }
        public int TeamTotalGA { get; set; }
        public string TeamPlusRLPack { get; set; }
        public string TeamMinusRLPack { get; set; }
        public decimal? TeamControlPercentage { get; set; }
    }
}

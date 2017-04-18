using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Match
    {
        public int Id { get; set; }
        [Index(IsUnique = true)]
        [StringLength(200)]
        public string MatchId { get; set; }
        public string MatchType { get; set; }
        public string MapName { get; set; }
        public int MatchTimeLimit { get; set; }
        public DateTime Date { get; set; }
        public string MatchText { get; set; }
    }
}

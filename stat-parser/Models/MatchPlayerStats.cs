using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class MatchPlayerStats
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public int Ping { get; set; }
        public string IpAddress { get; set; }
        // begin skill stats
        public string TeamColor { get; set; }
        public decimal? KillEfficiency { get; set; }
        public decimal? WeaponEfficiency { get; set; }
        // begin quad stats
        public int NumberOfQuads { get; set; }
        public decimal? QuadEfficiency { get; set; }
        public int NumQuadEnemyKills { get; set; }
        public int NumQuadSelfKills { get; set; }
        public int NumQuadTeamKills { get; set; }
        // begin kill stats
        public int NumOfFrags { get; set; }
        public int NumOfEnemyKills { get; set; }
        public int NumOfSelfKills { get; set; }
        public int NumOfTeamKills { get; set; }
        public int NumOfDeaths { get; set; }
        // begin efficiency stats
        public decimal? BulletEfficiency { get; set; }
        public decimal? NailsEfficiency { get; set; }
        public decimal? RocketEfficiency { get; set; }
        public decimal? LightningEfficiency { get; set; }
        public decimal? TotalEfficiency { get; set; }
        // begin bad stats
        public int DroppedPaks { get; set; }
        public decimal? SelfDamage { get; set; }
        public decimal? TeamDamage { get; set; }
    }
}

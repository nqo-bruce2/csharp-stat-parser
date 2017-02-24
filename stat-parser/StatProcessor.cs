using Integration;
using Models;
using Newtonsoft.Json;
using NLog;
using stat_parser.PlayerStatObjects;
using stat_parser.StatGroups;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace stat_parser
{
    public class StatProcessor
    {
        private static Statistics MatchStatistics;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string ProcessedDirectory = ConfigurationManager.AppSettings["ProcessedDirectory"];
        private static readonly string ErrorDirectory = ConfigurationManager.AppSettings["ErrorDirectory"];
        private const int MATCH_STATS_INDEX = 2;
        private const int QUAD_STATS_INDEX = 3;
        private const int BAD_STATS_INDEX = 4;
        private const int EFFICIENCY_STATS_INDEX = 5;
        private const int KILL_STATS_INDEX = 6;

        public void StartParsing(string fileName, string fileToParse)
        {
            try
            {
                logger.Info("Starting Parsing........");
                MatchStatistics = new Statistics();
                MatchStatistics.MatchId = fileName;                
                var text = File.ReadAllText(fileToParse);
                // persist match and log contents before parsing as a backup of raw data
                SaveMatchBeforeParsing(fileName, text);

                string pattern = "\r\n\r\n";
                string[] substrings = Regex.Split(text, pattern, RegexOptions.Singleline);
                MatchStatistics.MapName = "temp";
                ProcessStats(substrings[MATCH_STATS_INDEX]);
                ProcessQStats(substrings[QUAD_STATS_INDEX]);
                ProcessBadStats(substrings[BAD_STATS_INDEX]);
                ProcessEfficiencyStats(substrings[EFFICIENCY_STATS_INDEX]);
                ProcessKillStats(substrings[KILL_STATS_INDEX]);

                //Do persistence
                PersistMatchData(MatchStatistics);
                //Move file
                File.Move(fileToParse, ProcessedDirectory + "\\" + fileName);

            }
            catch (Exception ex)
            {
                logger.Error("Exception during processing: " + ex.ToString());
                File.Move(fileToParse, ErrorDirectory + "\\" + fileName);
            }
        }

        private void PersistMatchData(Statistics matchStatistics)
        {
            logger.Info("Saving all data to database");
            using (var db = new StatsDbContext())
            {
                // update match details
                var match = db.Match
                    .Where(b => b.MatchId == MatchStatistics.MatchId)
                    .FirstOrDefault();
                match.MatchType = matchStatistics.MatchType;
                match.MapName = matchStatistics.MapName;
                db.SaveChanges();

                // create player
                foreach (var p in matchStatistics.Stats.Players)
                {
                    var playerId = 0;
                    var playa = db.Player
                        .Where(x => x.Name == p.Name)
                        .FirstOrDefault();
                    if (playa == null)
                    {
                        Random rnd = new Random();
                        var newPlayer = new Player() { Name = p.Name, IpAddress = "0.0.0." + rnd.Next(0,250), Password = "pw", UserName = "uname" };
                        db.Player.Add(newPlayer);
                        db.SaveChanges();
                        playerId = newPlayer.Id;
                    }

                    var mpStats = new MatchPlayerStats();
                    mpStats.MatchId = match.Id;
                    mpStats.PlayerId = playerId;
                    // populate team stats
                    mpStats.TeamColor = matchStatistics.Stats.Players.Single(x => x.Name == p.Name).Team;
                    mpStats.KillEfficiency = matchStatistics.Stats.Players.Single(x => x.Name == p.Name).Kill_Eff;
                    mpStats.WeaponEfficiency = matchStatistics.Stats.Players.Single(x => x.Name == p.Name).Weapon_Eff;
                    // populate quad stats
                    mpStats.NumberOfQuads = matchStatistics.QStats.Players.Single(x => x.Name == p.Name).Quads;
                    mpStats.QuadEfficiency = matchStatistics.QStats.Players.Single(x => x.Name == p.Name).Quad_Eff;
                    mpStats.NumQuadEnemyKills = matchStatistics.QStats.Players.Single(x => x.Name == p.Name).Quad_Enemy_Kills;
                    mpStats.NumQuadSelfKills = matchStatistics.QStats.Players.Single(x => x.Name == p.Name).Quad_Self_Kills;
                    mpStats.NumQuadTeamKills = matchStatistics.QStats.Players.Single(x => x.Name == p.Name).Quad_Team_Kills;
                    // populate kill stats
                    mpStats.NumOfFrags = matchStatistics.KStats.Players.Single(x => x.Name == p.Name).Frag_Count;
                    mpStats.NumOfEnemyKills = matchStatistics.KStats.Players.Single(x => x.Name == p.Name).Enemy_Kill_Count;
                    mpStats.NumOfSelfKills = matchStatistics.KStats.Players.Single(x => x.Name == p.Name).Self_Kill_Count;
                    mpStats.NumOfTeamKills = matchStatistics.KStats.Players.Single(x => x.Name == p.Name).Team_Kill_Count;
                    mpStats.NumOfDeaths = matchStatistics.KStats.Players.Single(x => x.Name == p.Name).Killed_Count;
                    // populate efficiency stats
                    mpStats.BulletEfficiency = matchStatistics.EfficiencyStats.Players.Single(x => x.Name == p.Name).Bullet_Eff;
                    mpStats.NailsEfficiency = matchStatistics.EfficiencyStats.Players.Single(x => x.Name == p.Name).Nails_Eff;
                    mpStats.RocketEfficiency = matchStatistics.EfficiencyStats.Players.Single(x => x.Name == p.Name).Rocket_Eff;
                    mpStats.LightningEfficiency = matchStatistics.EfficiencyStats.Players.Single(x => x.Name == p.Name).Lightning_Eff;
                    mpStats.TotalEfficiency = matchStatistics.EfficiencyStats.Players.Single(x => x.Name == p.Name).Total_Eff;
                    // populate bad stats
                    mpStats.DroppedPaks = matchStatistics.BadStats.Players.Single(x => x.Name == p.Name).Dropped_Paks;
                    mpStats.SelfDamage = matchStatistics.BadStats.Players.Single(x => x.Name == p.Name).Self_Damage;
                    mpStats.TeamDamage = matchStatistics.BadStats.Players.Single(x => x.Name == p.Name).Team_Damage;

                    db.MatchPlayerStats.Add(mpStats);
                    db.SaveChanges();
                }
            }
        }

        private void SaveMatchBeforeParsing(string fileName, string text)
        {
            logger.Info("Saving file before parsing");
            using (var db = new StatsDbContext())
            {
                db.Match.Add(new Models.Match() { MatchId = fileName, MatchType = "", MapName = "", MatchText = text, Date = DateTime.Now });
                db.SaveChanges();
            }
            
        }

        public static void ProcessStats(String s)
        {
            MatchStatistics.Stats = new Stats();
            var statsData = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            // Check if player count is uneven and throw exception if so.
            if (statsData.Length % 2 != 0)
                throw new Exception("Odd number of players. Rage quit must have occured. Is omi, bib, ck1, dave/pete playing?");
            // Determine match type by counting num of players. Work around until this can be done server side. 
            if (statsData.Length == 4)
                MatchStatistics.MatchType = "1v1";
            if (statsData.Length == 6)
                MatchStatistics.MatchType = "2v2";
            if (statsData.Length == 8)
                MatchStatistics.MatchType = "3v3";
            if (statsData.Length == 10)
                MatchStatistics.MatchType = "4v4";
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerStats p = new PlayerStats();
                p.Name = pdata[0].Trim();
                p.Team = pdata[1].Trim();
                p.Kill_Eff = pdata[2].Trim();
                p.Weapon_Eff = pdata[3].Trim();
                MatchStatistics.Stats.Players.Add(p);
            }
        }

        public static void ProcessQStats(String s)
        {
            MatchStatistics.QStats = new QuadStats();
            var statsData = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerQuadStats p = new PlayerQuadStats();
                p.Name = pdata[0].Trim();
                p.Quads = Convert.ToInt32(pdata[1].Trim());
                p.Quad_Eff = pdata[2].Trim();
                p.Quad_Enemy_Kills = Convert.ToInt32(pdata[3].Trim());
                p.Quad_Self_Kills = Convert.ToInt32(pdata[4].Trim());
                p.Quad_Team_Kills = Convert.ToInt32(pdata[5].Trim());
                MatchStatistics.QStats.Players.Add(p);
            }
        }

        private static void ProcessBadStats(string s)
        {
            MatchStatistics.BadStats = new BadStats();
            var statsData = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerBadStats p = new PlayerBadStats();
                p.Name = pdata[0].Trim();
                p.Dropped_Paks = Convert.ToInt32(pdata[1].Trim());
                p.Self_Damage = pdata[2].Trim();
                p.Team_Damage = pdata[3].Trim();
                MatchStatistics.BadStats.Players.Add(p);
            }
        }

        private static void ProcessEfficiencyStats(string s)
        {
            MatchStatistics.EfficiencyStats = new EfficiencyStats();
            var statsData = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerEfficiencyStats p = new PlayerEfficiencyStats();
                p.Name = pdata[0].Trim();
                p.Bullet_Eff = pdata[1].Trim();
                p.Nails_Eff = pdata[2].Trim();
                p.Rocket_Eff = pdata[3].Trim();
                p.Lightning_Eff = pdata[4].Trim();
                p.Total_Eff = pdata[5].Trim();
                MatchStatistics.EfficiencyStats.Players.Add(p);
            }
        }

        private static void ProcessKillStats(string s)
        {
            MatchStatistics.KStats = new KillStats();
            var statsData = s.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 2; i < statsData.Length; i++)
            {
                var pdata = statsData[i].Split('|');
                PlayerKillStats p = new PlayerKillStats();
                p.Name = pdata[0].Trim();
                p.Frag_Count = Convert.ToInt32(pdata[1].Trim());
                p.Enemy_Kill_Count = Convert.ToInt32(pdata[2].Trim());
                p.Self_Kill_Count = Convert.ToInt32(pdata[3].Trim());
                p.Team_Kill_Count = Convert.ToInt32(pdata[4].Trim());
                p.Killed_Count = Convert.ToInt32(pdata[5].Trim());
                MatchStatistics.KStats.Players.Add(p);
            }
        }
    }
}

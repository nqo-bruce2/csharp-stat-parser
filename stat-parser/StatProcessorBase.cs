using Integration;
using NLog;
using System;
using DTO;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Models;
using Utils;

namespace stat_parser
{
    public abstract class StatProcessorBase
    {
        public static MatchResultDTO MatchResults;

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string ProcessedDirectory = ConfigurationManager.AppSettings["ProcessedDirectory"];
        private static readonly string ErrorDirectory = ConfigurationManager.AppSettings["ErrorDirectory"];

        public void StartParsing(string fileName, string fileToParse)
        {
            try
            {
                logger.Info("Creating Match....");
                MatchResults = new MatchResultDTO();
                // for uniqueness, make matchId the filename. 
                MatchResults.MatchId = fileName;

                // TODO: format date from file name
                MatchResults.MatchDate = DateTime.ParseExact(fileName.Split('-')[0], "yyyyMMddHHmmss",
                           System.Globalization.CultureInfo.InvariantCulture);

                // read the whole file
                var text = File.ReadAllText(fileToParse);
                MatchResults.MatchText = text;

                // persist match and log contents before parsing as a backup of raw data
                logger.Info("Saving Match before parse....");
                SaveMatchBeforeParsing(fileName, text);

                // Begin processing
                SplitMatchStats(text);
                ProcessTeamStats();
                ProcessStats();
                ProcessQuadStats();
                ProcessBadStats();
                ProcessEfficiencyStats();
                ProcessKillStats();

                //Do persistence
                // Save all stats
                PersistMatchData();

                //Move file once processed. 
                File.Move(fileToParse, ProcessedDirectory + "\\" + fileName + DateTime.Now.Ticks + ".processed");

            }
            catch (Exception ex)
            {
                logger.Error("Exception during processing: " + ex.ToString());
                File.Move(fileToParse, ErrorDirectory + "\\" + fileName + "." + DateTime.Now.Ticks + ".processed");
            }
        }

        private void SaveMatchBeforeParsing(string fileName, string text)
        {
            logger.Info("Saving initial match details");
            using (var db = new StatsDbContext())
            {
                db.Match.Add(new Models.Match() { MatchId = fileName, MatchType = "", MapName = "", MatchText = text, Date = MatchResults.MatchDate });
                db.SaveChanges();
            }

        }

        // abstract methods. Each mod needs to implement these for custom parsing
        internal abstract void SplitMatchStats(String s);
        internal abstract void ProcessStats();
        internal abstract void ProcessTeamStats();
        internal abstract void ProcessQuadStats();
        internal abstract void ProcessBadStats();
        internal abstract void ProcessEfficiencyStats();
        internal abstract void ProcessKillStats();

        private void PersistMatchData()
        {
            logger.Info("Saving all data to database");
            using (var db = new StatsDbContext())
            {
                // update initial saved match with more details.
                var match = db.Match
                    .Where(b => b.MatchId == MatchResults.MatchId)
                    .FirstOrDefault();
                match.MatchType = MatchResults.MatchType;
                match.MapName = MatchResults.MapName;
                db.SaveChanges();

                // Save each individual team stats. 
                foreach (KeyValuePair<string, MatchTeamStatDTO> teamEntry in MatchResults.ListOfTeams)
                {
                    var newMatchTeamStat = new MatchTeamStats()
                    {
                        MatchId = match.Id,
                        TeamColor = teamEntry.Value.TeamColor,
                        TeamVerdict = teamEntry.Value.TeamVerdict,
                        TeamTotalFrags = teamEntry.Value.TeamTotalFrags.ConvertToInt(),
                        TeamTotalQuads = teamEntry.Value.TeamTotalQuads.ConvertToInt(),
                        TeamTotalPents = teamEntry.Value.TeamTotalPents.ConvertToInt(),
                        TeamTotalEyes = teamEntry.Value.TeamTotalEyes.ConvertToInt(),
                        TeamTotalRL = teamEntry.Value.TeamTotalRL.ConvertToInt(),
                        TeamTotalLG = teamEntry.Value.TeamTotalLG.ConvertToInt(),
                        TeamTotalGL = teamEntry.Value.TeamTotalGL.ConvertToInt(),
                        TeamTotalSNG = teamEntry.Value.TeamTotalSNG.ConvertToInt(),
                        TeamTotalNG = teamEntry.Value.TeamTotalNG.ConvertToInt(),
                        TeamTotalMH = teamEntry.Value.TeamTotalMH.ConvertToInt(),
                        TeamTotalRA = teamEntry.Value.TeamTotalRA.ConvertToInt(),
                        TeamTotalYA = teamEntry.Value.TeamTotalYA.ConvertToInt(),
                        TeamTotalGA = teamEntry.Value.TeamTotalGA.ConvertToInt(),
                        TeamPlusRLPack = teamEntry.Value.TeamPlusRLPack.Trim(),
                        TeamMinusRLPack = teamEntry.Value.TeamMinusRLPack.Trim(),
                        TeamControlPercentage = teamEntry.Value.TeamControlPercentage.ConvertEfficiency(),
                    };
                    db.MatchTeamStats.Add(newMatchTeamStat);
                    db.SaveChanges();
                }

                // add all player detailed stats
                foreach (KeyValuePair<string, MatchPlayerDTO> playerEntry in MatchResults.ListOfPlayers)
                {
                    var playerId = 0;
                    var playa = db.Player
                        .Where(x => x.Name == playerEntry.Key)
                        .FirstOrDefault();
                    if (playa == null)
                    {
                        Random rnd = new Random();
                        var newPlayer = new Player() { Name = playerEntry.Key, IpAddress = rnd.Next(0, 250) + "." + rnd.Next(0, 250) + "." + rnd.Next(0, 250) + "." + rnd.Next(0, 250), Password = "pw", UserName = "uname" };
                        db.Player.Add(newPlayer);
                        db.SaveChanges();
                        playerId = newPlayer.Id;
                    }
                    else
                        playerId = playa.Id;

                    var mpStats = new MatchPlayerStats();
                    mpStats.MatchId = match.Id;
                    mpStats.PlayerId = playerId;
                    // populate team stats
                    mpStats.TeamColor = playerEntry.Value.TeamColor;
                    mpStats.KillEfficiency = playerEntry.Value.KillEfficiency.ConvertEfficiency();
                    mpStats.WeaponEfficiency = playerEntry.Value.WeaponEfficiency.ConvertEfficiency();
                    // populate quad stats
                    mpStats.NumberOfQuads = playerEntry.Value.NumberOfQuads.ConvertToInt();
                    mpStats.QuadEfficiency = playerEntry.Value.QuadEfficiency.ConvertEfficiency();
                    mpStats.NumQuadEnemyKills = playerEntry.Value.NumQuadEnemyKills.ConvertToInt();
                    mpStats.NumQuadSelfKills = playerEntry.Value.NumQuadSelfKills.ConvertToInt();
                    mpStats.NumQuadTeamKills = playerEntry.Value.NumQuadTeamKills.ConvertToInt();
                    // populate kill stats
                    mpStats.NumOfFrags = playerEntry.Value.NumOfFrags.ConvertToInt();
                    mpStats.NumOfEnemyKills = playerEntry.Value.NumOfEnemyKills.ConvertToInt();
                    mpStats.NumOfSelfKills = playerEntry.Value.NumOfSelfKills.ConvertToInt();
                    mpStats.NumOfTeamKills = playerEntry.Value.NumOfTeamKills.ConvertToInt();
                    mpStats.NumOfDeaths = playerEntry.Value.NumOfDeaths.ConvertToInt();
                    // populate efficiency stats
                    mpStats.BulletEfficiency = playerEntry.Value.BulletEfficiency.ConvertEfficiency();
                    mpStats.NailsEfficiency = playerEntry.Value.NailsEfficiency.ConvertEfficiency();
                    mpStats.RocketEfficiency = playerEntry.Value.RocketEfficiency.ConvertEfficiency();
                    mpStats.LightningEfficiency = playerEntry.Value.LightningEfficiency.ConvertEfficiency();
                    mpStats.TotalEfficiency = playerEntry.Value.TotalEfficiency.ConvertEfficiency();
                    // populate bad stats
                    mpStats.DroppedPaks = playerEntry.Value.DroppedPaks.ConvertToInt();
                    mpStats.SelfDamage = playerEntry.Value.SelfDamage.ConvertEfficiency();
                    mpStats.TeamDamage = playerEntry.Value.TeamDamage.ConvertEfficiency();

                    db.MatchPlayerStats.Add(mpStats);
                    db.SaveChanges();
                }
            }
        }
    }
}

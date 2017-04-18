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

        /// <summary>
        /// This method begings the parsing process. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileToParse"></param>
        public void StartParsing(string fileName, string fileToParse)
        {
            try
            {
                logger.Info("Creating Match....");
                MatchResults = new MatchResultDTO();
                // for uniqueness, make matchId the filename. 
                MatchResults.MatchId = fileName;

                // Set match date. 
                // many of the database queries rely on this date format. Don't change unless you want smoke to do more work. 
                MatchResults.MatchDate = DateTime.ParseExact(fileName.Split('-')[0], "yyyyMMddHHmmss",
                           System.Globalization.CultureInfo.InvariantCulture);

                // read the whole file
                var text = File.ReadAllText(fileToParse);

                // we save the whole text file to the database in case we have to go back and reprocess matches. 
                // For instance, if sometime in the future we want to move to a Cassandra database for easier querying we could process 
                // each match text and store in the Cassandra database. 
                MatchResults.MatchText = text;

                // persist match and log contents before parsing as a backup of raw data
                // if something goes terribly wrong during processing, the initial match data and raw file contents are persisted for reprocessing. 
                logger.Info("Saving Match before parse....");
                SaveMatchBeforeParsing(fileName, text);

                // Here is where we beging parsing the stat file.
                SplitMatchStats(text);
                ProcessMatchInfo();
                ProcessTeamStats();
                ProcessPlayerStats();
                ProcessQuadStats();
                ProcessBadStats();
                ProcessEfficiencyStats();
                ProcessKillStats();

                // Do persistence
                // Save all stats
                PersistMatchData();

                // Once the file has been parsed, we move the file to a "processed" directory
                File.Move(fileToParse, ProcessedDirectory + "/" + fileName + DateTime.Now.Ticks + ".processed");

            }
            catch (Exception ex)
            {
                /* if something bad happens during parsing this catch block will catch the exception
                 * and move the file to an error directory.
                 */
                File.Move(fileToParse, ErrorDirectory + "/" + fileName + DateTime.Now.Ticks + ".error");
                logger.Error("Exception during processing: " + ex.ToString());
            }
        }
        /// <summary>
        /// This method saves the initial set of data to the database before we begin parsing. 
        /// This is done to ensure I have the raw match data saved in case I encounter bad data during a parse. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        private void SaveMatchBeforeParsing(string fileName, string text)
        {
            logger.Info("Saving initial match details");
            using (var db = new StatsDbContext())
            {
                db.Match.Add(new Match() { MatchId = fileName, MatchType = "", MapName = "", MatchText = text, Date = MatchResults.MatchDate });
                db.SaveChanges();
            }

        }

        // abstract methods. Each mod needs to implement these for custom parsing
        internal abstract void SplitMatchStats(String s);
        internal abstract void ProcessMatchInfo();
        internal abstract void ProcessPlayerStats();
        internal abstract void ProcessTeamStats();
        internal abstract void ProcessQuadStats();
        internal abstract void ProcessBadStats();
        internal abstract void ProcessEfficiencyStats();
        internal abstract void ProcessKillStats();

        /// <summary>
        /// Once all the match data is parsed this method begins inserting into the database. 
        /// </summary>
        private void PersistMatchData()
        {
            logger.Info("Saving all data to database");
            using (var db = new StatsDbContext())
            {
                /*
                 *We saved the initial match data in the SaveBeforeParsing method. 
                 *let's finish saving the match details now that we've parsed the data. 
                 */
                
                // find the match by the matchId
                var match = db.Match
                    .Where(b => b.MatchId == MatchResults.MatchId)
                    .FirstOrDefault();
                match.MatchType = MatchResults.MatchType;
                match.MapName = MatchResults.MapName;
                match.MatchTimeLimit = MatchResults.MatchTimeLimit.ConvertToInt();
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

                    var playerId = playerEntry.Key.ConvertToInt();
                    var mpStats = new MatchPlayerStats();

                    // Because I'm setting every unique non logged in player to an int < 0 in the dictionary, I basically check if this user is logged in.
                    // Of course this could be handled more elagantly with a proper object but time is limited at the moment.
                    // if the player in this entry is logged in
                    if (playerId > 0)
                    {
                        // let's test that the player id is actually a user in the database since we could have an ID in config but not in the database. 
                        var playerInDatabase = db.Player
                        .Where(x => x.Id == playerId)
                        .FirstOrDefault();

                        // if the player id isn't found in the database throw exception. 
                        if (playerInDatabase == null)
                            throw new Exception("PlayerId: " + playerId + " was deemed logged in but player was not found in the database. Please check id exists.");

                        // let's make sure the player it found wasn't the unknown player for whatever reason. 
                        if (playerId != 999 && playerId > 0)
                        {
                            // populate team stats
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
                        }
                        else
                            throw new Exception("Some how the query for playerId: " + playerId + " came back as 999");
                    }
                    else
                        // set id to unknown player
                        playerId = 999;

                    // Tracking Info
                    mpStats.MatchId = match.Id;
                    mpStats.PlayerId = playerId;
                    mpStats.TeamColor = playerEntry.Value.TeamColor;
                    db.MatchPlayerStats.Add(mpStats);
                    db.SaveChanges();
                }
            }
        }
    }
}

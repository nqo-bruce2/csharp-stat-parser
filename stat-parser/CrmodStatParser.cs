using DTO;
using NLog;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Generic;
using Utils;

namespace stat_parser
{
    public class CrmodStatParser : StatProcessorBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        // The pattern below is what we use to split the stats file into each individual stat data (skillstats, quadstats, teamstats, etc). 
        private const string PATTERN = "\n\n";
        // The line_splitter is what we use to split each individual line within a stat
        private const string LINE_SPLITTER = "\n";
        // Splits each column in a stat block
        private const char COLUMN_SPLITTER = '|';
        private string[] StatBlocks { get; set; }
        public MatchTeamStatDTO TeamOne { get; set; }
        public MatchTeamStatDTO TeamTwo { get; set; }

        /* The indexes section below is what we use to identify each stat location
         * For instance, The match header (the text that displays which team won) is the first block of text we parse
         */
        #region Indexes 
        private const int MATCH_HEADER_INDEX = 0;
        private const int MATCH_INFO_INDEX = 2;
        private const int PLAYER_STATS_INDEX = 3;
        private const int QUAD_STATS_INDEX = 4;
        private const int BAD_STATS_INDEX = 5;
        private const int EFFICIENCY_STATS_INDEX = 6;
        private const int KILL_STATS_INDEX = 7;
        private const int TEAM_STATS_INDEX = 8;
        #endregion
        
        public CrmodStatParser()
        {
            TeamOne = new MatchTeamStatDTO();
            TeamTwo = new MatchTeamStatDTO();
        }

        /// <summary>
        /// This method splits the entire stat file into blocks of stats (player, kill, quad, team, etc)
        /// </summary>
        /// <param name="s"></param>
        internal override void SplitMatchStats(string s)
        {
            logger.Info("Beginning to split stats into blocks");
            StatBlocks = Regex.Split(s, PATTERN, RegexOptions.Singleline);

            /* There should be 10 blocks after splitting the file. For the sake of short circuiting the process
             * if there aren't 10 blocks, throw an error and log the reason.
             */
            if (StatBlocks.Length != 10)
            {
                logger.Error("Unexpected number of stat blocks. Aborting parse");
                logger.Info(String.Join("\n", StatBlocks));
                throw new Exception("Unexpected number of stat blocks. Expected 10 got: " + StatBlocks.Length);
            }
        }

        /// <summary>
        /// This method process the match meta info setting Timelimit, matchtype, and mapname
        /// i.e. "5 minute 1v1 match on dm3"
        /// </summary>
        internal override void ProcessMatchInfo()
        {
            const int MAPNAME_INDEX = 5;
            const int TIME_LIMIT_INDEX = 0;
            const int MATCH_TYPE_INDEX = 2;

            logger.Info("Beginning to parse MatchInfo (type, timelimit, map");
            var s = StatBlocks[MATCH_INFO_INDEX];

            // split info line
            var matchInfoText = s.Split(new string[] { " " }, StringSplitOptions.None);
            MatchResults.MapName = matchInfoText[MAPNAME_INDEX];
            MatchResults.MatchTimeLimit = matchInfoText[TIME_LIMIT_INDEX];
            MatchResults.MatchType = matchInfoText[MATCH_TYPE_INDEX];
        }

        /// <summary>
        /// This method processes the teamstats stat block.
        /// </summary>
        private void ProcessTeamStatDetails()
        {
            // TODO: Sputnik, please change this format.
            // grab team stats section
            var s = StatBlocks[TEAM_STATS_INDEX];
            // split lines
            var data = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);
            logger.Info("Begin parsing Team Stats Details");

            /* This gets a little messy here.
             * Since the order of teams in this stat block is not consistent 
             * (i.e. winning team is ALWAYS on the left losing on right) .. (it's based on which team is created first)
             * I've decided to define the teams as teamX and teamY. I then use the variable TeamX to do a lookup in the dictionary
             * created during the parsing of the header. Yeah, we need to fix this.
             * TODO: Sputnik, when you change the format, make the winning team appear first
             */
            var teamX = data[0].Split(COLUMN_SPLITTER)[1].Trim();
            var teamY = data[0].Split(COLUMN_SPLITTER)[2].Trim();

            /* gave up creating constants here.....This simply needs to be redone given we're not on 13 inch monitors anymore. */
            // grab QUAD
            MatchResults.ListOfTeams[teamX].TeamTotalQuads = data[2].Split(COLUMN_SPLITTER)[1].Split(':')[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalQuads = data[2].Split(COLUMN_SPLITTER)[2].Split(':')[0].Trim();
            // grab 666
            MatchResults.ListOfTeams[teamX].TeamTotalPents = data[2].Split(COLUMN_SPLITTER)[1].Split(':')[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalPents = data[2].Split(COLUMN_SPLITTER)[2].Split(':')[1].Trim();
            // grab RING
            MatchResults.ListOfTeams[teamX].TeamTotalEyes = data[2].Split(COLUMN_SPLITTER)[1].Split(':')[2].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalEyes = data[2].Split(COLUMN_SPLITTER)[2].Split(':')[2].Trim();
            // grab RL
            MatchResults.ListOfTeams[teamX].TeamTotalRL = data[3].Split(COLUMN_SPLITTER)[1].Split(':')[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalRL = data[3].Split(COLUMN_SPLITTER)[2].Split(':')[0].Trim();
            // grab LG
            MatchResults.ListOfTeams[teamX].TeamTotalLG = data[3].Split(COLUMN_SPLITTER)[1].Split(':')[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalLG = data[3].Split(COLUMN_SPLITTER)[2].Split(':')[1].Trim();
            // grab GL
            MatchResults.ListOfTeams[teamX].TeamTotalGL = data[3].Split(COLUMN_SPLITTER)[1].Split(':')[2].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalGL = data[3].Split(COLUMN_SPLITTER)[2].Split(':')[2].Trim();
            // grab SNG
            MatchResults.ListOfTeams[teamX].TeamTotalSNG = data[4].Split(COLUMN_SPLITTER)[1].Split(':')[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalSNG = data[4].Split(COLUMN_SPLITTER)[2].Split(':')[0].Trim();
            // grab NG
            MatchResults.ListOfTeams[teamX].TeamTotalNG = data[4].Split(COLUMN_SPLITTER)[1].Split(':')[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalNG = data[4].Split(COLUMN_SPLITTER)[2].Split(':')[1].Trim();
            // grab MH
            MatchResults.ListOfTeams[teamX].TeamTotalMH = data[4].Split(COLUMN_SPLITTER)[1].Split(':')[2].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalMH = data[4].Split(COLUMN_SPLITTER)[2].Split(':')[2].Trim();
            // grab RA
            MatchResults.ListOfTeams[teamX].TeamTotalRA = data[5].Split(COLUMN_SPLITTER)[1].Split(':')[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalRA = data[5].Split(COLUMN_SPLITTER)[2].Split(':')[0].Trim();
            // grab YA
            MatchResults.ListOfTeams[teamX].TeamTotalYA = data[5].Split(COLUMN_SPLITTER)[1].Split(':')[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalYA = data[5].Split(COLUMN_SPLITTER)[2].Split(':')[1].Trim();
            // grab GA
            MatchResults.ListOfTeams[teamX].TeamTotalGA = data[5].Split(COLUMN_SPLITTER)[1].Split(':')[2].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalGA = data[5].Split(COLUMN_SPLITTER)[2].Split(':')[2].Trim();
            // grab +RL
            MatchResults.ListOfTeams[teamX].TeamPlusRLPack = data[6].Split(COLUMN_SPLITTER)[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamPlusRLPack = data[6].Split(COLUMN_SPLITTER)[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            // grab -RL
            MatchResults.ListOfTeams[teamX].TeamMinusRLPack = data[6].Split(COLUMN_SPLITTER)[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamMinusRLPack = data[6].Split(COLUMN_SPLITTER)[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            // grab CTRL
            MatchResults.ListOfTeams[teamX].TeamControlPercentage = data[7].Split(COLUMN_SPLITTER)[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamControlPercentage = data[7].Split(COLUMN_SPLITTER)[2].Trim();

        }

        private void ProcessHeader()
        {
            logger.Info("Beginning to parse header");

            // grab header text
            var s = StatBlocks[MATCH_HEADER_INDEX];

            // split header lines
            var statsData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // grab teams
            TeamOne.TeamColor = statsData[1].Split(new string[] { "The " }, StringSplitOptions.None)[1].Split(new string[] { "team" }, StringSplitOptions.None)[0].Trim();
            TeamTwo.TeamColor = statsData[2].Split(new string[] { "The " }, StringSplitOptions.None)[1].Split(new string[] { "team" }, StringSplitOptions.None)[0].Trim();
            // grab total frags
            TeamOne.TeamTotalFrags = statsData[1].Split(new string[] { "has " }, StringSplitOptions.None)[1].Split(new string[] { "f" }, StringSplitOptions.None)[0].Trim();
            TeamTwo.TeamTotalFrags = statsData[2].Split(new string[] { "has " }, StringSplitOptions.None)[1].Split(new string[] { "f" }, StringSplitOptions.None)[0].Trim();
            // determine winner and loser
            if (TeamOne.TeamTotalFrags == TeamTwo.TeamTotalFrags)
            {
                TeamOne.TeamVerdict = "tie";
                TeamTwo.TeamVerdict = "tie";
            }
            else if(TeamOne.TeamTotalFrags.ConvertToInt() > TeamTwo.TeamTotalFrags.ConvertToInt())
            {
                TeamOne.TeamVerdict = "win";
                TeamTwo.TeamVerdict = "lose";
            }
            else
            {
                TeamOne.TeamVerdict = "lose";
                TeamTwo.TeamVerdict = "win";
            }
            MatchResults.ListOfTeams.Add(TeamOne.TeamColor, TeamOne);
            MatchResults.ListOfTeams.Add(TeamTwo.TeamColor, TeamTwo);

            logger.Info("End of parsing header");
        }

        /// <summary>
        /// This method processes the Teamstats stat block.
        /// </summary>
        internal override void ProcessTeamStats()
        {
            // process the header to create the teams
            ProcessHeader();

            // now process the details
            ProcessTeamStatDetails();
        }

        /// <summary>
        /// This method process Playerstats stat block
        /// </summary>
        internal override void ProcessPlayerStats()
        {
            logger.Info("Beginning to parse Player Stats");
            // this var will be used to create unique guest ids that we'll then default to 999 at db entry
            var guestOrNotLoggedInStatId = 0;

            var s = StatBlocks[PLAYER_STATS_INDEX];

            // split each player to process individually
            var statsData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // Check if player count is uneven and throw exception if so.
            if (statsData.Length % 2 != 0)
                throw new Exception("Odd number of players. Rage quit must have occured. Is omi, bib, ck1, dave/pete playing?");
            
            // loop through each player creating a new player object and setting stats.
            for (int i = 2; i < statsData.Length; i++)
            {
                // indexes 
                var statIdIndex = 0;
                var nameIndex = 1;
                var teamColorIndex = 2;
                var killEfficiencyIndex = 3;
                var weaponEfficiencyIndex = 4;
                var pingIndex = 5;
                var ipIndex = 6;
                var pdata = statsData[i].Split(COLUMN_SPLITTER);

                // populate player object
                var p = new MatchPlayerDTO()
                {
                    StatId = pdata[statIdIndex].Trim(),
                    Name = pdata[nameIndex].Trim(),
                    TeamColor = pdata[teamColorIndex].Trim(),
                    KillEfficiency = pdata[killEfficiencyIndex].Trim(),
                    WeaponEfficiency = pdata[weaponEfficiencyIndex].Trim(),
                    Ping = pdata[pingIndex].Trim(),
                    // remove last octet per Turtlevan
                    IpAddress = pdata[ipIndex].Trim().Remove(pdata[ipIndex].Length - 3) + "xxx"
                    
                };

                // add players to list of players
                // if statid is 0 set it to less than 0 as we'll default this to UnknownPlayerId 999
                if (p.StatId.Equals("0"))
                    p.StatId = (guestOrNotLoggedInStatId - 1).ToString();
                MatchResults.ListOfPlayers.Add(p.StatId, p);
            }
            logger.Info("End parsing player stats");
        }

        /// <summary>
        /// This method processes Quadstats stat block
        /// </summary>
        internal override void ProcessQuadStats()
        {
            // indexes
            var statIdIndex = 0;
            //var nameIndex = 1;
            var numOfQuadsIndex = 2;
            var quadEfficiencyIndex = 3;
            var numOfEnemyQKillsIndex = 4;
            var numOfSelfQKillsIndex = 5;
            var numOfTeamQKillsIndex = 6;

            // grab quad section
            var s = StatBlocks[QUAD_STATS_INDEX];

            // grab lines
            var quadData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // iterate over players
            for (int i = 2; i < quadData.Length; i++)
            {
                var playerDetailedData = quadData[i].Split(COLUMN_SPLITTER);
                var statId = playerDetailedData[statIdIndex].Trim();
                if (!statId.Equals("0"))
                {
                    MatchResults.ListOfPlayers[statId].NumberOfQuads = playerDetailedData[numOfQuadsIndex].Trim();
                    MatchResults.ListOfPlayers[statId].QuadEfficiency = playerDetailedData[quadEfficiencyIndex].Trim();
                    MatchResults.ListOfPlayers[statId].NumQuadEnemyKills = playerDetailedData[numOfEnemyQKillsIndex].Trim();
                    MatchResults.ListOfPlayers[statId].NumQuadSelfKills = playerDetailedData[numOfSelfQKillsIndex].Trim();
                    MatchResults.ListOfPlayers[statId].NumQuadTeamKills = playerDetailedData[numOfTeamQKillsIndex].Trim();
                }
            }
        }

        /// <summary>
        /// This method processes Badstats stat block
        /// </summary>
        internal override void ProcessBadStats()
        {
            // indexes
            var statIdIndex = 0;
            //var nameIndex = 1;
            var DroppedPaksIndex = 2;
            var SelfDamageIndex = 3;
            var TeamDamageIndex = 4;

            // grab badstats section
            var s = StatBlocks[BAD_STATS_INDEX];

            // grab lines
            var badstatsData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // iterate over players
            for (int i = 2; i < badstatsData.Length; i++)
            {
                var playerDetailedData = badstatsData[i].Split(COLUMN_SPLITTER);
                var statId = playerDetailedData[statIdIndex].Trim();
                if (!statId.Equals("0"))
                {
                    MatchResults.ListOfPlayers[statId].DroppedPaks = playerDetailedData[DroppedPaksIndex].Trim();
                    MatchResults.ListOfPlayers[statId].SelfDamage = playerDetailedData[SelfDamageIndex].Trim();
                    MatchResults.ListOfPlayers[statId].TeamDamage = playerDetailedData[TeamDamageIndex].Trim();
                }
            }
        }

        /// <summary>
        ///  this method processes the Efficiency stat block
        /// </summary>
        internal override void ProcessEfficiencyStats()
        {
            // indexes
            var statIdIndex = 0;
            //var nameIndex = 1;
            var BulletEffIndex = 2;
            var NailsEffIndex = 3;
            var RocketEffIndex = 4;
            var LightningEffIndex = 5;
            var TotalEffIndex = 6;

            // grab badstats section
            var s = StatBlocks[EFFICIENCY_STATS_INDEX];

            // grab lines
            var efficiencyData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // iterate over players
            for (int i = 2; i < efficiencyData.Length; i++)
            {
                var playerDetailedData = efficiencyData[i].Split(COLUMN_SPLITTER);
                var statId = playerDetailedData[statIdIndex].Trim();
                if (!statId.Equals("0"))
                {
                    MatchResults.ListOfPlayers[statId].BulletEfficiency = playerDetailedData[BulletEffIndex].Trim();
                    MatchResults.ListOfPlayers[statId].NailsEfficiency = playerDetailedData[NailsEffIndex].Trim();
                    MatchResults.ListOfPlayers[statId].RocketEfficiency = playerDetailedData[RocketEffIndex].Trim();
                    MatchResults.ListOfPlayers[statId].LightningEfficiency = playerDetailedData[LightningEffIndex].Trim();
                    MatchResults.ListOfPlayers[statId].TotalEfficiency = playerDetailedData[TotalEffIndex].Trim();
                }

            }
        }

        /// <summary>
        /// This method processes the killstats stat block
        /// </summary>
        internal override void ProcessKillStats()
        {
            // indexes
            var statIdIndex = 0;
            //var nameIndex = 1;
            var FragCountIndex = 2;
            var EnemyKillCountIndex = 3;
            var SelfKillCountIndex = 4;
            var TeamKillCountIndex = 5;
            var KilledCountIndex = 6;

            // grab badstats section
            var s = StatBlocks[KILL_STATS_INDEX];

            // grab lines
            var killstatsData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // iterate over players
            for (int i = 2; i < killstatsData.Length; i++)
            {
                var playerDetailedData = killstatsData[i].Split(COLUMN_SPLITTER);
                var statId = playerDetailedData[statIdIndex].Trim();
                if (!statId.Equals("0"))
                {
                    MatchResults.ListOfPlayers[statId].NumOfFrags = playerDetailedData[FragCountIndex].Trim();
                    MatchResults.ListOfPlayers[statId].NumOfEnemyKills = playerDetailedData[EnemyKillCountIndex].Trim();
                    MatchResults.ListOfPlayers[statId].NumOfSelfKills = playerDetailedData[SelfKillCountIndex].Trim();
                    MatchResults.ListOfPlayers[statId].NumOfTeamKills = playerDetailedData[TeamKillCountIndex].Trim();
                    MatchResults.ListOfPlayers[statId].NumOfDeaths = playerDetailedData[KilledCountIndex].Trim();
                }

            }
        }


    }
}

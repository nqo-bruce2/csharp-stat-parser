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
        private const string PATTERN = "\n\n";
        private const string LINE_SPLITTER = "\n";
        private string[] Substrings { get; set; }
        public MatchTeamStatDTO TeamOne { get; set; }
        public MatchTeamStatDTO TeamTwo { get; set; }

        #region Indexes 
        private const int MATCH_HEADER_INDEX = 0;
        private const int MATCH_STATS_INDEX = 2;
        private const int QUAD_STATS_INDEX = 3;
        private const int BAD_STATS_INDEX = 4;
        private const int EFFICIENCY_STATS_INDEX = 5;
        private const int KILL_STATS_INDEX = 6;
        private const int TEAM_STATS_INDEX = 7;
        #endregion
        
        public CrmodStatParser()
        {
            TeamOne = new MatchTeamStatDTO();
            TeamTwo = new MatchTeamStatDTO();
        }

        private void ProcessTeamStatDetails()
        {
            // grab team stats section
            var s = Substrings[TEAM_STATS_INDEX];
            // split lines
            var data = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);
            logger.Debug("Processing Team Stats Details");
            logger.Debug(data.ToString());
            var teamX = data[0].Split('|')[1].Trim();
            var teamY = data[0].Split('|')[2].Trim();
            // grab QUAD
            MatchResults.ListOfTeams[teamX].TeamTotalQuads = data[2].Split('|')[1].Split(':')[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalQuads = data[2].Split('|')[2].Split(':')[0].Trim();
            // grab 666
            MatchResults.ListOfTeams[teamX].TeamTotalPents = data[2].Split('|')[1].Split(':')[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalPents = data[2].Split('|')[2].Split(':')[1].Trim();
            // grab RING
            MatchResults.ListOfTeams[teamX].TeamTotalEyes = data[2].Split('|')[1].Split(':')[2].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalEyes = data[2].Split('|')[2].Split(':')[2].Trim();
            // grab RL
            MatchResults.ListOfTeams[teamX].TeamTotalRL = data[3].Split('|')[1].Split(':')[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalRL = data[3].Split('|')[2].Split(':')[0].Trim();
            // grab LG
            MatchResults.ListOfTeams[teamX].TeamTotalLG = data[3].Split('|')[1].Split(':')[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalLG = data[3].Split('|')[2].Split(':')[1].Trim();
            // grab GL
            MatchResults.ListOfTeams[teamX].TeamTotalGL = data[3].Split('|')[1].Split(':')[2].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalGL = data[3].Split('|')[2].Split(':')[2].Trim();
            // grab SNG
            MatchResults.ListOfTeams[teamX].TeamTotalSNG = data[4].Split('|')[1].Split(':')[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalSNG = data[4].Split('|')[2].Split(':')[0].Trim();
            // grab NG
            MatchResults.ListOfTeams[teamX].TeamTotalNG = data[4].Split('|')[1].Split(':')[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalNG = data[4].Split('|')[2].Split(':')[1].Trim();
            // grab MH
            MatchResults.ListOfTeams[teamX].TeamTotalMH = data[4].Split('|')[1].Split(':')[2].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalMH = data[4].Split('|')[2].Split(':')[2].Trim();
            // grab RA
            MatchResults.ListOfTeams[teamX].TeamTotalRA = data[5].Split('|')[1].Split(':')[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalRA = data[5].Split('|')[2].Split(':')[0].Trim();
            // grab YA
            MatchResults.ListOfTeams[teamX].TeamTotalYA = data[5].Split('|')[1].Split(':')[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalYA = data[5].Split('|')[2].Split(':')[1].Trim();
            // grab GA
            MatchResults.ListOfTeams[teamX].TeamTotalGA = data[5].Split('|')[1].Split(':')[2].Trim();
            MatchResults.ListOfTeams[teamY].TeamTotalGA = data[5].Split('|')[2].Split(':')[2].Trim();
            // grab +RL
            //MatchResults.ListOfTeams[teamX].TeamPlusRLPack = data[6].Split('|')[1].Split(' ')[3].Trim();
            //MatchResults.ListOfTeams[teamY].TeamPlusRLPack = data[6].Split('|')[2].Split(' ')[3].Trim();
            MatchResults.ListOfTeams[teamX].TeamPlusRLPack = data[6].Split('|')[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            MatchResults.ListOfTeams[teamY].TeamPlusRLPack = data[6].Split('|')[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
            // grab -RL
            MatchResults.ListOfTeams[teamX].TeamMinusRLPack = data[6].Split('|')[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamMinusRLPack = data[6].Split('|')[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            // grab CTRL
            MatchResults.ListOfTeams[teamX].TeamControlPercentage = data[7].Split('|')[1].Trim();
            MatchResults.ListOfTeams[teamY].TeamControlPercentage = data[7].Split('|')[2].Trim();

        }

        private void ProcessHeader()
        {
            logger.Debug("starting header");
            logger.Debug("substrings length = " + Substrings.Length);

            // grab header text
            var s = Substrings[MATCH_HEADER_INDEX];
            logger.Debug("header text = " + s);
            // split header lines
            var statsData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);
            logger.Debug("header details length = " + statsData.Length);
            // grab teams
            TeamOne.TeamColor = statsData[1].Split(new string[] { "The " }, StringSplitOptions.None)[1].Split(new string[] { "team" }, StringSplitOptions.None)[0].Trim(); ;
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
            logger.Debug("ending header");
        }

        private void SetMatchType(string[] data)
        {
            // Check if player count is uneven and throw exception if so.
            if (data.Length % 2 != 0)
                throw new Exception("Odd number of players. Rage quit must have occured. Is omi, bib, ck1, dave/pete playing?");
            // Determine match type by counting num of players. Work around until this can be done server side. 
            if (data.Length == 4)
                MatchResults.MatchType = "1v1";
            //MatchStatistics.MatchType = "1v1";
            if (data.Length == 6)
                //MatchStatistics.MatchType = "2v2";
                MatchResults.MatchType = "2v2";
            if (data.Length == 8)
                //MatchStatistics.MatchType = "3v3";
                MatchResults.MatchType = "3v3";
            if (data.Length == 10)
                //MatchStatistics.MatchType = "4v4";
                MatchResults.MatchType = "4v4";
        }

        internal override void ProcessTeamStats()
        {
            ProcessHeader();
            ProcessTeamStatDetails();
        }

        internal override void ProcessStats()
        {
            logger.Debug("starting skillstats");
            var s = Substrings[MATCH_STATS_INDEX];
            // split each player to process individually
            var statsData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            SetMatchType(statsData);

            
            for (int i = 2; i < statsData.Length; i++)
            {
                // indexes 
                var nameIndex = 0;
                var teamColorIndex = 1;
                var killEfficiency = 2;
                var weaponEfficiency = 3;
                var pdata = statsData[i].Split('|');

                // populate player
                var p = new MatchPlayerDTO();
                p.Name = pdata[nameIndex].Trim();
                p.TeamColor = pdata[teamColorIndex].Trim();
                p.KillEfficiency = pdata[killEfficiency].Trim();
                p.WeaponEfficiency = pdata[weaponEfficiency].Trim();
                MatchResults.ListOfPlayers.Add(p.Name, p);
            }
            logger.Debug("ending skillstats");
        }

        internal override void SplitMatchStats(string s)
        {
            Substrings = Regex.Split(s, PATTERN, RegexOptions.Singleline);
            MatchResults.MapName = "temp";
        }

        internal override void ProcessQuadStats()
        {
            // indexes
            var nameIndex = 0;
            var numOfQuadsIndex = 1;
            var quadEfficiencyIndex = 2;
            var numOfEnemyQKillsIndex = 3;
            var numOfSelfQKillsIndex = 4;
            var numOfTeamQKillsIndex = 5;

            // grab quad section
            var s = Substrings[QUAD_STATS_INDEX];

            // grab lines
            var quadData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // iterate over players
            for (int i = 2; i < quadData.Length; i++)
            {
                var playerDetailedData = quadData[i].Split('|');
                var name = playerDetailedData[nameIndex].Trim();
                //MatchResults.ListOfPlayers[name].Name = name;
                MatchResults.ListOfPlayers[name].NumberOfQuads = playerDetailedData[numOfQuadsIndex].Trim();
                MatchResults.ListOfPlayers[name].QuadEfficiency = playerDetailedData[quadEfficiencyIndex].Trim();
                MatchResults.ListOfPlayers[name].NumQuadEnemyKills = playerDetailedData[numOfEnemyQKillsIndex].Trim();
                MatchResults.ListOfPlayers[name].NumQuadSelfKills = playerDetailedData[numOfSelfQKillsIndex].Trim();
                MatchResults.ListOfPlayers[name].NumQuadTeamKills = playerDetailedData[numOfTeamQKillsIndex].Trim();
            }
        }

        internal override void ProcessBadStats()
        {
            // indexes
            var nameIndex = 0;
            var DroppedPaksIndex = 1;
            var SelfDamageIndex = 2;
            var TeamDamageIndex = 3;

            // grab badstats section
            var s = Substrings[BAD_STATS_INDEX];

            // grab lines
            var badstatsData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // iterate over players
            for (int i = 2; i < badstatsData.Length; i++)
            {
                var playerDetailedData = badstatsData[i].Split('|');
                var name = playerDetailedData[nameIndex].Trim();
                //MatchResults.ListOfPlayers[name].Name = name;
                MatchResults.ListOfPlayers[name].DroppedPaks = playerDetailedData[DroppedPaksIndex].Trim();
                MatchResults.ListOfPlayers[name].SelfDamage = playerDetailedData[SelfDamageIndex].Trim();
                MatchResults.ListOfPlayers[name].TeamDamage = playerDetailedData[TeamDamageIndex].Trim();
            }
        }

        internal override void ProcessEfficiencyStats()
        {
            // indexes
            var nameIndex = 0;
            var BulletEffIndex = 1;
            var NailsEffIndex = 2;
            var RocketEffIndex = 3;
            var LightningEffIndex = 4;
            var TotalEffIndex = 5;

            // grab badstats section
            var s = Substrings[EFFICIENCY_STATS_INDEX];

            // grab lines
            var efficiencyData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // iterate over players
            for (int i = 2; i < efficiencyData.Length; i++)
            {
                var playerDetailedData = efficiencyData[i].Split('|');
                var name = playerDetailedData[nameIndex].Trim();
                //MatchResults.ListOfPlayers[name].Name = name;
                MatchResults.ListOfPlayers[name].BulletEfficiency = playerDetailedData[BulletEffIndex].Trim();
                MatchResults.ListOfPlayers[name].NailsEfficiency = playerDetailedData[NailsEffIndex].Trim();
                MatchResults.ListOfPlayers[name].RocketEfficiency = playerDetailedData[RocketEffIndex].Trim();
                MatchResults.ListOfPlayers[name].LightningEfficiency = playerDetailedData[LightningEffIndex].Trim();
                MatchResults.ListOfPlayers[name].TotalEfficiency = playerDetailedData[TotalEffIndex].Trim();

            }
        }

        internal override void ProcessKillStats()
        {
            // indexes
            var nameIndex = 0;
            var FragCountIndex = 1;
            var EnemyKillCountIndex = 2;
            var SelfKillCountIndex = 3;
            var TeamKillCountIndex = 4;
            var KilledCountIndex = 5;

            // grab badstats section
            var s = Substrings[KILL_STATS_INDEX];

            // grab lines
            var killstatsData = s.Split(new string[] { LINE_SPLITTER }, StringSplitOptions.None);

            // iterate over players
            for (int i = 2; i < killstatsData.Length; i++)
            {
                var playerDetailedData = killstatsData[i].Split('|');
                var name = playerDetailedData[nameIndex].Trim();
                //MatchResults.ListOfPlayers[name].Name = name;
                MatchResults.ListOfPlayers[name].NumOfFrags = playerDetailedData[FragCountIndex].Trim();
                MatchResults.ListOfPlayers[name].NumOfEnemyKills = playerDetailedData[EnemyKillCountIndex].Trim();
                MatchResults.ListOfPlayers[name].NumOfSelfKills = playerDetailedData[SelfKillCountIndex].Trim();
                MatchResults.ListOfPlayers[name].NumOfTeamKills = playerDetailedData[TeamKillCountIndex].Trim();
                MatchResults.ListOfPlayers[name].NumOfDeaths = playerDetailedData[KilledCountIndex].Trim();

            }
        }
    }
}

using DTO;
using System;
using System.Collections.Generic;

namespace DTO
{
    public class MatchResultDTO
    {
        public string MatchId { get; set; }
        public string MatchType { get; set; }
        public string MapName { get; set; }
        public DateTime MatchDate { get; set; }
        public string MatchText { get; set; }
        public string MatchTimeLimit { get; set; }
        public Dictionary<string, MatchTeamStatDTO> ListOfTeams { get; set; }
        public Dictionary<string, MatchPlayerDTO> ListOfPlayers { get; set; }

        public MatchResultDTO()
        {
            ListOfTeams = new Dictionary<string, MatchTeamStatDTO>();
            ListOfPlayers = new Dictionary<string, MatchPlayerDTO>();
        }
    }
}

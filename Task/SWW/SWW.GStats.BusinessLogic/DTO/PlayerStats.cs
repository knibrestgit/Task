using System;

namespace SWW.GStats.BusinessLogic.DTO
{

    public class PlayerStats
    {
        public int totalMatchesPlayed { get; set; }
        public int totalMatchesWon { get; set; }
        public string favoriteServer { get; set; }
        public int uniqueServers { get; set; }
        public string favoriteGameMode { get; set; }
        public float averageScoreboardPercent { get; set; }
        public int maximumMatchesPerDay { get; set; }
        public float averageMatchesPerDay { get; set; }
        public DateTime lastMatchPlayed { get; set; }
        public float killToDeathRatio { get; set; }
    }

}

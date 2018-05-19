using System.Collections.Generic;

namespace SWW.GStats.BusinessLogic.DTO
{

    public class EndpointStats
    {
        public int totalMatchesPlayed { get; set; }
        public int maximumMatchesPerDay { get; set; }
        public float averageMatchesPerDay { get; set; }
        public int maximumPopulation { get; set; }
        public float averagePopulation { get; set; }
        public IEnumerable<string> top5GameModes { get; set; }
        public IEnumerable<string> top5Maps { get; set; }
    }

}

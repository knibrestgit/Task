using System;

namespace SWW.GStats.BusinessLogic.DTO
{
    public class MatchReportItem
    {
        public string server { get; set; }
        public DateTime timestamp { get; set; }
        public MatchItem results { get; set; }
    }


}

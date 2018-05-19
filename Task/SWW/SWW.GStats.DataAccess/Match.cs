using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWW.GStats.DataAccess
{
    public class Match
    {
        public int Id { get; set; }
        [MaxLength(60)]
        public string Map { get; set; }
        [MaxLength(3)]
        public string GameMode { get; set; }
        public int FragLimit { get; set; }
        public int TimeLimit { get; set; }
        public float TimeElapsed { get; set; }
        public DateTime Timestamp { get; set; }
        public string EndpointId { get; set; }
        public int PlayersCount { get; set; }
        public ICollection<Scoreboard> Scoreboard { get; set; }
    }

    public class Scoreboard
    {
        public int Id { get; set; }
        [MaxLength(80)]
        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public float Rating { get; set; }
        public Match Match { get; set; }
    }
}

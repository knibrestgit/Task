using System.ComponentModel.DataAnnotations;

namespace SWW.GStats.BusinessLogic.DTO
{

    public class MatchItem
    {
        [Required]
        [StringLength(60)]
        public string map { get; set; }
        [Required]
        [StringLength(3)]
        public string gameMode { get; set; }
        [Required]
        public int fragLimit { get; set; }
        [Required]
        public int timeLimit { get; set; }
        [Required]
        public float timeElapsed { get; set; }
        [Required]
        public ScoreboardItem[] scoreboard { get; set; }
    }

    public class ScoreboardItem
    {
        [Required]
        [StringLength(60)]
        public string name { get; set; }
        [Required]
        public int frags { get; set; }
        [Required]
        public int kills { get; set; }
        [Required]
        public int deaths { get; set; }
    }

}

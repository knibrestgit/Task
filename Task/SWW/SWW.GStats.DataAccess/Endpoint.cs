using System.ComponentModel.DataAnnotations;

namespace SWW.GStats.DataAccess
{
    public class Endpoint
    {
        [Key]
        [MaxLength(60)]
        public string Id { get; set; }

        [MaxLength(60)]
        public string Name { get; set; }

        [MaxLength(60)]
        public string GameModes { get; set; }
    }

}

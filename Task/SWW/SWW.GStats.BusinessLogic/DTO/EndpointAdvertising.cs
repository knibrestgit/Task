using System.ComponentModel.DataAnnotations;

namespace SWW.GStats.BusinessLogic.DTO
{
    public class EndpointAdvertising
    {
        [Required]
        [StringLength(60)]
        public string name { get; set; }
        [Required]
        public string[] gameModes { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Newtonsoft.Json;

namespace SafeStreet.Models
{
    public class UserRating
    {
        public int Id { get; set; }

        [Required]
        [JsonProperty("cpd_neighborhood")]
        public required string Neighborhood { get; set; }

        [Range(0, 100)]
        [DisplayName("Safety Rating")]
        public int SafetyRating { get; set; }

        [StringLength(500)]
        public required string Description { get; set; }

        [DisplayName("Date")]
        public DateTime SubmittedOn { get; set; } = DateTime.Now;
    }
}
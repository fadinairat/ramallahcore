using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class LookupCountries
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        [DisplayName("Title")]
        public string Name { get; set; } = String.Empty;

        [Required]
        [DisplayName("Arabic Title")]
        [StringLength(250)]
        public string ArName { get; set; } = String.Empty;

        [StringLength(10)]
        [DisplayName("Prefix")]
        public string Prefix { get; set; } = String.Empty;

        [StringLength(10)]
        [DisplayName("Flag")]
        public string Flag { get; set; } = String.Empty;

        public Boolean Deleted { get; set; } = false;

    }
}
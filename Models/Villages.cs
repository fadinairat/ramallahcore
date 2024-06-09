using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Villages
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = String.Empty;

        [Display(Name = "Arabic Name")]
        public string ArName { get; set; } = String.Empty;

        [Required]
        [Display(Name = "City")]
        [ForeignKey("City")]
        public int? CityId { get; set; }
        public virtual City? City { get; set; }

        [Display(Name = "Deleted")]
        public Boolean Deleted { get; set; } = false;
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Lookups
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

        public string Type { get; set; } = "Public";   //the type to determine the lookup for which list (ex. Employers Type)

        public int Priority { get; set; } = 999999;  // To Sorting the list of items

        public Boolean Editable { get; set; } = true;

        public Boolean Deleted { get; set; } = false;

    }
}

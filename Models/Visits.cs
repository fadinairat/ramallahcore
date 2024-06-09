using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Visits
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VisitsCount { get; set; } = 1;

    }
}

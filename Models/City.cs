using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
	public class City
	{
		[Key]
		public int Id { get; set; }

		[Required]
        [Display(Name = "Name")]
        [StringLength(50)]
		public string Name { get; set; }

		[Required]
        [Display(Name = "Arabic Name")]
        [StringLength(50)]
		public string ArName { get; set; }

        [Display(Name = "District")]
        [Required]
		public string District { get; set; } = "WestBank";

        [Display(Name = "Priority")]
        public int Priority { get; set; } = 999999;

		public virtual ICollection<City>? Jobs { get; set; }
        public virtual ICollection<Villages>? Villages { get; set; }
    }
}

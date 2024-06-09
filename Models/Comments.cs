using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
	public class Comments
	{
		[Key]
		public int Id { get; set; }

		[Required]
        [Display(Name = "Name")]
        [StringLength(100)]
		public string Name { get; set; } = String.Empty;

        [Display(Name = "Email")]
        [Required, StringLength(100)]
		public string Email { get; set; } = String.Empty;

        [Display(Name = "Mobile")]
        [Required, StringLength(100)]
        public string Mobile { get; set; } = String.Empty;

        [Display(Name = "Location")]
        [Required, StringLength(100)]
		public string Location { get; set; } = String.Empty;

		[Required]
        [Display(Name = "Subject")]
        [StringLength(100)]
		public string Subject { get; set; } = String.Empty;

		[Required]
        [Display(Name = "Body")]
        [Column(TypeName = "nvarchar(max)")]
		public string Body { get; set; } = String.Empty;

        [Display(Name = "Published")]
        public Boolean? Published { get; set; } = false;

        [Display(Name = "Reviewed")]
        public Boolean? Reviewed { get; set; } = false;

        [Display(Name = "Add Time")]
        [DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
		public DateTime AddTime { get; set; }

		public Boolean Deleted { get; set; } = false;

	}
}

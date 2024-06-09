using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ramallah.Models
{
	public class Suggesstions
	{
		[Key]
		public int Id { get; set; }


		[Required]
		[Display(Name = "Suggestion title")]
		public string Title { get; set; } = string.Empty;

		[Required]
		public string Body { get;set; } = string.Empty;

	}
}

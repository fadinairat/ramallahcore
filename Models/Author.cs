using System.ComponentModel.DataAnnotations;

namespace Ramallah.Models
{
    public class Author
    {
        public int AuthorId { get; set; } 

        [Display(Name="First Name")]
        public string FirstName { get; set; } = String.Empty;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = String.Empty;

    }
}

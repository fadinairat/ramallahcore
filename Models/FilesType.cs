using System.ComponentModel.DataAnnotations;

namespace Ramallah.Models
{
    public class FilesType
    {
        [Key]
        public int Id { get; set; } 

        [StringLength(25)]
        public string Title { get; set; } = String.Empty;

        [StringLength(25)]
        public string ArTitle { get; set; } = String.Empty;

    }
}

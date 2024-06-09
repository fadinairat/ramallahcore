using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class FormsEntries
    {
        [Required]
        [Key]
        public int Id { get; set; }        

        [Required]
        [ForeignKey("Form")]
        public int FormId { get; set; }
        public virtual Forms? Form { get; set; }

        
        [Display(Name = "Form Entry For")]
        public string? Type { get; set; } = "Job"; //Job/Page/Project


        [Required]
        [Display(Name = "IP Address")]
        public string? IpAddress { get; set; } = String.Empty;


        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        [Display(Name = "Creation Time")]
        public DateTime AddedTime { get; set; } = DateTime.Now;


        [Display(Name = "Edition Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? EditedTime { get; set; } = DateTime.Now;
        public Boolean Deleted { get; set; } = false;

        public virtual ICollection<FormsEntriesFields>? Entries { get; set; }
    }
}

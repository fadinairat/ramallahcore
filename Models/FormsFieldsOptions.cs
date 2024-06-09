using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class FormsFieldsOptions
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Field")]
        public int FieldId { get; set; }
        public virtual FormsFields? Field { get; set; }

        [Required]
        [DisplayName("Value")]
        [StringLength(500)]
        public string Value { get; set; } = String.Empty;

        [Required]
        [DisplayName("Arabic Value")]
        [StringLength(500)]
        public string ArValue { get; set; } = String.Empty;

        [Required]
        [DisplayName("Label")]
        [StringLength(500)]
        public string Label { get; set; } = String.Empty;

        [Required]
        [DisplayName("Arabic Label")]
        [StringLength(500)]
        public string ArLabel { get; set; } = String.Empty;

        public Boolean Selected { get; set; } = false;


        [DisplayName("Priority")]
        public int Priority { get; set; } = 999999;

        [ForeignKey("User")]
        [DisplayName("Added By")]
        public int AddedBy { get; set; }
        public virtual User? User { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime AddedTime { get; set; } = DateTime.Now;

        public Boolean Active { get; set; } = true;

        public Boolean Deleted { get; set; } = false;
    }
}

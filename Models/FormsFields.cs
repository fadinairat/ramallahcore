using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class FormsFields
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Form")]
        public int FormId { get; set; }
        public virtual Forms? Form { get; set; }

        [Required]
        [DisplayName("Field Title")]
        [StringLength(500)]
        public string Title { get; set; } = String.Empty;

        [Required]
        [DisplayName("Field Arabic Title")]
        [StringLength(500)]
        public string ArTitle { get; set; } = String.Empty;

        [Required]
        [DisplayName("Field Label")]
        [StringLength(500)]
        public string Label { get; set; } = String.Empty;

        [Required]
        [DisplayName("Field Arabic Label")]
        [StringLength(500)]
        public string ArLabel { get; set; } = String.Empty;

        [DisplayName("Place Holder")]
        [StringLength(500)]
        public string? PlaceHolder { get; set; } = String.Empty;

        [DisplayName("Arabic Place Holder")]
        [StringLength(500)]
        public string? ArPlaceHolder { get; set; } = String.Empty;

        [DisplayName("Description (Hint)")]
        public string? Description { get; set; } = String.Empty;

        [Required]
        [DisplayName("Type")]
        [StringLength(50)]
        public string Type { get; set; } = String.Empty; // Text, Number, Select, Checkbox, Radio-Group, Number, File,Date,TextArea

        
        [DisplayName("Sub Type")]
        [StringLength(50)]
        public string? SubType { get; set; } = String.Empty;

        [DisplayName("Minimum Answer Value")]
        public int? MinAnsNumber { get; set; } = null;

        [DisplayName("Minimum Answer Value")]
        public int? MaxAnsNumber { get; set; } = null;

        [DisplayName("Number Step")]
        public double? Step { get; set; } = null;

        [DisplayName("Required")]
        public Boolean Required { get; set; } = false;

        [DisplayName("Priority")]
        public int Priority { get; set; } = 999999;

        [DisplayName("Default Value")]
        public string? DefaultValue { get; set; } = String.Empty;

        [DisplayName("Minmum Length")]
        public int MinLength { get; set; } = 0;

        [DisplayName("Maximum Length")]
        public int MaxLength { get; set; } = 500;


        [DisplayName("Rows")]
        public int Rows { get; set; } = 3;


        [DisplayName("Allow Multiple")]
        public Boolean AllowMultiple { get; set; } = false;

        [DisplayName("Enable Other")]
        public Boolean EnableOther { get; set; } = false;

        [DisplayName("Toggle")]
        public Boolean Toggle { get; set; } = false;

        [DisplayName("Inline")]
        public Boolean Inline { get; set; } = false;

        [DisplayName("Class")]
        public string? Class { get; set; } = String.Empty;

        [DisplayName("Style")]
        public string? Style { get; set; } = String.Empty;


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

        public virtual ICollection<FormsFieldsOptions>? Options { get; set; }
        public virtual ICollection<FormsEntriesFields>? Entries { get; set; }
    }
}

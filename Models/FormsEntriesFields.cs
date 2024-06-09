using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class FormsEntriesFields
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Entry ID")]
        [ForeignKey("Entry")]
        public int EntryId { get; set; }
        public virtual FormsEntries? Entry { get; set; }

        [Required]
        [DisplayName("Question")]
        [ForeignKey("Field")]
        public int FieldId { get; set; }
        public virtual FormsFields? Field { get; set; }

        [DisplayName("Title")]
        [StringLength(500)]
        public string Title { get; set; } = String.Empty;

        [DisplayName("Label")]
        [StringLength(500)]
        public string Label { get; set; } = String.Empty;

        [DisplayName("Type")]
        [StringLength(500)]
        public string Type { get; set; } = String.Empty;

        [Required]
        [DisplayName("Answer")]
        public string Answer { get; set; } = String.Empty;

        public Boolean Deleted { get; set; } = false;
    }
}

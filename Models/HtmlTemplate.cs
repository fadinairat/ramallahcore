using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class HtmlTemplate
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Template Name")]
        public string Name { get; set; } = String.Empty;

        [Display(Name = "Template Arabic Name")]
        [StringLength(50)]
        public string ArName { get; set; } = String.Empty;


        [ForeignKey("HtmlTemplatesType")]
        [Display(Name = "Template Type")]
        public int? Type { get; set; } = 0;

        public virtual HtmlTemplatesType? HtmlTemplatesType { get; set; }   

        [StringLength(250)]
        [Display(Name = "File Path")]
        public string? FilePath { get; set; } = String.Empty;

        [Display(Name = "Language")]
        [ForeignKey("Language")]
        public Nullable<int> LangId { get; set; } = 0;

        public virtual Language? Language { get; set; }

        [Display(Name = "Added By")]
        [ForeignKey("User")]
        public Nullable<int> UserId { get; set; } = null;

        public virtual User? User { get; set; }

        [Display(Name = "Deleted")]
        public byte Deleted { get; set; } = 0;

        public virtual ICollection<Category>? Categories { get; set; }
        public virtual ICollection<Page>? Pages { get; set; }
        public virtual ICollection<Tag>? Tags { get; set; }

    }
}

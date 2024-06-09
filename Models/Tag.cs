using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Tag Name")]
        [StringLength(50)]
        public string Name { get; set; } = String.Empty;

        [Display(Name = "Slug")]
        [StringLength(250)]
        public string? Url { get; set; } = String.Empty;

        [ForeignKey("Language")]
        [Display(Name = "Language")]
        public Nullable<int> LangId { get; set; } = 1;
        public virtual Language? Language { get; set; }

        [ForeignKey("HtmlTemplate")]
        [Display(Name = "Template")]
        public Nullable<int> TempId { get; set; }

        public virtual HtmlTemplate? HtmlTemplate { get; set; }

        [Display(Name = "Arabic Name")]
        [StringLength(50)]
        public string ArName { get; set; } = String.Empty;

        [Display(Name = "Thumb")]
        [StringLength(250)]
        public string? Thumb { get; set; }

        [Display(Name = "User")]
        [ForeignKey("User")]
        public Nullable<int> UserId { get; set; } = null;

        public virtual User? User { get; set; }

        [Display(Name = "Items Per Page")]
        public int ItemsPerPage { get; set; } = 20;

        [Display(Name = "Deleted")]
        public byte Deleted { get; set; } = 0;

        //public virtual ICollection<TagRel> TagRels { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Files
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Category")]
        [Display(Name = "Category")]
        public Nullable<int> CatId { get; set; }

        public virtual Category? Category { get; set; }

        [StringLength(50)]
        [Display(Name = "File Title")]
        public string Name { get; set; } = String.Empty;

        [StringLength(50)]
        [Display(Name = "Arabic File Title")]
        public string ArName { get; set; } = String.Empty;

        [Display(Name = "Year")]
        public int? Year { get; set; }

        [Display(Name = "Parent")]
        public int Parent { get; set; } = 0;

        [Display(Name = "Publish")]
        public Boolean Publish { get; set; } = true;

        [Display(Name = "Active")]
        public Boolean Active { get; set; } = true;

        [Display(Name = "Thumb")]
        [StringLength(250)]
        public string? Thumb { get; set; } = String.Empty;

        [Display(Name = "Language")]
        [ForeignKey("Language")]
        public Nullable<int> LangId { get; set; } = 1;

        public virtual Language? Language { get; set; }


        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "English Description")]
        public string? Description { get; set; } = String.Empty;

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "Arabic Description")]
        public string? ArDescription { get; set; } = String.Empty;

        [Display(Name = "File")]
        [StringLength(250)]
        public string? FilePath { get; set; } = String.Empty;

        [Display(Name = "Embedding Source")]
        [Column(TypeName = "nvarchar(max)")]
        public string? Source { get; set; } = String.Empty;

        [Display(Name = "Priority")]
        public int Priority { get; set; } = 999999;

        [Display(Name = "Show Home")]
        public Boolean ShowHome { get; set; } = false;

        [Display(Name = "Allow Comment")]
        public Boolean AllowComment { get; set; } = false;

        [Display(Name = "Added By")]
        [ForeignKey("User")]
        public Nullable<int> UserId { get; set; } = null;


        public virtual User? User { get; set; }

        [Display(Name = "File Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? Date { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Add Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime AddDate { get; set; }

        
        [Display(Name = "Updated At")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Deleted")]
        public byte Deleted { get; set; } = 0;

        public virtual ICollection<FilesList>? FileList { get; set; }
    }
}

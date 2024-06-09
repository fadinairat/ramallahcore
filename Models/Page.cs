using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Page
    {       
        [Key]
        public int PageId { get; set; } 
        public int? TranslateId { get; set; }

        [Display(Name = "Page Title")]
        [StringLength(500)]
        public string Title { get; set; } = String.Empty;

        [Display(Name = "Page Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime PageDate { get; set; }

        [Display(Name = "Log Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime AddDate { get; set; } = DateTime.Now;

        [Display(Name = "Language")]
        [ForeignKey("Language")]
        public int? LangId { get; set; } = 1;

        public virtual Language? Language { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "Page Body")]
        public string? Body { get; set; } = String.Empty;

        [Display(Name = "Slug")]
        [StringLength(500)]
        public string? Slug { get; set; } = String.Empty;

        [Display(Name = "Url")]
        [StringLength(500)]
        public string? Url { get; set; } = String.Empty;

        [Display(Name = "Redirect Url")]
        [StringLength(500)]
        public string? RedirectUrl { get; set; } = String.Empty;

        [Display(Name = "Thumb")]
        [StringLength(500)]
        public string? Thumb { get; set; } = String.Empty;

        [Display(Name = "Internal Thumb")]
        [StringLength(500)]
        public string? Thumb2 { get; set; } = String.Empty;

        [Display(Name = "Show Thumb")]
        public Boolean ShowThumb { get; set; } = false;

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "Meta Description")]
        public string? MetaDescription { get; set; } = String.Empty;

        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "Meta Keywords")]
        public string? MetaKeywords { get; set; } = String.Empty;

        [Display(Name = "Template")]
        [ForeignKey("HtmlTemplate")]
        public int? TemplateId { get; set; }

        [Display(Name = "Page Form")]
        [ForeignKey("Form")]
        public int? FormId { get; set; }
        public virtual Forms? Form { get; set; }

        public virtual HtmlTemplate? HtmlTemplate { get; set; }

        [Display(Name = "Priority")]
        public int Priority { get; set; } = 999999;

        [Display(Name = "Publish")]
        public Boolean Publish { get; set; } = true;

        [Display(Name = "Active")]
        public Boolean Active { get; set; } = true;

        [Display(Name = "Show as Menu")]
        public Boolean AsMenu { get; set; } = false;

        [Display(Name = "Show as Main")]
        public Boolean ShowAsMain { get; set; } = false;


        [Display(Name = "Parent Page")]
        [ForeignKey("PageRef")]
        public int? Parent { get; set; }

        public virtual Page? PageRef { get; set; }

        [Display(Name = "Show in Search")]
        public Boolean ShowInSearchPage { get; set; } = true;

        [Display(Name = "Show in Sitemap")]
        public Boolean ShowInSiteMap { get; set; } = false;

        [Display(Name = "Show Date")]
        public Boolean ShowDate { get; set; } = true;

        [Display(Name = "Allow Comment")]
        public Boolean AllowComment { get; set; } = false;

        [Display(Name = "Show as Related")]
        public Boolean ShowAsRelated { get; set; } = false;


        [Column(TypeName = "nvarchar(max)")]
        [Display(Name = "Page Summary")]
        public string? Summary { get; set; } = String.Empty;

        [Display(Name = "Valid Until")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        public DateTime? ValidDate { get; set; }

        [Display(Name = "Sub Title")]
        [StringLength(500)]
        public string? SubTitle { get; set; } = String.Empty;

        [Display(Name = "Gallery")]
        public int? Gallery { get; set; } = 0;

        [Display(Name = "Show Related")]
        public Boolean ShowRelated { get; set; } = false;

        [Display(Name = "Sticky")]
        public Boolean Sticky { get; set; } = false;

        [Display(Name = "Deleted")]
        public Boolean Deleted { get; set; } = false;

        [Display(Name = "Archive")]
        public Boolean Archive { get; set; } = false;

        [Display(Name = "Views")]
        public int Views { get; set; } = 0;

        [Display(Name = "Video")]
        public int? Video { get; set; } = 0;

        [Display(Name = "Audio")]
        public int? Audio { get; set; } = 0;

        [Display(Name = "Added By")]
        [ForeignKey("User")]
        public int? UserId { get; set; } = null;
        public virtual User? UserAdd { get; set; }


        [Display(Name = "Edited By")]
        public int? EditedBy { get; set; } 

        [Display(Name = "Last Edit")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? LastEdit { get; set; }

        public virtual ICollection<PageCategory>? PageCategories { get; set; }
        //public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<TagRel>? TagRels { get; set; }
        public virtual ICollection<Page>? Pages { get; set; }
    }
}

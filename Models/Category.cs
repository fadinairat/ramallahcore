﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Category
    {
      

        [Key]
        public int Id { get; set; }

        [Display(Name = "English Name")]
        [StringLength(250)]
        public string Name { get; set; } = String.Empty;

        [Display(Name = "Arabic Name")]
        [StringLength(250)]
        public string ArName { get; set; } = String.Empty;

        [Display(Name = "Slug")]
        [StringLength(250)]
        public string? Slug { get; set; } = String.Empty;

        [Display(Name = "Thumbnail Image")]
        [StringLength(250)]
        public string? Thumb { get; set; } = String.Empty;

        public int Priority { get; set; } = 999999;

        [Display(Name = "Parent Category")]
        public Nullable<int> ParentId { get; set; } = 0;

        [ForeignKey("HtmlTemplate")]
        [Display(Name = "Category Template")]
        public int? TemplateId { get; set; } = 0;

        public virtual HtmlTemplate? HtmlTemplate { get; set; }

        [Display(Name = "Items Per Page")]
        public int ItemsPerPage { get; set; } = 20;

        [Display(Name = "Active")]
        public Boolean Active { get; set; } = true;

        [Display(Name = "Publish")]
        public Boolean Publish { get; set; } = true;

        [Display(Name = "Category Type")]
        [ForeignKey("CategoryTypes")]
        public Nullable<int> TypeId { get; set; }

        public virtual CategoryTypes? CategoryTypes { get; set; }

        [Display(Name = "English Description")]
        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; } = String.Empty;

        [Display(Name = "Arabic Description")]
        [Column(TypeName = "nvarchar(max)")]
        public string? ArDescription { get; set; } = String.Empty;

        [Display(Name = "Language")]
        [ForeignKey("Language")]
        public int? LangId { get; set; }

        public virtual Language? Language { get; set; }

        [Display(Name = "Show As Main Item")]
        public Boolean ShowAsMain { get; set; } = false;

        [Display(Name = "Show in Sitemap")]
        public Boolean ShowInSiteMap { get; set; } = false;

        [Display(Name = "Show Description")]
        public Boolean ShowDescription { get; set; } = true;

        [Display(Name = "Show Title")]
        public Boolean ShowTitle { get; set; } = true;

        [Display(Name = "Show Pages Thumb")]
        public Boolean ShowThumb { get; set; } = true;

        [Display(Name = "Show In Path")]
        public Boolean ShowInPath { get; set; } = true;

        [Display(Name = "Show In Search")]
        public Boolean ShowInSearch { get; set; } = true;

        [Display(Name = "Show Date")]
        public Boolean ShowDate { get; set; } = false;

        [Display(Name = "Show In Cat List")]
        public Boolean ShowInCatList { get; set; } = true;

        [Display(Name = "User")]
        [ForeignKey("User")]
        public int? UserId { get; set; }
        [NotMapped]
        public virtual User? User { get; set; }

        [Display(Name = "Deleted")]
        public byte Deleted { get; set; } = 0;

        public virtual ICollection<Files>? Files1 { get; set; }
        public virtual ICollection<PageCategory>? PageCategories { get; set; }
        //public virtual ICollection<Page> Pages { get; set; }
        
    }
}

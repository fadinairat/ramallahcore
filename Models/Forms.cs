using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Forms
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; } = String.Empty;

        [Required]
        [Display(Name = "Arabic Title")]
        public string ArTitle { get; set; } = String.Empty;

        [Display(Name = "Form Description")]
        public string? Description { get; set; } = String.Empty;

        [Display(Name = "Arabic Form Description")]
        public string? ArDescription { get; set; } = String.Empty;

        [Required]
        [Display(Name = "Direction")]
        public string Direction { get; set; } = "Auto";

        [Required]
        [Display(Name = "Form Type")]
        public int Type { get; set; } = 0; //0:Default is Public, 1:Job Application form, 2:Projects Form

        [Display(Name = "Type of Form")]
        public Boolean IsPublic { get; set; } = false; 

        [Display(Name = "Type of Form")]
        public Boolean IsJobForm { get; set; } = true;

        [Required]
        [ForeignKey("Language")]
        [Display(Name = "Language")]
        public int LangId { get; set; } 

        public virtual Language? Language { get; set; }

        [Display(Name = "Submit Label")]
        public string SubmitLabel { get; set; } = "Save";

        [Display(Name = "Arabic Submit Label")]
        public string ArSubmitLabel { get; set; } = "حفظ";

        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [Display(Name = "Form Start Date")]
        public DateTime? StartDate { get; set; } = DateTime.Now;


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}")]
        [Display(Name = "Form Expiry Date")]
        public DateTime? ExpireDate { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        [Display(Name = "Added By")]
        public int AddedBy { get; set; }    

        public virtual User? User { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Added Time")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime AddedTime { get; set; } = DateTime.Now;

        [ForeignKey("EdUser")]
        public int? EditedBy { get; set; }

        //public virtual User EdUser { get; set; } = new User();


        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy HH:mm}")]
        public DateTime? LastEdit { get; set; }

        public Boolean Active { get; set; } = true;

        public Boolean Deleted { get; set; } = false;
       
        public virtual ICollection<Page>? Pages { get; set; }
        public virtual ICollection<FormsEntries>? FormsEntries { get; set; }
        public virtual ICollection<FormsFields>? FormsFields { get; set; }
    }
}

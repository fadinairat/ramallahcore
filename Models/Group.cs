using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Group
    {        
        public int Id { get; set; }

        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; } = String.Empty;

        [Display(Name = "Arabic Name")]
        [StringLength(50)]
        public string ArName { get; set; } = String.Empty;

        [ForeignKey("Language")]
        [Display(Name = "Language")]
        [Required]
        public int LangId { get; set; } = 0;

        public virtual Language? Language { get; set; }

        [Display(Name = "Create Page")]
        public Boolean P1 { get; set; } = false;


        [Display(Name = "Editing Page")]
        public Boolean P2 { get; set; } = false;


        [Display(Name = "Deleting Page")]
        public Boolean P3 { get; set; } = false;


        [Display(Name = "Create Category")]
        public Boolean P4 { get; set; } = false;


        [Display(Name = "Editing Category")]
        public Boolean P5 { get; set; } = false;

        [Display(Name = "Deleting Category")]
        public Boolean P6 { get; set; } = false;

        [Display(Name = "Control Files")]
        public Boolean P7 { get; set; } = false;

        [Display(Name = "Control Tags")]
        public Boolean P8 { get; set; } = false;

        [Display(Name = "Control Menus")]
        public Boolean P9 { get; set; } = false;

        [Display(Name = "Control Templates")]
        public Boolean P10 { get; set; } = false;

        [Display(Name = "Control Users")]
        public Boolean P11 { get; set; } = false;

        [Display(Name = "Control Groups")]
        public Boolean P12 { get; set; } = false;
        public Boolean P13 { get; set; } = false;
        public Boolean P14 { get; set; } = false;
        public Boolean P15 { get; set; } = false;
        public Boolean P16 { get; set; } = false;

        public Boolean P17 { get; set; } = false;

        public Boolean P18 { get; set; } = false;

        public Boolean P19 { get; set; } = false;
        public Boolean P20 { get; set; } = false;

        [Display(Name = "Active")]
        public byte? Active { get; set; } = 1;   //To allow null for this column

        [Display(Name = "User")]
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public virtual User? GroupUser { get; set; }

        [Display(Name = "Deleted")]
        public byte Deleted { get; set; } = 0;

        
        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<GroupPermissions>? Permissions { get; set; }
    }
}

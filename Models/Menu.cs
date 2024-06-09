using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Menu
    {

        [Key]
        public int Id { get; set; }

        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; } = String.Empty;

        [ForeignKey("MenuLocation")]
        [Display(Name ="Menu Location")]
        [Required]
        public Nullable<int> LocationId { get; set; }
        public virtual MenuLocation? MenuLocation { get; set; }

        [Display(Name = "Target")]
        [StringLength(50)]
        public string Target { get; set; } = String.Empty;

        [ForeignKey("MenuParentRef")]
        [Display(Name ="Menu Parent")]
        public int? ParentId { get; set; } = 0;
        public virtual Menu? MenuParentRef { get; set; }

        [Display(Name = "Priority")]
        public int Priority { get; set; } = 999999;

        [Display(Name = "Link")]
        [StringLength(250)]
        public string? Link { get; set; }

        [ForeignKey("Language")]
        [Display(Name ="Language")]
        public int LangId { get; set; }
        public virtual Language? Language { get; set; }

        [Display(Name = "Active")]
        public byte Active { get; set; } = 1;

        [Display(Name = "User")]
        [ForeignKey("User")]
        public int? UserId { get; set; } = null;
        public virtual User? User { get; set; }

        [Display(Name = "Deleted")]
        public byte Deleted { get; set; } = 0;

        public virtual ICollection<Menu>? ParentMenus { get; set; }
    }
}

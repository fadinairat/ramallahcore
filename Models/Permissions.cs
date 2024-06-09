using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class Permissions
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; } = String.Empty;

        [Display(Name = "Controller")]
        public string Controller { get; set; } = String.Empty;

        [Display(Name = "Action")]
        public string Action { get; set; } = String.Empty;

        [Display(Name = "Area")]
        public string Area { get; set; } = String.Empty;

        [Display(Name = "Allowed")]
        public Boolean Allowed { get; set; } = true;

        [Display(Name = "User")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        public Boolean Reserved { get; set; } = false;

        public virtual ICollection<GroupPermissions>? PermissionsList { get; set; }

    }
}

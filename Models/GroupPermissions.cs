using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class GroupPermissions
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        public virtual Group? Group { get; set; }

        [ForeignKey("Permissions")]
        public int? PermissionId { get; set; }
        public virtual Permissions? Permissions { get; set; }
    }
}

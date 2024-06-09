using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ramallah.Models
{
    public class FormsFieldsTypes
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [DisplayName("Title")]
        public string Title { get; set; } = String.Empty;

        [Required]
        [DisplayName("Arabic Title")]
        public string ArTitle { get; set; } = String.Empty;

        [Required]
        [DisplayName("Type")]
        public string Type { get; set; } = String.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ramallah.Models
{
    [Table("ContactUs")]
    public partial class ContactUs
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "الاسم الكامل")]
        [Required(ErrorMessage = "هذا الحقل اجباري")]
        public string fullName { get; set; } = String.Empty;

        [Display(Name = "البريد الالكتروني")]
        [Required(ErrorMessage = "هذا الحقل اجباري")]
        [EmailAddress]
        public string email { get; set; } = String.Empty;

        [Display(Name = "نص الرسالة")]
        [Required(ErrorMessage = "هذا الحقل اجباري")]
        public string message { get; set; } = String.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الادخال")]
        [DisplayFormat(DataFormatString = "{0:g}")]
        public DateTime SystemDate { get; set; }

    }
}

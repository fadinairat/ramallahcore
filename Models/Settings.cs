using System.ComponentModel.DataAnnotations;

namespace Ramallah.Models
{
    public class Settings
    {
        public int Id { get; set; }

        [Display(Name = "HeaderBg")]
        [StringLength(50)]
        public string HeaderBg { get; set; } = String.Empty;

        [Display(Name = "Footer Color")]
        [StringLength(50)]
        public string FooterColor { get; set; } = String.Empty;

        [Display(Name = "Menu Font Color")]
        [StringLength(50)]
        public string MenuFontColor { get; set; } = String.Empty;

        [Display(Name = "Menu Font Hover Color")]
        [StringLength(50)]
        public string MenuFontHoverColor { get; set; } = String.Empty;

        [Display(Name = "Body Color")]
        [StringLength(50)]
        public string BodyColor { get; set; } = String.Empty;

        [Display(Name = "Titles Color")]
        [StringLength(50)]
        public string TitlesColor { get; set; } = String.Empty;

        [Display(Name = "Summary Color")]
        [StringLength(50)]
        public string SummaryColor { get; set; } = String.Empty;

        [Display(Name = "Left Box Color")]
        [StringLength(50)]
        public string LeftBoxColor { get; set; } = String.Empty;

        [Display(Name = "Control Default Lang")]
        public int ControlDefaultLang { get; set; } = 1;

        [Display(Name = "Web Default Lang")]
        public int WebDefaultLang { get; set; } = 0;

    }
}

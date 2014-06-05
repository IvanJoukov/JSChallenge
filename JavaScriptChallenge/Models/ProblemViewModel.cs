using System.ComponentModel.DataAnnotations;

namespace JavaScriptChallenge.Models
{
    public class ProblemViewModel
    {
        [Required]
        [Display(Name = "Problem Number")]
        public int ProblemNumber { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string ProblemTitle { get; set; }

        [Display(Name = "Unlocked")]
        public bool Unlocked { get; set; }
    }
}

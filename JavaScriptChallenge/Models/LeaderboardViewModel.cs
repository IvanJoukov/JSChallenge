using System.ComponentModel.DataAnnotations;

namespace JavaScriptChallenge.Models
{
    public class LeaderboardViewModel
    {

        [Display(Name = "Score")]
        public int Score { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Failed Attempts")]
        public int FailedAttempts { get; set; }

        [Display(Name = "Time to Solve")]
        public string TimeToSolve { get; set; }

        [Display(Name = "Submitted Solution")]
        public string SubmittedSolution { get; set; }
    }
}

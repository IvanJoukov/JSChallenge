//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JavaScriptChallenge
{
    using System;
    using System.Collections.Generic;
    
    public partial class Problem
    {
        public Problem()
        {
            this.ProblemInstances = new HashSet<ProblemInstance>();
        }
    
        public int Id { get; set; }
        public int ProblemNumber { get; set; }
        public string StarterSolutionCode { get; set; }
        public string SetupJavaScript { get; set; }
        public string CorrectAnswer { get; set; }
        public string ProblemTitle { get; set; }
        public string ProblemDescription { get; set; }
        public string CSVDisallowedWords { get; set; }
    
        public virtual ICollection<ProblemInstance> ProblemInstances { get; set; }
    }
}

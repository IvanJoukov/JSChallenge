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
    
    public partial class ProblemInstance
    {
        public int Id { get; set; }
        public int ProblemId { get; set; }
        public string UserId { get; set; }
        public string ExpectedSolution { get; set; }
        public System.DateTime startTime { get; set; }
        public Nullable<System.DateTime> solveTime { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
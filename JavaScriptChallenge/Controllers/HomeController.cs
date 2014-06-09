using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Data.Entity.Core.Objects;
using JavaScriptChallenge;
using JavaScriptChallenge.Models;

namespace JavaScriptChallenge.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            using (var ctx = new Entities())
            {
               var problems = ctx.Problems.GroupBy(p => p.ProblemNumber).Select(s => new ProblemViewModel
               {
                    ProblemNumber = s.FirstOrDefault().ProblemNumber,
                    ProblemTitle = s.FirstOrDefault().ProblemTitle,
                    Unlocked = false
               }).ToList();

               foreach(var p in problems)
               {
                   p.Unlocked = ProblemUnlocked(p.ProblemNumber);
               }
               return View(problems);
            }
        }

        [Authorize]
        private ActionResult Problem1()
        {
            ViewBag.Message = "Problem 1.";
            int sumOfPropertiesEndingWithA = 0;
            Dictionary<string, int> result = new Dictionary<string,int>(2000);
            for (int x = 0; x < 2000; x++) {
                string input = Path.GetRandomFileName().ToLower();
                string charsOnly = new String(input.Where(c => (c >= 'a' && c <= 'z')).ToArray());

                if (result.ContainsKey(charsOnly)) {
                    continue;
                }
                result.Add(charsOnly, x);
                if (charsOnly.EndsWith("a")) {
                    sumOfPropertiesEndingWithA += x;
                }
            }
            Problem problem = new Problem();
            problem.ProblemNumber = 1;
            problem.CorrectAnswer = sumOfPropertiesEndingWithA.ToString();
            problem.ProblemDescription = "Oh no!  Due to a garbled hyperspace transmission, your input data has been corrupted.  Fortunately, you suspect that you can salvage the data you need."
                + "You suspect that the data you want is numerical, and is spread across all the input object's properties that end in 'a'.  Find the sum of all those properties.";
            problem.ProblemTitle = "Sum Properties";
            problem.StarterSolutionCode = "(function solution(input) {\n    var result = \"solve for me\";\n" +
                "   // Put solution \n    return result;\n})(input);";

            ProblemInstance probInstance = new ProblemInstance();
            probInstance.UserId = User.Identity.GetUserId();
            probInstance.StartTime = DateTime.Now;
            probInstance.Problem = problem;

            using (var ctx = new Entities())
            {
                ctx.Problems.Add(problem);
                ctx.ProblemInstances.Add(probInstance);
                ctx.SaveChanges();
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsonObject = serializer.Serialize(result);
            ViewBag.SetupJavaScriptCode = "var input = JSON.parse('" + jsonObject + "');";
            ViewBag.ProblemId = problem.Id;
            ViewBag.ProblemInstanceId = probInstance.Id;
            ViewBag.ProblemDescription = problem.ProblemDescription;
            ViewBag.StarterSolutionCode = problem.StarterSolutionCode;

            return View();
        }

        [Authorize]
        public ActionResult Problem(int? id)
        {
            if (id.HasValue && !ProblemUnlocked(id.Value))
                return HttpNotFound();

            ActionResult customResult = CustomProblemMethod(id ?? -1);
            if (customResult != null)
                return customResult;

            Problem problem;
            ProblemInstance probInstance;
            using (var ctx = new Entities())
            {
                problem = ctx.Problems.FirstOrDefault(p => p.ProblemNumber == id);

                if (problem == null)
                    return HttpNotFound();

                probInstance = new ProblemInstance();
                probInstance.UserId = User.Identity.GetUserId();
                probInstance.StartTime = DateTime.Now;
                probInstance.Problem = problem;

                ctx.ProblemInstances.Add(probInstance);
                ctx.SaveChanges();
            }

            ViewBag.SetupJavaScriptCode = problem.SetupJavaScript;
            ViewBag.problemInstance = problem.Id;

            ViewBag.ProblemId = problem.Id;
            ViewBag.ProblemInstanceId = probInstance.Id;
            ViewBag.ProblemDescription = problem.ProblemDescription;
            ViewBag.StarterSolutionCode = problem.StarterSolutionCode;

            return View("Problem");
        }
    

        [Authorize]
        [HttpPost]
        public JsonResult Problem(int problemId, int problemInstanceId, string proposedSolution, string solutionCode)
        {
            bool result = false;

            using (var ctx = new Entities())
            {
                var userId = User.Identity.GetUserId();
                var problemInstances = ctx.ProblemInstances.Where(p => p.Id == problemInstanceId && 
                    p.UserId == userId && p.ProblemId == problemId);

                if (problemInstances.Any())
                {
                    var problemInstance = problemInstances.First();
                    result = problemInstance.Problem.CorrectAnswer == proposedSolution;
                    if (result) // correct answer
                    {
                        problemInstance.SolveTime = DateTime.Now;
                        if (!ctx.ProblemUnlocks.Any(pu => pu.UserId == userId && pu.ProblemNumber == problemInstance.Problem.ProblemNumber + 1))
                        {
                            ProblemUnlock unlockNext = new ProblemUnlock()
                                {
                                    //Unlock the next problem.
                                    ProblemNumber = problemInstance.Problem.ProblemNumber + 1,
                                    UserId = userId
                                };
                            ctx.ProblemUnlocks.Add(unlockNext);
                        }
                    }
                    else // wrong answer
                    {
                        problemInstances.First().FailedAttempts++;
                    }
                    problemInstances.First().SubmittedSolution = solutionCode;
                }
                ctx.SaveChanges();
            }

            return Json(result);
        }

        private ActionResult CustomProblemMethod(int problemNumber)
        {
            ActionResult result;

            // Any problem methods that need custom code should be added here
            // as needed.
            switch(problemNumber) {
                case 1:
                    result = Problem1();
                    break;
                default:
                    result = null;
                    break;
            }
            return result;
        }

        private bool ProblemUnlocked(int problemNumber)
        {
            using (var ctx = new Entities())
            {
                var userId = User.Identity.GetUserId();
                return ctx.ProblemUnlocks.Any(p => (p.UserId == userId || string.IsNullOrEmpty(p.UserId))
                    && p.ProblemNumber == problemNumber);
            }
        }
    }
}
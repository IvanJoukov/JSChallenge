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

namespace JavaScriptChallenge.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult Problem1()
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
            ProblemInstance prob = new ProblemInstance();
            prob.UserId = User.Identity.GetUserId();
            prob.ProblemId = 1;
            prob.startTime = DateTime.Now;
            prob.ExpectedSolution = sumOfPropertiesEndingWithA.ToString();

            using (var ctx = new Entities())
            {
                ctx.ProblemInstances.Add(prob);
                ctx.SaveChanges();
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsonObject = serializer.Serialize(result);
            ViewBag.serializedData = jsonObject;
            ViewBag.problemInstance = prob.Id;

            ViewBag.ProblemDescription = "Oh no!  Due to a garbled hyperspace transmission, your input data has been corrupted.  Fortunately, you suspect that you can salvage the data you need."
                + "You suspect that the data you want is numerical, and is spread across all the input object's properties that end in 'a'.  Find the sum of all those properties.";

            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult Problem1(int problemId, string proposedSolution)
        {
            bool result = false;

            using (var ctx = new Entities())
            {
                var userId = User.Identity.GetUserId();
                var problemInstances = ctx.ProblemInstances.Where(p => p.Id == problemId && 
                    p.UserId == userId && p.ProblemId == 1);

                if (problemInstances.Any())
                {
                    result = problemInstances.First().ExpectedSolution == proposedSolution;
                    if (result)
                    {
                        problemInstances.First().solveTime = DateTime.Now;
                    }
                }
                ctx.SaveChanges();
            }

            return Json(result);
        }
    }
}
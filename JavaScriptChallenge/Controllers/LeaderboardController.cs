﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JavaScriptChallenge;
using JavaScriptChallenge.Models;
using Microsoft.AspNet.Identity;

namespace JavaScriptChallenge.Controllers
{
    [Authorize(Roles="admin")]
    public class LeaderboardController : Controller
    {
        private Entities db = new Entities();

        private static int CompareLeadersByScore(LeaderboardViewModel x, LeaderboardViewModel y)
        {
            return y.Score.CompareTo(x.Score);
        }

        // GET: /Leaderboard/Scores
        [AllowAnonymous]
        public ActionResult Scores(int id)
        {
            IQueryable<ProblemInstance> probleminstances = db.ProblemInstances.Where(p => p.Problem.ProblemNumber == id && p.SolveTime != null).Include(p => p.AspNetUser).OrderBy(p => p.Id);


            var groupedByUser = probleminstances.GroupBy(p => p.AspNetUser);
            List<LeaderboardViewModel> leaders = new List<LeaderboardViewModel>();
            foreach (var user in groupedByUser)
            {
                var solvedInstance = user.FirstOrDefault(p => p.SolveTime != null);
                LeaderboardViewModel leader = new LeaderboardViewModel();
                leader.UserName = user.Key.UserName;
                leader.FailedAttempts = user.Count(p => p.SolveTime == null);
                
                if (solvedInstance != null) {
                    leader.SubmittedSolution = "You must solve this problem before you can see other's solutions.";
                    string currentUserId = User.Identity.GetUserId();
                    if (probleminstances.Any(p => p.UserId == currentUserId))
                        leader.SubmittedSolution = solvedInstance.SubmittedSolution;
                    TimeSpan timeToSolve = solvedInstance.SolveTime.Value - solvedInstance.StartTime;
                    leader.TimeToSolve = timeToSolve.ToString();
                    leader.Score = 1000 - (50 * leader.FailedAttempts) - 1 * timeToSolve.Seconds;
                }
                leaders.Add(leader);
            }

            leaders.Sort(CompareLeadersByScore);
            ViewBag.AvailableProblems = db.Problems.Select(p => p.ProblemNumber).Distinct();
            ViewBag.ProblemNumber = id;
            return View(leaders);
        }

        // GET: /Leaderboard/Scores
        [AllowAnonymous]
        public ActionResult OverallScores()
        {
            IQueryable<ProblemInstance> probleminstances = db.ProblemInstances.Where(p => p.SolveTime != null).Include(p => p.AspNetUser);

            var groupedByUser = probleminstances.GroupBy(p => p.AspNetUser);
            List<LeaderboardViewModel> leaders = new List<LeaderboardViewModel>();
            foreach (var user in groupedByUser)
            {
                LeaderboardViewModel leader = new LeaderboardViewModel();
                leader.UserName = user.Key.UserName;
                leader.ProblemsSolved = user.Count(p => p.SolveTime != null);

                // sum solved problems
                foreach (var solvedInstance in user.Where(p => p.SolveTime != null))
                {
                    TimeSpan timeToSolve = solvedInstance.SolveTime.Value - solvedInstance.StartTime;
                    leader.Score += 1000 - (50 * leader.FailedAttempts) - 1 * timeToSolve.Seconds;
                }
                leaders.Add(leader);
            }

            leaders.Sort(CompareLeadersByScore);
            ViewBag.AvailableProblems = db.Problems.Select(p => p.ProblemNumber).Distinct();
            return View(leaders);
        }

        // GET: /Leaderboard/
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            var probleminstances = db.ProblemInstances.Include(p => p.AspNetUser);

            return View(probleminstances.ToList());
        }

        // GET: /Leaderboard/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProblemInstance probleminstance = db.ProblemInstances.Find(id);
            if (probleminstance == null)
            {
                return HttpNotFound();
            }
            return View(probleminstance);
        }

        // GET: /Leaderboard/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName");
            return View();
        }

        // POST: /Leaderboard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public ActionResult Create([Bind(Include="Id,ProblemId,UserId,ExpectedSolution,StartTime,SolveTime,SubmittedSolution")] ProblemInstance probleminstance)
        {
            if (ModelState.IsValid)
            {
                db.ProblemInstances.Add(probleminstance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName", probleminstance.UserId);
            return View(probleminstance);
        }

        // GET: /Leaderboard/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProblemInstance probleminstance = db.ProblemInstances.Find(id);
            if (probleminstance == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName", probleminstance.UserId);
            return View(probleminstance);
        }

        // POST: /Leaderboard/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,ProblemId,UserId,ExpectedSolution,StartTime,SolveTime,SubmittedSolution")] ProblemInstance probleminstance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(probleminstance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "UserName", probleminstance.UserId);
            return View(probleminstance);
        }

        // GET: /Leaderboard/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProblemInstance probleminstance = db.ProblemInstances.Find(id);
            if (probleminstance == null)
            {
                return HttpNotFound();
            }
            return View(probleminstance);
        }

        // POST: /Leaderboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProblemInstance probleminstance = db.ProblemInstances.Find(id);
            db.ProblemInstances.Remove(probleminstance);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JavaScriptChallenge;

namespace JavaScriptChallenge.Controllers
{
    public class ProblemController : Controller
    {
        private Entities db = new Entities();

        // GET: /Problem/
        public async Task<ActionResult> Index()
        {
            return View(await db.Problems.ToListAsync());
        }

        // GET: /Problem/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Problem problem = await db.Problems.FindAsync(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            return View(problem);
        }

        // GET: /Problem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Problem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="StarterSolutionCode,SetupJavaScript,CorrectAnswer,ProblemTitle,ProblemDescription,CSVDisallowedWords")] Problem problem)
        {
            if (ModelState.IsValid)
            {
                db.Problems.Add(problem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(problem);
        }

        // GET: /Problem/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Problem problem = await db.Problems.FindAsync(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            return View(problem);
        }

        // POST: /Problem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,ProblemNumber,StarterSolutionCode,SetupJavaScript,CorrectAnswer,ProblemTitle,ProblemDescription,CSVDisallowedWords")] Problem problem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(problem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(problem);
        }

        // GET: /Problem/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Problem problem = await db.Problems.FindAsync(id);
            if (problem == null)
            {
                return HttpNotFound();
            }
            return View(problem);
        }

        // POST: /Problem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Problem problem = await db.Problems.FindAsync(id);
            db.Problems.Remove(problem);
            await db.SaveChangesAsync();
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

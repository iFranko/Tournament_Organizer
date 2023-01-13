using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tournament_Organizer.Data;
using Tournament_Organizer.Models;

namespace Tournament_Organizer.Controllers
{
    public class TournamentTeamController : Controller
    {
        private readonly TournamentOrganizerContext _context;

        public TournamentTeamController(TournamentOrganizerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This action returns the view of teams are in tournament
        /// </summary>
        /// <param name="pg">stores page number</param>
        /// <returns>Index view</returns>
        [Authorize(Roles = "Organizer")]
        // GET: TournamentTeam
        public async Task<IActionResult> Index(int pg = 1)
        {
            // get user id
            try
            {
                var id = from u in _context.Users
                         select u;
                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                }

                var userID = id.FirstOrDefault().Id;

                // get user torunament ID
                var tournament_ID = from x in _context.Tournament_User
                                    where x.User_ID == userID
                                    select x;


                if (!tournament_ID.Any())
                {
                    TempData["noTournament"] = "You can not view page till you create a tournamnet";
                    return RedirectToAction("Index", "Tournaments", TempData["noTournament"]);

                }
                var tournamentID = tournament_ID.FirstOrDefaultAsync().Result.Tournament_ID;

                //return a list of teams that is realted to signed in organizer
                var tournamentOrganizerContext = _context.Tournament_Team.Include(t => t.Team).Include(t => t.Tournament);
                var tournamentByOrganizerContext = tournamentOrganizerContext.Where(t => t.Tournament_ID == tournamentID).OrderByDescending(x => x.Active).ThenByDescending(x => x.Points.Value).ThenByDescending(x => x.Scores.Value);

                // for paging logic
                const int pageSize = 8;
                if (pg < 1)
                {
                    pg = 1;
                }

                int recsCount = tournamentByOrganizerContext.Count();
                var pager = new Pager(recsCount, pg, pageSize);
                int recSkip = (pg - 1) * pageSize;
                var data = tournamentByOrganizerContext.Skip(recSkip).Take(pager.PageSize).ToListAsync();
                ViewBag.Pager = pager;

                return View(await data);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }


        /// <summary>
        /// This action views the team info that needs to be edited
        /// </summary>
        /// <param name="tournamentID"></param>
        /// <param name="teamID"></param>
        /// <returns>edit view</returns>
        [Authorize(Roles = "Organizer")]
        // GET: TournamentTeam/Edit/5
        public async Task<IActionResult> Edit( Guid? tournamentID, Guid? teamID)
        {
            try
            {
                if (tournamentID == null)
                {
                    return NotFound();
                }

                var tournament_Team = await _context.Tournament_Team.FindAsync(tournamentID, teamID);
                if (tournament_Team == null)
                {
                    return NotFound();
                }


                //create 2 goup list
                List<string> group = new List<string>();
                group.Add("A");
                group.Add("B");
                group.Add("Not Applicable");

                ViewBag.Groups = new SelectList(group);
                //ViewBag.Groups = group;

                return View(tournament_Team);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// This action edit the team in tournament
        /// </summary>
        /// <param name="Tournament_ID">store the tournament id</param>
        /// <param name="Team_ID">stores the team id</param>
        /// <param name="tournament_Team">stores the object of tournament team</param>
        /// <returns>index view</returns>
        // POST: TournamentTeam/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? Tournament_ID, Guid? Team_ID, [Bind("Team_ID,Tournament_ID,Status,Active,NumberOfMatches,Group,Points,Scores")] Tournament_Team tournament_Team)
        {
            try
            {
                //create 2 goup list
                List<string> group = new List<string>();
                group.Add("A");
                group.Add("B");
                group.Add("Not Applicable");
                ViewBag.Groups = new SelectList(group);
                //ViewBag.Groups = group;

                var tournament = from x in _context.Tournament
                                 where x.ID == Tournament_ID
                                 select x;
                //get tournament current max teams
                var nextStageMaxTeams = tournament.FirstOrDefaultAsync().Result.NextStageMaxTeams;

                //get tournament current max teams
                var eachGroupCount = nextStageMaxTeams / 2;


                //only if the group is A or B
                if (tournament_Team.Group == "A" || tournament_Team.Group == "B")
                {

                    var groupTeamsCount = (from x in _context.Tournament_Team
                                     .Where(x => x.Tournament_ID == Tournament_ID && x.Group == tournament_Team.Group)
                                           select x).Count();

                    if (groupTeamsCount >= eachGroupCount)
                    {
                        ModelState.AddModelError("Group", $"Group {tournament_Team.Group} is Full Try Another Group or The Team is Already in This Group");
                    }


                }
                if (Tournament_ID != tournament_Team.Tournament_ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        if (tournament_Team.Status == "Approved")
                        {

                            tournament_Team.Active = "Yes";
                        }
                        else
                        {

                            tournament_Team.Active = "No";
                            tournament_Team.Group = "Not Applicable";
                        }
                        _context.Update(tournament_Team);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Tournament_TeamExists(tournament_Team.Tournament_ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

                return View(tournament_Team);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// checks if the team exist in the tournament team table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool Tournament_TeamExists(Guid id)
        {
            return _context.Tournament_Team.Any(e => e.Tournament_ID == id);
        }
    }
}

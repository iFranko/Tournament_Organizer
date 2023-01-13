using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tournament_Organizer.Data;
using Tournament_Organizer.Models;

namespace Tournament_Organizer.Controllers
{
    [Authorize(Roles = "Manager")]
    public class TeamUserController : Controller
    {
        private readonly TournamentOrganizerContext _context;

        public TeamUserController(TournamentOrganizerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// get a list of all players in the team
        /// </summary>
        /// <returns></returns>
        // GET: TeamUser
        public async Task<IActionResult> Index()
        {
            try
            {
                // get user id
                var id = from u in _context.Users
                         select u;
                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                }

                var userID = id.FirstOrDefault().Id;

                // get user team ID
                var team_ID = from x in _context.Team_User
                              where x.User_ID == userID
                              select x;


                if (!team_ID.Any())
                {
                    TempData["noTeam"] = "You can not view page till you create a team";
                    return RedirectToAction("Index", "Teams", TempData["noTeam"]);

                }
                var teamID = team_ID.FirstOrDefaultAsync().Result.Team_ID;


                var tournamentOrganizerContext = _context.Team_User.Include(t => t.Team).Include(t => t.User);
                var tournamentByPlayerContext = tournamentOrganizerContext.Where(t => t.User_Role == "Player" && t.Team_ID == teamID);
                return View(await tournamentByPlayerContext.ToListAsync());
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Get the edit view
        /// </summary>
        /// <param name="teamID">team id</param>
        /// <param name="userID">user id</param>
        /// <returns></returns>
        // GET: TeamUser/Edit/5
        public async Task<IActionResult> Edit(Guid? teamID, string userID)
        {
            try
            {
                if (teamID == null)
                {
                    return NotFound();
                }

                var team_User = await _context.Team_User.FindAsync(userID, teamID);
                if (team_User == null)
                {
                    return NotFound();
                }

                return View(team_User);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Update the player in the team
        /// </summary>
        /// <param name="Team_ID"team id></param>
        /// <param name="User_ID">user id</param>
        /// <param name="team_User">team user object</param>
        /// <returns></returns>
        // POST: TeamUser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid? Team_ID, string User_ID, [Bind("Team_ID,User_ID,Status,User_Role")] Team_User team_User)
        {
            try
            {
                if (User_ID != team_User.User_ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(team_User);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Team_UserExists(team_User.User_ID))
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

                return View(team_User);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }


        private bool Team_UserExists(string id)
        {
            return _context.Team_User.Any(e => e.User_ID == id);
        }
    }
}

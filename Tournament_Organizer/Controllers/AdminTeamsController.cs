using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tournament_Organizer.Data;
using Tournament_Organizer.Models;

namespace Tournament_Organizer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTeamsController : Controller
    {
        private readonly TournamentOrganizerContext _context;

        public AdminTeamsController(TournamentOrganizerContext context)
        {
            _context = context;
        }


        /// <summary>
        /// View a list of teams
        /// </summary>
        /// <param name="searchString">team name</param>
        /// <returns>list of teams</returns>
        // GET: Teams
        public async Task<IActionResult> Index(string searchString)
        {
            try
            {
                var id = from u in _context.Users
                         select u;

                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                }

                // get user id
                var userID = id.FirstOrDefault().Id;


                var teams = from m in _context.Team
                            select m;

                ViewBag.searchString = "";
                if (!String.IsNullOrEmpty(searchString))
                {
                    teams = teams.Where(t => t.Team_Name.ToLower().Contains(searchString.ToLower()));
                    ViewBag.searchString = searchString;
                }



                // check if user is in Manager role
                ViewBag.DetailManagerRole = User.IsInRole("Manager");
                // check if user is in player role
                ViewBag.DetailPlayerRole = User.IsInRole("Player");
                // check if user is in Admin role
                ViewBag.DetailAdminRole = User.IsInRole("Admin");
                // check if user is in Organizer role
                ViewBag.DetailOrganizerRole = User.IsInRole("Organizer");

                ViewBag.userID = userID;

                var teamExist = from x in _context.Team_User
                                .Where(x => x.User_ID == userID && x.User_Role == "Manager")
                                select x;
                if (teamExist.Any())
                {
                    ViewBag.TeamID = teamExist.SingleOrDefaultAsync().Result.Team_ID;
                    ViewBag.TeamExistMessage = "Our Records Show That You Have a Team Assigned to you";

                }

                //show a link if team is assigned to a tournament
                var teamPlayer = from x in _context.Team_User
                                     .Where(x => x.User_ID == userID && x.User_Role == "Player")
                                 select x;
                if (teamPlayer.Any())
                {
                    ViewBag.playerExistMessage = "View the team you are enrolled in";
                    ViewBag.teamPlayerID = teamPlayer.FirstOrDefaultAsync().Result.Team_ID;

                }


                return View(await teams.ToListAsync());
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
        /// <param name="id">team id</param>
        /// <returns></returns>
        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }



                var team = await _context.Team.FindAsync(id);
                if (team == null)
                {
                    return NotFound();
                }

                ViewBag.imgValue = "";
                if (team.Image != null)
                {
                    ViewBag.imgValue = Convert.ToBase64String(team.Image);
                }

                var Divisions = from x in _context.Division
                                select x;
                ViewData["Divisions"] = new SelectList(Divisions.ToListAsync().Result, "Division_Id", "DivisionName");

                return View(team);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Update the team object
        /// </summary>
        /// <param name="id">team id</param>
        /// <param name="team">team object</param>
        /// <param name="Image">image a file</param>
        /// <param name="imgValue">original image value</param>
        /// <returns></returns>
        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]                            
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,Created_Date_Time,Team_Name,JerseyColour,Max_Player_Team,Image,Ban,Country,City,Address,Zip_Code,Review,Division_Id")] Team team, List<IFormFile> Image, string imgValue)
        {
            try
            {
                if (id != team.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        if (Image.Count == 0 && imgValue != null)
                        {
                            byte[] chartData = Convert.FromBase64String(imgValue);
                            team.Image = chartData;
                        }
                        else if (Image.Count > 0)
                        {
                            foreach (var item in Image)
                            {
                                if (item.Length > 0)
                                {
                                    using (var stream = new MemoryStream())
                                    {
                                        await item.CopyToAsync(stream);
                                        team.Image = stream.ToArray();
                                    }
                                }
                            }
                        }
                        _context.Update(team);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TeamExists(team.ID))
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
                return View(team);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }


        private bool TeamExists(Guid id)
        {
            return _context.Team.Any(e => e.ID == id);
        }
    }
}

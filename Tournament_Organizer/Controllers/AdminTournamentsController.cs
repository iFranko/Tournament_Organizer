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
    public class AdminTournamentsController : Controller
    {
        private readonly TournamentOrganizerContext _context;

        public AdminTournamentsController(TournamentOrganizerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This action return the tournaments view
        /// </summary>
        /// <param name="searchString">tournament name</param>
        /// <returns> list of tournaments</returns>
        // GET: Tournaments
        public async Task<IActionResult> Index(string searchString)
        {
            try
            {
                var tournaments = from m in _context.Tournament
                                  select m;

                ViewBag.searchString = "";
                if (!String.IsNullOrEmpty(searchString))
                {
                    tournaments = tournaments.Where(t => t.Tournament_Name.ToLower()!.Contains(searchString.ToLower()));
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


                // check if the user has tournament
                var id = from u in _context.Users
                         select u;
                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                }

                var userID = id.FirstOrDefault().Id;


                var tournamentExit = from x in _context.Tournament_User
                                     where x.User_ID == userID
                                     select x;

                if (tournamentExit.Any())
                {
                    ViewBag.TournamentID = tournamentExit.SingleOrDefaultAsync().Result.Tournament_ID;
                    ViewBag.TournamnetExistMessage = "Our Records Show That You Have a Tournament Running";

                }

                //show a link if team is assigned to a tournament
                var teamManager = from x in _context.Team_User
                                     .Where(x => x.User_ID == userID && x.User_Role == "Manager")
                                  select x;
                if (teamManager.Any())
                {
                    var teamID = teamManager.FirstOrDefaultAsync().Result.Team_ID;
                    var teamExist = from x in _context.Tournament_Team
                                         .Where(x => x.Team_ID == teamID)
                                    select x;
                    if (teamExist.Any())
                    {
                        ViewBag.teamExistMessage = "View the tournament you are enrolled in";
                        ViewBag.tournamentTeamID = teamExist.FirstOrDefaultAsync().Result.Tournament_ID;

                    }
                }


                return View(await tournaments.ToListAsync());
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
        /// <param name="id">Tournament id</param>
        /// <returns></returns>
        // GET: Tournaments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var tournament = await _context.Tournament.FindAsync(id);
                if (tournament == null)
                {
                    return NotFound();
                }

                ViewBag.imgValue = "";
                if (tournament.Image != null)
                {
                    ViewBag.imgValue = Convert.ToBase64String(tournament.Image);
                }

                var Divisions = from x in _context.Division
                                select x;
                ViewData["Divisions"] = new SelectList(Divisions.ToListAsync().Result, "Division_Id", "DivisionName");

                return View(tournament);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }
        /// <summary>
        /// return the tournament edit view
        /// </summary>
        /// <param name="id">tournament id</param>
        /// <param name="tournament">tournament object</param>
        /// <param name="Image">image file</param>
        /// <param name="imgValue">original image value</param>
        /// <returns></returns>
        // POST: Tournaments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ID,Created_Date_Time,Tournament_Name,Start_Date,End_Date,Tournament_Type,Image,Max_Teams_Tournament,Review,Ban,Country,City,Address,Zip_Code,Rule_One,Rule_Two,Rule_Three,Rule_Four,Stage,NextStageMaxTeams,Max_Players_Per_Team,Division_Id")] Tournament tournament, List<IFormFile> Image, string imgValue)
        {
            try
            {
                if (id != tournament.ID)
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
                            tournament.Image = chartData;
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
                                        tournament.Image = stream.ToArray();
                                    }
                                }
                            }
                        }
                        _context.Update(tournament);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TournamentExists(tournament.ID))
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
                return View(tournament);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        private bool TournamentExists(Guid id)
        {
            return _context.Tournament.Any(e => e.ID == id);
        }
    }
}

using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Tournament_Organizer.Data;
using Tournament_Organizer.Models;

namespace Tournament_Organizer.Controllers
{

    
    public class TournamentsController : Controller

    {
        private readonly TournamentOrganizerContext _context;

        public TournamentsController(TournamentOrganizerContext context)
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
        /// Return a tournament details view
        /// </summary>
        /// <param name="id">store the tournament id</param>
        /// <returns>tournament view</returns>
        // GET: Tournaments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                //get organizer ID assigned to tournament
                var getOrganizerID = from u in _context.Tournament_User
                                     select u;
                if (getOrganizerID.Any())
                {
                    getOrganizerID = getOrganizerID.Where(t => t.Tournament_ID == id);
                }
                else
                {
                    return NotFound();
                }
                var organizerID = getOrganizerID.FirstOrDefault().User_ID;

                //get user ID
                var getUserID = from u in _context.Users
                                select u;
                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    getUserID = getUserID.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                    ViewBag.loggedinUser = true;
                }
                else
                {
                    ViewBag.loggedinUser = false;
                }
                var userID = getUserID.FirstOrDefault().Id;



                if (userID == organizerID)
                {
                    ViewBag.organizerTournamentOwener = true;
                }
                else
                {
                    ViewBag.organizerTournamentOwener = false;
                }


                var userReview = from t in _context.Review_User_Tournament
                                   .Where(x => x.Tournament_ID == id && x.User_ID == userID)
                                 select t;
                if (userReview.Any())
                {
                    ViewBag.userReview = userReview.FirstOrDefaultAsync().Result.Review;
                }
                else
                {
                    ViewBag.userReview = 0;
                }

                // check if user is in Manager role
                ViewBag.DetailManagerRole = User.IsInRole("Manager");
                // check if user is in player role
                ViewBag.DetailPlayerRole = User.IsInRole("Player");
                // check if user is in Admin role
                ViewBag.DetailAdminRole = User.IsInRole("Admin");
                // check if user is in Organizer role
                ViewBag.DetailOrganizerRole = User.IsInRole("Organizer");



                // get team associated with user id
                var getTeamUserID = from u in _context.Team_User
                             .Where(t => t.User_ID == userID)
                                    select u;

                // get max teams
                var getTournamentMaxTeams = from x in _context.Tournament
                             .Where(t => t.ID == id)
                                            select x;
                var maxTeams = getTournamentMaxTeams.FirstOrDefaultAsync().Result.Max_Teams_Tournament;

                // get number of teams signed up
                var getMaxTeamsSignedUp = from x in _context.Tournament_Team
                             .Where(t => t.Tournament_ID == id && t.Status == "Approved")
                                          select x;
                var currentTeamsSignedUp = getMaxTeamsSignedUp.Count();


                if (getTeamUserID.Any())
                {
                    var teamUserID = getTeamUserID.FirstOrDefault().Team_ID;
                    ViewBag.TeamUserID = teamUserID;

                    // get team status if exists
                    var getTeamTournamentStatus = from u in _context.Tournament_Team
                                 .Where(t => t.Team_ID == teamUserID)
                                                  select u;


                    if (getTeamTournamentStatus.Any())
                    {
                        //check if the team is already assigned to another tournament
                        Guid tournamentIDAssociatedWithTeam = getTeamTournamentStatus.FirstOrDefaultAsync().Result.Tournament_ID;
                        if (tournamentIDAssociatedWithTeam == id)
                        {
                            ViewBag.existingTournament = true;

                        }
                        else
                        {
                            ViewBag.existingTournament = false;
                            ViewBag.tournamentIDAssociatedWithTeam = tournamentIDAssociatedWithTeam;
                        }

                        //check team status 
                        var teamStatus = getTeamTournamentStatus.FirstOrDefault().Status;


                        if (teamStatus == "Pending")
                        {
                            ViewBag.getTeamTournamentStatus = "Cancel";
                            ViewBag.buttonStatus = "submit";
                            ViewBag.statusMessage = "Your team is " + teamStatus + ".";
                            ViewBag.statuClass = "dark";


                        }
                        else if (teamStatus == "Approved")
                        {
                            ViewBag.getTeamTournamentStatus = "Leave";
                            ViewBag.buttonStatus = "submit";
                            ViewBag.statusMessage = "Your team was " + teamStatus + ".";
                            ViewBag.statuClass = "success";
                            var matches = from t in _context.Tournament_Leaderboard
                                          .Where(x => x.Tournament_ID == id)
                                          select t;
                            if (matches.Any())
                            {
                                ViewBag.PastApproved = true;
                            }
                            else
                            {
                                ViewBag.PastApproved = false;
                            }
                        }
                        else
                        {
                            ViewBag.getTeamTournamentStatus = "Cancel";
                            ViewBag.buttonStatus = "submit";
                            ViewBag.statusMessage = "Your team was " + teamStatus + ". Please Contact the tournamnet Organizer to be reconsidered";
                            ViewBag.statuClass = "danger";
                        }


                    }
                    else
                    {

                        ViewBag.getTeamTournamentStatus = "Join";
                        ViewBag.buttonStatus = "submit";
                        //if the tournament is full set to true
                        if (currentTeamsSignedUp >= maxTeams)
                        {
                            ViewBag.TournamnetMaxedUp = true;
                        }
                        //otherwise false
                        else
                        {
                            ViewBag.TournamnetMaxedUp = false;
                        }
                    }


                }

                if (id == null)
                {
                    return NotFound();
                }

                var tournament = await _context.Tournament
                    .FirstOrDefaultAsync(m => m.ID == id);
                if (tournament == null)
                {
                    return NotFound();
                }


                //get teams assigned with tournament and are Approved
                var tournamentOrganizerContext = _context.Tournament_Leaderboard.Include(t => t.Team_One).Include(t => t.Team_Two).Include(t => t.Tournament);
                var tournamentByOrganizerContext = tournamentOrganizerContext.Where(t => t.Tournament_ID == id).OrderByDescending(x => x.Game_Status).ThenByDescending(x => x.Stage); ;



                ViewBag.Games = await tournamentByOrganizerContext.ToListAsync();




                //get tournament current max teams
                var NextStageMaxTeams = tournament.NextStageMaxTeams;
                //get total games in the tournament
                uint GamesCount = 0;
                if (NextStageMaxTeams <= 1)
                {
                    GamesCount = 1;
                }
                else
                {
                    GamesCount = NextStageMaxTeams / 2;
                }

                // check if only one team is left with active status
                var teamIDs = from m in _context.Tournament_Team
                            .Where(x => x.Tournament_ID == id && x.Status == "Approved" && x.Active == "Yes")
                              select m;
                var AvaiableTeams = teamIDs.Count();

                // if only one Active then that is the winner team
                if (AvaiableTeams == 1 && GamesCount == 1)
                {

                    foreach (var teamID in teamIDs)
                    {
                        //get team that are equal to IDs assigned with tournament
                        var team = from x in _context.Team
                                    .Where(t => t.ID == teamID.Team_ID)
                                   select x;
                        ViewBag.TeamName = $"{team.FirstOrDefaultAsync().Result.Team_Name}";
                        ViewBag.TeamID = team.FirstOrDefaultAsync().Result.ID;
                        ViewBag.State = "success";
                    }
                }
                // get admin email
                var adminEmail = (from u in _context.Users
                                .Where(t => t.User_Role == "Admin")
                                  select u).FirstOrDefaultAsync().Result.Email;
                ViewBag.adminEmail = adminEmail;


                var divisionId = tournament.Division_Id;
                var Division = from m in _context.Division
                               .Where(x => x.Division_Id == divisionId)
                               select m;
                ViewBag.DivisionName = Division.FirstOrDefaultAsync().Result.DivisionName;

                return View(tournament);
            }
            catch 
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }


        /// <summary>
        /// this action gets the create view of tournament 
        /// </summary>
        /// <returns>tournament create view</returns>
        [Authorize(Roles = "Organizer")]
        // GET: Tournaments/Create
        public IActionResult Create()
        {
            try
            {
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
                    return RedirectToAction(nameof(Index));

                }

                var Divisions = from x in _context.Division
                               .OrderBy(d => d.DivisionName)
                                select x;
                if (Divisions.Any())
                {
                    ViewData["Divisions"] = new SelectList(Divisions.ToListAsync().Result, "Division_Id", "DivisionName");
                }

                return View();
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// This action create the tournament with post method
        /// </summary>
        /// <param name="tournament">Tournament object</param>
        /// <param name="Image">image value in list</param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        // POST: Tournaments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Created_Date_Time,Tournament_Name,Start_Date,End_Date,Tournament_Type,Image,Max_Teams_Tournament,Review,Ban,Country,City,Address,Zip_Code,Rule_One,Rule_Two,Rule_Three,Rule_Four,Max_Players_Per_Team,Division_Id")] Tournament tournament, List<IFormFile> Image)
        {
            try
            {
                var Divisions = from x in _context.Division
                               .OrderBy(d => d.DivisionName)
                                select x;
                if (Divisions.Any())
                {
                    ViewData["Divisions"] = new SelectList(Divisions.ToListAsync().Result, "Division_Id", "DivisionName");
                }
                //get user id
                var id = from u in _context.Users
                         select u;
                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                }

                var userID = id.FirstOrDefault().Id;

                //check if for duplicate tournament names
                var tournamentName = from m in _context.Tournament
                                     where m.Tournament_Name == tournament.Tournament_Name
                                     select m;
                //send error message if duplicate name exists
                if (tournamentName.Any())
                {
                    ModelState.AddModelError("Tournament_Name", "Name Exist, Please Try another Name.");
                }

                if (tournament.Start_Date < DateTimeOffset.Now)
                {
                    ModelState.AddModelError("Start_Date", "Start Date/Time can't be in past");
                }

                if (tournament.End_Date <= tournament.Start_Date)
                {
                    ModelState.AddModelError("End_Date", "End Date can't be before or same Start date");
                }

                if (Image.Count == 0)
                {
                    ModelState.AddModelError("Image", "Please upload a logo");
                }
                if (ModelState.IsValid)
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

                    tournament.Stage = 1;
                    tournament.NextStageMaxTeams = tournament.Max_Teams_Tournament;
                    tournament.ID = Guid.NewGuid();
                    Tournament_User tu = new Tournament_User();
                    tu.User_ID = userID;
                    tu.Tournament = tournament;
                    _context.Add(tournament);
                    _context.Add(tu);
                    await _context.SaveChangesAsync();
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

        /// <summary>
        /// Get the edit view
        /// </summary>
        /// <param name="id">Tournament id</param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
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
                                .OrderBy(d => d.DivisionName)
                                select x;
                if (Divisions.Any())
                {
                    ViewData["Divisions"] = new SelectList(Divisions.ToListAsync().Result, "Division_Id", "DivisionName");
                }

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
        [Authorize(Roles = "Organizer")]
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
                if (tournament.Start_Date < tournament.Created_Date_Time)
                {
                    TempData["DateIssues"] = "Start Date/time can't be set before the created date of this torunament";
                    ModelState.AddModelError("Start_Date", "Start Date/time can't be set before the created date of this torunament");
                    return RedirectToAction("Edit", new { id = tournament.ID });
                }

                if (tournament.End_Date <= tournament.Start_Date)
                {
                    TempData["DateIssues"] = "End Date can't be before or same Start date";
                    ModelState.AddModelError("End_Date", "End Date can't be before or same Start date");
                    return RedirectToAction("Edit", new { id = tournament.ID });
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

        /// <summary>
        /// Get the delete view of the selected tournament
        /// </summary>
        /// <param name="id">tournament id</param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        // GET: Tournaments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var tournament = await _context.Tournament
                    .FirstOrDefaultAsync(m => m.ID == id);
                if (tournament == null)
                {
                    return NotFound();
                }
                var NextStageMaxTeams = tournament.NextStageMaxTeams;
                var matches = from t in _context.Tournament_Leaderboard
                            .Where(x => x.Tournament_ID == id)
                              select t;

                if (matches.Any() && NextStageMaxTeams > 1)
                {
                    TempData["Delete"] = "You can't delete the tournament now due to its live status. Once the tournament is over, you can delete it if you wish to";
                    return RedirectToAction("Details", "Tournaments", new { id = tournament.ID });
                }
                else if (NextStageMaxTeams == 1)
                {
                    return View(tournament);
                }
                else
                {
                    return View(tournament);
                }
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }

        }
        /// <summary>
        /// Deletes teh selected tournament
        /// </summary>
        /// <param name="id">tournament id</param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        // POST: Tournaments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                //delete all teams assigned with tournament
                var teamAssigned = (from t in _context.Tournament_Team
                           .Where(x => x.Tournament_ID == id)
                                    select t).ToList();
                foreach (var rec in teamAssigned)
                {
                    _context.Tournament_Team.Remove(rec);
                }
                //delete tournamnet user relationship
                var userAssigned = (from t in _context.Tournament_User
                           .Where(x => x.Tournament_ID == id)
                                    select t).ToList();
                foreach (var rec in userAssigned)
                {
                    _context.Tournament_User.Remove(rec);
                }

                //delete the whole leaderboard of torunament
                var leaderBoard = (from t in _context.Tournament_Leaderboard
                           .Where(x => x.Tournament_ID == id)
                                   select t).ToList();
                foreach (var rec in leaderBoard)
                {
                    _context.Tournament_Leaderboard.Remove(rec);
                }


                var tournament = await _context.Tournament.FindAsync(id);
                _context.Tournament.Remove(tournament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// check if the tournament exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        private bool TournamentExists(Guid id)
        {
            return _context.Tournament.Any(e => e.ID == id);
        }


        /// <summary>
        /// This method adds team to tournament
        /// </summary>
        /// <param name="teamID">team id</param>
        /// <param name="TournamentID">tournament id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinTournament(Guid teamID, Guid TournamentID)
        {
            try
            {
                // check if team division is matching tournament division
                var teamDivisionID = (from m in _context.Team
                               .Where(x => x.ID == teamID)
                                      select m).FirstOrDefaultAsync().Result.Division_Id;
                var tournamentDivisionID = (from m in _context.Tournament
                              .Where(x => x.ID == TournamentID)
                                            select m).FirstOrDefaultAsync().Result.Division_Id;

                if (teamDivisionID != tournamentDivisionID)
                {

                    var Division = (from m in _context.Division
                                   .Where(x => x.Division_Id == tournamentDivisionID)
                                    select m).FirstOrDefaultAsync().Result.DivisionName;
                    TempData["divisionNotMatched"] = $"Your team is not in the same tournament division. This tournamnet division is {Division}";
                    return RedirectToAction("Details", new { id = TournamentID });
                }
                else
                {


                    Tournament_Team tournamnetTeam = new Tournament_Team();
                    tournamnetTeam.Tournament_ID = TournamentID;
                    tournamnetTeam.Team_ID = teamID;
                    tournamnetTeam.Active = "No";
                    tournamnetTeam.Status = "Pending";
                    _context.Add(tournamnetTeam);
                    await _context.SaveChangesAsync();


                    return RedirectToAction("Details", new { id = TournamentID });
                }
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }

        }


        /// <summary>
        /// this method removes a team from the tournament
        /// </summary>
        /// <param name="teamID">team id</param>
        /// <param name="TournamentID">tournament id</param>
        /// <returns></returns>
        // GET: Tournament_Team/Cancel/5
        public async Task<IActionResult> Cancel(Guid teamID, Guid TournamentID)
        {
            try
            {
                if (teamID == null)
                {
                    return NotFound();
                }

                var tournamnet_Team = await _context.Tournament_Team
                    .Include(t => t.Team)
                    .Include(t => t.Tournament)
                    .FirstOrDefaultAsync(m => m.Team_ID == teamID && m.Tournament_ID == TournamentID);


                var liveTournament = (from t in _context.Tournament_Leaderboard
                           .Where(x => x.Tournament_ID == TournamentID)
                                      select t).ToList();
                var tournament = (from t in _context.Tournament
                             .Where(x => x.ID == TournamentID)
                                  select t).FirstOrDefaultAsync().Result;

                if (tournamnet_Team == null)
                {
                    return NotFound();
                }
                //check if tournament is live
                if (liveTournament.Any() && tournamnet_Team.Status == "Approved" && tournament.NextStageMaxTeams != 1)
                {
                    TempData["liveTournament"] = $"You can't leave tournament now the tournament state live.";
                    return RedirectToAction("Details", new { id = TournamentID });
                }
                else
                {
                    _context.Tournament_Team.Remove(tournamnet_Team);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = TournamentID });
                }
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }

        }


        /// <summary>
        /// This method is used to rate the tournament
        /// </summary>
        /// <param name="review">review int</param>
        /// <param name="tournamentID">tournament id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int review, Guid tournamentID)
        {
            try
            {
                TempData["notValidReview"] = null;
                if (review < 1 || review > 5)
                {
                    TempData["notValidReview"] = review;
                    return RedirectToAction("Details", new { id = tournamentID });
                }
                else
                {

                    //get user id and role
                    var id = from u in _context.Users
                             select u;
                    if (!String.IsNullOrEmpty(User.Identity.Name))
                    {
                        id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                    }

                    var userID = id.FirstOrDefault().Id;
                    var userRole = id.FirstOrDefault().User_Role;

                    //delete the review if the user has one and submit a new one
                    var hasReview = (from t in _context.Review_User_Tournament
                                     .Where(x => x.Tournament_ID == tournamentID && x.User_ID == userID)
                                     select t).ToList();
                    foreach (var rec in hasReview)
                    {
                        _context.Review_User_Tournament.Remove(rec);
                    }




                    Review_User_Tournament reviewUserTournament = new Review_User_Tournament();
                    reviewUserTournament.Tournament_ID = tournamentID;
                    reviewUserTournament.User_ID = userID;
                    reviewUserTournament.User_Role = userRole;
                    reviewUserTournament.Review = review;
                    _context.Add(reviewUserTournament);

                    // update tournament review avg
                    var totalReviews = (from t in _context.Review_User_Tournament
                                       .Where(x => x.Tournament_ID == tournamentID)
                                        select t).ToList();
                    double count = 0.0;
                    double avg = 0.0;
                    double total = 0.0;
                    foreach (var rec in totalReviews)
                    {
                        total += rec.Review;
                        count++;
                    }
                    //if there is records
                    if (count > 0)
                    {
                        avg = total / count;
                    }
                    //otherwise use the default
                    else
                    {
                        total = review;
                        count = 1;
                        avg = total / count;
                    }

                    var tournament = await _context.Tournament.FindAsync(tournamentID);
                    tournament.Review = Math.Round(avg, 1);
                    _context.Update(tournament);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = tournamentID });
                }
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }

        }
    }
}

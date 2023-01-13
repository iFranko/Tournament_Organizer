using System;
using System.Collections.Generic;
using System.Data;
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
    public class TeamsController : Controller
    {
        private readonly TournamentOrganizerContext _context;


        public TeamsController(TournamentOrganizerContext context)
        {
            _context = context;

        }

        /// <summary>
        /// View a list of teams
        /// </summary>
        /// <param name="searchString">team name</param>
        /// <returns>list of teams</returns>
        // GET: Teams
        public async Task<IActionResult> Index( string searchString)
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
                    teams = teams.Where(t => t.Team_Name.ToLower()!.Contains(searchString.ToLower()));
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
        /// View of a team object with its details
        /// </summary>
        /// <param name="id">team id</param>
        /// <returns>a team object</returns>
        // GET: Teams/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            try
            {
                //get manager ID assigned to tournament
                var getManagerID = from u in _context.Team_User
                                   select u;
                if (getManagerID.Any())
                {
                    getManagerID = getManagerID.Where(t => t.Team_ID == id && t.User_Role == "Manager");
                }
                var managerID = getManagerID.FirstOrDefault().User_ID;

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



                if (userID == managerID)
                {
                    ViewBag.managerTeamOwener = true;
                }
                else
                {
                    ViewBag.managerTeamOwener = false;
                }


                var userReview = from t in _context.Review_User_Team
                                 .Where(x => x.Team_ID == id && x.User_ID == userID)
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


                // get max players
                var getTeamMaxPlayers = from x in _context.Team
                             .Where(t => t.ID == id)
                                        select x;
                var maxPlayers = getTeamMaxPlayers.FirstOrDefaultAsync().Result.Max_Player_Team;

                // get number of players signed up
                var getMaxPlayersSignedUp = from x in _context.Team_User
                             .Where(t => t.Team_ID == id && t.Status == "Approved" && t.User_Role == "Player")
                                            select x;
                var currentPlayersSignedUp = getMaxPlayersSignedUp.Count();



                if (userID.Any())
                {

                    ViewBag.PlayerUserID = userID;

                    // get player status if exists
                    var getPlayerTeamStatus = from u in _context.Team_User
                                 .Where(t => t.User_ID == userID)
                                              select u;


                    if (getPlayerTeamStatus.Any())
                    {
                        //check if the player is already assigned to another team
                        Guid teamIDAssociatedWithPlayer = getPlayerTeamStatus.FirstOrDefaultAsync().Result.Team_ID;
                        if (teamIDAssociatedWithPlayer == id)
                        {
                            ViewBag.existingPlayer = true;

                        }
                        else
                        {
                            ViewBag.existingPlayer = false;
                            ViewBag.teamIDAssociatedWithPlayer = teamIDAssociatedWithPlayer;
                        }

                        //check team status 
                        var playerStatus = getPlayerTeamStatus.FirstOrDefault().Status;


                        if (playerStatus == "Pending")
                        {
                            ViewBag.getPlayerTeamStatus = "Cancel";
                            ViewBag.buttonStatus = "submit";
                            ViewBag.statusMessage = "Your status is " + playerStatus + ".";
                            ViewBag.statuClass = "dark";
                        }
                        else if (playerStatus == "Approved")
                        {
                            ViewBag.getPlayerTeamStatus = "Leave";
                            ViewBag.buttonStatus = "submit";
                            ViewBag.statusMessage = "Your status was " + playerStatus + ".";
                            ViewBag.statuClass = "success";
                        }
                        else
                        {
                            ViewBag.getPlayerTeamStatus = "Cancel";
                            ViewBag.buttonStatus = "submit";
                            ViewBag.statusMessage = "Your status was " + playerStatus + ". Please Contact the team manager to be reconsidered";
                            ViewBag.statuClass = "danger";
                        }


                    }
                    else
                    {

                        ViewBag.getPlayerTeamStatus = "Join";
                        ViewBag.buttonStatus = "submit";
                        //if the team is full set to true
                        if (currentPlayersSignedUp >= maxPlayers)
                        {
                            ViewBag.TeamMaxedUp = true;
                        }
                        //otherwise false
                        else
                        {
                            ViewBag.TeamMaxedUp = false;
                        }
                    }

                }


                if (id == null)
                {
                    return NotFound();
                }

                var team = await _context.Team
                    .FirstOrDefaultAsync(m => m.ID == id);
                if (team == null)
                {
                    return NotFound();
                }



                //get teams assigned with tournament and are Approved
                var tournamentOrganizerContext = _context.Team_User.Include(t => t.Team).Include(t => t.User);
                var tournamentByOrganizerContext = tournamentOrganizerContext.Where(t => t.Team_ID == id && t.User_Role == "Player" && t.Status == "Approved");

                ViewBag.Players = await tournamentByOrganizerContext.ToListAsync();
                // get admin email
                var adminEmail = (from u in _context.Users
                                .Where(t => t.User_Role == "Admin")
                                  select u).FirstOrDefaultAsync().Result.Email;
                ViewBag.adminEmail = adminEmail;


                var divisionId = team.Division_Id;
                var Division = from m in _context.Division
                               .Where(x => x.Division_Id == divisionId)
                               select m;
                ViewBag.DivisionName = Division.FirstOrDefaultAsync().Result.DivisionName;

                return View(team);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error Occurred {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Get the create view of object
        /// </summary>
        /// <param name="user">user object</param>
        /// <returns></returns>
        // GET: Teams/Create
        [Authorize(Roles = "Manager")]
        public IActionResult Create(User user)
        {
            try
            {
                var id = from u in _context.Users
                         select u;

                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                }

                // get user id and role
                var userID = id.FirstOrDefault().Id;



                var Team_Exit = from x in _context.Team_User
                                where x.User_ID == userID
                                select x;

                if (Team_Exit.Any())
                {
                    ViewBag.TeamID = Team_Exit.SingleOrDefaultAsync().Result.Team_ID;
                    ViewBag.TeamExistMessage = "Our Records Show That You Have a Team";
                    return RedirectToAction(nameof(Index));

                }

                var Divisions = from x in _context.Division
                                .OrderBy(d => d.DivisionName)
                                select x;
                ViewData["Divisions"] = new SelectList(Divisions.ToListAsync().Result, "Division_Id", "DivisionName");

                return View();
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Create a team object
        /// </summary>
        /// <param name="team">team object</param>
        /// <param name="Image">image a file</param>
        /// <returns></returns>
        // POST: Teams/Create
        [Authorize(Roles = "Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Created_Date_Time,Team_Name,JerseyColour,Max_Player_Team,Image,Ban,Country,City,Address,Zip_Code,Review,Division_Id")] Team team, List<IFormFile> Image)
        {
            try
            {

                var teamName = from m in _context.Team
                               where m.Team_Name == team.Team_Name
                               select m;
                //send error message if duplicate name exists
                if (teamName.Any())
                {
                    ModelState.AddModelError("Team_Name", "Name Exist, Please Try another Name.");
                }

                if (Image.Count == 0)
                {
                    ModelState.AddModelError("Image", "Please upload a logo");
                }


                if (ModelState.IsValid)
                {
                    var id = from u in _context.Users
                             select u;

                    if (!String.IsNullOrEmpty(User.Identity.Name))
                    {
                        id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                    }

                    // get user id and role
                    var userID = id.FirstOrDefault().Id;
                    var userRole = id.FirstOrDefault().User_Role;

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


                    team.ID = Guid.NewGuid();
                    Team_User team_user = new Team_User();
                    team_user.User_ID = userID;
                    team_user.Team = team;
                    team_user.Status = "Approved";
                    team_user.User_Role = userRole;
                    _context.Add(team);
                    _context.Add(team_user);
                    await _context.SaveChangesAsync();
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
        /// <summary>
        /// Get the edit view
        /// </summary>
        /// <param name="id">team id</param>
        /// <returns></returns>
        // GET: Teams/Edit/5
        [Authorize(Roles = "Manager")]
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
                //get all divisions
                var Divisions = from x in _context.Division
                                .OrderBy(d => d.DivisionName)
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
        [Authorize(Roles = "Manager")]
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

        /// <summary>
        /// Get the delete view of object
        /// </summary>
        /// <param name="id">team id</param>
        /// <returns></returns>
        // GET: Teams/Delete/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var team = await _context.Team
                    .FirstOrDefaultAsync(m => m.ID == id);
                if (team == null)
                {
                    return NotFound();
                }


                return View(team);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Delets the team object
        /// </summary>
        /// <param name="id">team id</param>
        /// <returns></returns>
        // POST: Teams/Delete/5
        [Authorize(Roles = "Manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                

                //delete all users assigned with team
                var usersAssigned = (from t in _context.Team_User
                           .Where(x => x.Team_ID == id)
                                     select t).ToList();
                foreach (var users in usersAssigned)
                {
                    var user = await _context.Team_User.FindAsync(users.User_ID, id);
                    _context.Team_User.Remove(user);
                }


                //update team one in leader board that forfeited the tournament
                var teamOneInLeaderBoard = (from t in _context.Tournament_Leaderboard
                                       .Where(x => x.Team_One_ID == id)
                                            select t).ToList();

                var tournamentID = (from t in _context.Tournament_Team
                                      .Where(x => x.Team_ID == id)
                                            select t).FirstOrDefaultAsync().Result.Tournament_ID;

                var tournamentNExtStage = (from t in _context.Tournament
                           .Where(x => x.ID == tournamentID)
                                           select t).FirstOrDefaultAsync().Result.NextStageMaxTeams;
                //if tournament is not over
                if (tournamentNExtStage > 1)
                {


                    foreach (var teams in teamOneInLeaderBoard)
                    {

                        var tournamentTeamOne = await _context.Tournament_Team.FindAsync(teams.Tournament_ID, teams.Team_One_ID);
                        var tournamentTeamTwo = await _context.Tournament_Team.FindAsync(teams.Tournament_ID, teams.Team_Two_ID);

                        if (teams.Stage == 1)
                        {
                            if (teams.Game_Status != "Completed" && teams.Game_Status != "Terminate")
                            {
                                teams.Team_One_Score = 0;
                                teams.Team_Two_Score = 1;
                                teams.Game_Status = "Completed";
                                //set team one
                                tournamentTeamOne.Scores += 0;
                                tournamentTeamOne.Points += 0;
                                //set team two 
                                tournamentTeamTwo.Scores += 1;
                                tournamentTeamTwo.Points += 3;
                            }
                        }
                        else
                        {
                            if (teams.Game_Status != "Completed" && teams.Game_Status != "Terminate")
                            {
                                teams.Team_One_Score = 0;
                                teams.Team_Two_Score = 1;
                                teams.Game_Status = "Completed";
                                //set team one
                                tournamentTeamOne.Active = "No";
                                //set team two 
                                tournamentTeamTwo.Active = "Yes";

                            }
                        }

                        _context.Tournament_Leaderboard.Update(teams);
                        _context.Tournament_Team.Update(tournamentTeamOne);
                        _context.Tournament_Team.Update(tournamentTeamTwo);
                    }

                    //update team two in leader board that forfeited the tournament
                    var teamTwoInLeaderBoard = (from t in _context.Tournament_Leaderboard
                                           .Where(x => x.Team_Two_ID == id)
                                                select t).ToList();
                    foreach (var teams in teamTwoInLeaderBoard)
                    {

                        var tournamentTeamOne = await _context.Tournament_Team.FindAsync(teams.Tournament_ID, teams.Team_One_ID);
                        var tournamentTeamTwo = await _context.Tournament_Team.FindAsync(teams.Tournament_ID, teams.Team_Two_ID);
                        if (teams.Stage == 1)
                        {
                            if (teams.Game_Status != "Completed" && teams.Game_Status != "Terminate")
                            {
                                teams.Team_One_Score = 1;
                                teams.Team_Two_Score = 0;
                                teams.Game_Status = "Completed";
                                //set team one
                                tournamentTeamOne.Scores += 1;
                                tournamentTeamOne.Points += 3;
                                //set team two 
                                tournamentTeamTwo.Scores += 0;
                                tournamentTeamTwo.Points += 0;
                            }
                        }
                        else
                        {
                            if (teams.Game_Status != "Completed" && teams.Game_Status != "Terminate")
                            {
                                teams.Team_One_Score = 1;
                                teams.Team_Two_Score = 2;
                                teams.Game_Status = "Completed";
                                //set team one
                                tournamentTeamOne.Active = "Yes";
                                //set team two 
                                tournamentTeamTwo.Active = "No";

                            }
                        }

                        _context.Tournament_Leaderboard.Update(teams);
                        _context.Tournament_Team.Update(tournamentTeamOne);
                        _context.Tournament_Team.Update(tournamentTeamTwo);
                    }

                    var team = await _context.Team.FindAsync(id);
                    team.Team_Name = team.Team_Name + " Forfeited " + team.ID;
                    team.Ban = true;
                    _context.Team.Update(team);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                //otherwise if tournament is over simply delete the team
                else
                {
                    var team = await _context.Team.FindAsync(id);
                    team.Team_Name = team.Team_Name + " Forfeited " + team.ID;
                    team.Ban = true;
                    _context.Team.Update(team);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
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



        /// <summary>
        /// This method adds player to team
        /// </summary>
        /// <param name="PlayerID">player id</param>
        /// <param name="TeamID">team id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinTournament(string PlayerID, Guid TeamID)
        {
            try
            {
                var id = from u in _context.Users
                         select u;

                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                }

                // get user id and role
                var userID = id.FirstOrDefault().Id;
                var userRole = id.FirstOrDefault().User_Role;

                Team_User teamUser = new Team_User();
                teamUser.Team_ID = TeamID;
                teamUser.User_ID = PlayerID;
                teamUser.Status = "Pending";
                teamUser.User_Role = userRole;
                _context.Add(teamUser);
                await _context.SaveChangesAsync();


                return RedirectToAction("Details", new { id = TeamID });
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }

        }


        /// <summary>
        /// This method removes the player from team
        /// </summary>
        /// <param name="PlayerID"></param>
        /// <param name="TeamID"></param>
        /// <returns></returns>
        public async Task<IActionResult> Cancel(string PlayerID, Guid TeamID)
        {
            try
            {
                if (PlayerID == null)
                {
                    return NotFound();
                }

                var Team_User = await _context.Team_User
                    .Include(t => t.Team)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(m => m.User_ID == PlayerID && m.Team_ID == TeamID);




                if (Team_User == null)
                {
                    return NotFound();
                }

                _context.Team_User.Remove(Team_User);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = TeamID });
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }
        /// <summary>
        /// This method is used rate a team
        /// </summary>
        /// <param name="review">review int</param>
        /// <param name="teamID">team id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int review, Guid teamID)
        {
            try
            {
                TempData["notValidReview"] = null;
                if (review < 1 || review > 5)
                {
                    TempData["notValidReview"] = review;
                    return RedirectToAction("Details", new { id = teamID });
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
                    var hasReview = (from t in _context.Review_User_Team
                                     .Where(x => x.Team_ID == teamID && x.User_ID == userID)
                                     select t).ToList();
                    foreach (var rec in hasReview)
                    {
                        _context.Review_User_Team.Remove(rec);
                    }




                    Review_User_Team reviewUserTeam = new Review_User_Team();
                    reviewUserTeam.Team_ID = teamID;
                    reviewUserTeam.User_ID = userID;
                    reviewUserTeam.User_Role = userRole;
                    reviewUserTeam.Review = review;
                    _context.Add(reviewUserTeam);

                    // update team review avg
                    var totalReviews = (from t in _context.Review_User_Team
                                       .Where(x => x.Team_ID == teamID)
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

                    var team = await _context.Team.FindAsync(teamID);
                    team.Review = Math.Round(avg, 1);
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = teamID });
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

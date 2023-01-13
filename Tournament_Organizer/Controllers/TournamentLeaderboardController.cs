using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
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
    public class TournamentLeaderboardController : Controller
    {
        private readonly TournamentOrganizerContext _context;

        public TournamentLeaderboardController(TournamentOrganizerContext context)
        {
            _context = context;
        }


        private string groupFilter = "A";
        private string groupFilterOppsite = "B";


        /// <summary>
        /// return a list of matches in tournament
        /// </summary>
        /// <param name="pg">page index</param>
        /// <returns>list of matches</returns>
        // GET: TournamentLeaderboard
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Index(int pg=1)
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

                // get user torunament ID
                var tournament_IDs = from x in _context.Tournament_User
                                     where x.User_ID == userID
                                     select x;

                if (!tournament_IDs.Any())
                {
                    TempData["noTournament"] = "You can not view page till you create a tournamnet";
                    return RedirectToAction("Index", "Tournaments", TempData["noTournament"]);

                }
                var tournament_ID = tournament_IDs.FirstOrDefaultAsync().Result.Tournament_ID;

                var tournamentOrganizerContext = _context.Tournament_Leaderboard.Include(t => t.Team_One).Include(t => t.Team_Two).Include(t => t.Tournament);
                var tournamentByOrganizerContext = tournamentOrganizerContext.Where(t => t.Tournament_ID == tournament_ID).OrderByDescending(x => x.Game_Status).ThenByDescending(x => x.Stage);

                // view a button to let user go next step
                var tournament = from x in _context.Tournament
                                 .Where(x => x.ID == tournament_ID)
                                 select x;
                //get tournament current stage
                var tournamentStage = tournament.FirstOrDefaultAsync().Result.Stage;

                //get tournament current max teams
                var NextStageMaxTeams = tournament.FirstOrDefaultAsync().Result.NextStageMaxTeams;
                //get total games in the tournament

                uint GamesCount = 0;
                uint total = 0;
                var totalGames = 0;
                var totalMatchesPerTeam = 0;
                if (NextStageMaxTeams <= 1)
                {
                    GamesCount = 1;
                }
                else
                {
                    GamesCount = NextStageMaxTeams / 2;
                }
                if (GamesCount == 1)
                {
                    total = 1;
                    totalGames = 1;
                    totalMatchesPerTeam = 1;
                }
                else
                {
                    uint sum = GamesCount;
                    for (var i = 0; i < GamesCount; i++)
                    {
                        sum -= 1;
                        total += sum;
                    }

                    totalGames = (int)(total * 2);
                    if (total == 1)
                    {
                        totalMatchesPerTeam = 1;
                    }
                    else if (tournamentStage == 1)
                    {
                        totalMatchesPerTeam = (int)(GamesCount - 1);
                    }
                    else
                    {
                        totalMatchesPerTeam = 1;
                        totalGames = (int)GamesCount;

                    }


                }


                // check if only one team is left with active status
                var teamIDs = from m in _context.Tournament_Team
                            .Where(x => x.Tournament_ID == tournament_ID && x.Status == "Approved" && x.Active == "Yes")
                              select m;

                var AvaiableTeams = teamIDs.Count();

                // else if only one Active then that is the winner team
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
                    if (NextStageMaxTeams == 1)
                    {

                        ViewBag.NextStage = false;
                        ViewBag.stageName = "";
                    }
                    else
                    {
                        ViewBag.NextStage = true;
                        ViewBag.stageName = "End";
                    }

                }

                else
                {

                    //check if all teams are completed in each stage 
                    var GamesCompleted = from x in _context.Tournament_Leaderboard
                     .Where(x => x.Tournament_ID == tournament_ID && x.Stage == tournamentStage
                     && x.Game_Status == "Completed" || x.Game_Status == "Terminate")
                                         select x;

                    if (GamesCompleted.Count() >= totalGames)
                    {

                        ViewBag.NextStage = true;
                        ViewBag.stageName = "Next Stage";
                    }
                    else
                    {

                        ViewBag.NextStage = false;
                        ViewBag.stageName = "";
                    }

                }



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
        /// Get the create view
        /// </summary>
        /// <param name="Group">tournament Group value</param>
        /// <returns></returns>
        // GET: TournamentLeaderboard/Create
        [Authorize(Roles = "Organizer")]
        public IActionResult Create(string Group)
        {
          

            try
            {
             
                if (Group != null)
                {
                    groupFilter = Group;

                    if (Group == "A")
                    {
                        groupFilterOppsite = "B";
                    }
                    else
                    {
                        groupFilterOppsite = "A";

                    }
                }

                List<string> group = new List<string>();
                group.Add(groupFilter);
                group.Add(groupFilterOppsite);
                ViewBag.Groups = new SelectList(group);

                // get user id
                var id = from u in _context.Users
                         select u;
                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    id = id.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                }

                var userID = id.FirstOrDefault().Id;

                // get user torunament ID
                var tournament_ID = (from x in _context.Tournament_User
                                     where x.User_ID == userID
                                     select x).FirstOrDefaultAsync().Result.Tournament_ID;

                if (tournament_ID == null)
                {
                    return NotFound();

                }



                var tournament = from x in _context.Tournament
                                 where x.ID == tournament_ID
                                 select x;
                // get count of teams that are approved
                var tournamentTeamCount = from m in _context.Tournament_Team
                                .Where(x => x.Tournament_ID == tournament_ID && x.Status == "Approved")
                                          select m;
                if (tournamentTeamCount.Count() < tournament.FirstOrDefaultAsync().Result.Max_Teams_Tournament)
                {

                    TempData["teamsNotCompleted"] = $"You can't add teams till the number of teams match {tournament.FirstOrDefaultAsync().Result.Max_Teams_Tournament} registered/approved teams. Currently registered/approved teams {tournamentTeamCount.Count()}";
                    return RedirectToAction("Index");
                }
                else if (tournament.FirstOrDefaultAsync().Result.NextStageMaxTeams == 1)
                {
                    TempData["teamsNotCompleted"] = $"You can't add teams this tournament is over. If you wish to start another one delete this first then start a new one";
                    return RedirectToAction("Index");
                }
                else
                {
                    //get tournament current stage
                    var tournamentStage = tournament.FirstOrDefaultAsync().Result.Stage;
                    //get tournament current max teams
                    var NextStageMaxTeams = tournament.FirstOrDefaultAsync().Result.NextStageMaxTeams;
                    //get total games in the tournament
                    var GamesCount = NextStageMaxTeams / 2;
                    uint total = 0;
                    var totalGames = 0;
                    var totalMatchesPerTeam = 0;
                    if (GamesCount == 1)
                    {
                        total = 1;
                        totalGames = 1;
                        totalMatchesPerTeam = 1;
                    }
                    else
                    {
                        var sum = GamesCount;
                        for (var i = 0; i < GamesCount; i++)
                        {
                            sum -= 1;
                            total += sum;
                        }

                        totalGames = (int)(total * 2);
                        if (total == 1)
                        {
                            totalMatchesPerTeam = 1;
                        }
                        else if (tournamentStage == 1)
                        {
                            totalMatchesPerTeam = (int)(GamesCount - 1);
                        }
                        else
                        {
                            totalMatchesPerTeam = 1;
                            totalGames = (int)GamesCount;

                        }

                    }

                    // track number of teams in each group to show the user
                    var teamCountA = from m in _context.Tournament_Team
                                         .Where(x => x.Tournament_ID == tournament_ID && x.Status == "Approved" && x.Group == "A" && x.NumberOfMatches < totalMatchesPerTeam)
                                     select m;
                    var teamCountB = from m in _context.Tournament_Team
                                         .Where(x => x.Tournament_ID == tournament_ID && x.Status == "Approved" && x.Group == "B" && x.NumberOfMatches < totalMatchesPerTeam)
                                     select m;
                    ViewBag.teamCountA = teamCountA.Count();
                    ViewBag.teamCountB = teamCountB.Count();

                    var teamIDs = from m in _context.Tournament_Team
                                  .Where(x => x.Tournament_ID == tournament_ID && x.Status == "Approved" && x.Active == "Yes")
                                  select m;
                    var AvaiableTeams = teamIDs.Count();

                    // if only one Active then that is the winner team
                    if (AvaiableTeams == 1)
                    {

                        foreach (var teamID in teamIDs)
                        {
                            //get team that are equal to IDs assigned with tournament
                            var team = from x in _context.Team
                                        .Where(t => t.ID == teamID.Team_ID)
                                       select x;
                            ViewBag.Message = $"Tournament is over and the winner team is {team.FirstOrDefaultAsync().Result.Team_Name}";
                            ViewBag.State = "success";
                        }



                        return View();
                    }
                    else if (AvaiableTeams == 2)
                    {
                        //add teams to a list
                        List<Team> Teams = new List<Team>();

                        foreach (var teamID in teamIDs)
                        {
                            //get team that are equal to IDs assigned with tournament
                            var team = from x in _context.Team
                                        .Where(t => t.ID == teamID.Team_ID)
                                       select x;


                            if (teamID.NumberOfMatches < 1)
                            {
                                Teams.Add(team.ToListAsync().Result.FirstOrDefault());
                                ViewBag.Message = "Group filter has no affect here as these are last two teams. Winner takes the cup ";
                                ViewBag.State = "info";
                            }
                            else
                            {
                                ViewBag.Message = "There are no teams. The last match was set with final teams.";
                                ViewBag.State = "warning";
                            }

                        }


                        ViewData["Team_One_ID"] = new SelectList(Teams, "ID", "Team_Name");
                        ViewData["Team_Two_ID"] = new SelectList(Teams, "ID", "Team_Name");
                        ViewBag.Tournament_ID = tournament_ID;
                        return View();
                    }
                    else
                    {
                       
                        var getCurrentGamesInTournament = from x in _context.Tournament_Leaderboard
                                                             .Where(x => x.Tournament_ID == tournament_ID && x.Stage == tournamentStage)
                                                          select x;

                        if (getCurrentGamesInTournament.Count() < totalGames)
                        {

                            
                            //get teams assigned with tournament and are Approved and does not exist in leaderboard table
                            teamIDs = from m in _context.Tournament_Team
                                      .Where(x => x.Tournament_ID == tournament_ID && x.Status == "Approved" && x.Group == groupFilter)
                                      select m;



                            // if tournament stage is greater than 1 only get teams with Active status 
                            if (tournamentStage > 1)
                            {
                                teamIDs = from m in _context.Tournament_Team
                                         .Where(x => x.Tournament_ID == tournament_ID && x.Status == "Approved" && x.Group == groupFilter && x.Active == "Yes")
                                          select m;
                            }

                          
                            //add teams to a list
                            List<Team> Teams = new List<Team>();
                            foreach (var teamID in teamIDs)
                            {
                                
                                //get team that are equal to IDs assigned with tournament
                                var team = from x in _context.Team
                                            .Where(t => t.ID == teamID.Team_ID)
                                           select x;

                                
                                if (teamID.NumberOfMatches < totalMatchesPerTeam)
                                {
                                    
                                    Teams.Add(team.ToListAsync().Result.FirstOrDefault());
                                 
                                }
                               
                            }

                         

                            ViewData["Team_One_ID"] = new SelectList(Teams, "ID", "Team_Name");
                            ViewData["Team_Two_ID"] = new SelectList(Teams, "ID", "Team_Name");
                            ViewBag.Tournament_ID = tournament_ID;
                            return View();




                        }

                        else
                        {
                            // send a viewbag here with message notifying th user that the tournamnet is still on the stage one 
                            ViewBag.Message = "You will not be able to add matches till the current stage of tournamnet is over";
                            ViewBag.State = "warning";
                            ViewBag.Groups = new SelectList(group);
                            return View();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error Occurred {ex.Message}";
                return RedirectToAction("Index", "Home");
            }

        }

        /// <summary>
        /// Create a match in the tournamnet 
        /// </summary>
        /// <param name="tournament_Leaderboard">leaderboard or game object</param>
        /// <param name="Group">tournament group</param>
        /// <returns></returns>
        // POST: TournamentLeaderboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Create([Bind("Team_One_ID,Team_Two_ID,Tournament_ID,Stage,Game_Date,Game_Status")] Tournament_Leaderboard tournament_Leaderboard, string Group)
        {
            try
            {
                if (tournament_Leaderboard.Game_Date < DateTimeOffset.Now)
                {
                    ModelState.AddModelError("Game_Date", "A match with simillar teams already exist. Please try setting different teams");
                    TempData["teamSelectError"] = "Game Date can't be in the past";
                    return RedirectToAction("Create", TempData["teamSelectError"]);
                }

                // update match with current tournament stage
                var tournamentCurrentStage = (from m in _context.Tournament
                           .Where(x => x.ID == tournament_Leaderboard.Tournament_ID)
                                              select m).FirstOrDefaultAsync().Result.Stage;
                tournament_Leaderboard.Stage = tournamentCurrentStage;

                if (Group != null)
                {
                    groupFilter = Group;

                    if (Group == "A")
                    {
                        groupFilterOppsite = "B";
                    }
                    else
                    {
                        groupFilterOppsite = "A";

                    }
                }

                List<string> group = new List<string>();
                group.Add(groupFilter);
                group.Add(groupFilterOppsite);
                ViewBag.Groups = new SelectList(group);


                // check if the
                var matchExist = from x in _context.Tournament_Leaderboard
                                 .Where(x => x.Tournament_ID == tournament_Leaderboard.Tournament_ID && x.Team_One_ID == tournament_Leaderboard.Team_One_ID
                                 && x.Team_Two_ID == tournament_Leaderboard.Team_Two_ID && x.Stage == tournament_Leaderboard.Stage)
                                 select x;
                // check if the
                var matchExistOppsite = from x in _context.Tournament_Leaderboard
                                 .Where(x => x.Tournament_ID == tournament_Leaderboard.Tournament_ID && x.Team_One_ID == tournament_Leaderboard.Team_Two_ID
                                 && x.Team_Two_ID == tournament_Leaderboard.Team_One_ID && x.Stage == tournament_Leaderboard.Stage)
                                        select x;

                if (matchExist.Any() || matchExistOppsite.Any())
                {
                    ModelState.AddModelError("Team_One_ID", "A match with simillar teams already exist. Please try setting different teams");
                    TempData["teamSelectError"] = "A match with simillar teams already exist. Please try setting different teams";
                    return RedirectToAction("Create", TempData["teamSelectError"]);
                }
                if (tournament_Leaderboard.Team_One_ID == tournament_Leaderboard.Team_Two_ID)
                {
                    ModelState.AddModelError("Team_One_ID", "Invalid. You Can't Select Same Team, Please Try Another Team Name");
                    TempData["teamSelectError"] = "Teams are Invalid. You Can't Select Same Team, Please Try Another Team Name";
                    return RedirectToAction("Create", TempData["teamSelectError"]);
                }
                if (ModelState.IsValid)
                {


                    var tournament_TeamOne = await _context.Tournament_Team.FindAsync(tournament_Leaderboard.Tournament_ID, tournament_Leaderboard.Team_One_ID);

                    var tournament_TeamTwo = await _context.Tournament_Team.FindAsync(tournament_Leaderboard.Tournament_ID, tournament_Leaderboard.Team_Two_ID);
                    //get team one matches and increment it
                    var teamOneMatches = (from m in _context.Tournament_Team
                                      .Where(x => x.Team_ID == tournament_Leaderboard.Team_One_ID && x.Tournament_ID == tournament_Leaderboard.Tournament_ID)
                                          select m).FirstOrDefaultAsync().Result.NumberOfMatches;
                    //get team two matches and increment it 
                    var teamTwoMatches = (from m in _context.Tournament_Team
                                      .Where(x => x.Team_ID == tournament_Leaderboard.Team_Two_ID && x.Tournament_ID == tournament_Leaderboard.Tournament_ID)
                                          select m).FirstOrDefaultAsync().Result.NumberOfMatches;


                    teamOneMatches += 1;
                    teamTwoMatches += 1;
                    tournament_TeamOne.NumberOfMatches = teamOneMatches;
                    tournament_TeamTwo.NumberOfMatches = teamTwoMatches;
                    _context.Update(tournament_TeamOne);
                    _context.Update(tournament_TeamTwo);
                    _context.Add(tournament_Leaderboard);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
        /// Get the edit view
        /// </summary>
        /// <param name="tournamentID">tournament id</param>
        /// <param name="teamOneID">team one id</param>
        /// <param name="teamTwoID">team two id</param>
        /// <param name="Stage">stage of the match</param>
        /// <returns></returns>
        // GET: TournamentLeaderboard/Edit/5
        public async Task<IActionResult> Edit(Guid? tournamentID, Guid? teamOneID, Guid? teamTwoID, uint Stage)
        {
            try
            {
                if (tournamentID == null)
                {
                    return NotFound();
                }

                var tournament_Leaderboard = await _context.Tournament_Leaderboard.FindAsync(tournamentID, teamOneID, teamTwoID, Stage);
                if (tournament_Leaderboard == null)
                {
                    return NotFound();
                }


                var tournament = from x in _context.Tournament
                               .Where(x => x.ID == tournamentID)
                                 select x;
                //get tournament current stage
                var tournamentStage = tournament.FirstOrDefaultAsync().Result.Stage;
                if (tournamentStage > tournament_Leaderboard.Stage)
                {
                    TempData["previousTeams"] = $"Can't edit these teams as they are in previous stage ({tournament_Leaderboard.Stage}) and the current tournament stage is ({tournamentStage})";
                    return RedirectToAction("Index", TempData["previousTeams"]);
                }
                else
                {
                    return View(tournament_Leaderboard);
                }
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Post the edited object
        /// </summary>
        /// <param name="Tournament_ID">tournament id </param>
        /// <param name="Team_One_ID">team one id</param>
        /// <param name="Team_Two_ID">team two id</param>
        /// <param name="tournament_Leaderboard">leaderboard or game object</param>
        /// <returns></returns>
        // POST: TournamentLeaderboard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Edit(Guid? Tournament_ID, Guid? Team_One_ID, Guid? Team_Two_ID, [Bind("Team_One_ID,Team_One_Score,Team_Two_ID,Team_Two_Score,Tournament_ID,Stage,Game_Date,GameNotes,Game_Status")] Tournament_Leaderboard tournament_Leaderboard)
        {
            try
            {
                if (Tournament_ID != tournament_Leaderboard.Tournament_ID)
                {
                    return NotFound();
                }

                var leaderBoard = _context.Tournament_Leaderboard.AsNoTracking().Where(x => x.Tournament_ID == Tournament_ID
                              && x.Team_One_ID == Team_One_ID && x.Team_Two_ID == Team_Two_ID);

                var tournament_TeamOne = await _context.Tournament_Team.FindAsync(Tournament_ID, Team_One_ID);

                var tournament_TeamTwo = await _context.Tournament_Team.FindAsync(Tournament_ID, Team_Two_ID);

                if (tournament_Leaderboard.Team_One_Score == tournament_Leaderboard.Team_Two_Score && tournament_Leaderboard.Stage > 1)
                {
                    ModelState.AddModelError("Team_One_Score", "Home Team Score can't be even with Away Team Score");
                    ModelState.AddModelError("Team_Two_Score", "Away Team Score can't be even with Home Team Score ");
                    TempData["teamEven"] = "Teams scores can't be even. This mtach has to be detemined by a winner.";
                }

                if (ModelState.IsValid)
                {
                    try
                    {

                        // shoow teams if tournament is on stage one
                        if (tournament_Leaderboard.Stage == 1)
                        {

                            //get the team one matches and increment it
                            var teamOnePoints = (from m in _context.Tournament_Team
                                              .Where(x => x.Team_ID == Team_One_ID && x.Tournament_ID == Tournament_ID)
                                                 select m).FirstOrDefaultAsync().Result.Points;


                            //get the team one matches and increment it 
                            var teamTwoPoints = (from m in _context.Tournament_Team
                                              .Where(x => x.Team_ID == Team_Two_ID && x.Tournament_ID == Tournament_ID)
                                                 select m).FirstOrDefaultAsync().Result.Points;
                            //get the team one matches and increment it
                            var teamOneScores = (from m in _context.Tournament_Team
                                              .Where(x => x.Team_ID == Team_One_ID && x.Tournament_ID == Tournament_ID)
                                                 select m).FirstOrDefaultAsync().Result.Scores;


                            //get the team one matches and increment it 
                            var teamTwoScores = (from m in _context.Tournament_Team
                                              .Where(x => x.Team_ID == Team_Two_ID && x.Tournament_ID == Tournament_ID)
                                                 select m).FirstOrDefaultAsync().Result.Scores;


                            if (leaderBoard.FirstOrDefaultAsync().Result.Game_Status == "New")
                            {
                                if (tournament_Leaderboard.Team_One_Score > tournament_Leaderboard.Team_Two_Score)
                                {
                                    teamOnePoints += 3;
                                    teamTwoPoints += 0;

                                }
                                else if (tournament_Leaderboard.Team_One_Score == tournament_Leaderboard.Team_Two_Score)
                                {
                                    teamOnePoints += 1;
                                    teamTwoPoints += 1;

                                }
                                else if (tournament_Leaderboard.Team_One_Score < tournament_Leaderboard.Team_Two_Score)
                                {
                                    teamOnePoints += 0;
                                    teamTwoPoints += 3;

                                }
                                teamOneScores += tournament_Leaderboard.Team_One_Score;
                                teamTwoScores += tournament_Leaderboard.Team_Two_Score;


                            }
                            //if the same matches is updated more than once then check the old values and based on that set the new points
                            else
                            {

                                if (tournament_Leaderboard.Team_One_Score > tournament_Leaderboard.Team_Two_Score)
                                {
                                    if (leaderBoard.FirstOrDefaultAsync().Result.Team_One_Score < leaderBoard.FirstOrDefaultAsync().Result.Team_Two_Score)
                                    {
                                        teamOnePoints += 3;
                                        teamTwoPoints -= 3;

                                    }
                                    else if (leaderBoard.FirstOrDefaultAsync().Result.Team_One_Score == leaderBoard.FirstOrDefaultAsync().Result.Team_Two_Score)
                                    {
                                        teamOnePoints += 2;
                                        teamTwoPoints -= 1;
                                    }
                                }
                                else if (tournament_Leaderboard.Team_One_Score == tournament_Leaderboard.Team_Two_Score)
                                {
                                    if (leaderBoard.FirstOrDefaultAsync().Result.Team_One_Score < leaderBoard.FirstOrDefaultAsync().Result.Team_Two_Score)
                                    {
                                        teamOnePoints += 1;
                                        teamTwoPoints -= 2;
                                    }
                                    else if (leaderBoard.FirstOrDefaultAsync().Result.Team_One_Score > leaderBoard.FirstOrDefaultAsync().Result.Team_Two_Score)
                                    {
                                        teamOnePoints -= 2;
                                        teamTwoPoints += 1;
                                    }

                                }
                                else if (tournament_Leaderboard.Team_One_Score < tournament_Leaderboard.Team_Two_Score)
                                {
                                    if (leaderBoard.FirstOrDefaultAsync().Result.Team_One_Score > leaderBoard.FirstOrDefaultAsync().Result.Team_Two_Score)
                                    {
                                        teamOnePoints -= 3;
                                        teamTwoPoints += 3;
                                    }
                                    else if (leaderBoard.FirstOrDefaultAsync().Result.Team_One_Score == leaderBoard.FirstOrDefaultAsync().Result.Team_Two_Score)
                                    {
                                        teamOnePoints -= 1;
                                        teamTwoPoints += 2;
                                    }

                                }



                                teamOneScores -= leaderBoard.FirstOrDefaultAsync().Result.Team_One_Score;
                                teamTwoScores -= leaderBoard.FirstOrDefaultAsync().Result.Team_Two_Score;
                                teamOneScores += tournament_Leaderboard.Team_One_Score;
                                teamTwoScores += tournament_Leaderboard.Team_Two_Score;

                            }

                            tournament_TeamOne.Points = teamOnePoints;
                            tournament_TeamTwo.Points = teamTwoPoints;
                            tournament_TeamOne.Scores = teamOneScores;
                            tournament_TeamTwo.Scores = teamTwoScores;
                        }
                        else if (tournament_Leaderboard.Stage > 1)
                        {

                            if (tournament_Leaderboard.Team_One_Score > tournament_Leaderboard.Team_Two_Score)
                            {
                                tournament_TeamTwo.Active = "No";
                                tournament_TeamOne.Active = "Yes";
                            }
                            else if (tournament_Leaderboard.Team_One_Score < tournament_Leaderboard.Team_Two_Score)
                            {
                                tournament_TeamTwo.Active = "Yes";
                                tournament_TeamOne.Active = "No";
                            }


                        }



                        _context.Update(tournament_TeamOne);
                        _context.Update(tournament_TeamTwo);
                        _context.Update(tournament_Leaderboard);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Tournament_LeaderboardExists(tournament_Leaderboard.Tournament_ID))
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

                return View(tournament_Leaderboard);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }


        /// <summary>
        /// Get the delete view
        /// </summary>
        /// <param name="tournamentID">tournament id</param>
        /// <param name="teamOneID">team one id</param>
        /// <param name="teamTwoID">team two id</param>
        /// <param name="Stage">game stage</param>
        /// <returns></returns>
        // GET: TournamentLeaderboard/Delete/5
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Delete(Guid? tournamentID, Guid? teamOneID, Guid? teamTwoID, uint Stage)
        {
            try
            {
                if (tournamentID == null)
                {
                    return NotFound();
                }

                var tournament_Leaderboard = await _context.Tournament_Leaderboard
                    .Include(t => t.Team_One)
                    .Include(t => t.Team_Two)
                    .Include(t => t.Tournament)
                    .FirstOrDefaultAsync(m => m.Tournament_ID == tournamentID && m.Team_One_ID == teamOneID && m.Team_Two_ID == teamTwoID && m.Stage == Stage);
                if (tournament_Leaderboard == null)
                {
                    return NotFound();
                }


                var tournament = from x in _context.Tournament
                             .Where(x => x.ID == tournamentID)
                                 select x;


                //get tournament current stage
                var tournamentStage = tournament.FirstOrDefaultAsync().Result.Stage;
                if (tournamentStage > tournament_Leaderboard.Stage)
                {
                    TempData["previousTeams"] = $"You can't delete this match becuase this match is determined and completed in stage({tournament_Leaderboard.Stage}) and the current tournament stage is ({tournamentStage})";
                    return RedirectToAction("Index", TempData["previousTeams"]);
                }
                else if (tournament_Leaderboard.Game_Status == "Completed")
                {
                    TempData["previousTeams"] = $"You can't delete this match becuase this match is completed";
                    return RedirectToAction("Index", TempData["previousTeams"]);
                }
                else
                {
                    return View(tournament_Leaderboard);
                }
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }

        }

        /// <summary>
        /// Deletes the game or match
        /// </summary>
        /// <param name="Team_One_ID">team one id</param>
        /// <param name="Team_Two_ID">team two id</param>
        /// <param name="Tournament_ID">tournament id</param>
        /// <param name="Stage">game stage</param>
        /// <returns></returns>
        // POST: TournamentLeaderboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> DeleteConfirmed(Guid? Team_One_ID, Guid? Team_Two_ID, Guid? Tournament_ID, uint Stage)
        {
            try
            {
                var tournament_Leaderboard = await _context.Tournament_Leaderboard.FindAsync(Tournament_ID, Team_One_ID, Team_Two_ID, Stage);

                var tournament_TeamOne = await _context.Tournament_Team.FindAsync(Tournament_ID, Team_One_ID);

                var tournament_TeamTwo = await _context.Tournament_Team.FindAsync(Tournament_ID, Team_Two_ID);

                tournament_TeamOne.NumberOfMatches -= 1;
                tournament_TeamTwo.NumberOfMatches -= 1;
                _context.Update(tournament_TeamOne);
                _context.Update(tournament_TeamTwo);
                _context.Tournament_Leaderboard.Remove(tournament_Leaderboard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        private bool Tournament_LeaderboardExists(Guid id)
        {
            return _context.Tournament_Leaderboard.Any(e => e.Tournament_ID == id);
        }


        /// <summary>
        /// This method set the tournament to next stage
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NextStage()
        {
            try
            {
                //get user ID
                var getUserID = from u in _context.Users
                                select u;
                if (!String.IsNullOrEmpty(User.Identity.Name))
                {
                    getUserID = getUserID.Where(t => t.UserName.ToLower() == (User.Identity.Name.ToLower()));
                }

                var userID = getUserID.FirstOrDefault().Id;

                // get user torunament ID
                var Tournament_ID = (from x in _context.Tournament_User
                                    .Where(x => x.User_ID == userID)
                                     select x).FirstOrDefaultAsync().Result.Tournament_ID;


                var tournament = from x in _context.Tournament
                                 .Where(x => x.ID == Tournament_ID)
                                 select x;
                //get tournament current stage
                var tournamentStage = tournament.FirstOrDefaultAsync().Result.Stage;

                //get tournament current max teams
                var NextStageMaxTeams = tournament.FirstOrDefaultAsync().Result.NextStageMaxTeams;
                //get total games in the tournament
                var GamesCount = NextStageMaxTeams / 2;
                uint total = 0;
                var totalGames = 0;
                var totalMatchesPerTeam = 0;
                if (GamesCount == 1)
                {
                    total = 1;
                    totalGames = 1;
                    totalMatchesPerTeam = 1;
                }
                else
                {
                    var sum = GamesCount;
                    for (var i = 0; i < GamesCount; i++)
                    {
                        sum -= 1;
                        total += sum;
                    }

                    totalGames = (int)(total * 2);
                    if (total == 1)
                    {
                        totalMatchesPerTeam = 1;
                    }
                    else if (tournamentStage == 1)
                    {
                        totalMatchesPerTeam = (int)(GamesCount - 1);
                    }
                    else
                    {
                        totalMatchesPerTeam = 1;
                        totalGames = (int)GamesCount;

                    }
                }

                //check if all teams are completed in each stage 
                var GamesCompleted = from x in _context.Tournament_Leaderboard
                 .Where(x => x.Tournament_ID == Tournament_ID && x.Stage == tournamentStage
                 && x.Game_Status == "Completed" || x.Game_Status == "Terminate")
                                     select x;


                // if all games of this stage is completed then set tournamnet stage to next stage
                if (GamesCompleted.Count() >= totalGames && tournamentStage == 1)
                {

                    var lostTeamsA = (from m in _context.Tournament_Team
                                        .Where(x => x.Tournament_ID == Tournament_ID && x.Group == "A")
                                        .OrderBy(x => x.Points.Value).ThenBy(n => n.Scores.Value)
                                      select m).ToList();
                    //get lost teams and set their active to NO for group B
                    var lostTeamsB = (from m in _context.Tournament_Team
                                    .Where(x => x.Tournament_ID == Tournament_ID && x.Group == "B")
                                    .OrderBy(x => x.Points.Value).ThenBy(n => n.Scores.Value)
                                      select m).ToList();


                    int count = 1;
                    foreach (var teamPoints in lostTeamsA)
                    {
                        var updateTeamActive = await _context.Tournament_Team.FindAsync(Tournament_ID, teamPoints.Team_ID);
                        if (count <= (int)GamesCount / 2)
                        {
                            updateTeamActive.Active = "No";

                        }
                        else
                        {
                            updateTeamActive.NumberOfMatches = 0;
                        }
                        count++;
                        _context.Update(updateTeamActive);
                    }

                    count = 1;
                    foreach (var teamPoints in lostTeamsB)
                    {
                        var updateTeamActive = await _context.Tournament_Team.FindAsync(Tournament_ID, teamPoints.Team_ID);
                        if (count <= (int)GamesCount / 2)
                        {
                            updateTeamActive.Active = "No";

                        }
                        else
                        {
                            updateTeamActive.NumberOfMatches = 0;
                        }
                        count++;
                        _context.Update(updateTeamActive);
                    }


                    NextStageMaxTeams = NextStageMaxTeams / 2;
                    tournamentStage += 1;
                    var tournamentStageUpdate = await _context.Tournament.FindAsync(Tournament_ID);
                    tournamentStageUpdate.Stage = tournamentStage;
                    tournamentStageUpdate.NextStageMaxTeams = NextStageMaxTeams;
                    _context.Update(tournamentStageUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else if (GamesCompleted.Count() >= totalGames && tournamentStage > 1)
                {
                    var WinTeams = from m in _context.Tournament_Team
                                  .Where(x => x.Tournament_ID == Tournament_ID && x.Active == "Yes")
                                   select m;
                    foreach (var teamID in WinTeams)
                    {
                        var updateTeamMatches = await _context.Tournament_Team.FindAsync(Tournament_ID, teamID.Team_ID);
                        updateTeamMatches.NumberOfMatches = 0;
                        _context.Update(updateTeamMatches);
                    }

                    NextStageMaxTeams = NextStageMaxTeams / 2;
                    tournamentStage += 1;
                    var tournamentStageUpdate = await _context.Tournament.FindAsync(Tournament_ID);
                    tournamentStageUpdate.Stage = tournamentStage;
                    tournamentStageUpdate.NextStageMaxTeams = NextStageMaxTeams;
                    _context.Update(tournamentStageUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");

                }
                else
                {
                    return RedirectToAction("Index");
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

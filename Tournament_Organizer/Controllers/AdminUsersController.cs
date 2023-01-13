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
    public class AdminUsersController : Controller
    {
        private readonly TournamentOrganizerContext _context;

        public AdminUsersController(TournamentOrganizerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of users view
        /// </summary>
        /// <param name="searchString">Username</param>
        /// <returns></returns>
        // GET: Teams
        public async Task<IActionResult> Index(string searchString)
        {
            try
            {
                var users = from u in _context.Users
                            select u;
                ViewBag.searchString = "";
                if (!String.IsNullOrEmpty(searchString))
                {
                    users = users.Where(t => t.UserName.ToLower().Contains(searchString.ToLower()));
                    ViewBag.searchString = searchString;
                }



                return View(await users.ToListAsync());
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Get the edit view of user object
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }



                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                ViewBag.imgValue = "";
                if (user.Image != null)
                {
                    ViewBag.imgValue = Convert.ToBase64String(user.Image);
                }

                return View(user);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Update the user object
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="Image">image file</param>
        /// <param name="imgValue">origianl image value</param>
        /// <param name="ban">ban value</param>
        /// <returns></returns>
        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
    
        public async Task<IActionResult> Edit(string id, List<IFormFile> Image, string imgValue, bool ban)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (ModelState.IsValid)
                {
                    try
                    {

                        if (Image.Count == 0 && imgValue != null)
                        {
                            byte[] chartData = Convert.FromBase64String(imgValue);
                            user.Image = chartData;
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
                                        user.Image = stream.ToArray();
                                    }
                                }
                            }
                        }
                        user.Ban = ban;
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TeamExists(user.Id))
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
                return View(user);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }


        private bool TeamExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

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
    [Authorize(Roles = "Admin")]
    public class AdminDivisionsController : Controller
    {
        private readonly TournamentOrganizerContext _context;

        public AdminDivisionsController(TournamentOrganizerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of divisions
        /// </summary>
        /// <returns></returns>
        // GET: AdminDivisions
        public async Task<IActionResult> Index()
        {
            try
            {
                var division = from d in _context.Division
                               .OrderBy(d => d.DivisionName)
                               select d;
                return View(await division.ToListAsync());
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }


        /// <summary>
        /// Get the create view of divisions
        /// </summary>
        /// <returns></returns>
        // GET: AdminDivisions/Create
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create a new divisions object
        /// </summary>
        /// <param name="division"></param>
        /// <returns></returns>
        // POST: AdminDivisions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Division_Id,DivisionName")] Division division)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    division.Division_Id = Guid.NewGuid();
                    _context.Add(division);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(division);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Get the edit view of divisions
        /// </summary>
        /// <param name="id">division id</param>
        /// <returns></returns>
        // GET: AdminDivisions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var division = await _context.Division.FindAsync(id);
                if (division == null)
                {
                    return NotFound();
                }
                return View(division);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Update the division object
        /// </summary>
        /// <param name="id">division id</param>
        /// <param name="division">division object</param>
        /// <returns></returns>
        // POST: AdminDivisions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Division_Id,DivisionName")] Division division)
        {
            try
            {
                if (id != division.Division_Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(division);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DivisionExists(division.Division_Id))
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
                return View(division);
            }
            catch
            {
                TempData["Error"] = "Error Occurred";
                return RedirectToAction("Index", "Home");
            }
        }

        private bool DivisionExists(Guid id)
        {
            return _context.Division.Any(e => e.Division_Id == id);
        }
    }
}

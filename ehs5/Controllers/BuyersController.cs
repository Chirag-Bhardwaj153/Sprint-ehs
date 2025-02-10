using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ehs5.Models;

namespace ehs5.Controllers
{
    public class BuyersController : Controller
    {
        private readonly Ehs1Context _context;

        public BuyersController(Ehs1Context context)
        {
            _context = context;
        }

        // GET: Buyers
        public async Task<IActionResult> Index()
        {
            var ehs1Context = _context.Buyers.Include(b => b.BuyerCity).Include(b => b.BuyerState).Include(b => b.UserNameNavigation);
            return View(await ehs1Context.ToListAsync());
        }

        // GET: Buyers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buyer = await _context.Buyers
                .Include(b => b.BuyerCity)
                .Include(b => b.BuyerState)
                .Include(b => b.UserNameNavigation)
                .FirstOrDefaultAsync(m => m.BuyerId == id);
            if (buyer == null)
            {
                return NotFound();
            }

            return View(buyer);
        }

        // GET: Buyers/Create
        public IActionResult Create()
        {
            ViewData["BuyerCityId"] = new SelectList(_context.Cities, "CityId", "CityId");
            ViewData["BuyerStateId"] = new SelectList(_context.States, "StateId", "StateId");
            ViewData["UserName"] = new SelectList(_context.Users, "UserName", "UserName");
            return View();
        }

        // POST: Buyers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BuyerId,FirstName,LastName,DateOfBirth,PhoneNo,EmailId,UserName,BuyerStateId,BuyerCityId")] Buyer buyer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(buyer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerCityId"] = new SelectList(_context.Cities, "CityId", "CityId", buyer.BuyerCityId);
            ViewData["BuyerStateId"] = new SelectList(_context.States, "StateId", "StateId", buyer.BuyerStateId);
            ViewData["UserName"] = new SelectList(_context.Users, "UserName", "UserName", buyer.UserName);
            return View(buyer);
        }

        // GET: Buyers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buyer = await _context.Buyers.FindAsync(id);
            if (buyer == null)
            {
                return NotFound();
            }
            ViewData["BuyerCityId"] = new SelectList(_context.Cities, "CityId", "CityId", buyer.BuyerCityId);
            ViewData["BuyerStateId"] = new SelectList(_context.States, "StateId", "StateId", buyer.BuyerStateId);
            ViewData["UserName"] = new SelectList(_context.Users, "UserName", "UserName", buyer.UserName);
            return View(buyer);
        }

        // POST: Buyers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BuyerId,FirstName,LastName,DateOfBirth,PhoneNo,EmailId,UserName,BuyerStateId,BuyerCityId")] Buyer buyer)
        {
            if (id != buyer.BuyerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(buyer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuyerExists(buyer.BuyerId))
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
            ViewData["BuyerCityId"] = new SelectList(_context.Cities, "CityId", "CityId", buyer.BuyerCityId);
            ViewData["BuyerStateId"] = new SelectList(_context.States, "StateId", "StateId", buyer.BuyerStateId);
            ViewData["UserName"] = new SelectList(_context.Users, "UserName", "UserName", buyer.UserName);
            return View(buyer);
        }

        // GET: Buyers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buyer = await _context.Buyers
                .Include(b => b.BuyerCity)
                .Include(b => b.BuyerState)
                .Include(b => b.UserNameNavigation)
                .FirstOrDefaultAsync(m => m.BuyerId == id);
            if (buyer == null)
            {
                return NotFound();
            }

            return View(buyer);
        }

        // POST: Buyers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buyer = await _context.Buyers.FindAsync(id);
            if (buyer != null)
            {
                _context.Buyers.Remove(buyer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuyerExists(int id)
        {
            return _context.Buyers.Any(e => e.BuyerId == id);
        }
    }
}

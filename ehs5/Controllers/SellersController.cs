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
    public class SellersController : Controller
    {
        private readonly Ehs1Context _context;

        public SellersController(Ehs1Context context)
        {
            _context = context;
        }

        // GET: Sellers
        public async Task<IActionResult> Index()
        {
            var ehs1Context = _context.Sellers.Include(s => s.City).Include(s => s.SellerCity).Include(s => s.SellerState).Include(s => s.State).Include(s => s.UserNameNavigation);
            return View(await ehs1Context.ToListAsync());
        }

        // GET: Sellers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                .Include(s => s.City)
                .Include(s => s.SellerCity)
                .Include(s => s.SellerState)
                .Include(s => s.State)
                .Include(s => s.UserNameNavigation)
                .FirstOrDefaultAsync(m => m.SellerId == id);
            if (seller == null)
            {
                return NotFound();
            }

            return View(seller);
        }

        // GET: Sellers/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId");
            ViewData["SellerCityId"] = new SelectList(_context.Cities, "CityId", "CityId");
            ViewData["SellerStateId"] = new SelectList(_context.States, "StateId", "StateId");
            ViewData["StateId"] = new SelectList(_context.States, "StateId", "StateId");
            ViewData["UserName"] = new SelectList(_context.Users, "UserName", "UserName");
            return View();
        }

        // POST: Sellers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SellerId,UserName,FirstName,LastName,DateOfBirth,PhoneNo,Address,StateId,CityId,EmailId,SellerCityId,SellerStateId")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                _context.Add(seller);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId", seller.CityId);
            ViewData["SellerCityId"] = new SelectList(_context.Cities, "CityId", "CityId", seller.SellerCityId);
            ViewData["SellerStateId"] = new SelectList(_context.States, "StateId", "StateId", seller.SellerStateId);
            ViewData["StateId"] = new SelectList(_context.States, "StateId", "StateId", seller.StateId);
            ViewData["UserName"] = new SelectList(_context.Users, "UserName", "UserName", seller.UserName);
            return View(seller);
        }

        // GET: Sellers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers.FindAsync(id);
            if (seller == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId", seller.CityId);
            ViewData["SellerCityId"] = new SelectList(_context.Cities, "CityId", "CityId", seller.SellerCityId);
            ViewData["SellerStateId"] = new SelectList(_context.States, "StateId", "StateId", seller.SellerStateId);
            ViewData["StateId"] = new SelectList(_context.States, "StateId", "StateId", seller.StateId);
            ViewData["UserName"] = new SelectList(_context.Users, "UserName", "UserName", seller.UserName);
            return View(seller);
        }

        // POST: Sellers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SellerId,UserName,FirstName,LastName,DateOfBirth,PhoneNo,Address,StateId,CityId,EmailId,SellerCityId,SellerStateId")] Seller seller)
        {
            if (id != seller.SellerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seller);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerExists(seller.SellerId))
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
            ViewData["CityId"] = new SelectList(_context.Cities, "CityId", "CityId", seller.CityId);
            ViewData["SellerCityId"] = new SelectList(_context.Cities, "CityId", "CityId", seller.SellerCityId);
            ViewData["SellerStateId"] = new SelectList(_context.States, "StateId", "StateId", seller.SellerStateId);
            ViewData["StateId"] = new SelectList(_context.States, "StateId", "StateId", seller.StateId);
            ViewData["UserName"] = new SelectList(_context.Users, "UserName", "UserName", seller.UserName);
            return View(seller);
        }

        // GET: Sellers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Sellers
                .Include(s => s.City)
                .Include(s => s.SellerCity)
                .Include(s => s.SellerState)
                .Include(s => s.State)
                .Include(s => s.UserNameNavigation)
                .FirstOrDefaultAsync(m => m.SellerId == id);
            if (seller == null)
            {
                return NotFound();
            }

            return View(seller);
        }

        // POST: Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seller = await _context.Sellers.FindAsync(id);
            if (seller != null)
            {
                _context.Sellers.Remove(seller);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SellerExists(int id)
        {
            return _context.Sellers.Any(e => e.SellerId == id);
        }
    }
}

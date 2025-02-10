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
    public class PropertyImagesController : Controller
    {
        private readonly Ehs1Context _context;

        public PropertyImagesController(Ehs1Context context)
        {
            _context = context;
        }

        // GET: PropertyImages
        public async Task<IActionResult> Index()
        {
            var ehs1Context = _context.PropertyImages.Include(p => p.Property).Include(p => p.PropertyImageId1Navigation);
            return View(await ehs1Context.ToListAsync());
        }

        // GET: PropertyImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyImage = await _context.PropertyImages
                .Include(p => p.Property)
                .Include(p => p.PropertyImageId1Navigation)
                .FirstOrDefaultAsync(m => m.PropertyImageId == id);
            if (propertyImage == null)
            {
                return NotFound();
            }

            return View(propertyImage);
        }

        // GET: PropertyImages/Create
        public IActionResult Create()
        {
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId");
            ViewData["PropertyImageId1"] = new SelectList(_context.PropertyImages, "PropertyImageId", "PropertyImageId");
            return View();
        }

        // POST: PropertyImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyImageId,PropertyId,ImageUrl,PropertyImageId1")] PropertyImage propertyImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(propertyImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId", propertyImage.PropertyId);
            ViewData["PropertyImageId1"] = new SelectList(_context.PropertyImages, "PropertyImageId", "PropertyImageId", propertyImage.PropertyImageId1);
            return View(propertyImage);
        }

        // GET: PropertyImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyImage = await _context.PropertyImages.FindAsync(id);
            if (propertyImage == null)
            {
                return NotFound();
            }
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId", propertyImage.PropertyId);
            ViewData["PropertyImageId1"] = new SelectList(_context.PropertyImages, "PropertyImageId", "PropertyImageId", propertyImage.PropertyImageId1);
            return View(propertyImage);
        }

        // POST: PropertyImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyImageId,PropertyId,ImageUrl,PropertyImageId1")] PropertyImage propertyImage)
        {
            if (id != propertyImage.PropertyImageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyImageExists(propertyImage.PropertyImageId))
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
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId", propertyImage.PropertyId);
            ViewData["PropertyImageId1"] = new SelectList(_context.PropertyImages, "PropertyImageId", "PropertyImageId", propertyImage.PropertyImageId1);
            return View(propertyImage);
        }

        // GET: PropertyImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyImage = await _context.PropertyImages
                .Include(p => p.Property)
                .Include(p => p.PropertyImageId1Navigation)
                .FirstOrDefaultAsync(m => m.PropertyImageId == id);
            if (propertyImage == null)
            {
                return NotFound();
            }

            return View(propertyImage);
        }

        // POST: PropertyImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyImage = await _context.PropertyImages.FindAsync(id);
            if (propertyImage != null)
            {
                _context.PropertyImages.Remove(propertyImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyImageExists(int id)
        {
            return _context.PropertyImages.Any(e => e.PropertyImageId == id);
        }
    }
}

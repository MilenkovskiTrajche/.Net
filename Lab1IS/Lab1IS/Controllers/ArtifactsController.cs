using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab1IS.Data;
using Lab1IS.Models;

namespace Lab1IS.Controllers
{
    public class ArtifactsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArtifactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Artifacts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Artifacts.Include(a => a.Collection);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Artifacts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artifact = await _context.Artifacts
                .Include(a => a.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artifact == null)
            {
                return NotFound();
            }

            return View(artifact);
        }

        // GET: Artifacts/Create
        public IActionResult Create()
        {
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Currator");
            return View();
        }

        // POST: Artifacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Origin,Year,Description,CollectionId")] Artifact artifact)
        {
            if (ModelState.IsValid)
            {
                artifact.Id = Guid.NewGuid();
                _context.Add(artifact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Currator", artifact.CollectionId);
            return View(artifact);
        }

        // GET: Artifacts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artifact = await _context.Artifacts.FindAsync(id);
            if (artifact == null)
            {
                return NotFound();
            }
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Currator", artifact.CollectionId);
            return View(artifact);
        }

        // POST: Artifacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Origin,Year,Description,CollectionId")] Artifact artifact)
        {
            if (id != artifact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artifact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtifactExists(artifact.Id))
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
            ViewData["CollectionId"] = new SelectList(_context.Collections, "Id", "Currator", artifact.CollectionId);
            return View(artifact);
        }

        // GET: Artifacts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artifact = await _context.Artifacts
                .Include(a => a.Collection)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artifact == null)
            {
                return NotFound();
            }

            return View(artifact);
        }

        // POST: Artifacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var artifact = await _context.Artifacts.FindAsync(id);
            if (artifact != null)
            {
                _context.Artifacts.Remove(artifact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtifactExists(Guid id)
        {
            return _context.Artifacts.Any(e => e.Id == id);
        }
    }
}

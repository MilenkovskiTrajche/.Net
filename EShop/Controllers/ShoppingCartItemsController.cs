using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EShop.Data;
using EShop.Models;

namespace EShop.Controllers
{
    public class ShoppingCartItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShoppingCartItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ShoppingCartItems.Include(s => s.Product).Include(s => s.ShoppingCart);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ShoppingCartItems/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartItem = await _context.ShoppingCartItems
                .Include(s => s.Product)
                .Include(s => s.ShoppingCart)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCartItem == null)
            {
                return NotFound();
            }

            return View(shoppingCartItem);
        }

        // GET: ShoppingCartItems/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description");
            ViewData["ShoppingCartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id");
            return View();
        }

        // POST: ShoppingCartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,Quantity,ShoppingCartId")] ShoppingCartItem shoppingCartItem)
        {
            if (ModelState.IsValid)
            {
                shoppingCartItem.Id = Guid.NewGuid();
                _context.Add(shoppingCartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", shoppingCartItem.ProductId);
            ViewData["ShoppingCartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id", shoppingCartItem.ShoppingCartId);
            return View(shoppingCartItem);
        }

        // GET: ShoppingCartItems/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartItem = await _context.ShoppingCartItems.FindAsync(id);
            if (shoppingCartItem == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", shoppingCartItem.ProductId);
            ViewData["ShoppingCartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id", shoppingCartItem.ShoppingCartId);
            return View(shoppingCartItem);
        }

        // POST: ShoppingCartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ProductId,Quantity,ShoppingCartId")] ShoppingCartItem shoppingCartItem)
        {
            if (id != shoppingCartItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingCartItemExists(shoppingCartItem.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Description", shoppingCartItem.ProductId);
            ViewData["ShoppingCartId"] = new SelectList(_context.ShoppingCarts, "Id", "Id", shoppingCartItem.ShoppingCartId);
            return View(shoppingCartItem);
        }

        // GET: ShoppingCartItems/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartItem = await _context.ShoppingCartItems
                .Include(s => s.Product)
                .Include(s => s.ShoppingCart)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoppingCartItem == null)
            {
                return NotFound();
            }

            return View(shoppingCartItem);
        }

        // POST: ShoppingCartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var shoppingCartItem = await _context.ShoppingCartItems.FindAsync(id);
            if (shoppingCartItem != null)
            {
                _context.ShoppingCartItems.Remove(shoppingCartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartItemExists(Guid id)
        {
            return _context.ShoppingCartItems.Any(e => e.Id == id);
        }
    }
}

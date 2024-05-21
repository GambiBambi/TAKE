using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcNews.Data;
using MvcNews.Models;
using Newtonsoft.Json.Linq;
using NuGet.Packaging.Signing;

namespace MvcNews.Controllers {
    public class NewsController : Controller {
        private readonly NewsDbContext _context;

        public NewsController(NewsDbContext context) {
            _context = context;
        }

        // GET: News
        public async Task<IActionResult> Index() {
            return View(await _context.News.ToListAsync());
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var newsItem = await _context.News
                .FirstOrDefaultAsync(m => m.id == id);
            if (newsItem == null) {
                return NotFound();
            }

            return View(newsItem);
        }

        // GET: News/Create
        public IActionResult Create() {
            var data = new NewsItem {
                timeStamp = DateTime.Now
            };
            return View(data);
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,timeStamp,text,RowVersion")] NewsItem newsItem) {
            if (ModelState.IsValid) {
                _context.Add(newsItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newsItem);
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var newsItem = await _context.News.FindAsync(id);
            if (newsItem == null) {
                return NotFound();
            }
            return View(newsItem);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,timeStamp,text,RowVersion")] NewsItem newsItem) {
            if (id != newsItem.id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(newsItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e) {
                    if (!NewsItemExists(newsItem.id)) {
                        return NotFound();
                    }
                    else {
                        ModelState.AddModelError("", "Object modified or deleted by another user");

                        var entry = e.Entries.Single();
                        var databaseEntry = entry.GetDatabaseValues();
                        var databaseEntity = (NewsItem)databaseEntry.ToObject();

                        newsItem.RowVersion = (byte[])databaseEntity.RowVersion;
                        ModelState.Remove("RowVersion");

                        ModelState.AddModelError("TimeStamp", "Current value: " +
                                                              (DateTime)databaseEntity.timeStamp);
                        ModelState.AddModelError("Text", "Current value: " +
                                                         (string)databaseEntity.text);

                        return View();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(newsItem);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null || _context.News == null) {
                return NotFound();
            }

            var newsItem = await _context.News
                .FirstOrDefaultAsync(m => m.id == id);
            if (newsItem == null) {
                return NotFound();
            }

            return View(newsItem);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, NewsItem newsItem) {
            if (_context.News == null) {
                return Problem("Entity set 'NewsDbContext.News'  is null.");
            }

            try {
                _context.News.Remove(newsItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e) {
                if (!NewsItemExists(newsItem.id)) {
                    return NotFound();
                }
                else {
                    ModelState.AddModelError("", "Unable to save changes. The record was modified by another user after you got the original value");
                   
                    var entry = e.Entries.Single();
                    var databaseEntry = entry.GetDatabaseValues();
                    var databaseEntity = (NewsItem)databaseEntry.ToObject();
                    ModelState.Remove("RowVersion");
                    return View(databaseEntity);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NewsItemExists(int id) {
            return _context.News.Any(e => e.id == id);
        }
    }
}

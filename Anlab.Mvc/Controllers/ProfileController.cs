using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class ProfileController : ApplicationController
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.SingleOrDefaultAsync(x=>x.Id == CurrentUserId));
        }

        public async Task<IActionResult> Edit()
        {
            return View(await _context.Users.SingleOrDefaultAsync(x => x.Id == CurrentUserId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User data)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == CurrentUserId);

            if (await TryUpdateModelAsync(user))
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(user);
        }
    }
}
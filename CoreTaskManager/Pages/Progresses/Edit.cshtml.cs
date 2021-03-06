﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoreTaskManager.Model;
using CoreTaskManager.Models;

namespace CoreTaskManager.Pages.Progresses
{
    public class EditModel : PageModel
    {
        private readonly CoreTaskManager.Models.CoreTaskManagerContext _context;

        public EditModel(CoreTaskManager.Models.CoreTaskManagerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Progress Progress { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Progress = await _context.Progresses.FirstOrDefaultAsync(m => m.Id == id);

            if (Progress == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Progress).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgressExists(Progress.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProgressExists(int id)
        {
            return _context.Progresses.Any(e => e.Id == id);
        }
    }
}

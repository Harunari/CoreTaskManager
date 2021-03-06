﻿using CoreTaskManager.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace CoreTaskManager.Pages.Progresses
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly CoreTaskManager.Models.CoreTaskManagerContext _context;
        private HostingApplication.Context context;

        public CreateModel(CoreTaskManager.Models.CoreTaskManagerContext context, IHostingEnvironment e)
        {
            _context = context;
            _hostingEnvironment = e;
        }

        public IActionResult OnGet(IFormFile pic)
        {
            return Page();
        }

        [BindProperty]
        public Progress Progress { get; set; }
        
        [Authorize]
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var imageName = "empty.jpg";
            if (file != null)
            {
                imageName = Path.GetFileName(file.FileName);
                // 画像ファイルの絶対パス取得
                var fileName = Path.Combine(_hostingEnvironment.WebRootPath, "userImages", imageName);
                file.CopyTo(new FileStream(fileName, FileMode.Create));

            }

            // 自動入力項目
            Progress.UserName = User.Identity.Name;
            Progress.RegisteredDateTime = DateTime.Now;
            // 進捗が作成された時点で設定されたタスクは0。タスクはOwnerPageで設定できる
            Progress.NumberOfItems = 0;
            Progress.Image = "/userImages/" + imageName;


            _context.Progresses.Add(Progress);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
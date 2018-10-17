﻿using CoreTaskManager.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreTaskManager.Pages.Progresses
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly Models.CoreTaskManagerContext _context;
        private readonly Data.ApplicationDbContext _userContext;
        private readonly int _pageSize;

        public const string SessionCurrentPage = "CurrentPage";
        public const string SessionProgressGenre = "ProgressGenre";
        public const string SessionSearchString = "SearchString";
        public const string SessionNumOfProgresses = "NumOfProgresses";
        public const string SessionLastPage = "LastPage";


        public IndexModel(Models.CoreTaskManagerContext context, Data.ApplicationDbContext userContext)
        {
            _context = context;
            _userContext = userContext;
            // 一ページに表示する項目の量
            _pageSize = 6;
        }

        public IList<Progress> Progresses { get; set; }
        public SelectList Genres { get; set; }
        public IList<Participant> Participants { get; set; }
        public string ProgressGenre { get; set; }
        public IList<MyIdentityUser> ServiceUsers { get; set; }

        public async Task OnGetAsync(string progressGenre, string searchString, string currentPageString)
        {
            if (!HttpContext.Session.IsAvailable)
            {
                await HttpContext.Session.LoadAsync();
            }
            if (String.IsNullOrEmpty(currentPageString))
            {
                HttpContext.Session.SetInt32(SessionCurrentPage, 1);
            }
            HttpContext.Session.SetString(SessionProgressGenre, progressGenre ?? "");
            HttpContext.Session.SetString(SessionSearchString, searchString ?? "");

            var progresses = FilterProgresses(progressGenre, searchString, currentPageString);
            Progresses = await progresses.ToListAsync();
            Genres = new SelectList(await GenerateGenreList().ToListAsync());
            ServiceUsers = await _userContext.MyIdentityUsers.ToListAsync();
            Participants = new List<Participant>();
            await progresses.Select(p => p.Id).ForEachAsync(id =>
             {
                 // 各進捗の参加者を5人ランダムに抽出
                 var concerned5People = _context.Participants.Where(p => p.ProgressId == id).OrderBy(i => Guid.NewGuid()).Take(5).ToList();
                 if (concerned5People.Count > 0)
                 {
                     concerned5People.ForEach(person => Participants.Add(person));
                 }
             });
        }
        public async Task<IActionResult> OnPostCurrentPageAsync()
        {
            if (!HttpContext.Session.IsAvailable)
            {
                await HttpContext.Session.LoadAsync();
            }
            var genre = HttpContext.Session.GetString(SessionProgressGenre);
            var searchString = HttpContext.Session.GetString(SessionSearchString);
            var currentPage = HttpContext.Session.GetInt32(SessionCurrentPage);
            return Redirect($"Progresses?progressesGenre={genre}&searchString={searchString}&currentPageString={currentPage}");
        }
        public async Task<IActionResult> OnPostNextPageAsync()
        {
            if (!HttpContext.Session.IsAvailable)
            {
                await HttpContext.Session.LoadAsync();
            }
            // sessionページ数がnullであれば1を代入
            int currentPage = 1;
            if (HttpContext.Session.GetInt32(SessionCurrentPage) != null)
            {
                currentPage = (int)HttpContext.Session.GetInt32(SessionCurrentPage);
            }
            int lastPage;
            if (HttpContext.Session.GetInt32(SessionLastPage) != null)
            {
                lastPage = (int)HttpContext.Session.GetInt32(SessionLastPage);
            }
            else
            {
                lastPage = _context.Progresses.Count() / _pageSize + 1;
            }
            currentPage++;
            if (currentPage > lastPage)
            {
                currentPage = lastPage;
            }
            else
            {
                HttpContext.Session.SetInt32(SessionCurrentPage, currentPage);
            }
            var progressGenre = HttpContext.Session.GetString(SessionProgressGenre);
            var searchString = HttpContext.Session.GetString(SessionSearchString);
            return Redirect($"Progresses?progressesGenre={progressGenre}&searchString={searchString}&currentPageString={currentPage}");
        }
        public async Task<IActionResult> OnPostPrevPageAsync()
        {
            if (!HttpContext.Session.IsAvailable)
            {
                await HttpContext.Session.LoadAsync();
            }
            int currentPage = 1;
            if (HttpContext.Session.GetInt32(SessionCurrentPage) != null)
            {
                currentPage = (int)HttpContext.Session.GetInt32(SessionCurrentPage);
            }
            currentPage--;
            if (currentPage < 1)
            {
                currentPage = 1;
                HttpContext.Session.SetInt32(SessionCurrentPage, 1);
            }
            else
            {
                HttpContext.Session.SetInt32(SessionCurrentPage, currentPage);
            }
            var progressGenre = HttpContext.Session.GetString(SessionProgressGenre);
            var searchString = HttpContext.Session.GetString(SessionSearchString);
            var progresses = FilterProgresses(progressGenre, searchString, currentPage.ToString());
            return Redirect($"Progresses?progressesGenre={progressGenre}&searchString={searchString}&currentPageString={currentPage}");
        }

        IQueryable<Progress> FilterProgresses(string progressGenre, string searchString, string currentPageString)
        {
            int currentPage = 1;
            ViewData["CurrentPage"] = "1";
            if (!String.IsNullOrEmpty(currentPageString))
            {
                currentPage = int.Parse(currentPageString);
                ViewData["CurrentPage"] = currentPageString;
            }

            var progresses = from p in _context.Progresses
                             select p;
            progresses = progresses.OrderByDescending(p => p.RegisteredDateTime);
            if (!String.IsNullOrEmpty(searchString))
            {
                progresses = progresses.Where(p => p.Title.Contains(searchString));
            }
            if (!String.IsNullOrEmpty(progressGenre))
            {
                progresses = progresses.Where(x => x.Genre == progressGenre);
            }

            HttpContext.Session.SetInt32(SessionNumOfProgresses, progresses.Count());
            int lastPage = progresses.Count() / _pageSize + 1;
            HttpContext.Session.SetInt32(SessionLastPage, lastPage);
            return progresses = Paging(progresses, currentPage, _pageSize);
        }
        IQueryable<string> GenerateGenreList()
        {
            var genreQuery = from m in _context.Progresses
                             orderby m.Genre
                             select m.Genre;
            return genreQuery.Distinct();
        }
        IQueryable<Progress> Paging(IQueryable<Progress> progresses, int currentPage, int pageSize)
        {
            // もし変数currenPageが不正な値であればページは１とする
            if (currentPage < 1)
            {
                currentPage = 1;
            }
            return progresses.Skip((currentPage - 1) * pageSize).Take(pageSize);

        }
    }
}

using Assignment10.Models;
using Assignment10.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment10.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BowlingLeagueContext context { get; set; }

        public HomeController(ILogger<HomeController> logger, BowlingLeagueContext ctx)
        {
            _logger = logger;
            context = ctx;
        }

        public IActionResult Index(long? teamnameid, string teamname, int pageNum = 1)
        {
            int pageSize = 5;

            return View(new IndexViewModel
            {
                Bowlers = context.Bowlers
                .Where(b => b.TeamId == teamnameid || teamnameid == null)
                .OrderBy(b => b.BowlerLastName)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToList(),

                PageNumberingInfo = new PageNumberingInfo
                {
                    NumItemsPerPage = pageSize,
                    CurrentPage = pageNum,

                    // if no team has been selected then get the full count, otherwise only count the number of bowlers from the team selected
                    TotalNumItems = (teamnameid == null ? context.Bowlers.Count() :
                        context.Bowlers.Where(x => x.TeamId == teamnameid).Count())
                },

                TeamName = teamname
            });
                
                
        }

        [HttpPost]
        public IActionResult Index(string teamSearch)
        {
            return View(context.Bowlers
                .FromSqlInterpolated($"SELECT * FROM Bowlers WHERE Team LIKE {teamSearch}")
                .ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

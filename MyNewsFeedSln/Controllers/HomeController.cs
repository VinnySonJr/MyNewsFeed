using MyNewsFeedSln.App_Data;
using MyNewsFeedSln.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyNewsFeedSln.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DBManager dataManager = new DBManager();
            List<NewsSource> listOfSources = dataManager.ListSources();
            ViewBag.Articles = dataManager.GetAllArticles();
            return View(listOfSources);
        }
    }
}
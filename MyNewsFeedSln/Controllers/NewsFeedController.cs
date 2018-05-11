using MyNewsFeedSln.App_Data;
using MyNewsFeedSln.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyNewsFeedSln.Controllers
{
    public class NewsFeedController : Controller
    {
        DBManager dataManager = new DBManager();

        // GET: NewsFeed
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Articles()
        {
            dataManager = new DBManager();
            List<Article> listOfArticles = dataManager.GetAllArticles();
            return View(listOfArticles);
        }
        public ActionResult Sources()
        {
            dataManager = new DBManager();
            List<NewsSource> listOfSources = dataManager.ListSources();
            foreach (var source in listOfSources)
            {
                if (source.SourceImageLink == "")
                    source.SourceImageLink = "https://www.wikihow.com/images/3/39/Newspaper-Color-6.jpg";
            }
            ViewBag.Sources = listOfSources;
            return View(listOfSources);
        }
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int Id)
        {
            return View(dataManager.SearchSourceById(Id.ToString()));
        }

        public ActionResult EditSource(int Id)
        {
            return View(dataManager.SearchSourceById(Id.ToString()));
        }

        public ActionResult Delete(int Id)
        {
            dataManager.RemoveSource(Id.ToString());
            return View("Sources",dataManager.ListSources());
        }
        public ActionResult Details(int Id)
        {
            return View();
        }

        public ActionResult TestSub(int Id)
        {
            return View();
        }


    }
}
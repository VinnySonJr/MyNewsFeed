using MyNewsFeedSln.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Net;
using System.IO;
using Google.Apis.Customsearch;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Web;
using MyNewsFeedSln.App_Data;

namespace TestApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            DBManager dataManager = new DBManager();
            //Console.WriteLine(dataManager.ListSources().First().SourceName);
            Console.WriteLine(dataManager.GetAllArticles().Last().UrlImage);
           // Console.WriteLine(dataManager.GetArticlesGivenCategory("Technology").First().name);
           // Console.WriteLine(dataManager.GetArticlesGivenCategoryAndLocation("Technology","US").First().name);
           // Console.WriteLine(dataManager.GetNewsSourceGivenArticle(dataManager.GetAllArticles().First()).SourceName);

            NewsSource source = new NewsSource
            {
                SourceId = "402",
                SourceName = "Igihe",
                SourceLink = "Igihe.com",
                SourceImageLink = "",
                SourceType = "",
                SourceLocation = "Rwanda",
                SourceFavorite = false
            };

        }
        
    }
}

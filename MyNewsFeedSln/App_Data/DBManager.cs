﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using Dapper;
using MyNewsFeedSln.Models;
using Newtonsoft.Json.Linq;

namespace MyNewsFeedSln.App_Data
{
    public class DBManager
    {
        String connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=MyNewsFeedDB;Integrated Security=True";
        string newsApiKey = "106a58c18c96495a949f2649e077dd75";
        public List<NewsSource> ListSources()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<NewsSource>("Select * From NewsSource").AsList();
            }
        }

        public int AddNewSource(NewsSource source)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO NewsSource VALUES (@SourceName," +
                                                                "@SourceLink," +
                                                                "@SourceImageLink," +
                                                                "@SourceType," +
                                                                "@SourceLocation," +
                                                                "@SourceFavorite)";
                int rowsAffected = db.Execute(sqlQuery, source);
                return rowsAffected;
            }
        }

        public int UpdateSource(NewsSource source)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE NewsSource SET SourceName = @SourceName," +
                                                     "SourceLink = @SourceLink," +
                                                     "SourceImageLink = @SourceImageLink," +
                                                     "SourceType = @SourceType," +
                                                     "SourceLocation = @SourceLocation," +
                                                     "SourceFavorite = @SourceFavorite " +
                                                     "WHERE SourceId = @SourceId";
                int rowsAffected = db.Execute(sqlQuery, source);
                return rowsAffected;
            }
        }

        public int RemoveSource(string SourceId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = "Delete From NewsSource " +
                    "WHERE SourceId = @SourceId";
                int rowsAffected = db.Execute(sqlQuery, new { SourceId });
                return rowsAffected;
            }
        }

        public NewsSource SearchSourceByName(string SourceName)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<NewsSource>("Select * From NewsSource " +
                    "WHERE SourceName = @SourceName", new { SourceName }).SingleOrDefault();
            }
        }

        public NewsSource SearchSourceById(string SourceId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<NewsSource>("Select * From NewsSource " +
                    "WHERE SourceId = @SourceId", new { SourceId }).SingleOrDefault();
            }
        }  

        public List<Article> GetAllArticles()
        {
            string currentCountryCode = GetUserCountryLocation();
            string todayDate = GetTodayDateString();
            List<Article> allArticles = new List<Article>();
            List<string> visitedHosts = new List<string>();

            var url = "https://newsapi.org/v2/top-headlines?" +
            "country=" + currentCountryCode + "&" +
            "from=" + todayDate + "&" +
            "to=" + todayDate + "&" +
            "apiKey="+newsApiKey;

            foreach (var article in RequestArticles(url))
            {
                visitedHosts.Add((new Uri(article.Url)).Host);
                allArticles.Add(article);
            }
            
            foreach (var source in ListSources())
            {
                try
                {
                    if (!visitedHosts.Contains((new Uri(source.SourceLink)).Host))
                    {
                        List<Article> articles = GetArticlesGivenUrl(source.SourceLink);
                        allArticles.AddRange(articles);
                    }
                }
                catch (Exception)
                {
                    //The link to source does not work
                }
            }
            return allArticles;
        }

        public List<Article> GetArticlesGivenCategory(string type)
        {
            string currentCountryCode = GetUserCountryLocation();
            string todayDate = GetTodayDateString();

            var url = "https://newsapi.org/v2/top-headlines?" +
            "from=" + todayDate + "&" +
            "to=" + todayDate + "&" +
            "category=" +type+"&"+
            "apiKey=" + newsApiKey;

            return RequestArticles(url);
        }

        public List<Article> GetArticlesGivenCategoryAndLocation(string type, string countryCode)
        {
            string todayDate = GetTodayDateString();

            var url = "https://newsapi.org/v2/top-headlines?" +
            "country=" + countryCode + "&" +
            "from=" + todayDate + "&" +
            "to=" + todayDate + "&" +
            "category=" + type + "&" +
            "apiKey=" + newsApiKey; 

            return RequestArticles(url);
        }

        private List<Article> RequestArticles(string url)
        {
            List<Article> articles = new List<Article>();

            WebClient myClient = new WebClient();
            var json = myClient.DownloadString(url);

            var jsonData = myClient.DownloadData(url);
            JObject jsonObject = JObject.Parse(json);

            foreach (var article in jsonObject["articles"])
            {
                Article aArticle = new Article
                {
                    Id = (string)article["source"]["id"],
                    Name = (string)article["source"]["name"],
                    Author = (string)article["author"],
                    Title = (string)article["title"],
                    Description = (string)article["description"],
                    Url = (string)article["url"],
                    UrlImage = (string)article["urlToImage"],
                    PublishedAt = (string)article["publishedAt"]
                };
                if (aArticle.Author == null)
                    aArticle.Author = "Unkown";
                else if (aArticle.Author.Contains("www"))
                {
                    aArticle.Author = "Unkown";
                }
                articles.Add(aArticle);
            }
            return articles;
        }

        public string GetUserCountryLocation()
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

            IPAddress[] addr = ipEntry.AddressList;

            string userIPAddress = addr[addr.Length - 1].ToString();

            string url = "http://freegeoip.net/json/";
            WebClient client = new WebClient();
            string jsonstring = client.DownloadString(url);
            JObject locationInfo = JObject.Parse(jsonstring);

            return ((string)locationInfo["country_code"]);
        }

        public string GetTodayDateString()
        {
            return DateTime.Today.ToString("yyyy-MM-dd");
        }

        public NewsSource GetNewsSourceGivenArticle(Article article)
        {
            NewsSource source = null;
            foreach (var item in this.ListSources())
            {
                if (article.Id.ToLower().Equals(item.SourceName.ToLower()) ||
                    article.Name.ToLower().Equals(item.SourceName.ToLower()))
                {
                    source = item;
                    break;
                }
            }
            return source;
        }

        public List<Article> GetArticlesGivenUrl(string uri)
        {
            string pageHtmlText = "";
            char newLine = '\n';

            List<Article> extractedArticles = new List<Article>();
            List<String> linesWithImageLinks = new List<string>();
           
            using (var client = new WebClient())
            {
                pageHtmlText = client.DownloadString(new Uri(uri));
            }
          
            string[] pageTextLines = pageHtmlText.Split(newLine);
            
            foreach (var line in pageTextLines)
            {
                if(line.Contains("<a") && (line.Contains("<img") && line.Contains("href")))
                {
                    linesWithImageLinks.Add(line);
                }
            }

            XmlDocument htmlDocument = new XmlDocument();
            foreach (var line in linesWithImageLinks)
            {
                try
                {
                    htmlDocument.Load(new StringReader(
                        "<html>" +
                            line+
                        "</html>"));
            
                    string imageInArticle = htmlDocument.GetElementsByTagName("img").Item(0).Attributes.GetNamedItem("src").Value;
                    string linkToArticle = htmlDocument.GetElementsByTagName("a").Item(0).Attributes.GetNamedItem("href").Value;
                    string titleOfArticle = htmlDocument.GetElementsByTagName("a").Item(0).InnerText;
                    string hostName = (new Uri(uri)).Host;

                    if (!linkToArticle.Contains(hostName))
                    {
                        linkToArticle = "http://" + hostName + "/" + linkToArticle.TrimStart('/');
                    }

                    if (!((imageInArticle.Replace("http","").Replace("www","")).Substring(0,20).Contains(".")))
                    {
                        imageInArticle = "http://" + hostName + "/" + imageInArticle.TrimStart('/');
                    }

                    if (titleOfArticle.Equals(""))
                    {
                        titleOfArticle = linkToArticle.Split('/').Last();
                    }
                    if (imageInArticle.Equals(""))
                    {
                        titleOfArticle = "https://www.wikihow.com/images/3/39/Newspaper-Color-6.jpg";
                    }

                    Article article = new Article()
                    {
                        Url = linkToArticle,
                        UrlImage = imageInArticle,
                        Title = titleOfArticle,
                        Author = "unknown",
                        Description = titleOfArticle,
                        Name = hostName,
                        PublishedAt = GetTodayDateString(),
                        
                    };
                    if (!(article.Title.Length<2) && (article.UrlImage.Contains("jpg")|| article.UrlImage.Contains("jpeg")|| article.UrlImage.Contains("png")))
                        extractedArticles.Add(article);
                }
                catch (Exception)
                {
                     
                }               
            }
            Console.WriteLine(extractedArticles.Count);
            return extractedArticles;
        }
    }
}